using Communication;
using ImageService.Infrastructure.Enums;
using ImageService.Logging.Modal;
using ImageServiceGUI.communication;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

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
            Object logLock = new Object();
            BindingOperations.EnableCollectionSynchronization(LogMessages, logLock);
            ClientCommSingelton.getInstance().DataReceived += GetMessage;
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

        public void GetMessage(object sender, DataRecivedEventArgs dataArgs)
        {
            if (dataArgs.CommandID == (int)CommandEnum.LogCommand)
            {
                MessageRecievedEventArgs logMessage = FromJson(dataArgs.Args);
                this.log_messages.Add(logMessage);
                ClientCommSingelton.getInstance().sendMessage("succeeded", (int)CommandEnum.LogCommand);
            }
        }

        public MessageRecievedEventArgs FromJson(string args)
        {
            MessageTypeEnum messageType = MessageTypeEnum.INFO;
            JObject messageObj = JObject.Parse(args);
            string type = (string)messageObj["Type"];
            if (type == "INFO")
            {
                messageType = MessageTypeEnum.INFO;
            } else if (type == "FAIL")
            {
                messageType = MessageTypeEnum.FAIL;
            } else if (type == "WARNING")
            {
                messageType = MessageTypeEnum.WARNING;
            }
            MessageRecievedEventArgs eventArgs = new MessageRecievedEventArgs(
                messageType, (string)messageObj["Message"]);
            return eventArgs;
        }
    }
}
