using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Kinesense.Interfaces.Clarification
{
    /// <summary>
    /// Interaction logic for EnhancementBaseUI.xaml
    /// </summary>
    public partial class EnhancementBaseUI : UserControl, INotifyPropertyChanged
    {
        public EnhancementBaseUI()
        {
            InitializeComponent();
        }
        public event EventHandler ThisElementRequestsToBeDeleted;

        private void _Button_ApplyEnhancement_Click(object sender, RoutedEventArgs e)
        {
            if (ThisElementRequestsThatItsChangesBeApplied != null)
                ThisElementRequestsThatItsChangesBeApplied(this, null);
        }

        public event EventHandler ThisElementRequestsThatItsChangesBeApplied;

        public bool HasChanges
        {
            set
            {
                _hasChanges = value;
                OnPropertyChanged("HasChanges");
            }
        }
        private bool _hasChanges = false;

        public Grid ContentGrid
        {
            get { return _ContentGrid; }
            set { _ContentGrid = value; }
        }


        #region Notification
        private void OnPropertyChanged(string proname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(proname));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
