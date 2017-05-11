using Kinesense.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace FireApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            _windControl.WindChanged += _windControl_WindChanged;
        }

        private void _windControl_WindChanged(object sender, EventArgs e)
        {
            firemap.WindXIntensity = _windControl.WindXIntensity;
            firemap.WindYIntensity = _windControl.WindYIntensity;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _continue = false;
        }

        FireMap firemap;

        bool _continue = true;

        private void _GenerateRandomDatat(object sender, RoutedEventArgs e)
        {
            firemap = new FireMap(200, 200, true);
            _image.Source = firemap.ToByteArrayBitmap().ToBitmapSource();
            //Start();
        }
        DateTime time = DateTime.Now;
        private void Start()
        {
            _continue = true;
            Task.Factory.StartNew(() => {
                while (_continue)
                {
                    Dispatcher.BeginInvoke(new Action(() => {
                        try
                        {
                            _time.Text = time.ToString();
                            if (bkg == null)
                                _image.Source = firemap.ToByteArrayBitmap().ToBitmapSource();
                            else
                            {
                                ByteArrayBitmap b = bkg.Clone();
                                firemap.PaintFire(b);
                                _image.Source = b.ToBitmapSource();
                            }
                        }
                        catch(Exception ee)
                        {

                        }
                    }));
                    
                    firemap.Update();
                    time = time.AddHours(6);
                    Thread.Sleep(150);
                }
            });
        }

        ByteArrayBitmap bkg;
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            //NationalPark
            //string burnedDataFile = @"D:\Dropbox\Personal\NASASpaceApps\data\BurnedArea.csv";
            //string fireStartPointDataFile = @"D:\Dropbox\Personal\NASASpaceApps\data\FireStartPoints.csv";
            //string VegitationDataFile = @"D:\Dropbox\Personal\NASASpaceApps\data\VegitationResources_2.csv";
            //string WaterDataFile = @"D:\Dropbox\Personal\NASASpaceApps\data\WaterResources.csv";
            //bkg = ByteArrayBitmap.FromFile(@"D:\Dropbox\Personal\NASASpaceApps\data\Bkg_Map.jpg");

            //Brisbane
            //string burnedDataFile = @"D:\Dropbox\Personal\NASASpaceApps\data\BurnedArea.csv";
            //string fireStartPointDataFile = @"D:\Dropbox\Personal\NASASpaceApps\data\FireStartPoints.csv";
            string VegitationDataFile = @"D:\Dropbox\Personal\NASASpaceApps\Brisbane\BrisbaneSmallVeg.csv";
            //string WaterDataFile = @"D:\Dropbox\Personal\NASASpaceApps\data\WaterResources.csv";
            bkg = ByteArrayBitmap.FromFile(@"D:\Dropbox\Personal\NASASpaceApps\Brisbane\SmallBrisbane.jpg");

            //int[,] waterdata = CSVReader.Read(WaterDataFile);
            int[,] VegData = CSVReader.Read(VegitationDataFile);
            //int[,] firestartData = CSVReader.Read(fireStartPointDataFile);
            //int[,] burnedData = CSVReader.Read(burnedDataFile);

            int width = VegData.GetUpperBound(0) + 1;
            int height = VegData.GetUpperBound(1) + 1;
            firemap = new FireMap(width, height);
            firemap.FireCells = new FireCell[width, height];

            for(int x = 0; x < width; x ++)
                for(int y = 0; y < height; y++)
                {
                    firemap.FireCells[x, y] = new FireCell()
                    {
                        Fuel = VegData[x,y],
                        //FireIntensity = firestartData[x, y]*255,
                        //Water = waterdata[x,y]
                    };
                }

            firemap.RandomFireStarts();

            //firemap.WindXIntensity = 15;

            _image.Source = bkg.ToBitmapSource();
            
        }

        private void _time_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }

        private void _image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var pos = e.GetPosition(_image);
                int x = (int)(firemap.Width * (pos.X / _image.ActualWidth));
                int y = (int)(firemap.Height * (pos.Y / _image.ActualHeight));

                if(_FireRadioButton.IsChecked.Value)
                    firemap.FireCells[x, y].FireIntensity = 255;
                else
                {
                    //water
                    int area = 5;
                    for(int i = x - area; i < x + area; i++)
                        for(int j = y - area; j < y + area; j++)
                        {
                            if (i >= 0 && i < firemap.Width
                                && j >= 0 && j < firemap.Height)
                                firemap.FireCells[i, j].Water = 155;
                        }
                }

                
            }
            catch (Exception ee)
            {

            }

        }

        private void _StartButtonClick(object sender, RoutedEventArgs e)
        {
            if (firemap != null)
                Start();
        }
    }
    
}
