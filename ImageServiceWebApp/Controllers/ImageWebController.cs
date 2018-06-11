using Communication;
using ImageServiceWebApp.communication;
using ImageServiceWebApp.Models;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ImageServiceWebApp.Controllers
{
    public class ImageWebController : Controller
    {
        #region members
        public static ConfigModel configModel = new ConfigModel();
        public static PhotosModel photosModel = new PhotosModel();
        public static ImageWebModel imageWebModel = new ImageWebModel(photosModel);
        public static LogModel logModel = new LogModel();
        public static bool remove = false;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public ImageWebController()
        {
            ClientCommSingelton.getInstance().DataReceived += GetRemoveMessage;
            imageWebModel.NumberPictuers = photosModel.GetNumberOfPhotos();
        }

        /// <summary>
        /// Return home page.
        /// </summary>
        /// <returns>home page</returns>
        // GET: ImageWeb
        [HttpGet]
        public ActionResult Index()
        {
            if (imageWebModel.NumberPictuers == 0)
            {
                Thread.Sleep(1000);
            }
            return View(imageWebModel);
        }

        /// <summary>
        /// Return config page.
        /// </summary>
        /// <returns>config page</returns>
        [HttpGet]
        public ActionResult Config()
        {
            if (remove)
            {
                Thread.Sleep(1000);
                remove = false;
            }
            return View(configModel);
        }

        /// <summary>
        /// Return logs page.
        /// </summary>
        /// <param name="filterByType">the selected type</param>
        /// <returns>logs page</returns>
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

        /// <summary>
        /// Return photos page.
        /// </summary>
        /// <returns>photos page</returns>
        public ActionResult Photos()
        {
            return View(photosModel);
        }

        /// <summary>
        /// Return RemoveHandler page.
        /// </summary>
        /// <param name="SelectedHandler">the selected handler to remove</param>
        /// <returns>RemoveHandler page</returns>
        public ActionResult RemoveHandler(string SelectedHandler)
        {
            configModel.SelectedHandler = SelectedHandler;
            return View(configModel);
        }

        /// <summary>
        /// Return delete page.
        /// </summary>
        /// <param name="SelectedHandler">the selected handler to remove</param>
        /// <returns>delete page</returns>
        public ActionResult Delete(string SelectedHandler)
        {
            configModel.RemoveHandlerCommand();
            configModel.SelectedHandler = null;
            remove = true;
            return View();
        }

        /// <summary>
        /// This function activated when the client get data from server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataArgs">Data arguments.</param>
        public void GetRemoveMessage(object sender, DataRecivedEventArgs dataArgs)
        {
            if (dataArgs.CommandID == (int)CommandEnum.CloseCommand)
            {
                if (configModel.Handlers.Contains(dataArgs.Args))
                {
                    configModel.Handlers.Remove(dataArgs.Args);
                }
            }
        }

        /// <summary>
        /// Return ShowPhotos page.
        /// </summary>
        /// <param name="infoID">photo's id</param>
        /// <returns>ShowPhotos page</returns>
        public ActionResult ShowPhoto(int infoID)
        {
            foreach (PhotoInfo photo in photosModel.PhotosInfo)
            {
                if (photo.ID == infoID)
                {
                    return View(photo);
                }
            }
            return View();
        }

        /// <summary>
        /// Return RemovePhoto page.
        /// </summary>
        /// <param name="infoID">photo's id</param>
        /// <returns>RemovePhoto page</returns>
        public ActionResult RemovePhoto(int infoID)
        {
            foreach (PhotoInfo photo in photosModel.PhotosInfo)
            {
                if (photo.ID == infoID)
                {
                    return View(photo);
                }
            }
            return View();
        }

        /// <summary>
        /// Remove the photo from directory.
        /// </summary>
        /// <param name="infoID">photo's id</param>
        /// <returns>photos page</returns>
        public ActionResult RemovePhotoFromDirectory(int infoID)
        {
            photosModel.RemovePhoto(infoID);
            return RedirectToAction("Photos");
        }

        /// <summary>
        /// Count how many pictures exsit in the folder
        /// </summary>
        /// <param name="OutputDirPath">the directory</param>
        /// <returns>the number of pictures</returns>
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