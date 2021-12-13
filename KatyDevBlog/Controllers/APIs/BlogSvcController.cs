using KatyDevBlog.Data;
using KatyDevBlog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KatyDevBlog.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogSvcController : ControllerBase
    {

        private readonly ApplicationDbContext _dbContext;

        public BlogSvcController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Returns the most recent x number of Blog Posts
        /// </summary>
        /// <remarks> Jason, 3/29/2021. </remarks>
        /// <param name="num">The number of Blog Posts you want </param>
        /// <returns>List of type Blog Post</returns>

        //Here is the GetTopXPosts Action or Endpoint
        [HttpGet("/GetTopXPosts/{num}")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetTopXPosts(int num)
        {
            //Return the most recent {num} blog posts
            return await _dbContext.BlogPosts.OrderByDescending(p => p.Created).Take(num).ToListAsync();
        }
    }
}
