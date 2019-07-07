using System.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;

namespace ServerApp
{
    /// <summary>
    /// Logica di interazione per Esp32Tooltip.xaml
    /// </summary>
    public partial class Esp32Tooltip : IChartTooltip
    {
        private TooltipData _data;

        public Esp32Tooltip()
        {
            InitializeComponent();

            //LiveCharts will inject the tooltip data in the Data property
            //your job is only to display this data as required

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TooltipData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }

        public TooltipSelectionMode? SelectionMode { get; set; }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
