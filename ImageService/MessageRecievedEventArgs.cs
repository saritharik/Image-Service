using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging.Modal
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

        public MessageRecievedEventArgs(MessageTypeEnum status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}