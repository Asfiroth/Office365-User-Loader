using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Office365.User.Loader.Windows;

namespace Office365.User.Loader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var splash = new StartWindow();
            splash.Show();

            var worker = new BackgroundWorker {WorkerReportsProgress = true};
            worker.DoWork += delegate
            {
                worker.ReportProgress(1, "Verificando conexión a red...");
                var isNetworkEnabled = NetworkInterface.GetIsNetworkAvailable();
                Thread.Sleep(2000);
                if (isNetworkEnabled)
                {
                    worker.ReportProgress(1, "Estableciendo conexión...");
                    Thread.Sleep(2000);
                }
                else
                {
                    worker.ReportProgress(1, "Usted no cuenta con conexión a red...");
                    Thread.Sleep(2000);
                }
            };
            worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs args)
            {
                splash.Loader.Text = (string) args.UserState;
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                var isNetworkEnabled = NetworkInterface.GetIsNetworkAvailable();
                if (isNetworkEnabled)
                {
                    var main = new MainWindow();
                    main.Show();
                }
                splash.Close();
            };
            worker.RunWorkerAsync();
            
            
            
            base.OnStartup(e);
            

        }
    }
}
