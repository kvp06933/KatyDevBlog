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

        public DataService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //I'm free to use _dbContext anywhere inside the class...
        public async Task ManageDataAsync()
        {
            await _dbContext.Database.MigrateAsync();    
        }
            
    }
}