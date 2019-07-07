using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    public class Esp32Label
    {
        public Esp32Label(double x, double y, string mac)
        {
            Mac = mac;
            X = x;
            Y = y;
        }
        public string Mac { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
