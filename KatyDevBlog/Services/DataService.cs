using KatyDevBlog.Data;
using KatyDevBlog.Models;
using KatyDevBlog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace KatyDevBlog.Services
{
    //Responsibe for making sure thee is at least one user and some data in the database
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ISlugService _slugService;
        public DataService(ApplicationDbContext dbContext, IImageService imageService, UserManager<BlogUser> userManager, RoleManager<IdentityRole> roleManager, ISlugService slugService)
        {
            _dbContext = dbContext;
            _imageService = imageService;
            _userManager = userManager;
            _roleManager = roleManager;
            _slugService = slugService;
        }

        //I'm free to use _dbContext anywhere inside the class...
        public async Task ManageDataAsync()
        {
            await _dbContext.Database.MigrateAsync();
            //Create 2 Roles
            await SeedRolesAsync();
            //Create(seed) 2 roles and assign the roles
            await SeedUsersAsync();

            await SeedBlogsAsync();

            await SeedBlogPostsAsync();


        }

        private async Task SeedUsersAsync()
        {
            //Ask if there are any Users at all already in the AspNet users table
            if (_dbContext.Users.Any()) return;
            BlogUser admin = new()
            {
                Email = "KatherinePitts@mailinator.com",
                UserName = "KatherinePitts@mailinator.com",
                FirstName = "Katherine",
                LastName = "Pitts",
                PhoneNumber = "555-1212",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync("defaultUser.png"),
                ImageType = "png"
            };
            await _userManager.CreateAsync(admin, "Abc&123!");
            await _userManager.AddToRoleAsync(admin, "Administrator");

            //Todo: Now seed a user who will occupy the moderator role.
            BlogUser moderator = new()
            {
                Email = "PeterParker@mailinator.com",
                UserName = "PeterParker@mailinator.com",
                FirstName = "Peter",
                LastName = "Parker",
                PhoneNumber = "555-3434",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync("defaultUser.png"),
                ImageType = "png"
            };
            await _userManager.CreateAsync(moderator, "Abc&123!");
            await _userManager.AddToRoleAsync(moderator, "Moderator");
            BlogUser moderator2 = new()
            {
                Email = "DrewRussell@mailinator.com",
                UserName = "DrewRussell@mailinator.com",
                FirstName = "Drew",
                LastName = "Russell",
                PhoneNumber = "555-2424",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync("defaultUser.png"),
                ImageType = "png"
            };
            await _userManager.CreateAsync(moderator2, "Abc&123!");
            await _userManager.AddToRoleAsync(moderator2, "Moderator");
        }
        private async Task SeedRolesAsync()
        {
            //Ask if there are any roles in the roles table
            if (_dbContext.Roles.Any()) return;
            IdentityRole adminRole = new("Administrator");
            await _roleManager.CreateAsync(adminRole);
            var moderatorRole = new IdentityRole("Moderator");
            await _roleManager.CreateAsync(moderatorRole);
        }

        private async Task SeedBlogsAsync()
        {
            if (_dbContext.Blogs.Any())
                return;
            for (var blogs = 1; blogs <= 20; blogs++)
            {
                _dbContext.Add(new Blog()
                {
                    Name = $"Blog for Application {blogs}",
                    Description = $"Everything I learned while building application {blogs}",
                    Created = DateTime.Now.AddDays(blogs),
                    ImageData = await _imageService.EncodeImageAsync("newblog.png"),
                    ImageType = "png",

                });
            }
            await _dbContext.SaveChangesAsync();

        }
        private async Task SeedBlogPostsAsync()
        {
            if (_dbContext.BlogPosts.Any())
                return;

            var blogId = (await _dbContext.Blogs.AsNoTracking().OrderBy(b => b.Created).FirstOrDefaultAsync()).Id;
            for (var loop = 1; loop <= 20; loop++)
            {
                var title = $"Post Number {loop}";
                _dbContext.Add(new BlogPost()
                {
                    BlogId = blogId,
                    Title = title,
                    ReadyStatus = Enums.ReadyState.Ready,
                    Slug = _slugService.UrlFriendly(title),
                    Abstract = $"Abstract for Post Number {loop}",
                    Content = $"Content of Post Number {loop}",
                    Created = DateTime.Now.AddDays(-loop),
                    ImageData = await _imageService.EncodeImageAsync("newblog.png"),
                    ImageType = "png",

                });
            }
            await _dbContext.SaveChangesAsync();
        }


    }
}