using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BooksController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Books
        public async Task<IActionResult> Index(string? searchString)
        {
            var books = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title!.Contains(searchString) || b.Author!.Contains(searchString));
            }

            ViewBag.SearchString = searchString;
            return View(await books.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.BookID == id);
            if (book == null) return NotFound();

            ViewBag.Feedbacks = await _context.Feedbacks
                .Include(f => f.Member)
                .Where(f => f.BookID == id && f.IsApproved)
                .OrderByDescending(f => f.SubmittedDate)
                .ToListAsync();

            return View(book);
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Create(
            [Bind("BookID,Title,Author,Genre,ISBN,IsAvailable,Summary")] Book book,
            IFormFile? coverImage)
        {
            if (ModelState.IsValid)
            {
                if (coverImage != null && coverImage.Length > 0)
                {
                    book.CoverImagePath = await SaveCoverImage(coverImage);
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int id,
            [Bind("BookID,Title,Author,Genre,ISBN,IsAvailable,Summary,CoverImagePath")] Book book,
            IFormFile? coverImage)
        {
            if (id != book.BookID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (coverImage != null && coverImage.Length > 0)
                    {
                        book.CoverImagePath = await SaveCoverImage(coverImage);
                    }

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.BookID == id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookID == id);
        }

        private async Task<string> SaveCoverImage(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "/uploads/" + fileName;
        }
    }
}
