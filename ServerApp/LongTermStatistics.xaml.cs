using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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
    /// Logica di interazione per LongTermStatistics.xaml
    /// </summary>
    public partial class LongTermStatistics : Window
    {
        SnifferServer Server;
       // Save all date time values once for convertion into x values
       private List<DateTime> TimeIntervalGlobal;
       //List of tuple containint all interval to be processed
       private List<Tuple<string, DateTime>> GlobalInterval;
        
        public LongTermStatistics(SnifferServer ServerInput)
        {
            InitializeComponent();


            Server = ServerInput;
            
            TimeIntervalGlobal = new List<DateTime>();
            GlobalInterval = new List<Tuple<string, DateTime>>();
            Mac = new List<string>();
            Time = new List<string>();
            SeriesCollection = new SeriesCollection();           

         

            DataContext = this;


        }

        private void UpdateTimeline(List<Tuple<string,DateTime>> NewInterval)
        {
            //New series collection to update actual series collection
            SeriesCollection SC = new SeriesCollection();
          
            List<LineSeries> ListOfMacLines = new List<LineSeries>();
            GlobalInterval.Clear();
            Mac.Clear();
            Time.Clear();
            SeriesCollection.Clear();
            TimeIntervalGlobal.Clear();
            //Insert new intervals
            GlobalInterval.AddRange(NewInterval);
            //Sort all list of itervals
            GlobalInterval.Sort((x, y) => DateTime.Compare(x.Item2, y.Item2));
            
            //Dictionary containing the first part of the interval detected inside global interval
            Dictionary<string, DateTime> InitializedPoints = new Dictionary<string, DateTime>();
            foreach (Tuple<string, DateTime> t in GlobalInterval)
            {
                //Check if mac is new or inside list
                if (!Mac.Contains(t.Item1))
                {
                    //Mac is new, is not inside list
                    Mac.Add(t.Item1);
                    LineSeries ls = new LineSeries();
                    ls.Title = t.Item1;
                    ls.Values = new ChartValues<ObservablePoint>();
                    ListOfMacLines.Add(ls);
                }
                //Check if interval is already in graph
                if (!TimeIntervalGlobal.Contains(t.Item2))
                {
                    //Time value is not inside graph
                    UpdateTimeInterval(t.Item2);
                }

                //Check if first part of interval is already inside dictionary
                if (!InitializedPoints.ContainsKey(t.Item1))
                {
                    //First part of interval has been found
                    InitializedPoints.Add(t.Item1, t.Item2);
                }
                else
                {
                    //Last part of interval has been found
                    ObservablePoint p1 = new ObservablePoint(TimeIntervalGlobal.IndexOf(InitializedPoints[t.Item1]), Mac.IndexOf(t.Item1));
                    ObservablePoint p2 = new ObservablePoint(TimeIntervalGlobal.IndexOf(t.Item2), Mac.IndexOf(t.Item1));
                    //List of observable point
                    ChartValues<ObservablePoint> Values = new ChartValues<ObservablePoint>();
                    Values.Add(new ObservablePoint(double.NaN, double.NaN));
                    Values.Add(p1);
                    Values.Add(p2);
                    Values.Add(new ObservablePoint(double.NaN,double.NaN));
                    //Insert interval inside LineSeries
                    ListOfMacLines.Find(x => x.Title.CompareTo(t.Item1) == 0).Values = Values;
                }
                

               


            }
            SeriesCollection.Clear();

            foreach(LineSeries ls in ListOfMacLines)
            {
                SeriesCollection.Add(ls);

            }
            
            

        }

        private void UpdateTimeInterval(DateTime time)
        {
            //Insert new time
            TimeIntervalGlobal.Add(time);
            //Sort all values
            TimeIntervalGlobal.Sort((x, y) => DateTime.Compare(x, y));
            //List of datetime values, converted in string to display them in graph
            Time.Clear();
            foreach(DateTime t in TimeIntervalGlobal)
            {
                Time.Add(t.ToString());
            }
        }

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Time { get; set; }
        public List<string> Mac { get; set; }

        private void DateChanged(object sender, EventArgs e) {
           /* String format = "ddd MMM dd HH:mm:ss yyyy";
            DateTime dt = (DateTime) Interval1.Value;
            Console.WriteLine(dt.ToString(format));*/
            //Console.WriteLine( Server.ConvertStringToDateTime(Interval1.Value.ToString()).ToString());
           if(Interval1.Value != null && Interval2.Value != null)
            {
                DateTime start = (DateTime)Interval1.Value;
                DateTime end = (DateTime)Interval2.Value;
                List<Packet> PkFound =  Server.LongTermStatisticsOnPK(start, end);
                List<Tuple<string, DateTime>> MacInterval = new List<Tuple<string, DateTime>>();

                foreach(Packet pk in PkFound)
                {
                    Tuple<string, DateTime> t = new Tuple<string, DateTime>(pk.MacSource, pk.Timestamp);
                    MacInterval.Add(t);
                }

                UpdateTimeline(MacInterval);
               

            }

        }



    }
}
