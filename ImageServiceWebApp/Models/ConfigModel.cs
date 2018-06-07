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
        /// <summary>
        /// Constructor.
        /// </summary>
        public ConfigModel()
        {
            this.Handlers = new List<string>();
            ClientCommSingelton.getInstance().DataReceived += GetMessage;
        }

        /// <summary>
        /// OutputDirectory
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "OutputDirectory")]
        public string OutputDirectory { get; set; }

        /// <summary>
        /// SourceName
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "SourceName")]
        public string SourceName { get; set; }

        /// <summary>
        /// LogName
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "LogName")]
        public string LogName { get; set; }

        /// <summary>
        /// ThumbnailSize
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ThumbnailSize")]
        public string ThumbnailSize { get; set; }

        /// <summary>
        /// SelectedHandler
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "SelectedHandler")]
        public string SelectedHandler { get; set; }

        /// <summary>
        /// Handlers
        /// </summary>
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Handlers")]
        public List<string> Handlers { get; set; }

        /// <summary>
        /// This function activated when the client get data from server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataArgs">Data arguments.</param>
        public void GetMessage(object sender, DataRecivedEventArgs dataArgs)
        {
            if (dataArgs.CommandID == (int)CommandEnum.GetConfigCommand)
            {
                FromJson(dataArgs.Args);
                ClientCommSingelton.getInstance().sendMessage("succeeded", (int)CommandEnum.GetConfigCommand);
            }
        }

        /// <summary>
        /// Convert message with json.
        /// </summary>
        /// <param name="data">to convert</param>
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
                if (path != "")
                {
                    Handlers.Add(path);
                }
            }
        }

        /// <summary>
        /// Remove Handler.
        /// </summary>
        public void RemoveHandlerCommand()
        {
            ClientCommSingelton.getInstance().sendMessage(SelectedHandler, (int)CommandEnum.CloseCommand);
        }
    }
}