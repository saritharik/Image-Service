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
            PhotosNames = new List<String>();
            ClientCommSingelton.getInstance().DataReceived += GetNameOutputDir;
        }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "PhotosNames")]
        public List<String> PhotosNames { get ; set; }

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
            foreach (string filename in files)
            {
                this.PhotosNames.Add(filename);
            }
        }

        public int GetNumberOfPhotos()
        {
            return this.PhotosNames.Count();
        }
    }
}