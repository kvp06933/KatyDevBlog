using KatyDevBlog.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KatyDevBlog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        public int BlogId { get; set; }

        //Descriptive properties
        [Required]
        [StringLength(100, ErrorMessage = "Your {0} must be at least {2} characters and at most {1}", MinimumLength = 2)]
        public string Title { get; set; }

        //A property to get the user interested without forcing them to read entire post
        [Required]
        [StringLength(200, ErrorMessage = "Your {0} must be at least {2} characters and at most {1}", MinimumLength = 2)]
        public string Abstract { get; set; }

        [Required]
        public string Content { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        //Add a ProductionReady bool
        //public bool ProductionReady { get; set; }
        [Required]
        [Display(Name = "Ready Status")]
        public ReadyState ReadyStatus { get; set; }

        //This will be the title run through a formatter
        public string Slug { get; set; }

        //Image fields - IFormFile, Image Data, Image Type

        public byte[] ImageData { get; set; }
        public string ImageType { get; set; }
        [NotMapped]
        public IFormFile Image {get;set;}
        //Navigational properties
        //parent
        public virtual Blog Blog { get; set; }
        //Children
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
