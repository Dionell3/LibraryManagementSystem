using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var thirtyDaysAgo = now.AddDays(-30);

            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.AvailableBooks = await _context.Books.CountAsync(b => b.IsAvailable);
            ViewBag.TotalMembers = await _context.Members.CountAsync();
            ViewBag.TotalBorrows = await _context.BorrowTransactions.CountAsync();
            ViewBag.ActiveBorrows = await _context.BorrowTransactions.CountAsync(t => t.Status == "Borrowed" || t.Status == "Overdue");
            ViewBag.OverdueBorrows = await _context.BorrowTransactions.CountAsync(t => t.Status == "Overdue" || (t.Status == "Borrowed" && t.DueDate < now));
            ViewBag.ReturnedBorrows = await _context.BorrowTransactions.CountAsync(t => t.Status == "Returned");
            ViewBag.RecentBorrows = await _context.BorrowTransactions.CountAsync(t => t.BorrowDate >= thirtyDaysAgo);
            ViewBag.TotalReservations = await _context.Reservations.CountAsync();
            ViewBag.ActiveReservations = await _context.Reservations.CountAsync(r => r.Status == "Reserved");
            ViewBag.TotalFines = await _context.Fines.SumAsync(f => (decimal?)f.Amount) ?? 0;
            ViewBag.UnpaidFines = await _context.Fines.Where(f => !f.IsPaid).SumAsync(f => (decimal?)f.Amount) ?? 0;

            ViewBag.OverdueList = await _context.BorrowTransactions
                .Include(t => t.Book)
                .Include(t => t.Member)
                .Include(t => t.Fine)
                .Where(t => t.Status == "Overdue" || (t.Status == "Borrowed" && t.DueDate < now))
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            ViewBag.PopularBooks = await _context.Books
                .Select(b => new { Book = b, BorrowCount = b.BorrowTransactions.Count })
                .OrderByDescending(x => x.BorrowCount)
                .Take(10)
                .ToListAsync();

            ViewBag.ActiveMembers = await _context.Members
                .Select(m => new { Member = m, BorrowCount = m.BorrowTransactions.Count })
                .OrderByDescending(x => x.BorrowCount)
                .Take(10)
                .ToListAsync();

            return View();
        }
    }
}
