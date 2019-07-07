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
using InteractiveDataDisplay;
using InteractiveDataDisplay.WPF;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;

namespace ServerApp
{
    /// <summary>
    /// Logica di interazione per Window1.xaml
    /// </summary>
    public partial class Localization : Window
    {
        List<Device> Esp32Devices;
        List<Device> Devices;
        SnifferServer Server;
        public Localization(List<Device> Esp32Devices,SnifferServer ServerInput)
        {
            InitializeComponent();
            this.Esp32Devices = Esp32Devices;
            this.Server = ServerInput;
            Server.ProgressChanged += Esp32CoordinateObtained;

            Esp32Render = new ChartValues<DeviceLabel>();
            DevicesRenderer = new ChartValues<DeviceLabel>();
            DrawEsp32(Esp32Devices);



            /*for (var i = 0; i < 20; i++)
            {
                ValuesA.Add(new ObservablePoint(r.NextDouble() * 10, r.NextDouble() * 10));
                ValuesB.Add(new ObservablePoint(r.NextDouble() * 10, r.NextDouble() * 10));
                ValuesC.Add(new ObservablePoint(r.NextDouble() * 10, r.NextDouble() * 10));
            }

            */
            DataContext = this;

        }

       


        void DrawEsp32(List<Device> Esp32Devices)
        {
            
            for(int i = 0; i<Esp32Devices.Count; i++)
            {
                Esp32Render.Add(new DeviceLabel(Esp32Devices[i].X, Esp32Devices[i].Y, Esp32Devices[i].Mac,""));
            }

           

            var Esp32Mapper = Mappers.Xy<Esp32Label>()
                .X(value=> value.X) //let's use the position of the item as X
                .Y(value => value.Y); //And Y property as y
            //lets save the mapper globally
            Charting.For<Esp32Label>(Esp32Mapper);

            var DeviceMapper = Mappers.Xy<DeviceLabel>()
                   .X(value => value.X)
                   .Y(value => value.Y);

            Charting.For<DeviceLabel>(DeviceMapper);




        }

        public void Esp32CoordinateObtained(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 3)
            {
                Console.WriteLine("Obtained data about devices position");
                //Clear previously data
               if(Devices!= null)
                    Devices.Clear();

                Devices = Server.List_Devices;

                foreach(Device d in Devices)
                {
                    DevicesRenderer.Add(new DeviceLabel(d.X, d.Y, d.Mac, "SSID: " + d.Ssid));
                }

                
                


            }
        }

        //Property to set value of render elements
        public ChartValues<DeviceLabel> Esp32Render { get; set; }
        public ChartValues<DeviceLabel> DevicesRenderer { get; set; }





    }
}
