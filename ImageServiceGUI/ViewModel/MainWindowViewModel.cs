using ImageServiceGUI.communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.ViewModel
{
    class MainWindowViewModel
    {
        private bool connected;
        /// <summary>
        /// Connected property.
        /// </summary>
        public bool Connected
        {
            get { return this.connected; }
            set
            {
                this.connected = value;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindowViewModel()
        {
            this.connected = ClientCommSingelton.getInstance().Connected;
        }



    }
}
