using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerApp
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        int numEsp32 = 0;
        private List<TextBox> Coordinatex;
        private List<TextBox> Coordinatey;
        private List<TextBlock> CoordinateTxt;
        private Button Configure;
        //Dialog box to connect esp32 to server
        private EspConnection ConfigurationDlg;
        private List<Device> Esp32List;

        //TextBox NumEsp32Txt;
        public StartupWindow()
        {
            InitializeComponent();
            //Set max lenght of esp 32
            NumEsp32Txt.MaxLength = 2;
            //Event to delete default text or add default text
            NumEsp32Txt.GotFocus += RemoveText;
            NumEsp32Txt.LostFocus += AddText;
            //Event to control when something on keyboard is pressed
            NumEsp32Txt.KeyUp += CheckText;
            //List of textbox and textvlock
            Coordinatex = new List<TextBox>();
            Coordinatey = new List<TextBox>();
            CoordinateTxt = new List<TextBlock>();

            Esp32List = new List<Device>();

            //Button to complete configuration
            Configure = new Button();
            Configure.Background = Brushes.White;
            Configure.Width = 133;
            Configure.FontSize = 22;
            Configure.FontFamily = new FontFamily("Tw Cen MT Condensed Extra Bold");
            Configure.BorderThickness = new Thickness(5);
            Configure.Height = 47;
            Configure.Opacity = 0.5;
            Configure.Name = "ButtonConfiguration";
            Configure.Content = "Configura";

            

            //Lambda function to open main window
            Configure.Click += (sender, e) =>
            {
                
                int i = 0;
                foreach (TextBox txt in Coordinatex)
                {
                    //Check if textbox contains letters
                    int numLettersx = Regex.Matches(txt.Text, @"[a-zA-Z]").Count;
                    int numLettersy = Regex.Matches(Coordinatey[i].Text, @"[a-zA-Z]").Count;
                    if (numLettersx > 0)
                    {
                        //If user insert letters, error
                        txt.Background = Brushes.Red;
                        Esp32List.Clear();
                        return;
                    }else if (numLettersy > 0)
                    {
                        Coordinatey[i].Background = Brushes.Red;
                        Esp32List.Clear();
                        return;
                    }

                    //Convert TextBox string into int
                    int x = Int32.Parse(txt.Text);
                    int y = Int32.Parse(Coordinatey[i].Text);

                    //Create new device from coordinate
                    Device d = new Device(x, y);

                    //Insert into list of devices
                    Esp32List.Add(d);
                    i++;

                }

                
                ShowDialogBox();
                /*MainWindow mainWindow = new MainWindow();
                App.Current.MainWindow = mainWindow;
                this.Close();
                mainWindow.Show();*/
            };
        






        }

        

        public void CheckText(object sender, KeyEventArgs e)
        {
            TextBox senderObj = (TextBox)sender;
            int numLetters = Regex.Matches(senderObj.Text, @"[a-zA-Z]").Count;
            if (numLetters > 0)
            {
                //If user insert letters, error
                senderObj.Background = Brushes.Red;
                return;
            }
            else
            {
                senderObj.Background = Brushes.White;
            }

            if(senderObj.Name == "NumEsp32Txt" && senderObj.Text != "")
            {
                //If user press enter, clear panel and populate with new coordinate
                ClearCoordinatePanel();
                numEsp32 = Int32.Parse(senderObj.Text);
                UpdateCoordinatePanel();
                //senderObj.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                
                
               
            }

            
        }

        private void UpdateCoordinatePanel()
        {
           
            int i = 0;

            while (i != numEsp32)
            {
                i++;
                //Setup for text pos
                TextBlock TxtPos = new TextBlock();
                TxtPos.Name = "TxtPos" + i;
                TxtPos.TextWrapping = TextWrapping.Wrap;
                TxtPos.FontFamily = new FontFamily("Segoe UI Emoji");
                TxtPos.Width = 204;
                TxtPos.Height = 43;
                TxtPos.FontSize = 22;
                TxtPos.Text = "Posizione Esp32 " + i;

                //Setup for coordinate
                TextBox xCoordinate = new TextBox();
                xCoordinate.Name = "xCoordinate" + i;
                xCoordinate.Height = 38;
                xCoordinate.TextWrapping = TextWrapping.Wrap;
                xCoordinate.Text = "x";
                xCoordinate.VerticalAlignment = VerticalAlignment.Top;
                xCoordinate.Width = 42;
                xCoordinate.FontSize = 24;
                xCoordinate.FontFamily = new FontFamily("Tw Cen MT Condensed Extra Bold");
                xCoordinate.SelectionOpacity = 0;
                xCoordinate.Opacity = 0.5;
                xCoordinate.BorderThickness = new Thickness(5);
                xCoordinate.HorizontalContentAlignment = HorizontalAlignment.Center;
                xCoordinate.MaxLines = 2;
                xCoordinate.Margin = new Thickness(0, 0, 5, 0);

                


                TextBox yCoordinate = new TextBox();
                yCoordinate.Name = "yCoordinate" + i;
                yCoordinate.HorizontalAlignment = HorizontalAlignment.Left;
                yCoordinate.Height = 38;
                yCoordinate.TextWrapping = TextWrapping.Wrap;
                yCoordinate.Text = "y"; 
                yCoordinate.VerticalAlignment = VerticalAlignment.Top;
                yCoordinate.Width = 42;
                yCoordinate.FontSize = 24;
                yCoordinate.FontFamily = new FontFamily("Tw Cen MT Condensed Extra Bold");
                yCoordinate.SelectionOpacity = 0;
                yCoordinate.Opacity = 0.5;
                yCoordinate.BorderThickness = new Thickness(5);
                yCoordinate.HorizontalContentAlignment = HorizontalAlignment.Center;
                yCoordinate.MaxLines = 2;
                yCoordinate.Margin = new Thickness(5, 0, 0, 0);




                //Add text to ui
                CoordinatePanel.Children.Add(TxtPos);
                //Add text to list of textblock
                CoordinateTxt.Add(TxtPos);
                CoordinatePanel.Children.Add(xCoordinate);

                //Add coordinate x to list
                Coordinatex.Add(xCoordinate);
                CoordinatePanel.Children.Add(yCoordinate);
                //Add coordinate y to list
                Coordinatey.Add(yCoordinate);

                xCoordinate.GotFocus += RemoveText;
                xCoordinate.LostFocus += AddText;
                xCoordinate.KeyUp += CheckText;


                yCoordinate.GotFocus += RemoveText;
                yCoordinate.LostFocus += AddText;
                yCoordinate.KeyUp += CheckText;






            }
            //Add button to ui
            CoordinatePanel.Children.Add(Configure);
            
        }

        private void ClearCoordinatePanel()
        {
            //Clear all textbox and textblock in window
            //int i = 0;
            if (CoordinateTxt.Count != 0 && Coordinatex.Count != 0)
            {
                foreach (TextBlock txt in CoordinateTxt)
                {
                    CoordinatePanel.Children.Remove(txt);
                }

                foreach (TextBox txt in Coordinatex)
                {
                    CoordinatePanel.Children.Remove(txt);
                }
                foreach(TextBox txt in Coordinatey)
                {
                    CoordinatePanel.Children.Remove(txt);
                }
                CoordinateTxt.Clear();
                Coordinatex.Clear();
                Coordinatey.Clear();
                //Remove button
                CoordinatePanel.Children.Remove(Configure);
            }


            numEsp32 = 0;

           
        }

        private void RemoveText(object sender, EventArgs e)
        {
            TextBox senderObj = (TextBox)sender;
            senderObj.Text = "";
            senderObj.CaretBrush = null;

        }

        private void AddText(object sender, EventArgs e)
        {
            TextBox senderObj = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(senderObj.Text))
            {
                if (senderObj.Name == "NumEsp32Txt")
                    senderObj.Text = "0";
                else if (senderObj.Name.Contains("xCoordinate"))
                    senderObj.Text = "x";
                else if (senderObj.Name.Contains("yCoordinate"))
                    senderObj.Text = "y";
            }
                
        }

        private void ShowDialogBox()
        {
            //Instantiate dialogBox
            ConfigurationDlg = new EspConnection(Esp32List);

            // Configure the dialog box
            ConfigurationDlg.Owner = this;

            // Open the dialog box modally 
            ConfigurationDlg.ShowDialog();
        }

       



    }
}
