//    OpenSGSGLibrary is an open-source library for Grand Strategy Games
//    Copyright (C) 2019-2020  Torben Kneesch
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <https://www.gnu.org/licenses/>.
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

using System.IO;

using OpenGSGLibrary.Tools;
using OpenGSGLibraryV2.Maps;
using ColdWarGameLogic.Simulation;
using OpenGSGLibrary.WorldData;

namespace WPFUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MasterController gameController_ = new MasterController();

        private ProvinceBitmap provBitmap_ = new ProvinceBitmap();
        private NationModeMap nationMap_ = null;

        public MainWindow()
        {
            InitializeComponent();

            gameController_.Init();

            // need to get some information from V1 library
            OpenGSGLibrary.Map.ProvinceMap provRgbMap = new OpenGSGLibrary.Map.ProvinceMap();
            provRgbMap.LoadProvinceRGBs(System.IO.Path.Combine("../../../ColdWarPrototype/GameData/map", "definitions.csv"));

            // V2 of provine mapping
            provBitmap_.FromFile(System.IO.Path.Combine("../../../ColdWarPrototype/GameData/map", "provinces24.bmp"));
            nationMap_ = new NationModeMap(provBitmap_, provRgbMap);

            WorldState currentState = gameController_.tickHandler.GetState();
            IDictionary<int, string> provOwners = GetProvinceOwners(currentState);
            nationMap_.Update(provOwners, currentState);

            MainWindow win = (MainWindow)Application.Current.MainWindow;
            image_worldmap.Stretch = Stretch.Uniform;
            image_worldmap.Source = provBitmap_.SourceBitmap;
        }

        private IDictionary<int, string> GetProvinceOwners(WorldState currentState)
        {
            IDictionary<int, Province> provTable = currentState.GetProvinceTable();
            IDictionary<int, string> provOwners = new Dictionary<int, string>();

            foreach (KeyValuePair<int, Province> provKV in provTable)
            {
                provOwners.Add(provKV.Key, provKV.Value.GetOwner());
            }

            return provOwners;
        }

        private void UpdateMapMode(object sender, RoutedEventArgs e)
        {
            if (radioButton_mapProvince.IsChecked == true)
            {
                image_worldmap.Source = provBitmap_.SourceBitmap;
            }
            else
            {
                image_worldmap.Source = nationMap_.modeBitmap;
            }
        }

    }
}
