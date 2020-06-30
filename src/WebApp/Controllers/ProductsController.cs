using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class ProductsController : Controller
    {
        [HttpGet("/products/details/{id}")]
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
