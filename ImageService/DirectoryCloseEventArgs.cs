using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    public class DirectoryCloseEventArgs : EventArgs
    {
        public string DirectoryPath
        {
            get { return DirectoryPath; }
            set { DirectoryPath = value; }
        }

        // The Message That goes to the logger
        public string Message
        {
            get { return Message; }
            set { Message = value; }
        }             

        public DirectoryCloseEventArgs(string dirPath, string message)
        {
            DirectoryPath = dirPath;                    // Setting the Directory Name
            Message = message;                          // Storing the String
        }

    }
}
