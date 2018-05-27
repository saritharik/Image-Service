using Communication;
using ImageServiceGUI.communication;
using ImageServiceGUI.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;

namespace ImageServiceGUI.ViewModel
{
    class LogViewModel
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogViewModel()
        {
            this.LogModel = new LogModel();
            LogModel.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e) {
                NotifyPropertyChanged(e.PropertyName);
            };
        }

        /// <summary>
        /// NotifyPropertyChanged event.
        /// </summary>
        /// <param name="name"></param>
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// LogModel property.
        /// </summary>
        public LogModel LogModel { get; set; }

        /// <summary>
        /// LogMessages property.
        /// </summary>
        public ObservableCollection<MessageRecievedEventArgs> LogMessages
        {
            get { return this.LogModel.LogMessages; }
            private set { }
        }
    }
}
