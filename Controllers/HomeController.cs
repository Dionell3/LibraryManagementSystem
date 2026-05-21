using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.AvailableBooks = await _context.Books.CountAsync(b => b.IsAvailable);
            ViewBag.TotalMembers = await _context.Members.CountAsync();
            ViewBag.ActiveBorrows = await _context.BorrowTransactions.CountAsync(b => b.Status == "Borrowed" || b.Status == "Overdue");
            ViewBag.NewArrivals = await _context.Books.OrderByDescending(b => b.BookID).Take(4).ToListAsync();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> MemberDashboard()
        {
            var userId = _userManager.GetUserId(User);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);

            if (member == null)
                return RedirectToAction("Index");

            var now = DateTime.Now;

            var borrows = await _context.BorrowTransactions
                .Include(t => t.Book)
                .Include(t => t.Fine)
                .Where(t => t.MemberId == member.MemberID)
                .OrderByDescending(t => t.BorrowDate)
                .ToListAsync();

            var reservations = await _context.Reservations
                .Include(r => r.Book)
                .Where(r => r.MemberID == member.MemberID && r.Status == "Reserved")
                .ToListAsync();

            ViewBag.MemberName = member.FullName;
            ViewBag.MemberID = member.MemberID;
            ViewBag.AvailableBooks = await _context.Books.CountAsync(b => b.IsAvailable);
            ViewBag.BorrowedBooks = borrows.Count(b => b.Status == "Borrowed" || b.Status == "Overdue");
            ViewBag.ReservedBooks = reservations.Count;
            ViewBag.OverdueBooks = borrows.Count(b => (b.Status == "Borrowed" || b.Status == "Overdue") && b.DueDate < now);
            ViewBag.BorrowHistory = borrows.Take(5).ToList();
            ViewBag.Reservations = reservations;
            ViewBag.RecommendedBooks = await _context.Books.Where(b => b.IsAvailable).Take(4).ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
