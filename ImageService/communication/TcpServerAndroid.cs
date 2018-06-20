using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.communication
{
    class TcpServerAndroid
    {
        #region members
        private int port;
        TcpListener listener;
        TcpClient client;
        NetworkStream stream;
        //BinaryWriter writer;
        BinaryReader reader;
        string handlersList;
        string[] handlers;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public TcpServerAndroid()
        {
            this.port = 8888;
            handlersList = ConfigurationManager.AppSettings["Handler"];
            handlers = handlersList.Split(';');
            StartTcpCommunication();
        }

        /// <summary>
        /// Start tcp communication.
        /// </summary>
        public void StartTcpCommunication()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.port);
            this.listener = new TcpListener(ep);
            Console.WriteLine("try something");
            this.listener.Start();
            Console.WriteLine("Waiting for connections...");
            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        Console.WriteLine("Got new connection");
                        new Task(() =>
                        {
                            while (true)
                            {
                                receiveMessage(client);
                            }
                        }).Start();
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
                Console.WriteLine("Server stopped");
            });
            task.Start();
        }

        /// <summary>
        /// Rececive message from the client.
        /// </summary>
        /// <param name="client">the client that send the message</param>
        /// <returns>the message</returns>
        public void receiveMessage(TcpClient client)
        {
            try
            {
                stream = client.GetStream();
                reader = new BinaryReader(stream);

                // read the size of the image
                byte[] sizeMessage = reader.ReadBytes(4);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(sizeMessage);
                }
                if (sizeMessage == null)
                {
                    return;
                }
                int size = BitConverter.ToInt32(sizeMessage, 0);

                // raed the image and convert it to Image
                byte[] message = reader.ReadBytes(size);
                Image image = (Bitmap)((new ImageConverter()).ConvertFrom(message));

                // read the size of the name of the image
                sizeMessage = reader.ReadBytes(4);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(sizeMessage);
                }
                size = BitConverter.ToInt32(sizeMessage, 0);

                // read the name of the image
                byte[] imageName = reader.ReadBytes(size);
                string name = Encoding.UTF8.GetString(imageName, 0, imageName.Length);

                // add the image to the handlers
                AddImage(image, name);
            }
            catch (IOException)
            {
            }
        }

        /// <summary>
        /// Move images to handler directory.
        /// </summary>
        /// <param name="image">to move</param>
        /// <param name="name">of image</param>
        private void AddImage(Image image, String name)
        {
            for (int i = 0; i < handlers.Length; i++)
            {
                if (Directory.Exists(handlers[i]))
                {
                    image.Save(handlers[0] + "/" + name);//+ ".jpg");
                    break;
                }
            }
        }
    }
}
