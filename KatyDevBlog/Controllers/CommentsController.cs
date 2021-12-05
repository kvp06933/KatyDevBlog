using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KatyDevBlog.Data;
using KatyDevBlog.Models;
using KatyDevBlog.Enums;
using KatyDevBlog.Services.Interfaces;

namespace KatyDevBlog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly ISlugService _slugService;

        public CommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager, ISlugService slugService)
        {
            _context = context;
            _userManager = userManager;
            _slugService = slugService;
        }

        // GET: Comments
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comments.Include(c => c.BlogPost).Include(c => c.BlogUser).Include(c => c.Moderator);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.BlogPost)
                .Include(c => c.BlogUser)
                .Include(c => c.Moderator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogPostId,CommentBody")] Comment comment, string slug)
        {
            if (ModelState.IsValid)
            {
                comment.Created = DateTime.Now;

                //I need an injected instance of UserManager...
                comment.BlogUserId = _userManager.GetUserId(User);

                _context.Add(comment);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));

                //Return the user back to the Details page
                return RedirectToAction("Details", "BlogPosts", new { slug });

            }

            return RedirectToAction("Details", "BlogPosts", new { slug });
        }


        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int commentId, string body, string slug)
        {
            if (commentId == 0)
                return NotFound();

            try
            {
                var comment = await _context.Comments.FindAsync(commentId);
                comment.CommentBody = body;
                comment.Updated = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "BlogPosts", new { slug = slug }, "fragComment");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(commentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Moderate(int commentId, int moderationType, string moderatedBody)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            comment.Moderated = DateTime.Now;
            comment.ModeratorId = _userManager.GetUserId(User);
            comment.ModerationType = (ModType)moderationType;
            comment.ModeratedBody = moderatedBody;

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "BlogPosts", new { id = comment.BlogPostId });
        }


        // GET: Comments/Delete/5
        [Authorize(Roles = "Administrator")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.BlogPost)
                .Include(c => c.BlogUser)
                .Include(c => c.Moderator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}