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
using System.Threading;

namespace ImageServiceWebApp.communication
{
    class ClientCommSingelton : ITcpCommunication
    {
        #region members
        private static ClientCommSingelton instance = null;
        private TcpClient client;
        private NetworkStream stream;
        private BinaryReader reader;
        private BinaryWriter writer;
        #endregion

        // DateReceived event.
        public event EventHandler<DataRecivedEventArgs> DataReceived;

        /// <summary>
        /// Private constructor - to singelton class.
        /// </summary>
        private ClientCommSingelton()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            this.client = new TcpClient();
            try
            {
                client.Connect(ep);
                Console.WriteLine("You are connected");
                this.stream = client.GetStream();
                this.reader = new BinaryReader(stream);
                this.writer = new BinaryWriter(stream);
            } catch (Exception e) { }
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

        /// <summary>
        /// getInstance method for singelton class.
        /// </summary>
        /// <returns>Instance of this object</returns>
        public static ClientCommSingelton getInstance()
        {
            if (instance == null)
            {
                instance = new ClientCommSingelton();
            }
            return instance;
        }

        /// <summary>
        /// Send message according to arguments.
        /// </summary>
        /// <param name="message">the message to send</param>
        /// <param name="id">the id of the command</param>
        public void sendMessage(string message, int id)
        {
            writer.Write(ToJson(id, message));
            writer.Flush();
        }

        /// <summary>
        /// Receive message.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>the data</returns>
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

        /// <summary>
        /// Convert message with jsom.
        /// </summary>
        /// <param name="command">id command</param>
        /// <param name="message">arguments for command</param>
        /// <returns></returns>
        public String ToJson(int command, string message)
        {
            JObject dataObj = new JObject();
            dataObj["Id"] = command;
            dataObj["Args"] = message;
            return JsonConvert.SerializeObject(dataObj);
        }

        /// <summary>
        /// Convert message to DataRecivedEventArgs with json.
        /// </summary>
        /// <param name="data">to convert</param>
        /// <returns>DataRecivedEventArgs</returns>
        public DataRecivedEventArgs FromJson(string data)
        {
            JObject dataObj = JsonConvert.DeserializeObject<JObject>(data);
            int id = (int)dataObj["Id"];
            string args = (string)dataObj["Args"];

            DataRecivedEventArgs dataArgs = new DataRecivedEventArgs(id, args);
            return dataArgs;
        }

        /// <summary>
        /// Check if the connection successful.
        /// </summary>
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
