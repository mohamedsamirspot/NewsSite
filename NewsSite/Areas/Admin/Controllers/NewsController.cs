using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using NewsSite.Data;
using NewsSite.Models;
using NewsSite.Models.ViewModels;
using NewsSite.Repository.IRepostiory;
using NewsSite.Utility;

namespace NewsSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Admin)]
    public class NewsController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        [TempData]
        public string StatusMessage { get; set; }
        private int PageSize = 4;

        //private readonly INewsRepository _dbNews;
        //private readonly ICategoryRepository _dbCategory;
        //public NewsController(INewsRepository dbNews, ICategoryRepository dbCategory, IWebHostEnvironment hostingEnvironment)
        //{
        //    _dbNews = dbNews;
        //    _dbCategory = dbCategory;
        //    _hostingEnvironment = hostingEnvironment;
        //}


        private readonly IUnitOfWork _unitOfWork;

        public NewsController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
        }

        //Get INDEX
        public async Task<IActionResult> Index(int productPage = 1, string searchByTitle = null)
        {
            StringBuilder param = new StringBuilder();
            param.Append("/Admin/News/Index?productPage=:");
            param.Append("&searchByTitle=");
            if (searchByTitle != null)
            {
                param.Append(searchByTitle);
            }
            NewsViewModel NewsVM = new NewsViewModel();
            if (searchByTitle != null)
                NewsVM.News = await _unitOfWork.News.GetAllAsync(u => u.Title.ToLower().Contains(searchByTitle.ToLower()), includeProperties: "Category");
            else
                NewsVM.News = await _unitOfWork.News.GetAllAsync(includeProperties: "Category");


            var count = NewsVM.News.Count;
            NewsVM.News = NewsVM.News.OrderByDescending(p => p.Id)
                                 .Skip((productPage - 1) * PageSize)
                                 .Take(PageSize).ToList();
            NewsVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                urlParam = param.ToString()
            };
            return View(NewsVM);
        }

        //GET - CREATE
        public async Task<IActionResult> Create()
        {
            NewsAndCategoryViewModel model = new NewsAndCategoryViewModel()
            {
                CategoryList = await _unitOfWork.Categories.GetAllAsync(),
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
                var doesNewsExists = _unitOfWork.News.GetAllAsyncIQueryable(s => s.Title == model.News.Title && s.Category.Id == model.News.CategoryId);

                if (doesNewsExists.Count()>0)
                {
                    //Error
                    StatusMessage = "Error : News exists under " + doesNewsExists.First().Category.Name + " category. Please use another Title.";
                }
                else
                {
                    await _unitOfWork.News.CreateAsync(model.News);
                    await _unitOfWork.News.SaveAsync();
                    //Work on the image saving section

                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;

                    var NewsFromDb = await _unitOfWork.News.GetAsync(m => m.Id == model.News.Id);

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


                    await _unitOfWork.News.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
            }


            NewsAndCategoryViewModel modelVM = new NewsAndCategoryViewModel()
            {
                CategoryList = await _unitOfWork.Categories.GetAllAsync(),
                News = model.News,
                StatusMessage = StatusMessage
            };

            return View(modelVM);
        }



        //[ActionName("GetNews")]
        //public async Task<IActionResult> GetNews(int id)
        //{
        //    List<News> News = new List<News>();
        //    var newsss = _dbNews.GetAll();

        //    News = (from newss in newsss
        //                  where newss.CategoryId == id
        //                     select newss).ToList();
        //    return Json(new SelectList(News, "Id", "Title"));
        //}


        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var NewsFromDb = await _unitOfWork.News.GetAsync(m => m.Id == id);

            if(NewsFromDb == null)
            {
                return NotFound();
            }

            NewsAndCategoryViewModel model = new NewsAndCategoryViewModel()
            {
                CategoryList = await _unitOfWork.Categories.GetAllAsync(),
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

            var NewsFromDb = await _unitOfWork.News.GetAsync(m => m.Id == NewsVm.News.Id);

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



            await _unitOfWork.News.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET Details
        public async Task<IActionResult> Details(int? id, NewsAndCategoryViewModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            model.News = await _unitOfWork.News.GetAsync(m => m.Id == id, includeProperties: "Category");

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
            model.News = await _unitOfWork.News.GetAsync(m => m.Id == id, includeProperties: "Category");

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
            News News = await _unitOfWork.News.GetAsync(m => m.Id == id);

            if (News != null)
            {
                var imagePath = Path.Combine(webRootPath, News.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
               await _unitOfWork.News.RemoveAsync(News);
                await _unitOfWork.News.SaveAsync();

            }

            return RedirectToAction(nameof(Index));


        }

    }
}