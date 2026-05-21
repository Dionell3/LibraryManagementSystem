using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class BorrowingParametersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowingParametersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.BorrowingParameters.ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new BorrowingParameters());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Label,LoanDurationDays,RenewalLimit,OverduePenaltyPerDay,MaxBorrowableItems")] BorrowingParameters parameters)
        {
            if (ModelState.IsValid)
            {
                _context.Add(parameters);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Borrowing parameters created.";
                return RedirectToAction(nameof(Index));
            }
            return View(parameters);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var p = await _context.BorrowingParameters.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BorrowingParametersID,Label,LoanDurationDays,RenewalLimit,OverduePenaltyPerDay,MaxBorrowableItems")] BorrowingParameters parameters)
        {
            if (id != parameters.BorrowingParametersID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(parameters);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Borrowing parameters updated.";
                return RedirectToAction(nameof(Index));
            }
            return View(parameters);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var p = await _context.BorrowingParameters.FindAsync(id);
            if (p != null) { _context.BorrowingParameters.Remove(p); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}
