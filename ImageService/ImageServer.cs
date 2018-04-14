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

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        //private Dictionary<String, CommandRecievedEventArgs> commands;
        #endregion

        /// <summary>
        /// The server responsible for creating the handlers and sending orders
        /// </summary>
        /// <param name="logging"></param> the logger to update on the success or fail of the commands
        public ImageServer(ILoggingService logging)
        {
            m_logging = logging;
            IImageServiceModal imageServiceModal = new ImageServiceModal(logging);
            m_controller = new ImageController(imageServiceModal);
            string directories = ConfigurationManager.AppSettings["Handler"];
            string[] pathes = directories.Split(';');
            foreach (string path in pathes)
            {
                createHandler(path);
            }
            
        }
        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

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
        public void sendCommand()
        {
            CommandRecievedEventArgs eventArgs = 
                new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, null);
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
