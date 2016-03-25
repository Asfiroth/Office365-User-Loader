using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Office365.User.Loader.Models;
using Office365.User.Loader.Services;
using Syncfusion.XlsIO;

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

            if (string.IsNullOrEmpty(openFileDialog.FileName)) return;

            var filepathParts = openFileDialog.FileName.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);

            _fileName = filepathParts[filepathParts.Length - 1].ToLower();
            _filePath = openFileDialog.FileName;

            RaisePropertyChanged(nameof(FileName));
        }

        private void ProcessExcelExecute()
        {
            var userList = new List<OfficeUser>();
            using (var engine = new ExcelEngine())
            {
                var workbook = engine.Excel.Workbooks.Open(_filePath);
                workbook.Version = ExcelVersion.Excel2013;

                var sheet = workbook.Worksheets[0];
                var count = 0;
                var rd = new Random();

                foreach (var range in sheet.Rows)
                {
                    count++;
                    if(count == 1) continue;
                    userList.Add(new OfficeUser
                    {
                        Id = count,
                        UserName = CleanString(range["A" + count].Text),
                        Name = CleanString(range["B" + count].Text),
                        LastName = CleanString(range["C" + count].Text),
                        ShowOffName = CleanString(range["D" + count].Text),
                        Title = CleanString(range["E" + count].Text),
                        Department = CleanString(range["F" + count].Text),
                        Office = CleanString(range["G" + count].Text),
                        City = CleanString(range["H" + count].Text),
                        Country = CleanString(range["I" + count].Text),
                        Password = !_randomPassword ? CleanString(range["J" + count].Text) : GeneratePassword(rd),
                        License = CleanString(range["K" + count].Text),

                    });

                }
                workbook.Close();
            }
            _officeUsers = new ObservableCollection<OfficeUser>(userList);
            RaisePropertyChanged(nameof(OfficeUsers));
        }

        private void UploadUsersExecute()
        {
            
        }
        #endregion
        #region Private Methods

        private static string CleanString(string value)
        {
            const RegexOptions options = RegexOptions.None;
            var regex = new Regex("[ ]{2,}", options);
            value = regex.Replace(value, " ");

            return value.TrimStart()
                .TrimEnd()
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .Replace("ñ", "n");
        }

        private static string GeneratePassword(Random rdIndex)
        {
            const int passwordLength = 8;
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            var chars = new char[passwordLength];

            for (var i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rdIndex.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        #endregion
    }
}