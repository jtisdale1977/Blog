using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Models
{
    public class Comment
    {
        public int id { get; set; }

        public int PostId { get; set; }

        public string AuthorId { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Updated { get; set; }

        public string UpdateReason { get; set; }


        public virtual ApplicationUser Author { get; set; }

        public virtual BlogPosts Post { get; set; }

        public string Slug { get; set; }

    }
}