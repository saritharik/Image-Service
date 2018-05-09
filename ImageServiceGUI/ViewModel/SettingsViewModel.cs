using ImageServiceGUI.Model;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageServiceGUI.ViewModel
{
    class SettingsViewModel
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public SettingsViewModel()
        {
            this.SettingsModel = new SettingsModel();
            SettingsModel.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e) {
                NotifyPropertyChanged(e.PropertyName);
            };

            this.RemoveCommand = new DelegateCommand<object>(this.OnRemove, this.CanRemove);
        }

        private ObservableCollection<String> handlers;
        public ObservableCollection<String> Handlers
        {
            get { return this.settingsModel.Handlers; }
        }

        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private SettingsModel settingsModel;
        public SettingsModel SettingsModel
        {
            get { return this.settingsModel; }
            set
            {
                this.settingsModel = value;
            }
        }

        public ICommand RemoveCommand { get; private set; }

        private void OnRemove(object obj)
        {
            settingsModel.RemoveHandlerCommand();
            this.settingsModel.SelectedHandler = null;
        }

        private bool CanRemove(object obj)
        {
            if (string.IsNullOrEmpty(this.settingsModel.SelectedHandler))
            {
                return false;
            }
            return true;
        }
    }
}