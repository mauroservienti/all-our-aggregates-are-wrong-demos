using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(int id)
        {
            return View();
        }
    }
}
