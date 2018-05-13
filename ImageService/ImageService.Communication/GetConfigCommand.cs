using ImageService.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.ImageService.communication
{
    class GetConfigCommand : ICommand
    {
        public string Execute(string[] args, out bool result)
        {
            result = true;
            JObject configObj = new JObject();
            configObj["Handler"] = ConfigurationManager.AppSettings["Handler"];
            configObj["OutputDir"] = ConfigurationManager.AppSettings["OutputDir"];
            configObj["SourceName"] = ConfigurationManager.AppSettings["SourceName"];
            configObj["LogName"] = ConfigurationManager.AppSettings["LogName"];
            configObj["ThumbnailSize"] = ConfigurationManager.AppSettings["ThumbnailSize"];
            return configObj.ToString();
        }
    }
}
