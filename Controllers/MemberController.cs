using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class MemberController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MemberController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Members.OrderBy(m => m.LastName).ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var member = await _context.Members
                .Include(m => m.BorrowTransactions).ThenInclude(t => t.Book)
                .Include(m => m.Reservations).ThenInclude(r => r.Book)
                .FirstOrDefaultAsync(m => m.MemberID == id);
            if (member == null) return NotFound();
            return View(member);
        }

        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Create()
        {
            return View(new Member { MembershipNumber = NextMembershipNumber(), MemberSince = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Phone,MemberSince,Address,IsActive")] Member member)
        {
            member.MembershipNumber = NextMembershipNumber();
            ModelState.Remove(nameof(Member.MembershipNumber));

            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Member {member.FullName} created with Membership Number {member.MembershipNumber}.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        private string NextMembershipNumber()
        {
            const string prefix = "MEM";
            var maxNum = _context.Members
                .Where(m => m.MembershipNumber != null && m.MembershipNumber.StartsWith(prefix))
                .Select(m => m.MembershipNumber!.Substring(prefix.Length))
                .AsEnumerable()
                .Select(s => int.TryParse(s, out var n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();
            return $"{prefix}{(maxNum + 1).ToString("D4")}";
        }

        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var member = await _context.Members.FindAsync(id);
            if (member == null) return NotFound();
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int id, [Bind("MemberID,MembershipNumber,FirstName,LastName,Email,Phone,MemberSince,Address,IsActive,UserId")] Member member)
        {
            if (id != member.MemberID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Members.Any(m => m.MemberID == id)) return NotFound();
                    throw;
                }
                TempData["Success"] = "Member updated.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var member = await _context.Members.FirstOrDefaultAsync(m => m.MemberID == id);
            if (member == null) return NotFound();
            return View(member);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var userId = _userManager.GetUserId(User);
            var member = await _context.Members
                .Include(m => m.BorrowTransactions).ThenInclude(t => t.Book)
                .Include(m => m.Reservations).ThenInclude(r => r.Book)
                .FirstOrDefaultAsync(m => m.UserId == userId);
            if (member == null) return RedirectToAction("Index", "Home");
            return View("Details", member);
        }
    }
}
