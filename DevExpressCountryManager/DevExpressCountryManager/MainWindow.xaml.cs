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

namespace DevExpressCountryManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ModifyGermanAllegiance(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            MainWindowViewModel vm = this.DataContext as MainWindowViewModel;
            if (vm == null)
            {
                return;
            }

            vm.ModifyGermanAllegiance("EU");
        }
    }
}
