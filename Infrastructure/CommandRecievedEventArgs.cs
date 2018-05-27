using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class CommandRecievedEventArgs : EventArgs
    {
        // The Command ID
        public int CommandID
        {
            get /*{ return CommandID; }*/;
            set /*{ CommandID = value; }*/;
        }

        public string[] Args
        {
            get /*{ return Args; }*/;
            set /*{ Args = value; }*/;
        }

        // The Request Directory
        public string RequestDirPath
        {
            get /*{ return RequestDirPath; }*/;
            set /*{ RequestDirPath = value; }*/;
        }  

        /// <summary>
        /// The properties of the command that recieved.
        /// </summary>
        /// <param name="id"></param> the identifier number
        /// <param name="args"></param> the command
        /// <param name="path"></param> the required path for the command
        public CommandRecievedEventArgs(int id, string[] args, string path)
        {
            CommandID = id;
            Args = args;
            RequestDirPath = path;
        }
    }
}
