using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Communication;
using Newtonsoft.Json.Linq;
using Infrastructure;
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace ImageService.communication
{
    class ServerCommSingelton
    {
        #region members
        private static ServerCommSingelton instance = null;

        private NetworkStream stream;
        private BinaryReader reader;
        private BinaryWriter writer;
        #endregion

        /// <summary>
        /// Data recived event.
        /// </summary>
        public event EventHandler<DataRecivedEventArgs> DataReceived;
        private static Mutex mut;

        /// <summary>
        /// Private constructor - to singelton class.
        /// </summary>
        private ServerCommSingelton()
        {
            mut = new Mutex();
        }

        /// <summary>
        /// getInstance method for singelton class.
        /// </summary>
        /// <returns>Instance of this object</returns>
        public static ServerCommSingelton getInstance()
        {
            if (instance == null)
            {
                instance = new ServerCommSingelton();
            }
            return instance;
        }

       
        /// <summary>
        /// Send the recived message to the client.
        /// </summary>
        /// <param name="message">the message to send</param>
        /// <param name="client">the client to send him the message</param>
        public void sendMessage(string message, TcpClient client)
        {
            stream = client.GetStream();
            writer = new BinaryWriter(stream);
            try
            {
                //mut.WaitOne();
                writer.Write(message);
                writer.Flush();
                //mut.ReleaseMutex();
            } catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Rececive message from the client.
        /// </summary>
        /// <param name="client">the client that send the message</param>
        /// <returns>the message</returns>
        public string receiveMessage(TcpClient client)
        {
            stream = client.GetStream();
            reader = new BinaryReader(stream);
            string message;
            try
            {
                //mut.WaitOne();
                message = reader.ReadString();
                //mut.ReleaseMutex();
                DataReceived?.Invoke(this, fromJsonRecvCommand(message));
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                message = e.ToString();
            }
            return message;
        }

        /// <summary>
        /// Convert message with json.
        /// </summary>
        /// <param name="id">Command if</param>
        /// <param name="data">the arguments</param>
        /// <param name="client">the client to send him the message</param>
        public void ConvertMsg(int id, string data, TcpClient client)
        {
            JObject dataObj = new JObject();
            dataObj["Id"] = id;
            dataObj["Args"] = data;
            sendMessage(JsonConvert.SerializeObject(dataObj), client);
        }

        /// <summary>
        /// The function that activated when get new log message and
        /// convert the log message with json to send the message.
        /// </summary>
        /// <param name="data">MessageRecievedEventArgs</param>
        /// <param name="client">the client to send him the message</param>
        public void logMessage(MessageRecievedEventArgs data, TcpClient client)
        {
            JObject logObj = new JObject();
            logObj["Type"] = data.Status.ToString();
            logObj["Message"] = data.Message;
            ConvertMsg((int)CommandEnum.LogCommand, JsonConvert.SerializeObject(logObj), client);
        }

        /// <summary>
        /// Send the settings message.
        /// </summary>
        /// <param name="data">the settings.</param>
        /// <param name="client">the client to send him the message</param>
        public void settingsMessage(string[] data, TcpClient client)
        {
            JObject configObj = new JObject();
            configObj["Handlers"] = data[0];
            configObj["OutputDir"] = data[1];
            configObj["SourceName"] = data[2];
            configObj["LogName"] = data[3];
            configObj["ThumbnailSize"] = data[4];
            ConvertMsg((int)CommandEnum.GetConfigCommand, JsonConvert.SerializeObject(configObj), client);
        }

        /// <summary>
        /// Convert the message to DataRecivedEventArgs with json.
        /// </summary>
        /// <param name="data">to convert</param>
        /// <returns>DataRecivedEventArgs</returns>
        public DataRecivedEventArgs fromJsonRecvCommand(string data)
        {
            JObject recv = JsonConvert.DeserializeObject<JObject>(data);
            DataRecivedEventArgs dataArgs = new DataRecivedEventArgs((int)recv["Id"], (string)recv["Args"]);
            return dataArgs;
        }
    }
}
