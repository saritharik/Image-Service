using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        // Implement Here!
        public DirectoyHandler(String directory, IImageController controller)
        {
            m_controller = controller;
            m_path = directory;
        }
        // The Function Recieves the directory to Handle
        public void StartHandleDirectory(string dirPath)
        {
           // m_dirWatcher = new FileSystemWatcher(dirPath, "*.jpg");
        }

        // The Event that will be activated upon new Command
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {

        }
            
    }
}
