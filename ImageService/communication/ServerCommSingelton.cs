using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Communication;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json.Linq;
using ImageService.Logging.Modal;
using System.IO;
using Newtonsoft.Json;

namespace ImageService.communication
{
    class ServerCommSingelton// : ITcpCommunication
    {
        private static ServerCommSingelton instance = null;

        private NetworkStream stream;
        private BinaryReader reader;
        private BinaryWriter writer;

        public event EventHandler<DataRecivedEventArgs> DataReceived;

        private ServerCommSingelton() {}

        public static ServerCommSingelton getInstance()
        {
            if (instance == null)
            {
                instance = new ServerCommSingelton();
            }
            return instance;
        }

       

        public void sendMessage(string message, TcpClient client)
        {
            stream = client.GetStream();
            writer = new BinaryWriter(stream);
            try
            {
                writer.Write(message);
            } catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public string receiveMessage(TcpClient client)
        {
            stream = client.GetStream();
            reader = new BinaryReader(stream);
            string message;
            try
            {
                message = reader.ReadString();
                DataReceived?.Invoke(this, fromJsonRecvCommand(message));
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                message = e.ToString();
            }
            return message;
        }

        public void ConvertMsg(int id, string data, TcpClient client)
        {
            JObject dataObj = new JObject();
            dataObj["Id"] = id;
            dataObj["Args"] = data;
            sendMessage(JsonConvert.SerializeObject(dataObj), client);
        }

        public void logMessage(MessageRecievedEventArgs data, TcpClient client)
        {
            JObject logObj = new JObject();
            logObj["Type"] = data.Status.ToString();
            logObj["Message"] = data.Message;
            ConvertMsg((int)CommandEnum.LogCommand, JsonConvert.SerializeObject(logObj), client);
        }

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

        public DataRecivedEventArgs fromJsonRecvCommand(string data)
        {
            JObject recv = JsonConvert.DeserializeObject<JObject>(data);
            DataRecivedEventArgs dataArgs = new DataRecivedEventArgs((int)recv["Id"], (string)recv["Args"]);
            return dataArgs;
        }
    }
}
