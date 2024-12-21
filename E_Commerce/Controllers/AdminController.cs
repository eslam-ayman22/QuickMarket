using E_Commerce.Models;
using E_Commerce.Repository;
using E_Commerce.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Commerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public ICategoryRepository CategoryRepository;
        private readonly IProductRepository productRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOrderRepository orderRepository;

        public AdminController(ICategoryRepository categoryRepository ,
            IOrderRepository orderRepository,IProductRepository productRepository , UserManager<ApplicationUser> userManager)
        {
            CategoryRepository = categoryRepository;
            this.productRepository = productRepository;
            this.userManager = userManager;
            this.orderRepository = orderRepository;
        }
        public IActionResult Index()
        {
            var productCount = productRepository.Get(null).Count();
            var categoryCount = CategoryRepository.Get(null).Count();
            var OrderCount = orderRepository.Get(null).Count();
            var userCount = userManager.Users.Count();

            ViewBag.ProductCount = productCount;
            ViewBag.CategoryCount = categoryCount;
            ViewBag.OrderCount = OrderCount;
            ViewBag.UserCount = userCount;

            var AllCategory = CategoryRepository.Get(null);
            return View(AllCategory);
        }
        ///////////////////////Manage Category
        public IActionResult AllCategory()
        {
            var AllCategory = CategoryRepository.Get(null);
            return View(AllCategory);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                CategoryRepository.Add(category);

                TempData["stats"] = "Category Add Successfuly";
                return RedirectToAction("Index");
            }
            return View(category);

        }
        public IActionResult Delete(int id)
        {
            var result = CategoryRepository.GetOne(e => e.CategoryId == id);
            if (result != null)
            {
                CategoryRepository.Delete(result);
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        /////////////////////////Manage Product
        public IActionResult AllProduct()
        {
            var result = productRepository.Get(null, e => e.category);
            return View(result);
        }

        public IActionResult CreateProduct()
        {
            var result = CategoryRepository.Get(null).Select(e => new SelectListItem
            {
                Value = e.CategoryId.ToString(),
                Text = e.Name
            }).ToList();

            ViewData["CategoryList"] = result;
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(Product product)
        {


            if (ModelState.IsValid)
            {
                productRepository.Add(product);
                TempData["stats"] = "Product Added Successfuly";
                return RedirectToAction("AllProduct");

            }

            return View(product);
        }

        public IActionResult EditProduct(int id)
        {
            var listofcategory = CategoryRepository.Get(null).Select(e => new SelectListItem
            {
                Value = e.CategoryId.ToString(),
                Text = e.Name

            }).ToList();
            ViewData["categorylist"] = listofcategory;

            var product = productRepository.GetOne(e => e.ProductId == id);
            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                productRepository.Update(product);
                
                return RedirectToAction("Index");
            }
            return View(product);
        }


        //Delete Product
        public IActionResult DeleteProduct(int id)
        {
            var result = productRepository.GetOne(e => e.ProductId == id);
            if(result != null)
            {
                productRepository.Delete(result);
                return RedirectToAction("AllProduct");
            }
            return NotFound();
        }

        // search Category
        public IActionResult Search(string name)
        {
            var product = CategoryRepository.GetOne(e => e.Name.Contains(name));
            if (product != null)
            {
                return View(product);
            }
            else
            {
                return NotFound();
            }
        }


    }
}
