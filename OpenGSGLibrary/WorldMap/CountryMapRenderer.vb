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
Imports System.Diagnostics

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
            Dim sw = New Stopwatch
            Dim time1, time2, time3, time4, time5, time6 As Long
            sw.Start()
            For y = 0 To mapSize.Height - 1
                For x = 0 To mapSize.Width - 1
                    ' get country from province color
                    time1 = sw.ElapsedMilliseconds
                    Dim provinceRgb As Tuple(Of Byte, Byte, Byte) = provinceMap_.GetPixelRgb(x, y)
                    time5 = sw.ElapsedMilliseconds
                    Dim provinceId As Integer = provinceMap_.GetProvinceNumber(provinceRgb)
                    time2 = sw.ElapsedMilliseconds
                    Dim drawColor As Color = Color.AntiqueWhite
                    If provinceId <> -1 Then
                        Dim countryTag As String = provinces_.GetProvince(provinceId).owner
                        ' get country color code
                        Dim country As WorldData.Country = countries_.GetCountry(countryTag)
                        Dim countryColor As Tuple(Of Byte, Byte, Byte) = country.color
                        ' draw pixel in that color in destination map
                        drawColor = Color.FromArgb(countryColor.Item1, countryColor.Item2, countryColor.Item3)
                    End If
                    time3 = sw.ElapsedMilliseconds
                    renderedImage.SetPixel(x, y, drawColor)
                    time4 = sw.ElapsedMilliseconds
                Next
                Console.WriteLine("y: " & y)
            Next

            Return renderedImage
        End Function

        Private provinceMap_ As ProvinceMap
        Private provinces_ As WorldData.ProvinceManager
        Private countries_ As WorldData.CountryManager

    End Class

End Namespace