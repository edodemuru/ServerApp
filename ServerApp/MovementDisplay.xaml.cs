using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
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
    /// Logica di interazione per MovementDisplay.xaml
    /// </summary>
    public partial class MovementDisplay : Window
    {

        List<Device> Esp32Devices;
        List<Device> Devices;
        SnifferServer Server;




        public MovementDisplay(List<Device> Esp32Devices, SnifferServer ServerInput)
        {
            InitializeComponent();

            this.Esp32Devices = Esp32Devices;
            this.Server = ServerInput;
            Esp32Render = new ChartValues<DeviceLabel>();
            DevicesRenderer = new ChartValues<DeviceLabel>();
            NumDevicesInterval = new ChartValues<int>();
            NumDevicesInterval.Add(0);
            DrawEsp32(Esp32Devices);
            DataContext = this;
        }

        void DrawEsp32(List<Device> Esp32Devices)
        {

            for (int i = 0; i < Esp32Devices.Count; i++)
            {
                Esp32Render.Add(new DeviceLabel(Esp32Devices[i].X, Esp32Devices[i].Y, Esp32Devices[i].Mac, ""));
            }



            var Esp32Mapper = Mappers.Xy<Esp32Label>()
                .X(value => value.X) //let's use the position of the item as X
                .Y(value => value.Y); //And Y property as y
            //lets save the mapper globally
            Charting.For<Esp32Label>(Esp32Mapper);

            var DeviceMapper = Mappers.Xy<DeviceLabel>()
                   .X(value => value.X)
                   .Y(value => value.Y);

            Charting.For<DeviceLabel>(DeviceMapper);




        }

        private void DateChanged(object sender, EventArgs e)
        {
            /* String format = "ddd MMM dd HH:mm:ss yyyy";
             DateTime dt = (DateTime) Interval1.Value;
             Console.WriteLine(dt.ToString(format));*/
            //Console.WriteLine( Server.ConvertStringToDateTime(Interval1.Value.ToString()).ToString());
            if (Interval1.Value != null && Interval2.Value != null)
            {
                DateTime start = (DateTime)Interval1.Value;
                DateTime end = (DateTime)Interval2.Value;

                //Clear previously data
                if (Devices != null)
                    Devices.Clear();

                Devices = Server.GetPositionInInterval(start, end);

                foreach (Device d in Devices)
                {
                    DevicesRenderer.Add(new DeviceLabel(d.X, d.Y, d.Mac, "SSID: " + d.Ssid));
                }

            }
        }

        public void AnimationUpdated(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        //Property to set value of render elements
        public ChartValues<DeviceLabel> Esp32Render { get; set; }
        public ChartValues<DeviceLabel> DevicesRenderer { get; set; }
        public ChartValues<int> NumDevicesInterval { get; set; }

    }
}
