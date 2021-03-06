﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageServiceWebApp.Models
{
    public class PhotoInfo
    {
        #region Properties
        public string FullPath { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int ID { get; set; }
        public string Directory { get; set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public PhotoInfo(){}
    }
}