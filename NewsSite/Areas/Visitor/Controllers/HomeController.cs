using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsSite.Data;
using NewsSite.Models;
using NewsSite.Models.ViewModels;
using NewsSite.Repository.IRepostiory;
using NewsSite.Utility;

namespace NewsSite.Controllers
{

    [Area("Visitor")]
    public class HomeController : Controller
    {

        private readonly INewsRepository _dbNews;
        private readonly ICategoryRepository _dbCategory;

        public HomeController(INewsRepository dbNews,ICategoryRepository dbCategory)
        {
            _dbNews = dbNews;
            _dbCategory = dbCategory;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                News = await _dbNews.GetAllAsync(includeProperties: "Category"), 
                Category = await _dbCategory.GetAllAsync(),

            };


            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var NewsFromDb = await _dbNews.GetAsync(m => m.Id == id, includeProperties: "Category");
            return View(NewsFromDb);
        }




    }
}
