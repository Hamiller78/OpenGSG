'    OpenSGSGLibrary is an open-source library for Grand Strategy Games
'    Copyright (C) 2020  Torben Kneesch
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

Imports OpenGSGLibrary.WorldData

Namespace Map

    ''' <summary>
    ''' Map mode maker for the country map mode (aka political map).
    ''' The actually used derived province and country classes have to be specified.
    ''' </summary>
    Public Class CountryModeMapMaker
        Inherits ModeMapMaker

        Public Sub New(provinceMap As ProvinceMap)
            MyBase.New(provinceMap)
            provinceMap_ = provinceMap
        End Sub

        ''' <summary>
        ''' Makes the country map as an image object.
        ''' </summary>
        ''' <param name="sourceState">WorldState to pull the procinve and country oweners from.</param>
        ''' <returns>Image object with country map.</returns>
        Public Overrides Function MakeMap(ByRef sourceState As WorldState) As Image
            Dim mapSize As Size = provinceMap_.sourceBitmap.Size
            Dim provinceMap As IDictionary(Of Integer, Province) = sourceState.GetProvinceTable()
            Dim countryMpa As IDictionary(Of String, Country) = sourceState.GetCountryTable()

            Dim countryMap As New Bitmap(mapSize.Width, mapSize.Height, Imaging.PixelFormat.Format32bppArgb)
            For y = 0 To mapSize.Height - 1
                For x = 0 To mapSize.Width - 1
                    Dim provinceRgb As Tuple(Of Byte, Byte, Byte) = provinceMap_.GetPixelRgb(x, y)
                    Dim provinceId As Integer = provinceMap_.GetProvinceNumber(provinceRgb)
                    countryMap.SetPixel(x, y, GetCountryDrawColor(provinceId, sourceState))
                Next
            Next

            Return countryMap
        End Function

        Private Function GetCountryDrawColor(provinceId As Integer, sourceState As WorldState) As Color
            Dim drawColor As Color = Color.AntiqueWhite
            If provinceId <> -1 Then
                Dim countryTag As String = sourceState.GetProvinceTable()(provinceId).GetOwner()
                Dim country As Country = sourceState.GetCountryTable()(countryTag)
                Dim countryColor As Tuple(Of Byte, Byte, Byte) = country.GetColor()
                drawColor = Color.FromArgb(countryColor.Item1, countryColor.Item2, countryColor.Item3)
            End If
            Return drawColor
        End Function

        Private provinceMap_ As ProvinceMap

    End Class

End Namespace