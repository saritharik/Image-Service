
using ImageService.communication;
using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        /// <summary>
        /// event that invoke when update the logger.
        /// </summary>
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        /// <summary>
        /// this function invoke the event to pass the update data to the logger.
        /// </summary>
        /// <param name="message"> the masseage that need to be writen to the log.
        /// <param name="type"> the type of the given message. information / failed / warning.
        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecieved?.Invoke(this, new MessageRecievedEventArgs(type, message));
        }
    }
}