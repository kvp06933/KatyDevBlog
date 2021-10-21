using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KatyDevBlog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        public int BlogId { get; set; }

        //Descriptive properties
        public string Title { get; set; }

        //A property to get the user interested without forcing them to read entire post

        public string Abstract { get; set; }

        public string Content { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }


        //Navigational properties
        public virtual Blog Blog { get; set; }
    }
}
