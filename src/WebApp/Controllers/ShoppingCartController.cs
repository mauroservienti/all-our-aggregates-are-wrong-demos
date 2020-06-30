using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class ShoppingCartController : Controller
    {
        [HttpGet("/ShoppingCart")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/ShoppingCart/add/{id}")]
        public IActionResult Add(int id)
        {
            return View();
        }
    }
}
