using ImageServiceGUI.Model;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageServiceGUI.ViewModel
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
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
        /// <summary>
        /// Handlers property.
        /// </summary>
        public ObservableCollection<String> Handlers
        {
            get { return this.settingsModel.Handlers; }
        }

        /// <summary>
        /// NotifyPropertyChanged event.
        /// </summary>
        /// <param name="name"></param>
        protected void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private SettingsModel settingsModel;
        /// <summary>
        /// SettingsModel property.
        /// </summary>
        public SettingsModel SettingsModel
        {
            get
            {
                return this.settingsModel; }
            set { }
        }

        /// <summary>
        /// OutputDirectory property.
        /// </summary>
        public string OutputDirectory
        {
            get {
                //Thread.Sleep(1100);
                return this.settingsModel.OutputDirectory; }
            set { }
        }

        /// <summary>
        /// SourceName property.
        /// </summary>
        public string SourceName
        {
            get
            {
                return this.settingsModel.SourceName; }
            set { }
        }

        /// <summary>
        /// LogName property.
        /// </summary>
        public string LogName
        {
            get
            {
                return this.settingsModel.LogName; }
            set { }
        }

        /// <summary>
        /// ThumbnailSize property.
        /// </summary>
        public string ThumbnailSize
        {
            get
            {
                return this.settingsModel.ThumbnailSize; }
            set { }
        }

        /// <summary>
        /// SelectedHandler property.
        /// </summary>
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

        /// <summary>
        /// RemoveCommand property.
        /// </summary>
        public ICommand RemoveCommand { get; private set; }

        /// <summary>
        /// Propert changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //var command = this.RemoveCommand as DelegateCommand<object>;
            //command.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// On remove function, activated the function that remove handler.
        /// </summary>
        /// <param name="obj"></param>
        private void OnRemove(object obj)
        {
            SettingsModel.RemoveHandlerCommand();
            this.SelectedHandler = null;
        }

        /// <summary>
        /// Can Remove function, activated the remove button when handler selected.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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