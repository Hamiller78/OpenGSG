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
Imports System.Math

Namespace Map

    ''' <summary>
    ''' Class that contains methods for calculations of distances on a world map.
    ''' </summary>
    Public Class Navigator

        Const EarthRadiusInKm As Double = 6371D

        ''' <summary>
        ''' Gets distance between two points on the earth surface in kilometres.
        ''' </summary>
        ''' <param name="firstLatitude">Latitude in degrees of first point.</param>
        ''' <param name="firstLongitude">Longitude in degrees of first point.</param>
        ''' <param name="secondLatitude">Latitude in degrees of second point.</param>
        ''' <param name="secondLongitude">Longitude in degrees of second point.</param>
        ''' <returns></returns>
        Public Shared Function GetDistanceInKm(firstLatitude As Double, firstLongitude As Double,
                                    secondLatitude As Double, secondLongitude As Double) As Double
            Dim latitude1 As Double = firstLatitude * PI / 180.0
            Dim longitude1 As Double = firstLongitude * PI / 180.0
            Dim latitude2 As Double = secondLatitude * PI / 180.0
            Dim longitude2 As Double = secondLongitude * PI / 180.0

            Dim distanceAngleRad As Double = Acos(
                                                  Sin(latitude1) * Sin(latitude2) +
                                                  Cos(latitude1) * Cos(latitude2) * Cos(longitude2 - longitude1)
                                                  )

            Return Abs(distanceAngleRad * EarthRadiusInKm)
        End Function

        Public Shared Function GetDistanceInKm(firstLocation As GeoCoordinate, secondLocation As GeoCoordinate) As Double
            Return firstLocation.GetDistanceTo(secondLocation) / 1000D
        End Function

    End Class

End Namespace