using Communication;
using Infrastructure;
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

        private bool update;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsModel()
        {
            this.update = false;
            handlers = new ObservableCollection<string>();
            Object handlersLock = new Object();
            BindingOperations.EnableCollectionSynchronization(handlers, handlersLock);
            ClientCommSingelton.getInstance().DataReceived += GetMessage;
            ClientCommSingelton.getInstance().DataReceived += GetRemoveMessage;
            if (ClientCommSingelton.getInstance().Connected)
            {
                while (!update) { }
            }
        }

        private string output_directory;
        /// <summary>
        /// OutputDirectory property.
        /// </summary>
        public string OutputDirectory
        {
            get {
                return output_directory; }
            set
            {
                output_directory = value;
                OnPropertyChanged("OutputDirectory");
            }
        }

        private string source_name;
        /// <summary>
        /// SourceName property.
        /// </summary>
        public string SourceName
        {
            get { return source_name; }
            set
            {
                source_name = value;
                OnPropertyChanged("SourceName");
            }
        }

        private string log_name;
        /// <summary>
        /// LogName property.
        /// </summary>
        public string LogName
        {
            get { return log_name; }
            set
            {
                log_name = value;
                OnPropertyChanged("LogName");
            }
        }

        private string thumbnail_size;
        /// <summary>
        /// ThumbnailSize property.
        /// </summary>
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
        /// <summary>
        /// SelectedHandler property.
        /// </summary>
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
        /// <summary>
        /// Handlers property.
        /// </summary>
        public ObservableCollection<String> Handlers
        {
            get { return handlers; }
            set
            {
                handlers = value;
            }
        }

        /// <summary>
        /// remove the selected handler with the server
        /// </summary>
        public void RemoveHandlerCommand()
        {
            ClientCommSingelton.getInstance().sendMessage(selected_handler, (int)CommandEnum.CloseCommand);            
        }

        /// <summary>
        /// Check if the message it's a settings message.
        /// </summary>
        /// <param name="sender">the object that active the event</param>
        /// <param name="dataArgs">event args</param>
        public void GetMessage(object sender, DataRecivedEventArgs dataArgs)
        {
            if (dataArgs.CommandID == (int)CommandEnum.GetConfigCommand)
            {
                FromJson(dataArgs.Args);
                ClientCommSingelton.getInstance().sendMessage("succeeded", (int)CommandEnum.GetConfigCommand);
            }
        }

        /// <summary>
        /// Check if the message it's a remove message.
        /// </summary>
        /// <param name="sender">the object that active the event</param>
        /// <param name="dataArgs">event args</param>
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

        /// <summary>
        /// Convert message with json.
        /// </summary>
        /// <param name="data">to convert</param>
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
            update = true;
        }

    }
}
