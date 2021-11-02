using KatyDevBlog.Data;
using KatyDevBlog.Enums;
using KatyDevBlog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KatyDevBlog.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _dbContext;

        public SearchService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //return either an IQueriable or List for all posts that contain incomeing search string

        public async Task<List<BlogPost>> SearchAsync(string searchTerm)
        {

            List<BlogPost> posts = new();
            //What happens when user trys searching an empty term
            if (!string.IsNullOrEmpty(searchTerm))
                
            {
                searchTerm = searchTerm.ToLower();
                posts = await _dbContext.BlogPosts
                    .Include(b => b.Blog)
                    .Include(b => b.Comments)
                    .ThenInclude(c => c.BlogUser)
                    .Where(b => b.ReadyStatus == ReadyState.Ready)
                    .ToListAsync();

                posts = posts.Where(p => p.Title.ToLower().Contains(searchTerm) ||
                                         p.Abstract.ToLower().Contains(searchTerm) ||
                                         p.Content.ToLower().Contains(searchTerm) ||
                                         p.Blog.Name.ToLower().Contains(searchTerm) ||
                                         p.Comments.Any(c => c.CommentBody.ToLower().Contains(searchTerm) ||
                                                             c.BlogUser.FullName.ToLower().Contains(searchTerm)
                                                             )).ToList();
            }
            return posts;
                
        }

    }
}
