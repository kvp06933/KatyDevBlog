using KatyDevBlog.Data;
using KatyDevBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KatyDevBlog.Controllers
{
    public class HomeController : Controller

    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _dbContext = context;
        }

        public async Task<IActionResult> Index()
        {
            //Show all blogs
            return View(await _dbContext.Blogs.ToListAsync()); //ToListAsync lets us enumerate through a list ansycronously
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
