using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class SingleBlogVM
    {
        public List<Comment> comment { get; set; }
        public List<BlogPosts> blog { get; set; }
    }
}