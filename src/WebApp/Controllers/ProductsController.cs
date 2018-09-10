using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
