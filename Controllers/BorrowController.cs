using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class BorrowController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var transactions = _context.BorrowTransactions.ToList();
            return View(transactions);
        }

        public IActionResult BorrowBook(int bookId)
        {
            var transaction = new BorrowTransaction
            {
                BookId = bookId,
                MemberId = 1,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                Status = "Borrowed"
            };

            _context.BorrowTransactions.Add(transaction);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult ReturnBook(int id)
        {
            var transaction = _context.BorrowTransactions.Find(id);

            if (transaction != null)
            {
                transaction.ReturnDate = DateTime.Now;
                transaction.Status = "Returned";

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}