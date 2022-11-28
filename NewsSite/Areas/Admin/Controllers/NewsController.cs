﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsSite.Data;
using NewsSite.Models;
using NewsSite.Models.ViewModels;
using NewsSite.Utility;

namespace NewsSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Admin)]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        [TempData]
        public string StatusMessage { get; set; }

        public NewsController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;

        }

        //Get INDEX
        public async Task<IActionResult> Index()
        {

            var news = await _db.News.Include(s=>s.Category).ToListAsync();
            return View(news);
        }

        //GET - CREATE
        public async Task<IActionResult> Create()
        {
            NewsAndCategoryViewModel model = new NewsAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                News = new Models.News(),            
            };

            return View(model);
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsAndCategoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var doesNewsExists = _db.News.Include(s => s.Category).Where(s => s.Title == model.News.Title && s.Category.Id == model.News.CategoryId);

                if(doesNewsExists.Count()>0)
                {
                    //Error
                    StatusMessage = "Error : News exists under " + doesNewsExists.First().Category.Name + " category. Please use another Title.";
                }
                else
                {
                    _db.News.Add(model.News);
                    await _db.SaveChangesAsync();
                    //Work on the image saving section

                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;

                    var NewsFromDb = await _db.News.FindAsync(model.News.Id);

                    if (files.Count > 0)
                    {
                        //files has been uploaded
                        var uploads = Path.Combine(webRootPath, "images");
                        var extension = Path.GetExtension(files[0].FileName);

                        using (var filesStream = new FileStream(Path.Combine(uploads, model.News.Id + extension), FileMode.Create))
                        {
                            files[0].CopyTo(filesStream);
                        }
                        NewsFromDb.Image = @"\images\" + model.News.Id + extension;
                    }
                    else
                    {

                        var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultItemImage);
                        System.IO.File.Copy(uploads, webRootPath + @"\images\" + model.News.Id + ".png");
                        NewsFromDb.Image = @"\images\" + model.News.Id + ".png";
                    }


                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }


            NewsAndCategoryViewModel modelVM = new NewsAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                News = model.News,
                StatusMessage = StatusMessage
            };

            return View(modelVM);
        }



        [ActionName("GetNews")]
        public async Task<IActionResult> GetNews(int id)
        {
            List<News> News = new List<News>();


            News = await (from newss in _db.News
                             where newss.CategoryId == id
                             select newss).ToListAsync();
            return Json(new SelectList(News, "Id", "Title"));
        }


        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var NewsFromDb = await _db.News.SingleOrDefaultAsync(m => m.Id == id);

            if(NewsFromDb == null)
            {
                return NotFound();
            }

            NewsAndCategoryViewModel model = new NewsAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                News = NewsFromDb,
            };

            return View(model);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,NewsAndCategoryViewModel NewsVm)
        {
            if (id == null)
            {
                return NotFound();
            }




            if (!ModelState.IsValid)
            {
                return View(NewsVm);
            }

            //Work on the image saving section

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var NewsFromDb = await _db.News.FindAsync(NewsVm.News.Id);

            if (files.Count > 0)
            {
                //New Image has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original file
                var imagePath = Path.Combine(webRootPath, NewsFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //we will upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, NewsVm.News.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                NewsFromDb.Image = @"\images\" + NewsVm.News.Id + extension_new;
            }

            NewsFromDb.Title = NewsVm.News.Title;
            NewsFromDb.Description = NewsVm.News.Description;
            NewsFromDb.NewsDate = NewsVm.News.NewsDate;
            NewsFromDb.CategoryId = NewsVm.News.CategoryId;



            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET Details
        public async Task<IActionResult> Details(int? id, NewsAndCategoryViewModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            model.News = await _db.News.Include(m => m.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (model.News == null)
            {
                return NotFound();
            }

            return View(model);
        }


        //GET Delete
        public async Task<IActionResult> Delete(int? id, NewsAndCategoryViewModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            model.News = await _db.News.Include(m => m.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (model.News == null)
            {
                return NotFound();
            }

            return View(model);
        }

        //POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            News News = await _db.News.FindAsync(id);

            if (News != null)
            {
                var imagePath = Path.Combine(webRootPath, News.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.News.Remove(News);
                await _db.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));


        }

    }
}