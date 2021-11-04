using KatyDevBlog.Data;
using KatyDevBlog.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace KatyDevBlog.Controllers
{
    public class HomeController : Controller

    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IEmailSender emailService)
        {
            _logger = logger;
            _dbContext = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index(int? page)
        {

            //Page might be null... if it is I'll update to be 1
            {
                var pageNumber = page ?? 1;
                var pageSize = 5;

                var blogs = await _dbContext.Blogs.OrderBy(b => b.Created).ToPagedListAsync(pageNumber, pageSize);
                return View(blogs);
            }

            
            
        }
        public IActionResult ContactMe()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe(string name, string email, string phone, string message)
        {
            var subject = $"{name} has reached out to you from KatyDev Blog.";
            //TODO: Add code to call the Email Service

            var body = $"{message}.<br/><br/>{name} can be reached at {phone} if follow up is required.";
            await _emailService.SendEmailAsync(email, subject, body);
            return RedirectToAction(nameof(Index));
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
