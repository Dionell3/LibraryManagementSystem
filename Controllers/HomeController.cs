using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MemberDashboard()
        {
            ViewBag.MemberName = "Library Member";
            ViewBag.AvailableBooks = 25;
            ViewBag.BorrowedBooks = 3;
            ViewBag.ReservedBooks = 2;
            ViewBag.OverdueBooks = 1;

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
