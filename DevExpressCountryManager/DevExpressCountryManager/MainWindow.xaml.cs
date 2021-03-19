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
            vm.Countries.Add(newCountry);
            countryListBox.SelectedItem = newCountry;
        }

        private void DeleteCountry(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            MainWindowViewModel vm = DataContext as MainWindowViewModel;
            if (vm == null)
            {
                return;
            }
            DXCountry selectedCountry = countryListBox.SelectedItem as DXCountry;

            vm.Countries.Remove(selectedCountry);
        }

        private void ModifyGermanAllegiance(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            MainWindowViewModel vm = DataContext as MainWindowViewModel;
            if (vm == null)
            {
                return;
            }

            vm.ModifyGermanAllegiance("EU");
        }

        private void SaveInDb(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            DXCountry selectedCountry = countryListBox.SelectedItem as DXCountry;
            if (selectedCountry == null)
            {
                return;
            }
            CountryContext dbContext = new CountryContext();

            string countryTag = selectedCountry.Tag;
            DXCountry dbCountry = dbContext.Find<DXCountry>(countryTag);
            if (dbCountry == null)
            {
                dbContext.Add(selectedCountry);
            }
            else
            {
                dbCountry = selectedCountry;
            }
            dbContext.SaveChanges();
        }

        private void ImportFlagClick(object sender, RoutedEventArgs args)
        {
            MainWindowViewModel vm = DataContext as MainWindowViewModel;
            if (vm == null)
            {
                return;
            }
            DXCountry selectedCountry = countryListBox.SelectedItem as DXCountry;

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
