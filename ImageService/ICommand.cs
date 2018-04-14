using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// The handler execute the command
        /// </summary>
        /// <param name="args"></param> the args for the specific command
        /// <param name="result"></param> value that indicates the successful of the command
        /// <returns></returns> return a string of the new path if result = true,
        /// and message error otherwise.
        string Execute(string[] args, out bool result);          // The Function That will Execute The 
    }
}
