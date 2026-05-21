using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbackController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            IQueryable<Feedback> query = _context.Feedbacks
                .Include(f => f.Book)
                .Include(f => f.Member);

            if (User.IsInRole("Member"))
            {
                var userId = _userManager.GetUserId(User);
                var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
                if (member != null)
                    query = query.Where(f => f.MemberID == member.MemberID);
            }

            return View(await query.OrderByDescending(f => f.SubmittedDate).ToListAsync());
        }

        public async Task<IActionResult> Create(int? bookId)
        {
            ViewBag.Books = await _context.Books.OrderBy(b => b.Title).ToListAsync();
            ViewBag.SelectedBookId = bookId;
            return View(new Feedback { BookID = bookId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookID,Rating,Comment")] Feedback feedback)
        {
            var userId = _userManager.GetUserId(User);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);

            if (member == null)
            {
                TempData["Error"] = "Member profile not found.";
                return RedirectToAction("Index", "Books");
            }

            if (await _context.Feedbacks.AnyAsync(f => f.BookID == feedback.BookID && f.MemberID == member.MemberID))
                ModelState.AddModelError("", "You have already submitted feedback for this book.");

            if (ModelState.IsValid)
            {
                feedback.MemberID = member.MemberID;
                feedback.SubmittedDate = DateTime.Now;
                feedback.IsApproved = false;
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Your feedback has been submitted and is pending approval.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Books = await _context.Books.OrderBy(b => b.Title).ToListAsync();
            return View(feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Approve(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null) { feedback.IsApproved = true; await _context.SaveChangesAsync(); TempData["Success"] = "Feedback approved."; }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Delete(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null) { _context.Feedbacks.Remove(feedback); await _context.SaveChangesAsync(); TempData["Success"] = "Feedback deleted."; }
            return RedirectToAction(nameof(Index));
        }
    }
}
