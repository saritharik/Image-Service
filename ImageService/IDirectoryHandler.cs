using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;

namespace ImageService.Controller.Handlers
{
    public interface IDirectoryHandler
    {
        /// <summary>
        /// The Event That Notifies that the Directory is being closed
        /// </summary>
        event EventHandler<DirectoryCloseEventArgs> DirectoryClose;

        /// <summary>
        /// The Function Recieves the directory to Handle
        /// </summary>
        /// <param name="dirPath"></param> the path
        void StartHandleDirectory(string dirPath);

        /// <summary>
        ///  The Event that will be activated upon new Command
        /// </summary>
        /// <param name="sender"></param> the specific object that send the command
        /// <param name="e"></param> the parameters for the command
        void OnCommandRecieved(object sender, CommandRecievedEventArgs e);
    }
}
