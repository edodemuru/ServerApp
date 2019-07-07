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
        public Localization(List<Device> Esp32Devices)
        {
            InitializeComponent();
            this.Esp32Devices = Esp32Devices;
            Esp32Render = new ChartValues<Esp32Label>();
          

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
                Esp32Render.Add(new Esp32Label(Esp32Devices[i].X, Esp32Devices[i].Y, Esp32Devices[i].Mac));
            }

            var Esp32Mapper = Mappers.Xy<Esp32Label>()
                .X(value=> value.X) //let's use the position of the item as X
                .Y(value => value.Y); //And Y property as y
            //lets save the mapper globally
            Charting.For<Esp32Label>(Esp32Mapper);


        }

       private void DrawEsp32Info(object Sender, MouseEventArgs e)
        {
           Point coordinate =  e.GetPosition((IInputElement) Sender);
  
            

        }

        public ChartValues<Esp32Label> Esp32Render { get; set; }





    }
}
