using Communication;
using ImageServiceWebApp.communication;
using Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImageServiceWebApp.Models
{
    public class LogModel
    {
        public LogModel()
        {
            this.Logs = new List<MessageRecievedEventArgs>();
            this.BackupLogs = new List<MessageRecievedEventArgs>();
            BackupLogs.Add(new MessageRecievedEventArgs(MessageTypeEnum.INFO, "example"));
            BackupLogs.Add(new MessageRecievedEventArgs(MessageTypeEnum.FAIL, "example2"));
            Logs.Add(new MessageRecievedEventArgs(MessageTypeEnum.INFO, "example"));
            Logs.Add(new MessageRecievedEventArgs(MessageTypeEnum.FAIL, "example2"));
            ClientCommSingelton.getInstance().DataReceived += GetMessage;
        }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Logs")]
        public List<MessageRecievedEventArgs> Logs { get; set; }

        public List<MessageRecievedEventArgs> BackupLogs { get; set; }

        public void GetMessage(object sender, DataRecivedEventArgs dataArgs)
        {
            if (dataArgs.CommandID == (int)CommandEnum.LogCommand)
            {
                MessageRecievedEventArgs logMessage = FromJson(dataArgs.Args);
                this.Logs.Add(logMessage);
                this.BackupLogs.Add(logMessage);
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
            }
            else if (type == "FAIL")
            {
                messageType = MessageTypeEnum.FAIL;
            }
            else if (type == "WARNING")
            {
                messageType = MessageTypeEnum.WARNING;
            }
            MessageRecievedEventArgs eventArgs = new MessageRecievedEventArgs(
                messageType, (string)messageObj["Message"]);
            return eventArgs;
        }
    }
}