using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class LibrarianController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibrarianController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Librarian/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var now = DateTime.Now;
            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.AvailableBooks = await _context.Books.CountAsync(b => b.IsAvailable);
            ViewBag.TotalMembers = await _context.Members.CountAsync();
            ViewBag.ActiveBorrows = await _context.BorrowTransactions.CountAsync(t => t.Status == "Borrowed" || t.Status == "Overdue");
            ViewBag.OverdueBorrows = await _context.BorrowTransactions.CountAsync(t => t.Status == "Overdue" || (t.Status == "Borrowed" && t.DueDate < now));
            ViewBag.PendingFeedback = await _context.Feedbacks.CountAsync(f => !f.IsApproved);
            return View();
        }

        // GET: Librarian
        public async Task<IActionResult> Index()
        {
            return View(await _context.Librarians.ToListAsync());
        }

        // GET: Librarian/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var librarian = await _context.Librarians
                .FirstOrDefaultAsync(m => m.LibrarianID == id);
            if (librarian == null) return NotFound();

            return View(librarian);
        }

        // GET: Librarian/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Librarian/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LibrarianID,EmployeeID,FirstName,LastName,Email,Phone,Position,HireDate")] Librarian librarian)
        {
            if (ModelState.IsValid)
            {
                _context.Add(librarian);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(librarian);
        }

        // GET: Librarian/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian == null) return NotFound();
            return View(librarian);
        }

        // POST: Librarian/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LibrarianID,EmployeeID,FirstName,LastName,Email,Phone,Position,HireDate")] Librarian librarian)
        {
            if (id != librarian.LibrarianID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(librarian);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibrarianExists(librarian.LibrarianID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(librarian);
        }

        // GET: Librarian/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var librarian = await _context.Librarians
                .FirstOrDefaultAsync(m => m.LibrarianID == id);
            if (librarian == null) return NotFound();

            return View(librarian);
        }

        // POST: Librarian/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian != null)
            {
                _context.Librarians.Remove(librarian);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LibrarianExists(int id)
        {
            return _context.Librarians.Any(e => e.LibrarianID == id);
        }
    }
}