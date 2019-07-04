using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;



namespace ServerApp
{
    /// <summary>
    /// Logica di interazione per EspConnection.xaml
    /// </summary>
    public partial class EspConnection : Window
    {
        List<Device> Esp32Devices;
        SnifferServer Server;

        public EspConnection()
        {
            InitializeComponent();
          
        }

        public EspConnection(List<Device> esp32Devices)
        {
            InitializeComponent();
            Esp32Devices = new List<Device>();
            Esp32Devices = esp32Devices;
            //Create new thread which supports cancellation and report progress, all controlled by events
            Server = new SnifferServer(Esp32Devices) {
                WorkerReportsProgress = true,

                WorkerSupportsCancellation = true
                
            };
            //Register functions to events of background worker
            Server.DoWork += Server_DoWork;
            Server.ProgressChanged += Esp32ConfigurationCompleted;

            Server.RunWorkerAsync();
        }

        private void CloseDialogBox(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.E)
            //{
                
                MainWindow mainWindow = new MainWindow();
                App.Current.MainWindow = mainWindow;
                this.Close();
                this.Owner.Close();
                mainWindow.Show();

            //}
        }

        //First part of Server Work
        private void Esp32ConfigurationCompleted(object sender, ProgressChangedEventArgs e)
        {
            //All Esp32 has connected to server
            if(e.ProgressPercentage == 1)
            {
                Console.WriteLine("All Esp32 connected to Server");

                //Close dialog box
                MainWindow mainWindow = new MainWindow();
                App.Current.MainWindow = mainWindow;
                this.Close();
                this.Owner.Close();
                mainWindow.Show();

            }
        }

        private void Server_DoWork(object sender, DoWorkEventArgs e)
        {
            //Activate Server
            Server.Run();
        }

       


    }
}
