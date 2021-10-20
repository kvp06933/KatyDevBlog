using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KatyDevBlog.Models
{
    public class BlogUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(40, ErrorMessage = "The {0} must be at minimum {2} characters long and maximum {1} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        [StringLength(40, ErrorMessage = "The {0} must be at minimum {2} characters long and maximum {1} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }

        [Display(Name = "Display Name")]
        [StringLength(60, ErrorMessage = "The {0} must be at minimum {2} characters long and maximum {1} characters long.", MinimumLength = 2)]
        public string DisplayName { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
