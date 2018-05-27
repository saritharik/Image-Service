using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public class DataRecivedEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the command.
        /// </summary>
        public int CommandID { get; set; }

        /// <summary>
        /// The arguments to command.
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"> The ID of the command</param>
        /// <param name="args">The arguments to command</param>
        public DataRecivedEventArgs(int id, string args)
        {
            CommandID = id;
            Args = args;
        }
    }
}
