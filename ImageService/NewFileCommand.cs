using ImageService.Infrastructure;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModal m_modal;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="modal"> get IImageServiceModal to execute the commands
        public NewFileCommand(IImageServiceModal modal)
        {
            m_modal = modal;            // Storing the Modal
        }

        /// <summary>
        /// execute the given command by the modal.
        /// </summary>
        /// <param name="args"> array with the command
        /// <param name="result"> out- to update success or fail
        /// <returns> The String Will Return the New Path if result = true,
        /// and otherwise will return the error message
        public string Execute(string[] args, out bool result)
        {
            // 
            string executeResult = m_modal.AddFile(args[0], out result);
            return executeResult;
        }
    }
}
