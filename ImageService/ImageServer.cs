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
        public ImageServer(ILoggingService logging)
        {
            m_logging = logging;
            IImageServiceModal imageServiceModal = new ImageServiceModal();
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

        public void createHandler(String directory)
        {
            IDirectoryHandler directoryHandler = new DirectoyHandler(directory, m_controller, m_logging);
            CommandRecieved += directoryHandler.OnCommandRecieved;
            directoryHandler.DirectoryClose += onCloseServer;
            directoryHandler.StartHandleDirectory(directory);
        }

        public void sendCommand()
        {
            CommandRecievedEventArgs eventArgs = 
                new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, null);
            CommandRecieved.Invoke(this, eventArgs);
        }

        public void onCloseServer(Object sender, DirectoryCloseEventArgs commandRecieved)
        {
            IDirectoryHandler directoryHandler = (DirectoyHandler)sender;
            CommandRecieved -= directoryHandler.OnCommandRecieved;
            directoryHandler.DirectoryClose -= onCloseServer;
        }
    }
}
