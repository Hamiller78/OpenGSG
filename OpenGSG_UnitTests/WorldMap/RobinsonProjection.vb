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
Imports System.Device.Location
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports OpenGSGLibrary.Map

<TestClass()> Public Class Test_RobinsonProjection

    <TestMethod()> Public Sub Test_MapToGlobalCoordinates()
        Dim projection = New RobinsonProjection()

        projection.SetCapitalR(715D)  ' weaked around until the result was satisfying :-(

        Dim mapCoords As Tuple(Of Double, Double) = New Tuple(Of Double, Double)(-714D, 0D)

        Dim geoCoords As GeoCoordinate = projection.getGlobeCoordinates(mapCoords)

        Assert.IsTrue(-180D <= geoCoords.Longitude <= 179D)
        Assert.IsTrue(-1D <= geoCoords.Latitude <= 1D)

    End Sub

End Class