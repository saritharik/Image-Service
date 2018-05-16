using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Communication;


namespace ImageServiceGUI.communication
{
    class ClientCommSingelton : ITcpCommunication
    {
        private static ClientCommSingelton instance = null;
        private TcpClient client;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;

        public event EventHandler<DataRecivedEventArgs> DataReceived;

        private ClientCommSingelton()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            this.client = new TcpClient();
            client.Connect(ep);
            Console.WriteLine("You are connected");
            this.stream = client.GetStream();
            this.reader = new StreamReader(stream);
            this.writer = new StreamWriter(stream);
            {
                try
                {
                    while (true)
                    {
                        string data = reader.ReadLine();

                        Task task = new Task(() =>
                        {

                            string result = receiveMessage(data);
                            sendMessage(result);
                        });
                        task.Start();
                    }
                } catch (Exception e)
                {
                    Console.WriteLine("catch");
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
            writer.WriteLine(message);
        }

        public string receiveMessage(string data)
        {
            string result = "succeeded";
            try
            {
                DataRecivedEventArgs dataArgs = FromJson(data);
                DataReceived?.Invoke(this, dataArgs);
            } catch (Exception e)
            {
                result = e.ToString();
            }
            return result;
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
