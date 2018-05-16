using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Communication;

namespace ImageService.communication
{
    class ServerCommSingelton : ITcpCommunication
    {
        private static ServerCommSingelton instance = null;
        private int port;
        private TcpListener listener;
        private IClientHandler ch;

        public event EventHandler<DataRecivedEventArgs> DataReceived;

        private ServerCommSingelton(int port, IClientHandler ch)
        {
            this.port = port;
            this.ch = ch;
        }

        public static ServerCommSingelton getInstance(int port, IClientHandler ch)
        {
            if (instance == null)
            {
                instance = new ServerCommSingelton(port, ch);
            }
            return instance;
        }

        public void Start()
        {
            IPEndPoint ep = new
                IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            listener = new TcpListener(ep);

            listener.Start();
            Console.WriteLine("Waiting for connections...");

            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        Console.WriteLine("Got new connection");
                        ch.HandleClient(client);
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
                Console.WriteLine("Server stopped");
            });
            task.Start();
        }

        public void Stop()
        {
            listener.Stop();
        }

        public void sendMessage(string message)
        {

        }

        public string receiveMessage(string data)
        {

            return " ";
        }
    }
}
