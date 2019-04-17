'    OpenSGSGLibrary is an open-source library for Grand Strategy Games
'    Copyright (C) 2019  Torben Kneesch
'
'    This program is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.
'
'    This program is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License
'    along with this program.  If not, see <https://www.gnu.org/licenses/>.

Imports System.Drawing

Namespace Map

    Public Class CountryMapRenderer
        Inherits MapRenderer

        Public Sub New(provinceMap As ProvinceMap, provinces As WorldData.ProvinceManager, countries As WorldData.CountryManager)
            MyBase.New(provinceMap)
            provinceMap_ = provinceMap
            provinces_ = provinces
            countries_ = countries
        End Sub

        Public Overrides Function RenderMap() As Image
            Dim mapSize As Size = provinceMap_.sourceBitmap.Size

            Dim renderedImage As New Bitmap(mapSize.Width, mapSize.Height)

            For y = 0 To mapSize.Height
                For x = 0 To mapSize.Width
                    ' get country from province color
                    Dim provinceRgb As Tuple(Of Byte, Byte, Byte) = provinceMap_.GetPixelRgb(x, y)
                    Dim provinceId As Integer = provinceMap_.GetProvinceNumber(provinceRgb)
                    Dim countryTag As String = provinces_.GetProvince(provinceId).owner
                    ' get country color code
                    Dim country As WorldData.Country = countries_.GetCountry(countryTag)
                    Dim countryColor As Tuple(Of Byte, Byte, Byte) = country.color
                    ' draw pixel in that color in destination map
                    Dim drawColor As Color = Color.FromArgb(countryColor.Item1, countryColor.Item2, countryColor.Item3)
                    renderedImage.SetPixel(x, y, drawColor)
                Next
            Next

            Return renderedImage
        End Function

        Private provinceMap_ As ProvinceMap
        Private provinces_ As WorldData.ProvinceManager
        Private countries_ As WorldData.CountryManager

    End Class

End Namespace