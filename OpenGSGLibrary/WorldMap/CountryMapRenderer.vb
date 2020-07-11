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
Imports System.Diagnostics

Namespace Map

    ''' <summary>
    ''' Map renderer for the country map mode (aka political map).
    ''' The actually used derived province and country classes have to be specified.
    ''' </summary>
    Public Class CountryMapRenderer
        Inherits MapRenderer

        Public Sub New(provinceMap As ProvinceMap)
            MyBase.New(provinceMap)
            provinceMap_ = provinceMap
        End Sub

        ''' <summary>
        ''' Sets province and country tables respectively.
        ''' </summary>
        ''' <param name="provinceTable">Dictionary province id -> province objects.</param>
        ''' <param name="countryTable">Dictionary country tag -> country objects.</param>
        Public Sub SetDataTables(provinceTable As IDictionary(Of Integer, WorldData.Province),
                                 countryTable As IDictionary(Of String, WorldData.Country))

            provinceTable_ = provinceTable
            countryTable_ = countryTable

        End Sub

        ''' <summary>
        ''' Renders the country map as an image object.
        ''' </summary>
        ''' <returns>Image object with rendered map.</returns>
        Public Overrides Function RenderMap() As Image
            Dim mapSize As Size = provinceMap_.sourceBitmap.Size

            Dim renderedImage As New Bitmap(mapSize.Width, mapSize.Height, Imaging.PixelFormat.Format32bppArgb)
            For y = 0 To mapSize.Height - 1
                For x = 0 To mapSize.Width - 1
                    ' get country from province color
                    Dim provinceRgb As Tuple(Of Byte, Byte, Byte) = provinceMap_.GetPixelRgb(x, y)
                    Dim provinceId As Integer = provinceMap_.GetProvinceNumber(provinceRgb)
                    Dim drawColor As Color = Color.AntiqueWhite
                    If provinceId <> -1 Then
                        Dim countryTag As String = provinceTable_(provinceId).GetOwner()
                        ' get country color code
                        Dim country As WorldData.Country = countryTable_(countryTag)
                        Dim countryColor As Tuple(Of Byte, Byte, Byte) = country.GetColor()
                        ' draw pixel in that color in destination map
                        drawColor = Color.FromArgb(countryColor.Item1, countryColor.Item2, countryColor.Item3)
                    End If
                    renderedImage.SetPixel(x, y, drawColor)
                Next
            Next

            Return renderedImage
        End Function

        Private provinceMap_ As ProvinceMap
        Private provinceTable_ As IDictionary(Of Integer, WorldData.Province)
        Private countryTable_ As IDictionary(Of String, WorldData.Country)

    End Class

End Namespace