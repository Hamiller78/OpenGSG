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

Imports System.Device.Location

Imports OpenGSGLibrary.Map

Namespace MainWindowView

    Public Class GeoCoordinates

        Public Sub New(ByRef motherWindow As MainWindow)
            motherWindow_ = motherWindow
            mapProjection_.SetCapitalR(720)
        End Sub

        Public Sub UpdateCoordinates(e As MouseEventArgs)
            Dim mapX As Integer = e.X * mapScaling_
            Dim mapY As Integer = e.Y * mapScaling_

            Dim mapCoords = New Tuple(Of Double, Double)(mapX - 642, mapY - 362)
            Dim mouseGeoCoord As GeoCoordinate = mapProjection_.getGlobeCoordinates(mapCoords)
            motherWindow_.CoordinateDisplay.Text = mouseGeoCoord.ToString()
        End Sub

        Public Sub SetMapScaling(scalingFactor As Double)
            mapScaling_ = scalingFactor
        End Sub

        Private motherWindow_ As MainWindow = Nothing
        Private mapProjection_ As RobinsonProjection = New RobinsonProjection
        Private mapScaling_ As Double = 0.0

    End Class

End Namespace