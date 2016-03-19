using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Office365.User.Loader.Models;
using Office365.User.Loader.Services;

namespace Office365.User.Loader.ViewModel
{
    public class LoadPageVm : ViewModelBase
    {
        #region Members

        private ICommand _selectFileCommand;
        private ICommand _processCommand;
        private ICommand _uploadCommand;
        private string _fileName;
        private bool _randomPassword;
        private string _filePath;
        private ObservableCollection<OfficeUser> _officeUsers;
        private ILoaderNavigationService _navigationService;
        #endregion
        #region Properties

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged();
            }
        }

        public bool RandomPassword
        {
            get { return _randomPassword;}
            set
            {
                _randomPassword = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<OfficeUser> OfficeUsers
        {
            get { return _officeUsers; }
            set
            {
                _officeUsers = value;
                RaisePropertyChanged();
            }
        } 
        #endregion

        #region Command Properties

        public ICommand SelectFileCommand
            => _selectFileCommand ?? (_selectFileCommand = new RelayCommand(SelectFileExecute));

        public ICommand ProcessCommand => _processCommand ?? (_processCommand = new RelayCommand(ProcessExcelExecute));
        public ICommand UploadCommand => _uploadCommand ?? (_uploadCommand = new RelayCommand(UploadUsersExecute));
        #endregion

        #region Constructor

        public LoadPageVm(ILoaderNavigationService navigationService)
        {
            if(IsInDesignMode)return;
            _navigationService = navigationService;
        }
        #endregion
        #region Command Methods

        private void SelectFileExecute()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                DefaultExt = "*.xlsx",
                Filter = "Libro de Excel (*.xlsx) | *.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            openFileDialog.ShowDialog();
            if(string.IsNullOrEmpty(openFileDialog.FileName)) return;
            _fileName =
                openFileDialog.FileName.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries)[
                    openFileDialog.FileName.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries).Length - 1].ToLower();
            _filePath = openFileDialog.FileName;
            RaisePropertyChanged("FileName");
        }

        private void ProcessExcelExecute()
        {
            
        }

        private void UploadUsersExecute()
        {
            
        }
        #endregion
        #region Private Methods
        #endregion
    }
}