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
    public class DirectoryHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        #endregion

        /// <summary>
        /// The Event That Notifies that the Directory is being closed
        /// </summary>
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;

        /// <summary>
        /// The directory handler listen to a folder and execute the commands that it recieves.
        /// </summary>
        /// <param name="directory"></param> the path of the folder
        /// <param name="controller"></param> the controller execute the specific command
        /// <param name="logging"></param> a logger to update the seccess or failed missions.
        public DirectoryHandler(String directory, IImageController controller, ILoggingService logging)
        {
            m_controller = controller;
            m_path = directory;
            m_logging = logging;
        }

        /// <summary>
        /// The Function Recieves the directory to Handle 
        /// </summary>
        /// <param name="dirPath"></param> get the path of the folder
        public void StartHandleDirectory(string dirPath)
        {
            m_dirWatcher = new FileSystemWatcher(dirPath);
            m_dirWatcher.EnableRaisingEvents = true;
            try
            {
                m_dirWatcher.Created += new FileSystemEventHandler(OnChanged);
            } catch (Exception e)
            {
            }
        }

        /// <summary>
        /// The Event that will be activated upon new Command
        /// </summary>
        /// <param name="sender"></param> the specific object that send the command.
        /// <param name="e"></param> the arguments of the command
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            // the case that the command is 'close'.
                if (e.CommandID == (int)CommandEnum.CloseCommand)
                {
                    if (e.RequestDirPath == m_path || e.RequestDirPath == null)
                    {
                        OnClose();
                    }
                }
        }
        
        /// <summary>
        /// This function called when something changed in the folder.
        /// In our case- a new file is in the folder.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param> the data on the change that occured.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // check if the new file is a picture with extention that we want to move.
            string extention = Path.GetExtension(e.FullPath);
            if(extention.Equals(".jpg") || extention.Equals(".png") ||
                extention.Equals(".gif") || extention.Equals(".bmp"))
            {
                string[] args = new string[1];
                args[0] = e.FullPath;
                m_controller.ExecuteCommand((int)CommandEnum.NewFileCommand, args, out bool resultSuccesful);
                if (resultSuccesful)
                {
                    m_logging.Log("Picture passed successfully", MessageTypeEnum.INFO);
                } else
                {
                    m_logging.Log("Picture passed failed", MessageTypeEnum.FAIL);
                }
            }
        }

        /// <summary>
        /// This function is called when the server is closed.
        /// </summary>
        private void OnClose()
        {
            // stop the watching on the folder
            m_dirWatcher.EnableRaisingEvents = false;
            m_dirWatcher.Dispose();
            DirectoryCloseEventArgs directoryClose = new DirectoryCloseEventArgs(m_path, "Close handler");
            DirectoryClose?.Invoke(this, directoryClose);
        }
            
    }
}
