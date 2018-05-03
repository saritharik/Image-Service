using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Model
{
    class SettingsModel
    {
        private string output_directory;
        public string outputDirectory
        {
            get { return outputDirectory; }
            set
            {
                outputDirectory = value;
                OnPropertyChanged("Dirctory");
            }
        }

        private string source_name;
        public string sourceName
        {
            get { return source_name; }
            set
            {
                outputDirectory = value;
                OnPropertyChanged("Name");
            }
        }
    }
}
