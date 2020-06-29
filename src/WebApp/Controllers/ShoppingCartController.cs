using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("ShoppingCart")]
    public class ShoppingCartController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/add/{id}")]
        public IActionResult Add(int id)
        {
            return View();
        }
    }
}
