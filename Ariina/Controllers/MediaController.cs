using Ariina.Extensions;
using Ariina.Models;
using Ariina.Services;
using Ariina.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;



namespace Ariina.Controllers
{
    public class MediaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Video()
        {
            return new VideoResult("Media/sintel/vid7.mp4");
        }

        public ActionResult Videos()
        {
            return View();
        }

        public ActionResult Movie()
        {
            return View();
        }

        public ActionResult Mp4()
        {
            return View();
        }

        // GET: Media
        public ActionResult Index()
        {
            var media = db.Media;

            // AppHarbor deletes files on every deploy, clean the remaining records
            foreach (var m in media)
            {
                var baseDir = HostingEnvironment.MapPath("~/MediaData/Videos/" + m.Id);
                if (!Directory.Exists(baseDir))
                {
                    Media mediaFile = db.Media.Find(m.Id);
                    db.Media.Remove(mediaFile);
                }
            }
            db.SaveChanges();
            return View(media.Where(file => !file.IsBeingConverted).ToList());
        }

        public ActionResult Category(string id)
        {
            if (id == null)
                return HttpNotFound();

            var data = db.Media.Where(m => m.Title.Contains(id));
            return View("Index",data.ToList());
        }

        [Route("Media/{id:int}")]
        public ActionResult Display(int id)
        {
            Media mediaFile = db.Media.Find(id);
            if (mediaFile == null || mediaFile.IsBeingConverted)
                return HttpNotFound();

            var videoParams = ServerParams.VideoParams.GetVideoParams(mediaFile.VideoQuality);

            ViewBag.MediaId = id;
            ViewBag.Title = mediaFile.Title;
            ViewBag.Description = mediaFile.Description;
            ViewBag.IsHd = mediaFile.IsHd();
            ViewBag.videoParams = videoParams;
            return View();
        }

        // GET: Media/Details/5    
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Media mediaFile = db.Media.Find(id);
            if (mediaFile == null)
            {
                return HttpNotFound();
            }
            return View(mediaFile);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Upload()
        {
            ViewBag.Category = new SelectList(ServerParams.CategoriesList.List);
            return View();
        }

        // Called asynchronously
        // Return 200: success with redirect link response
        // Return 201: error with validation message
        [HttpPost]
        public ActionResult Upload([Bind(Include = "Title, Description, IsPrivate, Category")] Media mediaFile,
                                    HttpPostedFileBase file)
        {
            var isFileValid = (file != null && file.ContentLength > 0);

            mediaFile.ApplicationUserId = User.Identity.GetUserId();

            if (!ModelState.IsValid || !isFileValid || !ServerParams.CategoriesList.List.Contains(mediaFile.Category))
            {
                var ms = ModelState;
                Response.StatusCode = 400;
                var errors = ModelErrorsToDictionary(ModelState);
                if (!isFileValid)
                    errors.Add("File","File not supported.");

                return Json(errors);
            }

            mediaFile.IsBeingConverted = true;
            db.Media.Add(mediaFile);
            db.SaveChanges();

            var dir = Server.MapPath("~/MediaData/Videos/" + mediaFile.Id);

            // todo: check if dir exists
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(dir, fileName);
            file.SaveAs(path);

            MediaService.ConvertVideo(fileName, mediaFile.Id);

            ViewBag.Info = "Your video was successfully uploaded";
            Response.StatusCode = 200;
            return Json("Success");
        }

        private Dictionary<string, string> ModelErrorsToDictionary(ModelStateDictionary ModelState)
        {
            var errorsDictionary = new Dictionary<string, string>();
            if (!ModelState.IsValid)
            {
                StringBuilder errors;
                foreach (KeyValuePair<string, ModelState> state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        errors = new StringBuilder();
                        foreach (ModelError err in state.Value.Errors)
                        {
                            errors.AppendLine(err.ErrorMessage);
                        }
                        errorsDictionary.Add(state.Key, errors.ToString());
                    }
                }
            }
            return errorsDictionary;
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, UserId, Title, AssetId, IsPublic")] Media mediaFile)
        {
            if (ModelState.IsValid)
            {
                db.Media.Add(mediaFile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mediaFile);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Media mediaFile = db.Media.Find(id);
            if (mediaFile == null)
            {
                return HttpNotFound();
            }
            return View(mediaFile);
        }

        // POST: Media/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to
        // for more details see http://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, UserId, Title, AssetId, IsPublic")] Media mediaFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mediaFile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mediaFile);
        }

        // GET: Media/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Media mediaFile = db.Media.Find(id);
            if (mediaFile == null)
            {
                return HttpNotFound();
            }
            return View(mediaFile);
        }

        // POST: Media/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Media mediaFile = db.Media.Find(id);
            db.Media.Remove(mediaFile);
            db.SaveChanges();

            var dir = Server.MapPath("~/MediaData/Videos" + id);

            //todo
            if(Directory.Exists(dir))
                Directory.Delete(dir, true);

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

        public ActionResult Prog(int id)
        {
            ViewBag.Id = id;
            return View("Progress");
        }

        [HttpPost]
        public ActionResult Progress(List<int> id)
        {
            if (id == null || id.Count == 0)
                return null;

            Dictionary<int, double> objects = new Dictionary<int, double>();
            foreach (int i in id)
            {
                if (HttpContext.Cache[i.ToString()] != null)
                    objects[i] = Math.Ceiling((double) HttpContext.Cache[i.ToString()] * 100);
                else
                    objects[i] = 0.0;
            }

            var json = JsonConvert.SerializeObject(objects);
            return Content(json, "application/json");
        }
    }
}