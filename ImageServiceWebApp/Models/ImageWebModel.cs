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

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "NumberPictuers")]
        public int NumberPictuers
        {
            get { return this.photosModel.GetNumberOfPhotos(); }
            set { this.photosModel.GetNumberOfPhotos(); }
        }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ServerConnected")]
        public string ServerConnected = ConnectedServer();

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Students")]
        public List<StudentsInfo> Students { get; set; }
        private PhotosModel photosModel;

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

       /* public static void UpdateOutputDir(object sender, DataRecivedEventArgs dataArgs)
        {
            if (dataArgs.CommandID == (int)CommandEnum.GetConfigCommand)
            {
                JObject configObj = JsonConvert.DeserializeObject<JObject>(dataArgs.Args);
                outputDir = (string)configObj["OutputDir"];
            }
        }*/
    }
}