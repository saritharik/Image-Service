using ImageServiceWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageServiceWebApp.Controllers
{
    public class ImageWebController : Controller
    {
        static List<StudentsInfo> students = new List<StudentsInfo>()
        {
            new StudentsInfo {ID = 314759499 , LastName  = "Emanuel", FirstName = "Tamar"},
            new StudentsInfo {ID = 315788547 , LastName  = "Harik", FirstName = "Sarit"}
        };
        // GET: ImageWeb
        [HttpGet]
        public ActionResult Index()
        {
            return View(students);
        }

        public ActionResult Config()
        {
            return View();
        }
    }
}