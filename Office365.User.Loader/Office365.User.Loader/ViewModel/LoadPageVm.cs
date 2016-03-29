using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        private ICommand _acceptCommand;
        private ICommand _cancelCommand;
        private string _tenantAdminName;
        private string _tenantAdminPassword;
        private string _fileName;
        private bool _randomPassword;
        private bool _isActive;
        private bool _isLoading;
        private bool _forceChangePassword;
        private string _filePath;
        private string _loadingMessage;
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

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                RaisePropertyChanged();
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged();
            }
        }

        public string LoadingMessage
        {
            get { return _loadingMessage;}
            set
            {
                _loadingMessage = value;
                RaisePropertyChanged();
            }
        }

        public bool ForceChangePassword
        {
            get { return _forceChangePassword; }
            set
            {
                _forceChangePassword = value;
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

        public string TenantAdminName
        {
            get { return _tenantAdminName; }
            set
            {
                _tenantAdminName = value;
                RaisePropertyChanged();
            }
        }

        #endregion
        #region Command Properties

        public ICommand SelectFileCommand
            => _selectFileCommand ?? (_selectFileCommand = new RelayCommand(SelectFileExecute));

        public ICommand ProcessCommand => _processCommand ?? (_processCommand = new RelayCommand(ProcessExcelExecute));
        public ICommand UploadCommand => _uploadCommand ?? (_uploadCommand = new RelayCommand(UploadUsersExecute));
        public ICommand AcceptCommand => _acceptCommand ?? (_acceptCommand = new RelayCommand<object>(LoginTenantExecute));
        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(CancelExecute));
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

            FileName = filepathParts[filepathParts.Length - 1].ToLower();
            _filePath = openFileDialog.FileName;
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
                        UsageLocation = "PE",
                        Status = OfficeUserStatus.NotLoaded
                    });

                }
                workbook.Close();
            }
            OfficeUsers = new ObservableCollection<OfficeUser>(userList);
        }

        private void UploadUsersExecute()
        {
            IsActive = true;
        }

        private void LoginTenantExecute(object passwordBox)
        {
            var box = passwordBox as PasswordBox;
            if (box != null) _tenantAdminPassword = box.Password;
            var adminData = new[] {_tenantAdminName, _tenantAdminPassword};
            if (adminData.Any(string.IsNullOrEmpty))
            {
                MessageBox.Show("Debes ingresar los datos del Administrador del Tenant para Continuar", "Error", MessageBoxButton.OK);
                return;
            }

            try
            {
                IsActive = false;
                IsLoading = true;
                LoadingMessage = "Validando Credenciales...";
                var worker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true };
                worker.DoWork += (o, e) =>
                {
                    var session = InitialSessionState.CreateDefault();
                    session.ImportPSModule(new[] { "MSOnline" });
                    using (var runSpace = RunspaceFactory.CreateRunspace(session))
                    {
                        runSpace.Open();

                        var pipe = runSpace.CreatePipeline();

                        var connectCommand = new Command("Connect-MsolService");
                        connectCommand.Parameters.Add(
                            (new CommandParameter("Credential",
                                new PSCredential(_tenantAdminName, ToSecureString(_tenantAdminPassword)))));

                        pipe.Commands.Add(connectCommand);

                        pipe.Invoke();

                        if(pipe.Error.Count != 0)
                        {
                            e.Cancel = true;
                            return;
                        }
                        worker.ReportProgress(1, "Iniciando proceso de carga...");
                        Thread.Sleep(2000);
                        var count = 0;
                        foreach (var officeUser in OfficeUsers)
                        {
                            count++;
                            var userCommand = new Command("New-MsolUser");
                            userCommand.Parameters.Add((new CommandParameter("UserPrincipalName", officeUser.UserName)));
                            userCommand.Parameters.Add((new CommandParameter("DisplayName", officeUser.ShowOffName)));
                            userCommand.Parameters.Add((new CommandParameter("FirstName", officeUser.Name)));
                            userCommand.Parameters.Add((new CommandParameter("LastName", officeUser.LastName)));
                            userCommand.Parameters.Add((new CommandParameter("Title", officeUser.Title)));
                            userCommand.Parameters.Add((new CommandParameter("Country", officeUser.Country)));
                            userCommand.Parameters.Add((new CommandParameter("Department", officeUser.Department)));
                            userCommand.Parameters.Add((new CommandParameter("Office", officeUser.Office)));
                            userCommand.Parameters.Add((new CommandParameter("City", officeUser.City)));
                            userCommand.Parameters.Add((new CommandParameter("Password", officeUser.Password)));
                            userCommand.Parameters.Add((new CommandParameter("LicenseAssignment", officeUser.License)));
                            userCommand.Parameters.Add((new CommandParameter("UsageLocation", officeUser.UsageLocation)));
                            userCommand.Parameters.Add((new CommandParameter("ForceChangePassword", _forceChangePassword)));

                            worker.ReportProgress(count, $"Cargados {count} de {OfficeUsers.Count}...");
                        }
                    }
                };
                worker.ProgressChanged += (s, a) =>
                {
                    LoadingMessage = (string) a.UserState;

                };
                worker.RunWorkerCompleted += (s, a) =>
                {
                    if (a.Cancelled)
                    {
                        MessageBox.Show(
                                "Ocurrió un error durante el proceso de conexión, por favor revise las credenciales",
                                "Loader", MessageBoxButton.OK);
                        IsLoading = false;
                        IsActive = true;
                        return;
                    }
                    LoadingMessage = "Gracias...";
                    IsLoading = false;
                    Cleanup();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error, por favor comuniquese con su administrador de sistemas...");
                throw new Exception(ex.Message);
            }

        }

        private void CancelExecute()
        {
            IsActive = false;
            Cleanup();
        }
        #endregion
        #region Private Methods

        private static string CleanString(string value)
        {
            var regex = new Regex("[ ]{2,}", RegexOptions.None);
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

        private static SecureString ToSecureString(string plainPassword)
        {
            var securePass = new SecureString();
            foreach (var secureChar in plainPassword)
            {
                securePass.AppendChar(secureChar);
            }
            return securePass;
        }

        public override void Cleanup()
        {

            base.Cleanup();
        }

        #endregion
    }
}