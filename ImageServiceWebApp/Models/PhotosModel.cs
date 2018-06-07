using Communication;
using ImageServiceWebApp.communication;
using Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace ImageServiceWebApp.Models
{
    public class PhotosModel
    {
        private string outputDir;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PhotosModel()
        {
            PhotosInfo = new List<PhotoInfo>();
            ClientCommSingelton.getInstance().DataReceived += GetNameOutputDir;

        }

        /// <summary>
        /// PhotosInfo list.
        /// </summary>
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "PhotosInfo")]
        public List<PhotoInfo> PhotosInfo { get ; set; }

        /// <summary>
        /// This function activated when the client get data from server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataArgs">Data arguments.</param>
        public void GetNameOutputDir(object sender, DataRecivedEventArgs dataArgs)
        {
            if (dataArgs.CommandID == (int)CommandEnum.GetConfigCommand)
            {
                FromJson(dataArgs.Args);
                GetPhotos(outputDir);
            }
        }

        /// <summary>
        /// Convert data with json.
        /// </summary>
        /// <param name="data">to convert</param>
        public void FromJson(string data)
        {
            JObject configObj = JsonConvert.DeserializeObject<JObject>(data);
            this.outputDir = (string)configObj["OutputDir"];
        }

        /// <summary>
        /// Get list of photos from directory.
        /// </summary>
        /// <param name="directory"></param>
        public void GetPhotos(string directory)
        {
            var files = Directory.GetFiles(directory + "\\" + "Thumbnails", "*.*", SearchOption.AllDirectories);
            int i = 0;
            foreach (string filename in files)
            {
                String[] split = filename.Split('\\');
                int length = split.Length;
                String[] output = outputDir.Split('\\');
                this.PhotosInfo.Add(new PhotoInfo { FullPath = filename, Month = Int32.Parse(split[length - 2]),
                    Year = Int32.Parse(split[length - 3]),
                    Name = split[length - 1], ID = i, Directory = output[output.Length - 1]});
                i++;
            }
        }

        /// <summary>
        /// Return the number of photos in directory.
        /// </summary>
        /// <returns>number of photos in directory</returns>
        public int GetNumberOfPhotos()
        {
            return this.PhotosInfo.Count();
        }

        /// <summary>
        /// Remove photo by id.
        /// </summary>
        /// <param name="photoID">photo's id</param>
        public void RemovePhoto(int photoID)
        {
            string photoPath = null;
            PhotoInfo ph = null;
            foreach (PhotoInfo photo in PhotosInfo)
            {
                if (photo.ID == photoID)
                {
                    photoPath = photo.FullPath;
                    ph = photo;
                    break;
                }
            }

            // remove from the list
            if (PhotosInfo.Contains(ph))
            {
                PhotosInfo.Remove(ph);
            }

            // remove from the outputdir
            if (File.Exists(photoPath))
            {
                File.Delete(photoPath);
            }
        }
    }
}