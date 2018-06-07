using ImageService.Controller;
using ImageService.Controller.Handlers;
using Infrastructure;
using ImageService.Logging;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using ImageService.communication;
using System.Net.Sockets;
using System.Net;
using System.Collections.ObjectModel;
using Communication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private List<TcpClient> clients;
        private ObservableCollection<MessageRecievedEventArgs> logMessages;
        private int port;
        private TcpListener listener = null;
        private Dictionary<String, IDirectoryHandler> handlersList;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

        /// <summary>
        /// The server responsible for creating the handlers and sending orders
        /// </summary>
        /// <param name="logging"></param> the logger to update on the success or fail of the commands
        public ImageServer(ILoggingService logging)
        {
            m_logging = logging;
            clients = new List<TcpClient>();
            IImageServiceModal imageServiceModal = new ImageServiceModal(logging);
            m_controller = new ImageController(imageServiceModal);
            logMessages = new ObservableCollection<MessageRecievedEventArgs>();
            handlersList = new Dictionary<String, IDirectoryHandler>();
            m_logging.MessageRecieved += NewLogMsg;

            string directories = ConfigurationManager.AppSettings["Handler"];
            string[] pathes = directories.Split(';');
            foreach (string path in pathes)
            {
                createHandler(path);
            }

            ServerCommSingelton.getInstance().DataReceived += RemoveHandler;

            this.port = 8000;

            StartTcpCommunication();
        }

        /// <summary>
        /// Start tcp communication.
        /// </summary>
        public void StartTcpCommunication()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.port);
            this.listener = new TcpListener(ep);
            Console.WriteLine("try something");
            this.listener.Start();
            Console.WriteLine("Waiting for connections...");
            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        clients.Add(client);
                        Console.WriteLine("Got new connection");
                        Thread.Sleep(500);

                        SendConfig(client);
                        ServerCommSingelton.getInstance().receiveMessage(client);
                        foreach (MessageRecievedEventArgs msg in logMessages)
                        {
                            SendLogsList(msg, client);
                            ServerCommSingelton.getInstance().receiveMessage(client);
                        }
                        ServerCommSingelton.getInstance().sendMessage(HandlerToJson(), client);

                        Task clientTask = new Task(() => {
                            ServerCommSingelton.getInstance().receiveMessage(client);
                        });
                        clientTask.Start();

                        //ch.HandleClient(client);
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

        /// <summary>
        /// Send the app configuration details.
        /// </summary>
        /// <param name="client"> the client to send the information</param>
        public void SendConfig(TcpClient client)
        {
            string handlers = "";
            foreach (KeyValuePair<String, IDirectoryHandler> handler in handlersList)
            {
                if (handlersList[handler.Key].Equals(handlersList.Last().Value))
                {
                    handlers += handler.Key;
                }
                else
                {
                    handlers += handler.Key + ";";
                }
            }
            
            string[] config = {handlers, ConfigurationManager.AppSettings["OutputDir"],
            ConfigurationManager.AppSettings["SourceName"], ConfigurationManager.AppSettings["LogName"],
            ConfigurationManager.AppSettings["ThumbnailSize"]};
            ServerCommSingelton.getInstance().settingsMessage(config, client);
        }

        /// <summary>
        /// Send the log list, since service start.
        /// </summary>
        /// <param name="msgArgs"></param>
        /// <param name="client"></param>
        public void SendLogsList(MessageRecievedEventArgs msgArgs, TcpClient client)
        {
            ServerCommSingelton.getInstance().logMessage(msgArgs, client);
        }
        
        /// <summary>
        /// Save the log message to list, and send them to clients.
        /// </summary>
        /// <param name="sender"> the object that active the event</param>
        /// <param name="msg">the log message</param>
        public void NewLogMsg(object sender, MessageRecievedEventArgs msg)
        {
            List<TcpClient> toRemove = new List<TcpClient>();

            logMessages.Add(msg);
            foreach (TcpClient client in clients)
            {
                Task task = new Task(() =>
                {
                    try
                    {
                        ServerCommSingelton.getInstance().logMessage(msg, client);
                        ServerCommSingelton.getInstance().receiveMessage(client);
                    }
                    catch (Exception)
                    {
                        toRemove.Add(client);
                    }
                });
                task.Start();

            }

            foreach (TcpClient client in toRemove)
            {
                try {
                    client.Close();
                    this.clients.Remove(client);
                } catch (Exception) { }
            }
            toRemove.Clear();
        }

        /// <summary>
        /// Ramove handler from the dictionry.
        /// </summary>
        /// <param name="sender"> the object that active the event</param>
        /// <param name="dataArgs">the event arguments</param>
        public void RemoveHandler(object sender, DataRecivedEventArgs dataArgs)
        {
            if (dataArgs.CommandID == (int)CommandEnum.CloseCommand)
            {
                sendCommand(new CommandRecievedEventArgs((int)CommandEnum.CloseCommand,
                    null, dataArgs.Args));
                m_logging.Log("handler removed: " + dataArgs.Args, MessageTypeEnum.INFO);

                List<TcpClient> toRemove = new List<TcpClient>();

                foreach (TcpClient client in clients)
                {
                    Task task = new Task(() =>
                    {
                        try
                        {
                            Thread.Sleep(1000);
                            ServerCommSingelton.getInstance().sendMessage(RemoveHandlerToJson(dataArgs.Args), client);
                            ServerCommSingelton.getInstance().receiveMessage(client);
                        } catch (Exception)
                        {
                            toRemove.Add(client);
                        }
                    });
                    task.Start();
                }

                foreach (TcpClient client in toRemove)
                {
                    try
                    {
                        client.Close();
                        this.clients.Remove(client);
                    }
                    catch (Exception) { }
                }
                toRemove.Clear();
            }
        }

        /// <summary>
        /// Convert the message with json.
        /// </summary>
        /// <returns>the convert message</returns>
        public String HandlerToJson()
        {
            JObject dataObj = new JObject();
            dataObj["Id"] = (int)CommandEnum.CloseCommand;
            dataObj["Args"] = "Do you want to remove handler?";
            return JsonConvert.SerializeObject(dataObj);
        }

        /// <summary>
        /// Convert the remove message with json.
        /// </summary>
        /// <param name="path">the path to execute command</param>
        /// <returns>the convert message</returns>
        public string RemoveHandlerToJson(string path)
        {
            JObject dataObj = new JObject();
            dataObj["Id"] = (int)CommandEnum.CloseCommand;
            dataObj["Args"] = path;
            return JsonConvert.SerializeObject(dataObj);
        }
        /// <summary>
        /// Create a handler that listen to specific folder, and add it to the CommandRecieved event
        /// </summary>
        /// <param name="directory"></param>
        public void createHandler(String directory)
        {
            IDirectoryHandler directoryHandler = new DirectoryHandler(directory, m_controller, m_logging);
            CommandRecieved += directoryHandler.OnCommandRecieved;
            directoryHandler.DirectoryClose += onCloseServer;
            directoryHandler.StartHandleDirectory(directory);
            this.handlersList.Add(directory, directoryHandler);
        }

        /// <summary>
        /// Send a command to the handlers, by invoke the event.
        /// </summary>
        public void sendCommand(CommandRecievedEventArgs eventArgs)
        {
            CommandRecieved?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// This function update the handlers that the server is closed,
        /// and remove them from the event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="commandRecieved"></param>
        public void onCloseServer(Object sender, DirectoryCloseEventArgs commandRecieved)
        {
            IDirectoryHandler directoryHandler = (IDirectoryHandler)sender;
            CommandRecieved -= directoryHandler.OnCommandRecieved;
            directoryHandler.DirectoryClose -= onCloseServer;
            this.handlersList.Remove(commandRecieved.DirectoryPath);
        }
    }
}
