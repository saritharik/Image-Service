using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public interface ITcpCommunication
    {
        // DataRecived event.
        event EventHandler<DataRecivedEventArgs> DataReceived;

        /// <summary>
        /// Send message according to arguments.
        /// </summary>
        /// <param name="message">the message to send</param>
        /// <param name="id">the id of the command</param>
        void sendMessage(string message, int id);

        /// <summary>
        /// Receive message.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string receiveMessage(string data);
    }
}
