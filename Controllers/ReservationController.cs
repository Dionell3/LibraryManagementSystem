using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            IQueryable<Reservation> query = _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.Member);

            if (User.IsInRole("Member"))
            {
                var userId = _userManager.GetUserId(User);
                var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
                if (member != null)
                    query = query.Where(r => r.MemberID == member.MemberID);
            }

            return View(await query.OrderByDescending(r => r.ReservationDate).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int bookId)
        {
            var userId = _userManager.GetUserId(User);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);

            if (member == null)
            {
                TempData["Error"] = "Member profile not found.";
                return RedirectToAction("Index", "Books");
            }

            var existing = await _context.Reservations
                .AnyAsync(r => r.BookID == bookId && r.MemberID == member.MemberID && r.Status == "Reserved");
            if (existing)
            {
                TempData["Error"] = "You have already reserved this book.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return NotFound();

            _context.Reservations.Add(new Reservation
            {
                BookID = bookId,
                MemberID = member.MemberID,
                ReservationDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(7),
                Status = "Reserved"
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = $"'{book.Title}' has been reserved. We will notify you when it is available.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                reservation.Status = "Cancelled";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reservation cancelled.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Fulfill(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                reservation.Status = "Fulfilled";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reservation marked as fulfilled.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
