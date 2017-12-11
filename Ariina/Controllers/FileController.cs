using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.IO;
using Ariina.Extensions;

namespace Ariina.Controllers
{
    public class FileController : Controller
    {
        public ActionResult File(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                return HttpNotFound();

            if (!System.IO.File.Exists(HostingEnvironment.MapPath("~/Media/sintel" + id)))
                return HttpNotFound();

            if(id.Split('.').Last() == "mp4")
                return new VideoResult("Media/sintel/" + id);

            //ToDo: if movie return VideoResult cuz movie needs to be streamed, return error if not found
            var filename = HostingEnvironment.MapPath("~/Media/sintel" + id);

            string contentType = MimeMapping.GetMimeMapping(id);

            return File(filename, contentType, id);
        }

        [Route("File/MediaSegment/{id}/{filename}")]
        public ActionResult MediaSegment(int id, string filename)
        {
            var filepath = HostingEnvironment.MapPath("~/MediaData/Videos/" + id + "/segment" + filename);

            if (!System.IO.File.Exists(filepath))
                return HttpNotFound();

            string contentType = MimeMapping.GetMimeMapping(filepath);

            return File(filepath, contentType, filename);
        }

        public string LogFile()
        {
            if (System.IO.File.Exists(HostingEnvironment.MapPath("~/MediaData/log.txt")))
                return System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/MediaData/log.txt"));
            return "Log file doesn't exist";
        }

        public ActionResult GetFiles(int id)
        {
            var dir = HostingEnvironment.MapPath("~/MediaData/Videos/" + id);
            var segments = HostingEnvironment.MapPath("~/MediaData/Videos/" + id + "/segments");

            if (Directory.Exists(dir))
                ViewBag.Files = Directory.GetFiles(dir);
            else
                ViewBag.Files = new string[] {"No files found."};

            if (Directory.Exists(segments))
                ViewBag.Segments = Directory.GetFiles(segments);
            else
                ViewBag.Segments = new string[] {"No files found."};

            return View();
        }
    }
}