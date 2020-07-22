'    OpenSGSGLibrary is an open-source library for Grand Strategy Games
'    Copyright (C) 2019-2020  Torben Kneesch
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

Imports OpenGSGLibrary.Map
Imports OpenGSGLibrary.WorldData

Namespace MainWindowView

    Public Class WorldMap

        Private motherWindow_ As MainWindow
        Private viewedState_ As WorldState = Nothing

        Private provinceMap_ As ProvinceMap = Nothing
        Private countryModeMap_ As Image = Nothing
        Private mapScaling_ As Double = 0.0

        Public Sub New(ByRef motherWindow As MainWindow)
            motherWindow_ = motherWindow
        End Sub

        Public Sub SetSourceProvinceMap(newProvinceMap As ProvinceMap)
            provinceMap_ = newProvinceMap
            SetMapPicture()
        End Sub

        Public Sub UpdateCountryMap(ByRef currentState As WorldState)
            Dim ModeMapRenderer = New CountryModeMapMaker(provinceMap_)
            countryModeMap_ = ModeMapRenderer.MakeMap(currentState)
        End Sub

        Public Sub SetMapPicture()
            Dim renderedBitmap As Bitmap

            ' check if the required stuff is loaded
            If IsNothing(provinceMap_) Then Return

            ' Set new map
            If motherWindow_.MapModePolitical.Checked Then
                renderedBitmap = countryModeMap_
            Else
                renderedBitmap = provinceMap_.sourceBitmap
            End If

            ' Resize map for screen output
            Dim sourceSize As Size = provinceMap_.sourceBitmap.Size
            SetMapScalingFactor(motherWindow_.MapPictureBox.Size, sourceSize)
            Dim newSize As Size = New Size(sourceSize.Width / mapScaling_, sourceSize.Height / mapScaling_)
            Dim resizedBitmap As Bitmap = New Bitmap(renderedBitmap, newSize)

            ' Set the resized map in PictureBox
            motherWindow_.MapPictureBox.Image = resizedBitmap
            motherWindow_.MapPictureBox.Invalidate()
        End Sub

        Private Sub SetMapScalingFactor(newSize As Size, originalSize As Size)
            Dim xFactor As Double = originalSize.Width / newSize.Width
            Dim yFactor As Double = originalSize.Height / newSize.Height
            mapScaling_ = Math.Max(xFactor, yFactor)
        End Sub

    End Class

End Namespace