using E_Commerce.Data;
using E_Commerce.Models;
using E_Commerce.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            var result = categoryRepository.Get(null);
            return View(result);
        }


        /*Edit Category*/
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var result = categoryRepository.GetOne(e => e.CategoryId == id);
            return View(result);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Category category)
        {
            if(ModelState.IsValid)
            {
                categoryRepository.Update(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

       
    }
}
