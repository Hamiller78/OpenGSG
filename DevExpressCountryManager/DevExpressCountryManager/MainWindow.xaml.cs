using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
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
using DevExpressCountryManager.Database;
using DevExpressCountryManager.Models.WorldData;
using DevExpressCountryManager.Models.Common;
using DevExpressCountryManager.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace DevExpressCountryManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public static IConfigurationRoot Configuration { get; set; }

        public MainWindow()
        {
            Startup();

            InitializeComponent();
        }

        private void Startup()
        {
            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<MainWindow>();

            Configuration = builder.Build();
        }

        private void CreateCountry(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            MainWindowViewModel vm = DataContext as MainWindowViewModel;
            if (vm == null)
            {
                return;
            }

            DXCountry newCountry = new DXCountry();
            DXCountryViewModel newCountryVM = DXCountryViewModel.Create();
            vm.Countries.Add(newCountryVM);
            countryListBox.SelectedItem = newCountryVM;
        }

        private void DeleteCountry(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            MainWindowViewModel vm = DataContext as MainWindowViewModel;
            if (vm == null)
            {
                return;
            }
            DXCountryViewModel selectedCountry = countryListBox.SelectedItem as DXCountryViewModel;

            vm.Countries.Remove(selectedCountry);
        }

        private void SaveInDb(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            DXCountryViewModel selectedCountry = countryListBox.SelectedItem as DXCountryViewModel;
            if (selectedCountry == null)
            {
                return;
            }
            selectedCountry.SaveToDb();
        }

        private void ImportFlagClick(object sender, RoutedEventArgs args)
        {
            MainWindowViewModel vm = DataContext as MainWindowViewModel;
            if (vm == null)
            {
                return;
            }
            DXCountryViewModel selectedCountry = countryListBox.SelectedItem as DXCountryViewModel;

            OpenFileDialog importFlagDialog = new OpenFileDialog();
            bool? result = importFlagDialog.ShowDialog();
            if (result == false)
            {
                return;
            }

            string filePath = importFlagDialog.FileName;
            try
            {
                BlobbableImage newFlag = null;
                if (selectedCountry.Flag == null)
                {
                    newFlag = new BlobbableImage();
                }
                else
                {
                    newFlag = selectedCountry.Flag;
                }
                newFlag.ImageObj = new Bitmap(filePath);
                selectedCountry.Flag = newFlag;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception while loading flag: " + ex.Message);
            }
        }
    }
}
