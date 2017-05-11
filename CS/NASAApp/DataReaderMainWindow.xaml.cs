using Kinesense.Interfaces;
using Kinesense.Interfaces.Classes;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NASAApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        ByteArrayBitmap _bmp;
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if(dialog.ShowDialog().Value)
            {
                _bmp = ByteArrayBitmap.FromFile(dialog.FileName);
                _image.Source = _bmp.ToBitmapSource();
            }

        }

        int[] ParseBGR(string bgrstring)
        {
            string[] parts = bgrstring.Split(',', ' ');
            int[] bgr = new int[3];
            for(int i = 0; i < 3; i++)
                bgr[i] = int.Parse(parts[i]);
            return bgr;
        }
        int[,] numbergrid;
        private void _ThresholdButton_Click(object sender, RoutedEventArgs e)
        {
            int[] bgr = ParseBGR(_BGRTextBox.Text);
            ByteArrayBitmap rbmp = _bmp.Clone();

            byte[] truebyte = new byte[] { 255, 255, 255 };
            byte[] falsebyte = new byte[] { 0, 0, 0 };

            numbergrid = new int[_bmp.Width, _bmp.Height];
            for (int x = 0; x<rbmp.Width; x++)
                for(int y = 0; y < rbmp.Height; y++)
                {
                    byte[] c = rbmp.GetColor(x, y);
                    bool t = true;


                    if (c[0] != bgr[0] || c[1] != bgr[1] || c[2] != bgr[2])
                        t = false;

                    rbmp.SetColor(x, y, t?truebyte:falsebyte);
                    numbergrid[x, y] = t ? 1 : 0;
                }

            _image.Source = rbmp.ToBitmapSource();
        }

        private void _SaveCSVButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV|*.csv";
            dialog.AddExtension = true;
            if(dialog.ShowDialog().Value)
            {
                StringBuilder sb = new StringBuilder();
                for(int y = 0; y < _bmp.Height; y++)
                {
                    for(int x = 0; x < _bmp.Width - 1; x++)
                    {
                        sb.AppendFormat("{0}, ", numbergrid[x, y]);
                    }
                    sb.AppendFormat("{0}\n", numbergrid[_bmp.Width - 1, y]);
                }

                string tosave = sb.ToString();
                System.IO.File.WriteAllText(dialog.FileName, tosave);
            }
        }

        private void _MapChannelButton_Click(object sender, RoutedEventArgs e)
        {
            bool R = _RChannel.IsChecked.Value;
            bool G = _GChannel.IsChecked.Value;
            bool B = _BChannel.IsChecked.Value;

            ByteArrayBitmap rbmp = _bmp.Clone();
            double max = 255;
            numbergrid = new int[_bmp.Width, _bmp.Height];
            for (int x = 0; x < rbmp.Width; x++)
                for (int y = 0; y < rbmp.Height; y++)
                {
                    byte[] c = rbmp.GetColor(x, y);
                    byte[] outc = new byte[]
                    {
                        (byte)(B? c[0]:0),
                        (byte)(G? c[1]:0),
                        (byte)(R? c[2]:0)
                    };

                    numbergrid[x, y] = outc[0] + outc[1] + outc[2];
                    if (max < numbergrid[x, y])
                        max = numbergrid[x, y];

                    rbmp.SetColor(x, y, outc);
                    //numbergrid[x, y] = t ? 1 : 0;
                }

            _image.Source = rbmp.ToBitmapSource();

            //normalise
            for (int x = 0; x < rbmp.Width; x++)
                for (int y = 0; y < rbmp.Height; y++)
                    numbergrid[x, y] = (int)(100* ((double)numbergrid[x,y]/max));
        }

        private void _MapHueButton_Click(object sender, RoutedEventArgs e)
        {
            bool R = _RChannel.IsChecked.Value;
            bool G = _GChannel.IsChecked.Value;
            bool B = _BChannel.IsChecked.Value;

            double max = 255;

            byte[] refc = new byte[] { (byte)(R ? 255 : 0), (byte)(G ? 255 : 0), (byte)(B ? 255 : 0) };

            numbergrid = new int[_bmp.Width, _bmp.Height];
            double hue = HLSImage.RGBToHLS(refc)[0];
            ByteArrayBitmap rbmp = _bmp.Clone();
            for (int x = 0; x < rbmp.Width; x++)
                for (int y = 0; y < rbmp.Height; y++)
                {
                    byte[] bgr = rbmp.GetColor(x, y);
                    byte[] rgb = new byte[] { bgr[2], bgr[1], bgr[0] };
                    byte[] c = HLSImage.RGBToHLS(rgb);
                    //.GetColor(x, y);


                    byte dist = (byte)(Math.Abs(c[0] - hue));
                    byte val = (byte)(dist < 25?bgr[1]: 0);
                    byte[] outc = new byte[] { 0, val, 0 };
                    //new byte[]
                    //{
                    //    (byte)(B? c[0]:0),
                    //    (byte)(G? c[1]:0),
                    //    (byte)(R? c[2]:0)
                    //};

                    numbergrid[x, y] = val;
                    if (max < numbergrid[x, y])
                        max = numbergrid[x, y];

                    rbmp.SetColor(x, y, outc);
                }

            _image.Source = rbmp.ToBitmapSource();

            //normalise
            for (int x = 0; x < rbmp.Width; x++)
                for (int y = 0; y < rbmp.Height; y++)
                    numbergrid[x, y] = (int)(100 * ((double)numbergrid[x, y] / max));
        }
    }
}
