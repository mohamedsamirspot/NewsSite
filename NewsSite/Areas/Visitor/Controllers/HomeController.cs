using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsSite.Data;
using NewsSite.Models;
using NewsSite.Models.ViewModels;
using NewsSite.Utility;

namespace NewsSite.Controllers
{

    [Area("Visitor")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                News = await _db.News.Include(m => m.Category).ToListAsync(), 
                Category = await _db.Category.ToListAsync(),

            };


            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var NewsFromDb = await _db.News.Include(m => m.Category).Where(m => m.Id == id).FirstOrDefaultAsync();
            return View(NewsFromDb);
        }




    }
}
