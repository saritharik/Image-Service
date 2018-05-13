using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Communication
{
    public class DataRecivedEventArgs : EventArgs
    {
        // The Command ID
        public int CommandID { get; set; }

        public string[] Args { get; set; }

        public DataRecivedEventArgs(int id, string[] args)
        {
            CommandID = id;
            Args = args;
        }
    }
}
