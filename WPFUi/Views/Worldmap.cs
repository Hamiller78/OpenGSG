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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using OpenGSGLibraryV2.Maps;

namespace WPFUi.Views
{
    class Worldmap : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public int FocussedProvince { get; private set; } = -1;

        private ProvinceBitmap provBitmap_ = null;
        private double mapScaling_ = 0.0;

        private OpenGSGLibrary.Map.ProvinceMap provRgbMapping_ = null;

        public Worldmap(ProvinceBitmap provBitmap,
                        OpenGSGLibrary.Map.ProvinceMap provRgbMapping)
        {
            provBitmap_ = provBitmap;
            provRgbMapping_ = provRgbMapping;
        }

        public void SetMapScalingFactor(Tuple<double, double> newSize, Tuple<double, double> originalSize)
        {
            double xFactor = originalSize.Item1 / newSize.Item1;
            double yFactor = originalSize.Item2 / newSize.Item2;
            mapScaling_ = Math.Max(xFactor, yFactor);
        }

        public void MouseMoved(Point mouseCoords)
        {
            Point mapCoords = new Point(mouseCoords.X * mapScaling_, mouseCoords.Y * mapScaling_);
            Tuple<byte, byte, byte> provRgb = provBitmap_.GetPixelRgb((int)mapCoords.X, (int)mapCoords.Y);
            int mouseProvId = provRgbMapping_.GetProvinceNumber(provRgb);
            if (mouseProvId != -1 && mouseProvId != FocussedProvince)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("FocussedProvince"));
            }
        }

    }
}
