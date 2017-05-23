using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Microsoft.AspNet.Identity;

namespace Blog.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Post);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            var comment = new Comment();
            comment.Created = DateTimeOffset.Now;
            return View(comment);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId,Body")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var slug = db.Posts.FirstOrDefault(x => x.id == comment.PostId).Slug;
                comment.AuthorId = User.Identity.GetUserId();
                comment.Created = DateTime.Now;
                db.Comments.Add(comment);
                db.SaveChanges();

                return RedirectToAction("SingleBlog", "Home", new { Slug = slug });
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            Comment comment = db.Comments.Find(id);

            if (HttpContext.User.Identity.IsAuthenticated && User.IsInRole("Admin") || User.IsInRole("Moderator") || HttpContext.User.Identity.Name.Equals(comment.Author.UserName))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (comment == null)
                {
                    return HttpNotFound();
                }
                ViewBag.PostId = new SelectList(db.Posts, "id", "Title", comment.PostId);
                return View(comment);
            }
            return HttpNotFound();
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PostId,Body,AuthorId,Created,Updated,UpdateReason")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                db.Entry(comment).State = EntityState.Modified;
                comment.Updated = DateTimeOffset.Now;
                db.SaveChanges();
                var slug = db.Posts.FirstOrDefault(x => x.id == comment.PostId).Slug;

                return Redirect(Url.Action(slug, "Index"));
            }

            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comment.PostId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            var slug = db.Posts.FirstOrDefault(x => x.id == comment.PostId).Slug;

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
