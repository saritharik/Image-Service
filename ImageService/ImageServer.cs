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

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private Dictionary<String, CommandRecievedEventArgs> commands;
        #endregion
        public ImageServer(ILoggingService logging)
        {
            m_logging = logging;
            IImageServiceModal imageServiceModal = new ImageServiceModal();
            m_controller = new ImageController(imageServiceModal);
            commands = new Dictionary<String, CommandRecievedEventArgs>();
        }
        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

        public void createHandler(String directory)
        {
            IDirectoryHandler directoryHandler = new DirectoyHandler(directory, m_controller);
            CommandRecieved += directoryHandler.OnCommandRecieved;
            directoryHandler.DirectoryClose += onCloseServer;
        }

        /*public void sendCommand()
        {
            CommandRecieved("*", CloseHandler);
        }*/

        public void onCloseServer(Object sender, DirectoryCloseEventArgs commandRecieved)
        {
            IDirectoryHandler directoryHandler = (DirectoyHandler)sender;
            CommandRecieved -= directoryHandler.OnCommandRecieved;
            directoryHandler.DirectoryClose -= onCloseServer;
        }
    }
}
