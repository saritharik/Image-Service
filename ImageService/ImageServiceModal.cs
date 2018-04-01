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
            string res = "true"; //////////
            result = true; ///////////////
            createFolder();
            DateTime dateTime = getDateFromImage(path);
            movePicture(path, dateTime);

            return res;

        }

        private void createFolder()
        {
            if (!Directory.Exists(m_OutputFolder))
            {
                Directory.CreateDirectory(m_OutputFolder);
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

        private void movePicture(string path, DateTime dateTime)
        {
            File.Move(path, m_OutputFolder);
        }

    }
}
