using ImageServiceWebApp.communication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace ImageServiceWebApp.Models
{
    public class ImageWebModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ImageWebModel()
        {
            photosModel = new PhotosModel();
            string[] studentsInfo = 
                System.IO.File.ReadAllLines(HostingEnvironment.MapPath(@"~/App_Data/InfoStudents.txt"));
            this.Students = new List<StudentsInfo>()
            {
                new StudentsInfo {ID = studentsInfo[0] , LastName  = studentsInfo[1], FirstName = studentsInfo[2]},
                new StudentsInfo {ID = studentsInfo[3] , LastName  = studentsInfo[4], FirstName = studentsInfo[5]}
            };
        }

        /// <summary>
        /// NumberPictuers
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "NumberPictuers")]
        public int NumberPictuers
        {
            get { return this.photosModel.GetNumberOfPhotos(); }
            set { this.photosModel.GetNumberOfPhotos(); }
        }

        /// <summary>
        /// ServerConnected
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ServerConnected")]
        public string ServerConnected = ConnectedServer();

        /// <summary>
        /// photosModel
        /// </summary>
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Students")]
        public List<StudentsInfo> Students { get; set; }
        private PhotosModel photosModel;

        /// <summary>
        /// Check if server connected.
        /// </summary>
        /// <returns></returns>
        public static string ConnectedServer()
        {
            if(ClientCommSingelton.getInstance().Connected)
            {
                return "Service Connected";
            } else
            {
                return "Service Unconnected";
            }
        }
    }
}