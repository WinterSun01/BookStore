using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}