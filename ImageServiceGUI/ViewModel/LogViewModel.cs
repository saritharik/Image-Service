using ImageService.Logging.Modal;
using ImageServiceGUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.ViewModel
{
    class LogViewModel
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public LogViewModel()
        {
            this.LogModel = new LogModel();
            LogModel.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e) {
                NotifyPropertyChanged(e.PropertyName);
            };
        }

        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private LogModel logModel;
        public LogModel LogModel
        {
            get { return this.logModel; }
            set
            {
                this.logModel = value;
            }
        }
        private ObservableCollection<MessageRecievedEventArgs> log_messages;
        public ObservableCollection<MessageRecievedEventArgs> LogMessages
        {
            get { return this.logModel.LogMessages; }
            private set { }
        }
    }
}
