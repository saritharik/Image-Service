using Communication;
using Newtonsoft.Json.Linq;
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
    class ClientCommSingelton : ITcpCommunication
    {
        private static ClientCommSingelton instance = null;
        private TcpClient client;
        private NetworkStream stream;
        private BinaryReader reader;
        private BinaryWriter writer;

        public event EventHandler<DataRecivedEventArgs> DataReceived;

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
                try
                {
                    while (true)
                    {
                        string data = reader.ReadString();

                        Task task = new Task(() =>
                        {
                            receiveMessage(data);
                        });
                        task.Start();
                    }
                } catch (Exception e)
                {
                    Console.WriteLine("Tamar Hamaatzbenet");
                }
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

        public string receiveMessage(string data)
        {
            DataRecivedEventArgs dataArgs = FromJson(data);
            DataReceived?.Invoke(this, dataArgs);
            return " ";
        }

        public DataRecivedEventArgs FromJson(string data)
        {
            JObject dataObj = JObject.Parse(data);
            int id = (int)dataObj["Id"];
            string args = (string)dataObj["Args"];

            DataRecivedEventArgs dataArgs = new DataRecivedEventArgs(id, args);
            return dataArgs;
        }
    }
}
