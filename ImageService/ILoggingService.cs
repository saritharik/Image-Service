using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public interface ILoggingService
    {
        /// <summary>
        /// event that invoke when update the logger.
        /// </summary>
        event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        /// <summary>
        /// this function invoke the event to pass the update data to the logger.
        /// </summary>
        /// <param name="message"> the masseage that need to be writen to the log.
        /// <param name="type"> the type of the given message. information / failed / warning.
        void Log(string message, MessageTypeEnum type);           // Logging the Message
    }
}
