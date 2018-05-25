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
    class SettingsViewModel : INotifyPropertyChanged
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public SettingsViewModel()
        {
            this.settingsModel = new SettingsModel();
            settingsModel.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e) {
                NotifyPropertyChanged(e.PropertyName);
                settingsModel.PropertyChanged += propertyChanged;
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private SettingsModel settingsModel;
        public SettingsModel SettingsModel
        {
            get { return this.settingsModel; }
            set
            {
                //this.SettingsModel = value;
            }
        }

        public string OutputDirectory
        {
            get { return this.settingsModel.OutputDirectory; }
            set { }
        }

        public string SourceName
        {
            get { return this.settingsModel.SourceName; }
            set { }
        }

        public string LogName
        {
            get { return this.settingsModel.LogName; }
            set { }
        }

        public string ThumbnailSize
        {
            get { return this.settingsModel.ThumbnailSize; }
            set { }
        }

        public string SelectedHandler
        {
            get { return this.SettingsModel.SelectedHandler; }
            set
            {
                this.SettingsModel.SelectedHandler = value;

                var command = this.RemoveCommand as DelegateCommand<object>;
                command.RaiseCanExecuteChanged();
            }
        }

        public ICommand RemoveCommand { get; private set; }

        private void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //var command = this.RemoveCommand as DelegateCommand<object>;
            //command.RaiseCanExecuteChanged();
        }

        private void OnRemove(object obj)
        {
            SettingsModel.RemoveHandlerCommand();
            this.SelectedHandler = null;
        }

        private bool CanRemove(object obj)
        {
            if (string.IsNullOrEmpty(this.SelectedHandler))
            {
                return false;
            }
            return true;
        }
    }
}