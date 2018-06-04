using ImageServiceWebApp.Models;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageServiceWebApp.Controllers
{
    public class ImageWebController : Controller
    {
        public static ConfigModel configModel = new ConfigModel();
        public static ImageWebModel imageWebModel = new ImageWebModel();
        public static LogModel logModel = new LogModel();
        public static PhotosModel photosModel = new PhotosModel();

        // GET: ImageWeb
        [HttpGet]
        public ActionResult Index()
        {
            //imageWebModel.NumberPictuers = CountImages(configModel.OutputDirectory);
            return View(imageWebModel);
        }

        public ActionResult Config()
        {
            return View(configModel);
        }

        public ActionResult Logs(string filterByType)
        {
            ViewData["CurrentFilter"] = filterByType;
            if (!String.IsNullOrEmpty(filterByType))
            {
                IEnumerable<MessageRecievedEventArgs> temp = logModel.Logs.Where(l => l.Status.ToString().Equals(filterByType));
                logModel.Logs = temp.ToList();
            } else
            {
                logModel.Logs = logModel.BackupLogs;
            }
            return View(logModel);
        }

        public ActionResult Photos()
        {
            return View(photosModel);
        }

        public ActionResult RemoveHandler(string SelectedHandler)
        {
            return View(configModel);
        }

        public ActionResult Delete(string SelectedHandler)
        {
            configModel.SelectedHandler = SelectedHandler;
            configModel.RemoveHandlerCommand();
            return RedirectToAction("Config");
        }

        public int CountImages(string OutputDirPath)
        {
            string[] directoryFiles = Directory.GetFiles(OutputDirPath);
            int counter = 0;
            foreach (string filePath in directoryFiles)
            {
                if (Path.GetExtension("FilePath") == "jpg" || Path.GetExtension("FilePath") == "png" ||
                    Path.GetExtension("FilePath") == "gif" || Path.GetExtension("FilePath") == "bmp")
                    counter++;
            }
            return counter;
        }
    }
}