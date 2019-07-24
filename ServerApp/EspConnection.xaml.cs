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
            Server.NumEsp32 = esp32Devices.Count;
            //Register functions to events of background worker
            Server.DoWork += Server_DoWork;
            Server.ProgressChanged += Esp32ConfigurationCompleted;
            //Start server operations
            Server.RunWorkerAsync();
            KeyUp += CloseDialogBox;
            
        }

        private void CloseDialogBox(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E)
            {
                
                Localization locWindow = new Localization(Esp32Devices,Server);
                App.Current.MainWindow = locWindow;
                this.Close();
                this.Owner.Close();
                locWindow.Show();

            }
        }

        //First part of Server Work
        private void Esp32ConfigurationCompleted(object sender, ProgressChangedEventArgs e)
        {
            //All Esp32 has connected to server
            if(e.ProgressPercentage == 1)
            {
                Console.WriteLine("All Esp32 connected to Server");

                //Close dialog box
                Localization locWindow = new Localization(Esp32Devices,Server);
                App.Current.MainWindow = locWindow;
                this.Close();
                this.Owner.Close();
                locWindow.Show();

            }
        }

        //Events called when server starts operations
        private void Server_DoWork(object sender, DoWorkEventArgs e)
        {
            //Activate Server
            Server.Run();
        }

       


    }
}
