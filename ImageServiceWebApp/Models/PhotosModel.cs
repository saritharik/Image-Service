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
        public PhotosModel()
        {
            PhotosInfo = new List<PhotoInfo>();
            ClientCommSingelton.getInstance().DataReceived += GetNameOutputDir;
        }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "PhotosInfo")]
        public List<PhotoInfo> PhotosInfo { get ; set; }

        public void GetNameOutputDir(object sender, DataRecivedEventArgs dataArgs)
        {
            if (dataArgs.CommandID == (int)CommandEnum.GetConfigCommand)
            {
                FromJson(dataArgs.Args);
                ClientCommSingelton.getInstance().sendMessage("succeeded", (int)CommandEnum.GetConfigCommand);
                GetPhotos(outputDir);
            }
        }

        public void FromJson(string data)
        {
            JObject configObj = JsonConvert.DeserializeObject<JObject>(data);
            this.outputDir = (string)configObj["OutputDir"];
        }

        public void GetPhotos(string directory)
        {
            var files = Directory.GetFiles(directory + "\\" + "Thumbnails", "*.*", SearchOption.AllDirectories);
            int i = 0;
            foreach (string filename in files)
            {
                String[] split = filename.Split('\\');
                int length = split.Length;
                this.PhotosInfo.Add(new PhotoInfo { Path = filename, Month = Int32.Parse(split[length - 2]),
                    Year = Int32.Parse(split[length - 3]),
                    Name = split[length - 1], ID = i});
                i++;
            }
        }

        public int GetNumberOfPhotos()
        {
            return this.PhotosInfo.Count();
        }

        public void RemovePhoto(int photoID)
        {
            string photoPath = null;
            PhotoInfo ph = null;
            foreach (PhotoInfo photo in PhotosInfo)
            {
                if (photo.ID == photoID)
                {
                    photoPath = photo.Path;
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