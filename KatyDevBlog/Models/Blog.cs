using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KatyDevBlog.Models
{
    //This represents a category of individual Posts or BlogPosts
    //Security in MVC <-- Blog
        //Role based security <-- Posts
        //Front end security  <-- Posts
        //Tag
            //Comment1  <-- Comments 
            //Comment2  <-- Comments 
    public class Blog
    {
        //Non-descriptive administrative property
        public int Id { get; set; }

        //Security in MVC
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        //Add image and image type property
        //Add navigational property to reference all children

        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();


    }
}
