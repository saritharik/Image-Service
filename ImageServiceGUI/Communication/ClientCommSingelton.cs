using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Communication
{
    class ClientCommSingelton
    {
        private static ClientCommSingelton instance = null;
        private TcpClient client;
        private NetworkStream stream;
        private BinaryReader reader;
        private BinaryWriter writer;

        //public event EventHandler<DataRecivedEventArgs> DataReceived;

        private ClientCommSingelton()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            this.client = new TcpClient();
            client.Connect(ep);
            Console.WriteLine("You are connected");
            this.stream = client.GetStream();
            this.reader = new BinaryReader(stream);
            this.writer = new BinaryWriter(stream);
            {
                // Send data to server
                int num = int.Parse(Console.ReadLine());
                writer.Write(num);
                // Get result from server
                int result = reader.ReadInt32();
                Console.WriteLine("Result = {0}", result);
            }
            client.Close();
        }

        public static ClientCommSingelton getInstance()
        {
            if (instance == null)
            {
                instance = new ClientCommSingelton();
            }
            return instance;
        }

        public void sendMessage(string message)
        {
            writer.Write(message);
        }

        public string receiveMessage()
        {
            //DataRecivedEventArgs dataArgs = new DataRecivedEventArgs()
            //DataReceived.Invoke()
            return " ";
        }
    }
}
