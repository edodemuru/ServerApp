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
        
        public LongTermStatistics()
        {
            InitializeComponent();
            Server = new SnifferServer(3);
            

            /*SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Mac1",
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(1,1),
                        new ObservablePoint(2,1),
                        new ObservablePoint(double.NaN,double.NaN),
                        
                       
                    }

                },
                new LineSeries
                {
                     Title = "Mac2",
                    Values = new ChartValues<ObservablePoint>
                    {
                      
                    }
                },
                new LineSeries
                {
                    Title = "Mac3",
                    Values = new ChartValues<ObservablePoint>
                    {
                       
                    }
                }
            };*/


            //Mac = new[] { "90:fd:61:45:d9:21", "86:29:1c:d3:96:f1", "20:df:b9:99:fd:4d" };
            

            DateTime dt = new DateTime(1929, 1, 1);
            DateTime dt2 = new DateTime(1929, 1, 4);
            DateTime dt3 = new DateTime(1929, 1, 5);
            DateTime dt4 = new DateTime(1929, 1, 6);
            DateTime dt5 = new DateTime(1929, 1, 9);
            DateTime dt6 = new DateTime(1929, 1, 10);
            DateTime dt7 = new DateTime(1929, 1, 1);
            DateTime dt8 = new DateTime(1929, 1, 2);

            
            TimeIntervalGlobal = new List<DateTime>();
            GlobalInterval = new List<Tuple<string, DateTime>>();
            Mac = new List<string>();
            Time = new List<string>();
            SeriesCollection = new SeriesCollection();

            List<Tuple<string, DateTime>> NewInterval = new List<Tuple<string, DateTime>>();
            NewInterval.Add(new Tuple<string, DateTime>("20:df:b9:99:fd:4d", dt));
            NewInterval.Add(new Tuple<string, DateTime>("da:a1:19:d9:2d:a2", dt2));
            NewInterval.Add(new Tuple<string, DateTime>("20:df:b9:99:fd:4d", dt3));
            NewInterval.Add(new Tuple<string, DateTime>("da:a1:19:d9:2d:a2", dt4));

            UpdateTimeline(NewInterval);

            DataContext = this;


        }

        private void UpdateTimeline(List<Tuple<string,DateTime>> NewInterval)
        {
            //New series collection to update actual series collection
            SeriesCollection SC = new SeriesCollection();
          
            List<LineSeries> ListOfMacLines = new List<LineSeries>();
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
            
            //Console.WriteLine( Server.ConvertStringToDateTime(Interval1.Value.ToString()).ToString());
           if(Interval1.Value != null && Interval2.Value != null)
            {
                
               // Server.LongTermStatisticsOnPK(Interval1.Value, Interval2.Value);

            }

        }



    }
}
