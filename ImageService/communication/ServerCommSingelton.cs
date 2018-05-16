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

namespace ImageService.communication
{
    class ServerCommSingelton// : ITcpCommunication
    {
        private static ServerCommSingelton instance = null;
        //private int port;
        //private TcpListener listener;
        //private IClientHandler ch;

        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;

        public event EventHandler<DataRecivedEventArgs> DataReceived;

        private ServerCommSingelton(/*int port, IClientHandler ch*/)
        {
            //this.port = 8000;
            //this.ch = new ClientHandler();
            //this.DataReceived += ConvertMsg;
        }

        public static ServerCommSingelton getInstance()
        {
            if (instance == null)
            {
                instance = new ServerCommSingelton();
            }
            return instance;
        }

         /*public void Stop()
         {
             listener.Stop();
         }*/

        public void sendMessage(string message, TcpClient client)
        {
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            try
            {
                writer.WriteLine(message);
            } catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }

        public string receiveMessage(TcpClient client)
        {
            stream = client.GetStream();
            reader = new StreamReader(stream);
            string message;
            try
            {
                message = reader.ReadLine();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                message = e.ToString();
            }
            return message;
        }

        /*public void ConvertMsg(object sender, DataRecivedEventArgs args, )
        {
            JObject dataObj = new JObject();
            dataObj["Id"] = args.CommandID;
            dataObj["Args"] = args.Args;
            sendMessage(dataObj.ToString());
        }*/

        public void logMessage(MessageRecievedEventArgs data, TcpClient client)
        {
            JObject logObj = new JObject();
            logObj["Type"] = data.Status.ToString();
            logObj["Message"] = data.Message;
            sendMessage(logObj.ToString(), client);
        }

        public void settingsMessage(string[] data, TcpClient client)
        {
            JObject configObj = new JObject();
            configObj["Handlers"] = data[0];
            configObj["OutputDir"] = data[1];
            configObj["SourceName"] = data[2];
            configObj["LogName"] = data[3];
            configObj["ThumbnailSize"] = data[4];

            sendMessage(configObj.ToString(), client);
        }


    }
}
