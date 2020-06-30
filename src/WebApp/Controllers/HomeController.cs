using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("home/error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
