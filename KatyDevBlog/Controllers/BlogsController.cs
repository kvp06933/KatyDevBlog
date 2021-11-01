﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KatyDevBlog.Data;
using KatyDevBlog.Models;
using KatyDevBlog.Services.Interfaces;

namespace KatyDevBlog.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context; //Class Member
        private readonly IImageService _imageService;

        public BlogsController(ApplicationDbContext context, IImageService imageService)
        {
            //Dependency Injection
            _context = context;// constructor
            _imageService = imageService;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blogs.ToListAsync());
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Image")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                if (blog.Image is null)
                {
                    blog.ImageData = await _imageService.EncodeImageAsync("newblog.png");
                    blog.ImageType = "png";
                }
                else
                {
                    if (!_imageService.ValidImage(blog.Image))
                    {
                        //We need to add a custom Model Error and inform the user
                        ModelState.AddModelError("Image", "Please choose a valid image");
                        return View(blog);
                    }
                    else
                    {
                        blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                        blog.ImageType = _imageService.ImageType(blog.Image);
                    }
                }
                //Programmatically add in the Created Date
                blog.Created = DateTime.Now;
                //Adds new blog record to DbSet<Blog>
                _context.Add(blog);
                //This line generates Id
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Created,Image,ImageData,ImageType")] Blog blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Did the user choose a NEW image
                    if(blog.Image is not null)
                    {
                        //If the image fails validation, complain to the user
                        if (!_imageService.ValidImage(blog.Image))
                        {
                            ModelState.AddModelError("Image", "There was a problem with the image, please choose another one.");
                            return View(blog);
                        }
                        else
                        {
                            blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                            blog.ImageType = _imageService.ImageType(blog.Image);
                        }
                    }

                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
