using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageTypeEnum Status
        {
            get /*{ return Status; }*/;
            set /*{ Status = value; }*/;
        }
        public string Message
        {
            get /*{ return Message; }*/;
            set /*{ Message = value; }*/;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public MessageRecievedEventArgs(MessageTypeEnum status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}