using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class checkoutController : Controller
    {
        public IActionResult success()
        {
            return View();
        }

        public IActionResult cancel()
        {
            return View();
        }
    }
}
