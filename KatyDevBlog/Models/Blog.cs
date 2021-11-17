using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KatyDevBlog.Models
{
    //This represents a category of individual Posts or BlogPosts
    
    public class Blog
    {
        //Non-descriptive administrative property
        public int Id { get; set; }

        //Security in MVC
        [Required]
        [StringLength(100, ErrorMessage = "You messed up", MinimumLength = 5)]
        public string Name { get; set; }
        [Required]
        [StringLength(200, ErrorMessage = "Your {0} must be at least {2} characters and at most {1}", MinimumLength = 2)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        //This will be the title run through a formatter
        public string Slug { get; set; }

        //Add image and image type property
        public byte[] ImageData { get; set; }
        public string ImageType { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
        //Add navigational property to reference all children

        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();


    }
}
