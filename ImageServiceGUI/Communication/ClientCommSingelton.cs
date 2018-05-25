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
using Newtonsoft.Json;

namespace ImageServiceGUI.communication
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
            try
            {
                client.Connect(ep);
                //this.Connected = true;
            } catch (Exception e)
            {
                //this.Connected = false;
            }
            Console.WriteLine("You are connected");
            this.stream = client.GetStream();
            this.reader = new BinaryReader(stream);
            this.writer = new BinaryWriter(stream);
            try
            {
                Task task = new Task(() =>
                {
                    while (true)
                    {
                        try
                        {
                            string data = reader.ReadString();
                            string result = receiveMessage(data);
                        }
                        catch (Exception e)
                        {
                            break;
                        }
                    }
                });
                task.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("catch");
            }
            //client.Close();
        }

        public static ClientCommSingelton getInstance()
        {
            if (instance == null)
            {
                instance = new ClientCommSingelton();
            }
            return instance;
        }

        public void sendMessage(string message, int id)
        {
            writer.Write(ToJson(id, message));
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

        public String ToJson(int command, string message)
        {
            JObject dataObj = new JObject();
            dataObj["Id"] = command;
            dataObj["Args"] = message;
            return JsonConvert.SerializeObject(dataObj);
        }

        public DataRecivedEventArgs FromJson(string data)
        {
            JObject dataObj = JsonConvert.DeserializeObject<JObject>(data);
            int id = (int)dataObj["Id"];
            string args = (string)dataObj["Args"];

            DataRecivedEventArgs dataArgs = new DataRecivedEventArgs(id, args);
            return dataArgs;
        }

        public bool Connected
        {
            get { return this.client.Connected; }
            set
            {
                this.Connected = value;
            }
        }
    }
}
