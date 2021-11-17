using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KatyDevBlog.Data;
using KatyDevBlog.Models;
using KatyDevBlog.Services.Interfaces;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace KatyDevBlog.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context; //Class Member
        private readonly IImageService _imageService;
        private readonly ISlugService _slugService;

        public BlogsController(ApplicationDbContext context, IImageService imageService, ISlugService slugService)
        {
            //Dependency Injection
            _context = context;// constructor
            _imageService = imageService;
            _slugService = slugService;
        }

        // GET: Blogs
        public async Task<IActionResult> Index(int? page)
        {

            //Page might be null... if it is I'll update to be 1
            {
                var pageNumber = page ?? 1;
                var pageSize = 5;

                var blogs = await _context.Blogs.OrderBy(b => b.Created).ToPagedListAsync(pageNumber, pageSize);
                return View(blogs);

            }
        }
                // GET: Blogs/Details/5
                public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        [Authorize (Roles = "Administrator")]
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
                //Let's create and check the slug for uniqueness
                var slug = _slugService.UrlFriendly(blog.Name);
                if (!_slugService.IsUnique(slug))
                {
                    //Create a custom Model Error and complain to the user
                    ModelState.AddModelError("Title", "Error: Title has already been used.");
                    return View(blog);
                }
                else
                {
                    blog.Slug = slug;
                }
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
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
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
