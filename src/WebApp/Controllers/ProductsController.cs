using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("Products")]
    public class ProductsController : Controller
    {
        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
