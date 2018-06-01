using Communication;
using ImageServiceWebApp.communication;
using Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImageServiceWebApp.Models
{
    public class ConfigModel
    {
        public ConfigModel()
        {
            this.Handlers = new List<string>();
            ClientCommSingelton.getInstance().DataReceived += GetMessage;
        }


        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "OutputDirectory")]
        public string OutputDirectory { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "SourceName")]
        public string SourceName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "LogName")]
        public string LogName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ThumbnailSize")]
        public string ThumbnailSize { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Handlers")]
        public List<string> Handlers { get; set; }



        public void GetMessage(object sender, DataRecivedEventArgs dataArgs)
        {
            if (dataArgs.CommandID == (int)CommandEnum.GetConfigCommand)
            {
                FromJson(dataArgs.Args);
                ClientCommSingelton.getInstance().sendMessage("succeeded", (int)CommandEnum.GetConfigCommand);
            }
        }

        public void FromJson(string data)
        {
            JObject configObj = JsonConvert.DeserializeObject<JObject>(data);
            string directories = (string)configObj["Handlers"];
            this.OutputDirectory = (string)configObj["OutputDir"];
            this.SourceName = (string)configObj["SourceName"];
            this.LogName = (string)configObj["LogName"];
            this.ThumbnailSize = (string)configObj["ThumbnailSize"];

            string[] pathes = directories.Split(';');
            foreach (string path in pathes)
            {
                Handlers.Add(path);
            }
        }
    }
}