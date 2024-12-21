using E_Commerce.Data;
using E_Commerce.Models;
using E_Commerce.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IFavouriteRepository favouriteRepository;

        public UserManager<ApplicationUser> userManager;

        public ProductController(IProductRepository productRepository ,ICategoryRepository categoryRepository , UserManager<ApplicationUser> userManager , IFavouriteRepository favouriteRepository)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.userManager = userManager;
            this.favouriteRepository = favouriteRepository;
        }

        public IActionResult Index(int id)
        {
            var result = productRepository.Get(e => e.CategoryId == id);
            return View(result);
        }

        public IActionResult AllProduct()
        {
            var result = productRepository.Get(null , e=>e.category);
            return View(result);
        }
        public IActionResult Detals(int id)
        {
            var result = productRepository.GetOne(e => e.ProductId == id);
            return View(result);
        }

      

        //Add To Favourite 
        public IActionResult favouriteProduct(int id)
        {
            var userId = userManager.GetUserId(User);
            if (id != 0)
            {
                Favourite favourite = new()
                {
                    ProductId = id,
                    ApplicationUserId = userId
                };
                favouriteRepository.Add(favourite);
                return RedirectToAction("AllProduct");
            }
           var result =  favouriteRepository.Get(e => e.ApplicationUserId == userId , e=>e.product);
            return View(result);
        }

        //remove from favourite

        public IActionResult Remove(int id)
        {
            var userId = userManager.GetUserId(User);
            var result = favouriteRepository.GetOne(e => e.ProductId == id  );
            if(result != null)
            {
                favouriteRepository.Delete(result);
                return RedirectToAction("favouriteProduct");
            }
            return NotFound();
        }
    }
}
