using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class IndexVM
    {
        public List<BlogPosts> Posts { get; set; }
        public LoginViewModel LoginVM { get; set; }
        public Email Email { get; set; }
    }
}