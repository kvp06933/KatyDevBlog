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
using KatyDevBlog.Enums;

namespace KatyDevBlog.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public BlogPostsController(ApplicationDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<IActionResult> ChildIndex(int blogId)
        {
            var blogPosts = _context.BlogPosts
                .Include(b=>b.Blog)//Navigation property- drag parent reference along with child
                .Where(b => b.BlogId == blogId && b.ReadyStatus == ReadyState.Ready)
                .OrderByDescending(b =>b.Created);

            return View("Index", await blogPosts.ToListAsync());
        }
        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BlogPosts.Include(b => b.Blog);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BlogPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public IActionResult Create(int? id) // Why is this a nullable integer -refer to BlogPost index
        {
            //If i am given an id
            //1. It represents the BlogPost.BlogId
            //2. I dont show the select list to the user
            //3. I embed the incoming id into the form somehow so that it's treated as the BlogId
            if(id is not null)
            {
                BlogPost newBlogPost = new() //new up an instance of type BlogPost with newBlogPost
                {
                    BlogId = (int)id //Foreign key
                };
                return View(newBlogPost);
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,Image,ReadyStatus")] BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                if (blogPost.Image is null)
                {
                    blogPost.ImageData = await _imageService.EncodeImageAsync("newpost.jpg");
                    blogPost.ImageType = "jpg";
                }
                else
                {
                    if (!_imageService.ValidImage(blogPost.Image))
                    {
                        //We need to add a custom Model Error and inform the user
                        ModelState.AddModelError("Image", "Please choose a valid image");
                        return View(blogPost);
                    }
                    else
                    {
                        blogPost.ImageData = await _imageService.EncodeImageAsync(blogPost.Image);
                        blogPost.ImageType = _imageService.ImageType(blogPost.Image);
                    }
                }
                blogPost.Created = DateTime.Now;
                blogPost.Updated = DateTime.Now;
                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", blogPost.BlogId);
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", blogPost.BlogId);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,Created,Updated,Slug,ReadyStatus,ImageType,ImageData")] BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Did the user choose a NEW image
                    if (blogPost.Image is not null)
                    {
                        //If the image fails validation, complain to the user
                        if (!_imageService.ValidImage(blogPost.Image))
                        {
                            ModelState.AddModelError("Image", "There was a problem with the image, please choose another one.");
                            return View(blogPost);
                        }
                        else
                        {
                            blogPost.ImageData = await _imageService.EncodeImageAsync(blogPost.Image);
                            blogPost.ImageType = _imageService.ImageType(blogPost.Image);
                        }
                    }
                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
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
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", blogPost.BlogId);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}
