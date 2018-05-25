using Communication;
using ImageService.Infrastructure.Enums;
using ImageService.Logging.Modal;
using ImageService.Modal;
using ImageServiceGUI.communication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ImageServiceGUI.Model
{
    class SettingsModel
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public SettingsModel()
        {
            handlers = new ObservableCollection<string>();
            Object handlersLock = new Object();
            BindingOperations.EnableCollectionSynchronization(handlers, handlersLock);
            //Thread.Sleep(1000);
            ClientCommSingelton.getInstance().DataReceived += GetMessage;
            ClientCommSingelton.getInstance().DataReceived += GetRemoveMessage;
        }

        private string output_directory;// = "outputDirectory";
        public string OutputDirectory
        {
            get { return output_directory; }
            set
            {
                output_directory = value;
                OnPropertyChanged("OutputDirectory");
            }
        }

        private string source_name;// = "sourceName";
        public string SourceName
        {
            get { return source_name; }
            set
            {
                source_name = value;
                OnPropertyChanged("SourceName");
            }
        }

        private string log_name;// = "logName";
        public string LogName
        {
            get { return log_name; }
            set
            {
                log_name = value;
                OnPropertyChanged("LogName");
            }
        }

        private string thumbnail_size;// = "120";
        public string ThumbnailSize
        {
            get { return thumbnail_size; }
            set
            {
                thumbnail_size = value;
                OnPropertyChanged("ThumbnailSize");
            }
        }

        private string selected_handler;
        public string SelectedHandler
        {
            get { return selected_handler; }
            set
            {
                selected_handler = value;
                OnPropertyChanged("SelectedHandler");
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
            ClientCommSingelton.getInstance().sendMessage(selected_handler, (int)CommandEnum.CloseCommand);            
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
                ClientCommSingelton.getInstance().sendMessage("succeeded", (int)CommandEnum.GetConfigCommand);
            }
        }

        public void GetRemoveMessage(object sender, DataRecivedEventArgs dataArgs)
        {
            if (dataArgs.CommandID == (int)CommandEnum.CloseCommand)
            {
                if (this.Handlers.Contains(dataArgs.Args))
                {
                    this.Handlers.Remove(dataArgs.Args);
                }
            }
        }
        public void FromJson(string data)
        {
            JObject configObj = JsonConvert.DeserializeObject<JObject>(data);// JObject.Parse(args);
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
