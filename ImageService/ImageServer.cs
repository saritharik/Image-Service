using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ImageService.communication;
using System.Net.Sockets;
using System.Net;
using ImageService.Logging.Modal;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;

        private List<TcpClient> clients;
        private int port;
        private TcpListener listener;
        //private Dictionary<String, CommandRecievedEventArgs> commands;
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

            m_logging.MessageRecieved += newLogMsg;

            string directories = ConfigurationManager.AppSettings["Handler"];
            string[] pathes = directories.Split(';');
            foreach (string path in pathes)
            {
                createHandler(path);
            }

            port = 8888;
            StartTcpComuunication();
        }
        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

        public void StartTcpComuunication()
        {
            IPEndPoint ep = new
                IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            listener = new TcpListener(ep);

            try
            {
                listener.Start();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Waiting for connections...");

            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        clients.Add(client);
                        Console.WriteLine("Got new connection");
                        sendConfig(client);
                        ServerCommSingelton.getInstance().receiveMessage(client);
                        // send log + send request for handler remove

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


        public void sendConfig(TcpClient client)
        {
            string[] config = {ConfigurationManager.AppSettings["Handler"], ConfigurationManager.AppSettings["OutputDir"],
            ConfigurationManager.AppSettings["SourceName"], ConfigurationManager.AppSettings["LogName"],
            ConfigurationManager.AppSettings["ThumbnailSize"]};
            ServerCommSingelton.getInstance().settingsMessage(config, client);
        }

        public void newLogMsg(object sender, MessageRecievedEventArgs msg)
        {
            foreach (TcpClient client in clients)
            {
                ServerCommSingelton.getInstance().logMessage(msg, client);

            }
        }

        /// <summary>
        /// Create a handler that listen to specific folder, and add it to the CommandRecieved event
        /// </summary>
        /// <param name="directory"></param>
        public void createHandler(String directory)
        {
            IDirectoryHandler directoryHandler = new DirectoyHandler(directory, m_controller, m_logging);
            CommandRecieved += directoryHandler.OnCommandRecieved;
            directoryHandler.DirectoryClose += onCloseServer;
            directoryHandler.StartHandleDirectory(directory);
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
            IDirectoryHandler directoryHandler = (DirectoyHandler)sender;
            CommandRecieved -= directoryHandler.OnCommandRecieved;
            directoryHandler.DirectoryClose -= onCloseServer;
        }
    }
}
