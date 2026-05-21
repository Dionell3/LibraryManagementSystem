using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class BorrowController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BorrowController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;

            // Auto-mark overdue
            var toUpdate = await _context.BorrowTransactions
                .Where(t => t.Status == "Borrowed" && t.DueDate < now)
                .ToListAsync();
            foreach (var t in toUpdate) t.Status = "Overdue";
            if (toUpdate.Any()) await _context.SaveChangesAsync();

            IQueryable<BorrowTransaction> query = _context.BorrowTransactions
                .Include(t => t.Book)
                .Include(t => t.Member)
                .Include(t => t.Fine);

            if (User.IsInRole("Member"))
            {
                var userId = _userManager.GetUserId(User);
                var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
                if (member != null)
                    query = query.Where(t => t.MemberId == member.MemberID);
                else
                    return RedirectToAction("Index", "Home");
            }

            return View(await query.OrderByDescending(t => t.BorrowDate).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var userId = _userManager.GetUserId(User);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);

            if (member == null)
            {
                TempData["Error"] = "Member profile not found. Please contact the library.";
                return RedirectToAction("Index", "Books");
            }

            var book = await _context.Books.FindAsync(bookId);
            if (book == null || !book.IsAvailable)
            {
                TempData["Error"] = "This book is not available for borrowing.";
                return RedirectToAction("Index", "Books");
            }

            var parameters = await _context.BorrowingParameters.FirstOrDefaultAsync()
                ?? new BorrowingParameters();

            var currentBorrows = await _context.BorrowTransactions
                .CountAsync(t => t.MemberId == member.MemberID && (t.Status == "Borrowed" || t.Status == "Overdue"));

            if (currentBorrows >= parameters.MaxBorrowableItems)
            {
                TempData["Error"] = $"You have reached the maximum of {parameters.MaxBorrowableItems} borrowed books.";
                return RedirectToAction("Index", "Books");
            }

            var transaction = new BorrowTransaction
            {
                BookId = bookId,
                MemberId = member.MemberID,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(parameters.LoanDurationDays),
                Status = "Borrowed"
            };

            book.IsAvailable = false;
            _context.BorrowTransactions.Add(transaction);

            // Fulfill any open reservation
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.BookID == bookId && r.MemberID == member.MemberID && r.Status == "Reserved");
            if (reservation != null) reservation.Status = "Fulfilled";

            await _context.SaveChangesAsync();

            TempData["Success"] = $"You borrowed '{book.Title}'. Due: {transaction.DueDate:dd MMM yyyy}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var transaction = await _context.BorrowTransactions
                .Include(t => t.Book)
                .FirstOrDefaultAsync(t => t.BorrowTransactionId == id);

            if (transaction == null) return NotFound();

            transaction.ReturnDate = DateTime.Now;
            transaction.Status = "Returned";

            if (transaction.Book != null)
                transaction.Book.IsAvailable = true;

            var parameters = await _context.BorrowingParameters.FirstOrDefaultAsync()
                ?? new BorrowingParameters();

            if (transaction.ReturnDate > transaction.DueDate && !await _context.Fines.AnyAsync(f => f.BorrowTransactionId == id))
            {
                var overdueDays = (int)(transaction.ReturnDate.Value - transaction.DueDate).TotalDays;
                var fineAmount = overdueDays * parameters.OverduePenaltyPerDay;
                _context.Fines.Add(new Fine
                {
                    BorrowTransactionId = transaction.BorrowTransactionId,
                    Amount = fineAmount,
                    IssuedDate = DateTime.Now,
                    Notes = $"Overdue by {overdueDays} day(s) at ${parameters.OverduePenaltyPerDay}/day"
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Book returned successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewBook(int id)
        {
            var transaction = await _context.BorrowTransactions.FindAsync(id);
            if (transaction == null) return NotFound();

            var parameters = await _context.BorrowingParameters.FirstOrDefaultAsync()
                ?? new BorrowingParameters();

            if (transaction.RenewalCount >= parameters.RenewalLimit)
            {
                TempData["Error"] = $"Renewal limit of {parameters.RenewalLimit} already reached.";
                return RedirectToAction(nameof(Index));
            }

            transaction.DueDate = transaction.DueDate.AddDays(parameters.LoanDurationDays);
            transaction.RenewalCount++;
            transaction.Status = "Borrowed";

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Renewed. New due date: {transaction.DueDate:dd MMM yyyy}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> PayFine(int fineId)
        {
            var fine = await _context.Fines.FindAsync(fineId);
            if (fine != null)
            {
                fine.IsPaid = true;
                fine.PaidDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Fine marked as paid.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
