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

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size

        private static Regex r = new Regex(":");

        #endregion

        public string AddFile(string path, out bool result)
        {
            result = true; ///////////////
            String res;
            try
            {
                createFolder();
                DateTime dateTime = getDateFromImage(path);
                res = movePicture(path, dateTime);
            } catch (Exception e)
            {
                res = e.ToString();
            }

            return res;

        }

        private void createFolder()
        {
            if (!Directory.Exists(m_OutputFolder))
            {
                Directory.CreateDirectory(m_OutputFolder);
                Directory.CreateDirectory(m_OutputFolder + "\\" + "Thumbnails");
            }
        }

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
            int year = dateTime.Year;
            int month = dateTime.Month;
            string fileName = Path.GetFileName(path);
            Directory.CreateDirectory(m_OutputFolder + "\\" + year);
            Directory.CreateDirectory(m_OutputFolder + "\\" + year + "\\" + month);
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
            Image thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
            thumb.Save(Path.ChangeExtension(path, "thumb"));
        }
    }
}
