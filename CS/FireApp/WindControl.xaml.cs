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

namespace FireApp
{
    /// <summary>
    /// Interaction logic for WindControl.xaml
    /// </summary>
    public partial class WindControl : UserControl
    {
        public WindControl()
        {
            InitializeComponent();
        }

        public WindTypes WindTypes{get;set;}


        private void WindStrengthRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_NoWind.IsChecked.Value)
                WindTypes = WindTypes.NO_WIND;
            else
            if (_LightWind.IsChecked.Value)
                WindTypes = WindTypes.LIGHT_WIND;
            else if (_ModerateWind.IsChecked.Value)
                WindTypes = WindTypes.MODERATE_WIND;
            else
                WindTypes = WindTypes.STRONG_WIND;

            Calculate();

            if (WindChanged != null)
                WindChanged(this, EventArgs.Empty);

        }

        public event EventHandler WindChanged;

        public double WindXIntensity { get; set; }
        public double WindYIntensity { get; set; }

        private void Calculate()
        {
            try
            {
                double sqrt = Math.Sqrt(2);
                double strength = (int)WindTypes;

                if (_N.IsChecked.Value)
                {
                    WindYIntensity = strength;
                    WindXIntensity = 0;
                }
                else if (_S.IsChecked.Value)
                {
                    WindXIntensity = 0;
                    WindYIntensity = -1 * strength;
                }
                else if (_W.IsChecked.Value)
                {
                    WindXIntensity = strength;
                    WindYIntensity = 0;
                }
                else if (_E.IsChecked.Value)
                {
                    WindXIntensity = -1 * strength;
                    WindYIntensity = 0;
                }
                else if (_NE.IsChecked.Value)
                {
                    WindXIntensity = strength / sqrt;
                    WindYIntensity = WindXIntensity;
                }
                else if (_NW.IsChecked.Value)
                {
                    WindXIntensity = strength / sqrt;
                    WindYIntensity = -1 * WindXIntensity;
                }
                else if (_SE.IsChecked.Value)
                {
                    WindXIntensity = strength / sqrt;
                    WindYIntensity = -1 * WindXIntensity;
                }
                else if (_SW.IsChecked.Value)
                {
                    WindXIntensity = strength / sqrt;
                    WindYIntensity = WindXIntensity;
                }
            }
            catch (Exception ee)
            { }
        }

        private void _DirectionChanged(object sender, RoutedEventArgs e)
        {
            Calculate();


            if (WindChanged != null)
                WindChanged(this, EventArgs.Empty);

        }
    }
}
