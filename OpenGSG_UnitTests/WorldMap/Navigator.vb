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

<TestClass()> Public Class Test_Navigator

    <TestMethod()> Public Sub Test_CompareDistanceMethods()
        ' Coordinates London and New York according to Google
        Dim latitudeLondon As Double = 51.5074D
        Dim longitudeLondon As Double = -0.1278D
        Dim latitudeNewYork As Double = 40.7128D
        Dim longitudeNewYork As Double = -74.006D
        Dim coordinatesLondon = New GeoCoordinate(latitudeLondon, longitudeLondon)
        Dim coordinatesNewYork = New GeoCoordinate(latitudeNewYork, longitudeNewYork)

        ' Distance London to New York is 5567km according to Google, we allow 10km deviation
        Dim distanceMethod1 As Double = Navigator.GetDistanceInKm(latitudeLondon,
                                                                  longitudeLondon,
                                                                  latitudeNewYork,
                                                                  longitudeNewYork)
        Assert.IsTrue(Math.Abs(5567D - distanceMethod1) < 10D)

        Dim distanceMethod2 As Double = Navigator.GetDistanceInKm(coordinatesLondon, coordinatesNewYork)
        Assert.IsTrue(Math.Abs(5567D - distanceMethod2) < 10D)

    End Sub

End Class