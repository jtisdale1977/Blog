using Blog.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {

            var myIndexVM = new IndexVM();
            myIndexVM.Posts = db.Posts.ToList();
            myIndexVM.LoginVM = new LoginViewModel();

            return View(myIndexVM);
        }

        [HttpGet]
        public ActionResult Search(string searchStr, int? page)
        {
            ViewBag.ReturnUrl = Request.Url.LocalPath;
            var blogResults = db.Posts.AsQueryable();
            ViewBag.search = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                //This is the most efficient way to search.
                blogResults = db.Posts.Where(p => p.Body.Contains(searchStr) || p.Title.Contains(searchStr)
                                || p.Slug.Contains(searchStr) || p.Comments.Any(c => c.Body.Contains(searchStr)
                                || c.Author.DisplayName.Contains(searchStr) || c.Author.FirstName.Contains(searchStr)
                                || c.Author.LastName.Contains(searchStr)));

                int pageSize = 2;
                int pageNumber = (page ?? 1);


                return View(blogResults.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
            }
            else
            {
                int pageSize = 2; // display two blog posts at a time on this page
                int pageNumber = (page ?? 1);
                return View(blogResults.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
            }
        }

        //[HttpPost]
        public ActionResult SingleBlog(string Slug)
        {
            var myBlog = db.Posts.FirstOrDefault(x => x.Slug == Slug);
            var myComment = db.Comments.Where(x => x.Slug == Slug).ToList();
            return View(myBlog);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            Email model = new Email();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(Email model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message: </p><p>{2}</p>";
                    var from = "MyPortfolio<jtisdale1977@gmail.com>";
                    model.Body = "This is a message from your blog site. The name and the email of the contacting person is above.";
                    var email = new MailMessage(from,
                        ConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = "Portfolio Contact Email",
                        Body = String.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }

        public FileContentResult UserPhotos()
        {
            if (User.Identity.IsAuthenticated)
            {
                //let pass User.Identity into userId
                String userId = User.Identity.GetUserId();

                if (userId == null)
                {
                    //if there is no photo chosen then use Stock photo- I am using CoderFoundry image
                    string fileName = HttpContext.Server.MapPath(@"~/images/coderfoundry.png");
                    //convert import image into byte file that can read by using FileStream and BinaryReader Method
                    byte[] imageData = null;
                    FileInfo fileInfo = new FileInfo(fileName);
                    long imageFileLength = fileInfo.Length;
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    imageData = br.ReadBytes((int)imageFileLength);

                    return File(imageData, "image/png");

                }
                // to get the user details to load user Image 
                var bdUsers = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
                var UserImage = bdUsers.Users.Where(photo => photo.Id == userId).FirstOrDefault();

                return new FileContentResult(UserImage.UserPhoto, "image/jpeg");
            }
            else
            {
                string fileName = HttpContext.Server.MapPath(@"~/images/coderfoundry.png");

                byte[] imageData = null;
                FileInfo fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageData = br.ReadBytes((int)imageFileLength);
                return File(imageData, "image/png");

            }
        }

    }
}