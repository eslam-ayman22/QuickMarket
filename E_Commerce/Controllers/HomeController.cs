using E_Commerce.Data;
using E_Commerce.Models;
using E_Commerce.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace E_Commerce.Controllers
{
    [Authorize(Roles ="Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductRepository productRepository;

        public HomeController(ILogger<HomeController> logger , ICategoryRepository categoryRepository , IProductRepository productRepository)
        {
            _logger = logger;
            this.categoryRepository = categoryRepository;
            this.productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var result = categoryRepository.Get(null , e=>e.products);
            
            return View(result);
        }

        //SearchOf Product
        public IActionResult Search(string name)
        {
            var result = productRepository.Get(e => e.Name.Contains(name));
            if(result != null && result.Any())
            {
                return View(result);
            }
            else
            {
                return RedirectToAction ("NotFound");
            }
        }
        
        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult CategoryPerProduct(int id)
        {
            var result = productRepository.Get(e => e.CategoryId == id);
            return View(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
