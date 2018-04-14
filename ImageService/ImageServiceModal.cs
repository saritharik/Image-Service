using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using ImageService.Logging;
using ImageService.Logging.Modal;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size
        private ILoggingService m_logging;

        private static Regex r = new Regex(":");

        #endregion

         /// <summary>
         /// Constructor.
         /// defines the members according to the app settings file.
         /// </summary>
         /// <param name="logging"> get a logger to update it the actions
        public ImageServiceModal(ILoggingService logging)
        {
            m_logging = logging;
            m_OutputFolder = ConfigurationManager.AppSettings["OutputDir"];
            m_thumbnailSize = int.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);
        }

        /// <summary>
        /// This function called when a new picture is in the folder.
        /// </summary>
        /// <param name="path"> get the path of the file and move it to the right directory.
        /// <param name="result"> out value to update success or fail.
        /// <returns> return the new path of the picture.
        public string AddFile(string path, out bool result)
        {
            result = true;
            String res;
            try
            {
                // create the dest folder
                createFolder();
                m_logging.Log("Folder create successfully", MessageTypeEnum.INFO);
                DateTime dateTime = getDateFromImage(path);
                m_logging.Log("Get date time successfully", MessageTypeEnum.INFO);
                // move the picture to the dest folder
                res = movePicture(path, dateTime);
                m_logging.Log("Picture move successfully", MessageTypeEnum.INFO);
            } catch (Exception e)
            {
                // If an error occurred while moving the image - update the logger
                m_logging.Log("Move picture fail", MessageTypeEnum.FAIL);
                res = e.ToString();
                result = false;
            }
            // return true- success or false- failed
            return res;

        }

        /// <summary>
        /// If the required folder doesn't exist- create new one.
        /// </summary>
        private void createFolder()
        {
            DirectoryInfo di = Directory.CreateDirectory(m_OutputFolder);
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            Directory.CreateDirectory(m_OutputFolder + "\\" + "Thumbnails"); 
        }

        /// <summary>
        /// If the required folder doesn't exist- create new one.
        /// </summary>
        /// <param name="path"> get the path of the folder.
        /// <returns> return the date and time of the picture in the path
        private static DateTime getDateFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }

        }

        private String movePicture(string path, DateTime dateTime)
        {
            int i = 1;
            int year = dateTime.Year;
            int month = dateTime.Month;
            Directory.CreateDirectory(m_OutputFolder + "\\" + year);
            Directory.CreateDirectory(m_OutputFolder + "\\" + year + "\\" + month);
            while (File.Exists(m_OutputFolder + "\\" + year + "\\" + month + "\\" + Path.GetFileName(path))) {
                string newName = Path.GetFileNameWithoutExtension(path);
                string newPath = path.Replace(newName, newName + "(" + i + ")");
                File.Move(path, newPath);
                path = newPath;
                i++;
            }
            string fileName = Path.GetFileName(path);
            File.Move(path, m_OutputFolder + "\\" + year + "\\" + month + "\\" + fileName);
            Directory.CreateDirectory(m_OutputFolder + "\\" + "Thumbnails" + "\\" + year);
            Directory.CreateDirectory(m_OutputFolder + "\\" + "Thumbnails" + "\\" + year + "\\" + month);
            createThumbnails(m_OutputFolder + "\\" + year + "\\" + month + "\\" + fileName,
                m_OutputFolder + "\\" + "Thumbnails" + "\\" + year + "\\" + month + "\\" + fileName);
            return m_OutputFolder + "\\" + year + "\\" + month + "\\" + fileName;
        }

        private void createThumbnails(string fileName, string path)
        {
            Image image = Image.FromFile(fileName);
            Image thumb = image.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);
            thumb.Save(Path.ChangeExtension(path, "thumb"));
        }
    }
}
