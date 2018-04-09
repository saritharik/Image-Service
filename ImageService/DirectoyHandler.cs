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

        // The Event That Notifies that the Directory is being closed
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;

        // Implement Here!
        public DirectoyHandler(String directory, IImageController controller, ILoggingService logging)
        {
            m_controller = controller;
            m_path = directory;
            m_logging = logging;
        }
        // The Function Recieves the directory to Handle
        public void StartHandleDirectory(string dirPath)
        {
            m_dirWatcher = new FileSystemWatcher(dirPath);
            m_dirWatcher.BeginInit();
            m_dirWatcher.Changed += new FileSystemEventHandler(OnChanged);
        }

        // The Event that will be activated upon new Command
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if(e.RequestDirPath.Equals(m_path))
            {
                if (e.CommandID == (int)CommandEnum.CloseCommand)
                {
                    OnClose();
                }
            } 
        }
        
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            string extention = Path.GetExtension(e.FullPath);
            if(extention.Equals(".jpg") || extention.Equals(".png") ||
                extention.Equals(".gif") || extention.Equals(".bmp"))
            {
                string[] args = new string[1];
                args[0] = m_path;
                m_controller.ExecuteCommand((int)CommandEnum.NewFileCommand, args, out bool resultSuccesful);
            }
        }

        private void OnClose()
        {
            m_dirWatcher.Dispose();
            DirectoryCloseEventArgs directoryClose = new DirectoryCloseEventArgs(m_path, "Close handler");
            DirectoryClose.Invoke(this, directoryClose);
        }
            
    }
}
