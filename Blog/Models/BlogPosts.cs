using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Models
{
    public class BlogPosts
    {
        public BlogPosts ()
        {
            this.Comments = new HashSet<Comment>();
        }

        public int id { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Updated { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        [AllowHtml]
        public string Body { get; set; }
        
        public string MediaURL { get; set; }

        public string SingleBlogImg { get; set; }

        public bool Published { get; set; }
        
        public virtual ICollection<Comment> Comments { get; set; }

    }
}