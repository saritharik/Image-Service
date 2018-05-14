using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Model
{
    class LogModel
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public LogModel()
        {
            LogMessages = new ObservableCollection<MessageRecievedEventArgs>();
            LogMessages.Add(new MessageRecievedEventArgs(MessageTypeEnum.INFO, "check"));
        }

        private ObservableCollection<MessageRecievedEventArgs> log_messages;
        public ObservableCollection<MessageRecievedEventArgs> LogMessages
        {
            get { return log_messages; }
            set
            {
                log_messages = value;
                OnPropertyChanged("log messages");
            }
        }
    }
}
