using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsSite.Data;
using NewsSite.Models;
using NewsSite.Repository.IRepostiory;
using NewsSite.Utility;
using System.Data;
using System.Threading.Tasks;

namespace NewsSite.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Admin)]
    public class CategoryController : Controller
    {

        private readonly ICategoryRepository _dbCategory;

        public CategoryController(ICategoryRepository dbCategory)
        {
            _dbCategory = dbCategory;
        }


        //GET 
        public async Task<IActionResult> Index()
        {
            return View(await _dbCategory.GetAllAsync());
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }


        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                //if valid
                await _dbCategory.CreateAsync(category);
                await _dbCategory.SaveAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _dbCategory.GetAsync(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _dbCategory.UpdateAsync(category);

                await _dbCategory.SaveAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }



        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _dbCategory.GetAsync(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var category = await _dbCategory.GetAsync(u => u.Id == id);

            if (category == null)
            {
                return View();
            }
            await _dbCategory.RemoveAsync(category);
            await _dbCategory.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _dbCategory.GetAsync(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
    }
}
