using KatyDevBlog.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KatyDevBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public string BlogUserId { get; set; }
        public string ModeratorId { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string CommentBody { get; set;}
        public DateTime? Deleted { get; set; }
        public DateTime? Moderated { get; set; }
        public string ModeratedBody { get; set; }
        public ModType ModerationType { get; set; }
        public virtual BlogPost BlogPost { get; set; }
        public virtual BlogUser BlogUser { get; set; }
        public virtual BlogUser Moderator { get; set; }
    }
}
