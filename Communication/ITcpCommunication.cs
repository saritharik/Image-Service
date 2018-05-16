using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public interface ITcpCommunication
    {
        event EventHandler<DataRecivedEventArgs> DataReceived;

        void sendMessage(string message);

        string receiveMessage(string data);
    }
}
