using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using System.IO;

namespace Blog.Controllers
{
    [RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Posts.ToList());
        }

        // GET: BlogPosts/Details/5
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var postId = db.Posts.FirstOrDefault(x => x.Slug == Slug).id;

            BlogPosts blogPosts = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPosts == null)
            {
                return HttpNotFound();
            }
            return View(blogPosts);
        }

        // GET: BlogPosts/Create
        [Authorize]
        public ActionResult Create()
        {
            var blogposts = new BlogPosts();
            blogposts.Created = DateTimeOffset.Now;
            return View(blogposts);
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Created,Updated,Title,Slug,Body,MediaURL,SingleBlogImg,Published")] BlogPosts blogPosts, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                //check the file name to make sure its an image
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invalid Format.");
            }
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    //relative server path
                    var filePath = "/images/";
                    // path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative path
                    blogPosts.MediaURL = filePath + image.FileName;
                    blogPosts.SingleBlogImg = filePath + image.FileName;
                    //save image
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }

                var Slug = StringUtilities.URLFriendly(blogPosts.Title);
                if (String.IsNullOrWhiteSpace(Slug))

                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View (blogPosts);
                }

                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View (blogPosts);
                }

                blogPosts.Slug = Slug;
                blogPosts.Created = System.DateTime.Now;
                db.Posts.Add(blogPosts);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPosts);
        }

        // GET: BlogPosts/Edit/5
        [Authorize]
        public ActionResult Edit(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var postId = db.Posts.FirstOrDefault(x => x.Slug == Slug).id;

            BlogPosts blogPosts = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPosts == null)
            {
                return HttpNotFound();
            }
            return View(blogPosts);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Created,Updated,Title,Slug,Body,MediaURL,SingleBlogImg,Published")] BlogPosts blogPosts, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                //check the file name to make sure its an image
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invalid Format.");
            }
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    //relative server path
                    var filePath = "/images/";
                    // path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative path
                    blogPosts.MediaURL = filePath + image.FileName;
                    blogPosts.SingleBlogImg = filePath + image.FileName;
                    //save image
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }

                var Slug = StringUtilities.URLFriendly(blogPosts.Title);
                if (String.IsNullOrWhiteSpace(Slug))

                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPosts);
                }

                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPosts);
                }

                blogPosts.Slug = Slug;
                blogPosts.Updated = System.DateTime.Now;
                db.Entry(blogPosts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPosts);
        }

        // GET: BlogPosts/Delete/5
        [Authorize]
        public ActionResult Delete(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var postId = db.Posts.FirstOrDefault(x => x.Slug == Slug).id;

            BlogPosts blogPosts = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPosts == null)
            {
                return HttpNotFound();
            }
            return View(blogPosts);
        }

        // POST: BlogPosts/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string Slug)
        {
            var postId = db.Posts.FirstOrDefault(x => x.Slug == Slug).id;
            BlogPosts blogPosts = db.Posts.Find(postId);
            db.Posts.Remove(blogPosts);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
