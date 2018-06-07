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
        /// <summary>
        /// Constructor.
        /// </summary>
        public LogModel()
        {
            this.Logs = new List<MessageRecievedEventArgs>();
            this.BackupLogs = new List<MessageRecievedEventArgs>();
            ClientCommSingelton.getInstance().DataReceived += GetMessage;
        }

        /// <summary>
        /// Logs list.
        /// </summary>
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Logs")]
        public List<MessageRecievedEventArgs> Logs { get; set; }

        /// <summary>
        /// BackupLogs list.
        /// </summary>
        public List<MessageRecievedEventArgs> BackupLogs { get; set; }

        /// <summary>
        /// This function activated when the client get data from server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataArgs">Data arguments.</param>
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

        /// <summary>
        /// Convert message to MessageRecievedEventArgs with json.
        /// </summary>
        /// <param name="args">to convert</param>
        /// <returns>MessageRecievedEventArgs</returns>
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