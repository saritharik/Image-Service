using Communication;
using ImageServiceWebApp.communication;
using Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ImageServiceWebApp.Models
{
    public class PhotosModel
    {
        private List<String> filesFound = new List<String>();
        
        private string outputDir;
        public PhotosModel()
        {
            ClientCommSingelton.getInstance().DataReceived += GetNameOutputDir;
        }

        public void GetNameOutputDir(object sender, DataRecivedEventArgs dataArgs)
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
            this.outputDir = (string)configObj["OutputDir"];
        }
    }
}