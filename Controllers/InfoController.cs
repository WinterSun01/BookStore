using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About() => View();
        public IActionResult Delivery() => View();
        public IActionResult Returns() => View();
        public IActionResult FAQ() => View();
    }
}