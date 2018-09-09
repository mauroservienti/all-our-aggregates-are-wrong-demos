using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
