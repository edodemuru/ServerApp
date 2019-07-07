using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    public class DeviceLabel
    {
        public DeviceLabel(double x, double y, string mac, string ssid)
        {
            Mac = mac;
            X = x;
            Y = y;
            Ssid = ssid;
        }
        public string Mac { get; set; }
        public string Ssid { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
