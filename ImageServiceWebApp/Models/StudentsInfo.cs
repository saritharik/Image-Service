using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Hosting;

namespace ImageServiceWebApp.Models
{
    public class StudentsInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public StudentsInfo() {}

        /// <summary>
        /// ID.
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ID")]
        public string ID { get; set; }

        /// <summary>
        /// FirstName.
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// LastName.
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "LastName")]
        public string LastName { get; set; }
    }
}