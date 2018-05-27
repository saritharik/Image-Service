using ImageService.Commands;
using Infrastructure;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModal m_modal;
        private Dictionary<int, ICommand> commands;

        /// <summary>
        /// Constructor.
        /// define dictionary for the command. a command and the corresponding action.
        /// </summary>
        /// <param name="modal"></param>
        public ImageController(IImageServiceModal modal)
        {
            m_modal = modal;              // Storing the Modal Of The System
            commands = new Dictionary<int, ICommand>()
            {
                { (int)CommandEnum.NewFileCommand, new NewFileCommand(modal) }
            };
        }

        /// <summary>
        /// Execute the specific required command according to the action in the dictionary.
        /// </summary>
        /// <param name="commandID"></param> the identification number of the required command
        /// <param name="args"></param>
        /// <param name="resultSuccesful"></param> Update whether or not the action was successful
        /// <returns></returns> Will Return the New Path that return from the command if result = true,
        /// and otherwise will return the error message
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            String execute = commands[commandID].Execute(args, out resultSuccesful);
            return execute;
        }
    }
}
