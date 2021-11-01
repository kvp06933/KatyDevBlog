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

        public async Task<IActionResult> Index()
        {
            //Show all blogs
            return View(await _dbContext.Blogs.ToListAsync()); //ToListAsync lets us enumerate through a list ansycronously
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
