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
    public class BlogService
    {
        private readonly ApplicationDbContext _dbContext;

        public BlogService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Blog>> GetBlogsAsync()
        {
            var blogs = await _dbContext.Blogs.ToListAsync();
            return blogs;
        }

        public async Task<List<BlogPost>> GetBlogPostsAsync()
        {
            var readyPosts = await _dbContext.BlogPosts.Where(b => b.ReadyStatus == ReadyState.Ready).ToListAsync();
            return readyPosts;
        }
    }
}
