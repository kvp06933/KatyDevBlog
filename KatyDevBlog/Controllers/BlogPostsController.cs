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
using Microsoft.AspNetCore.Authorization;
using KatyDevBlog.Services;
using X.PagedList;

namespace KatyDevBlog.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly ISlugService _slugService;
        private readonly SearchService _searchService;

        public BlogPostsController(ApplicationDbContext context, IImageService imageService, ISlugService slugService, SearchService searchService)
        {
            _context = context;
            _imageService = imageService;
            _slugService = slugService;
            _searchService = searchService;
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchPosts(int? page, string searchTerm)
        {
            //I need to have a good page number and page might come in as null
            var pageNumber = page ?? 1;
            var pageSize = 7;

            //In order to propagate the search term from one page to the next I will
            //use a ViewData to push the term into the view
            ViewData["SearchTerm"] = searchTerm;

            var blogPosts = await _searchService.SearchAsync(searchTerm);
            return View(await blogPosts.ToPagedListAsync(pageNumber, pageSize));
        }

        public async Task<IActionResult> ChildIndex(int blogId)
        {
            //I don't want to get all of the BlogPosts....
            //I want to get all of the BlogPosts where the BlogId = blogId
            //Also...I only want to grab production ready BlogPosts
            var blogPosts = _context.BlogPosts
                .Include(b => b.Blog)
                .Where(b => b.BlogId == blogId && b.ReadyStatus == ReadyState.Ready)
                .OrderByDescending(b => b.Created);

            return View(await blogPosts.ToListAsync());

        }

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BlogPosts
                .Include(b => b.Blog)
                .Where(b => b.ReadyStatus == ReadyState.Ready)
                .OrderByDescending(b => b.Created);

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Preview()
        {
            var blogPosts = _context.BlogPosts
               .Include(b => b.Blog)
               .Where(b => b.ReadyStatus == ReadyState.InPreview)
               .OrderByDescending(b => b.Created);

            return View("Index", await blogPosts.ToListAsync());
        }

        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Blog)
                .Include(b => b.Tags)
                .Include(b => b.Comments)
                .ThenInclude(c => c.BlogUser)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        public async Task<IActionResult> TagIndex(string tag, int? page)
        {
            //Start with my pageing data
            var pageNumber = page ?? 1;
            var pageSize = 6;

            var allBlogPostIds = _context.Tags.Where(t => t.Text.ToLower() == tag.ToLower())
                                              .Select(t => t.BlogPostId);

            var blogPosts = await _context.BlogPosts
                                        .Where(b => allBlogPostIds.Contains(b.Id))
                                        .OrderByDescending(b => b.Created)
                                        .ToPagedListAsync(pageNumber, pageSize);

            ViewData["Tag"] = tag;
            return View(blogPosts);
        }



        // GET: BlogPosts/Create
        //[Authorize(Roles ="Administrator")]
        public IActionResult Create(int? blogId)
        {
            if (blogId is not null)
            {
                BlogPost newBlogPost = new()
                {
                    BlogId = (int)blogId
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
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image")] BlogPost blogPost, List<string> tagValues)
        {
            if (ModelState.IsValid)
            {
                //Let's create and check the slug for uniqueness
                var slug = _slugService.UrlFriendly(blogPost.Title);
                if (!_slugService.IsUnique(slug))
                {
                    //Create a custom Model Error and complain to the user
                    ModelState.AddModelError("Title", "Error: Title has already been used.");
                    return View(blogPost);
                }
                else
                {
                    blogPost.Slug = slug;
                }

                //Either record the incoming image or use the default image
                if (blogPost.Image is not null)
                {
                    if (!_imageService.ValidImage(blogPost.Image))
                    {
                        ModelState.AddModelError("Image", "Please choose a valid image.");
                        return View(blogPost);
                    }
                    else
                    {
                        blogPost.ImageData = await _imageService.EncodeImageAsync(blogPost.Image);
                        blogPost.ImageType = _imageService.ImageType(blogPost.Image);
                    }
                }
                else
                {
                    blogPost.ImageData = await _imageService.EncodeImageAsync("defaultBlogPost.jpg");
                    blogPost.ImageType = "jpg";
                }

                blogPost.Created = DateTime.Now;

                _context.Add(blogPost);
                await _context.SaveChangesAsync();

                foreach (var tag in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        BlogPostId = blogPost.Id,
                        Text = tag
                    });
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", blogPost.BlogId);
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        //[Authorize(Roles = "Administrator")]
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
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", blogPost.BlogId);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,Created,ReadyStatus,Slug,ImageData,ImageType,Image")] BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //In the Edit we have to make sure that the Title actually changed before checking slug uniqueness                    
                    var slug = _slugService.UrlFriendly(blogPost.Title);
                    if (slug != blogPost.Slug)
                    {
                        if (!_slugService.IsUnique(slug))
                        {
                            //Create a custom Model Error and complain to the user
                            ModelState.AddModelError("Title", "Error: Title has already been used.");
                            return View(blogPost);
                        }
                        else
                        {
                            blogPost.Slug = slug;
                        }
                    }

                    blogPost.Updated = DateTime.Now;
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
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", blogPost.BlogId);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        //[Authorize(Roles = "Administrator")]
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
