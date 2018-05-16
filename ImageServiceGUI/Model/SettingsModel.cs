﻿using Communication;
using ImageService.Infrastructure.Enums;
using ImageService.Logging.Modal;
using ImageService.Modal;
using ImageServiceGUI.communication;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Model
{
    class SettingsModel
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public SettingsModel()
        {
            handlers = new ObservableCollection<string>();
            handlers.Add("example");
            ClientCommSingelton.getInstance().DataReceived += GetMessage;
        }

        private string output_directory = "outputDirectory";
        public string OutputDirectory
        {
            get { return output_directory; }
            set
            {
                output_directory = value;
                OnPropertyChanged("Directory");
            }
        }

        private string source_name = "sourceName";
        public string SourceName
        {
            get { return source_name; }
            set
            {
                source_name = value;
                OnPropertyChanged("Source Name");
            }
        }

        private string log_name = "logName";
        public string LogName
        {
            get { return log_name; }
            set
            {
                log_name = value;
                OnPropertyChanged("Log Name");
            }
        }

        private string thumbnail_size = "120";
        public string ThumbnailSize
        {
            get { return thumbnail_size; }
            set
            {
                thumbnail_size = value;
                OnPropertyChanged("Thumbnails Size");
            }
        }

        private string selected_handler;
        public string SelectedHandler
        {
            get { return selected_handler; }
            set
            {
                selected_handler = value;
                OnPropertyChanged("Selected Handler");
            }
        }

        private ObservableCollection<String> handlers;
        
        public ObservableCollection<String> Handlers
        {
            get { return handlers; }
            set
            {
                handlers = value;
            }
        }

        public void RemoveHandlerCommand()
        {
            // remove the selected handler with the server
            CommandRecievedEventArgs args = 
                new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, selected_handler);
            //string command = ToJSON(args);
            
        }

        /*public string ToJSON(CommandRecievedEventArgs args)
        {
            JObject commandObj = new JObject();
            commandObj["CommandID"] = args.CommandID;
            commandObj["Args"] = args.Args[0];
            commandObj["Path"] = args.RequestDirPath;
            return commandObj.ToString();
        }*/

        public void GetMessage(object sender, DataRecivedEventArgs dataArgs)
        {
            if (dataArgs.CommandID == (int)CommandEnum.GetConfigCommand)
            {
                FromJson(dataArgs.Args);
            }
        }

        public void FromJson(string args)
        {
            JObject configObj = JObject.Parse(args);
            string directories = (string)configObj["Handlers"];
            this.output_directory = (string)configObj["OutputDir"];
            this.source_name = (string)configObj["SourceName"];
            this.log_name = (string)configObj["LogName"];
            this.thumbnail_size = (string)configObj["ThumbnailSize"];
            
            string[] pathes = directories.Split(';');
            foreach (string path in pathes)
            {
                handlers.Add(path);
            }
        }

    }
}
