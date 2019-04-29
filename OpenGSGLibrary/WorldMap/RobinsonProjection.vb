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

Imports System.Device.Location

Namespace Map

    ''' <summary>
    ''' Implementation of Robinson projection, which is used for some world maps.
    ''' This projection is implemented for prototyping, since there are world maps under a GNU license on Wikipedia.
    ''' </summary>
    Public Class RobinsonProjection
        Inherits Projection

        ''' <summary>
        ''' Returns the map coordinates based on a Robinson projection with R=1.
        ''' See: https://en.wikipedia.org/wiki/Robinson_projection
        ''' </summary>
        ''' <param name="globePoint">Global coordinate of point As GeoCoordinate.</param>
        ''' <returns></returns>
        Public Overrides Function getMapCoordinates(globePoint As GeoCoordinate) As Tuple(Of Double, Double)
            Dim capitalXY As Tuple(Of Double, Double) = getXY(globePoint.Latitude)
            Dim capitalR As Double = 1.0
            Dim centralMeridian As Double = 0.0 'chosen central meridian for map

            Dim x As Double = 0.8487 * capitalR * capitalXY.Item1 * (globePoint.Longitude - centralMeridian)
            Dim y As Double = 1.3523 * capitalR * capitalXY.Item2

            Return New Tuple(Of Double, Double)(x, y)
        End Function

        Public Overrides Function getGlobeCoordinates(mapPoint As Tuple(Of Double, Double)) As GeoCoordinate
            Throw New NotImplementedException()
        End Function

        Private Function getXY(latitude As Double) As Tuple(Of Double, Double)
            Dim capitalX As Double = 0
            Dim capitalY As Double = 0
            If (0 <= latitude And latitude < 5) Then
                capitalX = linearInterpolation(latitude, 0, 1, 5, 0.9986)
                capitalY = linearInterpolation(latitude, 0, 0.0, 5, 0.062)
            ElseIf (5 <= latitude And latitude < 10) Then
                capitalX = linearInterpolation(latitude, 5, 0.9986, 10, 0.9954)
                capitalY = linearInterpolation(latitude, 5, 0.062, 10, 0.124)
            ElseIf (10 <= latitude And latitude < 15) Then
                capitalX = linearInterpolation(latitude, 10, 0.9954, 15, 0.99)
                capitalY = linearInterpolation(latitude, 10, 0.124, 15, 0.186)
            ElseIf (15 <= latitude And latitude < 20) Then
                capitalX = linearInterpolation(latitude, 15, 0.99, 20, 0.9822)
                capitalY = linearInterpolation(latitude, 15, 0.186, 20, 0.248)
            ElseIf (20 <= latitude And latitude < 25) Then
                capitalX = linearInterpolation(latitude, 20, 0.9822, 25, 0.973)
                capitalY = linearInterpolation(latitude, 20, 0.248, 25, 0.31)
            ElseIf (25 <= latitude And latitude < 30) Then
                capitalX = linearInterpolation(latitude, 25, 0.973, 30, 0.96)
                capitalY = linearInterpolation(latitude, 25, 0.31, 30, 0.372)
            ElseIf (30 <= latitude And latitude < 35) Then
                capitalX = linearInterpolation(latitude, 30, 0.96, 35, 0.9427)
                capitalY = linearInterpolation(latitude, 30, 0.372, 35, 0.434)
            ElseIf (35 <= latitude And latitude < 40) Then
                capitalX = linearInterpolation(latitude, 35, 0.9427, 40, 0.9216)
                capitalY = linearInterpolation(latitude, 35, 0.434, 40, 0.4958)
            ElseIf (40 <= latitude And latitude < 45) Then
                capitalX = linearInterpolation(latitude, 40, 0.9216, 45, 0.8962)
                capitalY = linearInterpolation(latitude, 40, 0.4958, 45, 0.5571)
            ElseIf (45 <= latitude And latitude < 50) Then
                capitalX = linearInterpolation(latitude, 45, 0.8962, 50, 0.8679)
                capitalY = linearInterpolation(latitude, 45, 0.5571, 50, 0.6176)
            ElseIf (50 <= latitude And latitude < 55) Then
                capitalX = linearInterpolation(latitude, 50, 0.8679, 55, 0.835)
                capitalY = linearInterpolation(latitude, 50, 0.6176, 55, 0.6769)
            ElseIf (55 <= latitude And latitude < 60) Then
                capitalX = linearInterpolation(latitude, 55, 0.835, 60, 0.7986)
                capitalY = linearInterpolation(latitude, 55, 0.6769, 60, 0.7346)
            ElseIf (60 <= latitude And latitude < 65) Then
                capitalX = linearInterpolation(latitude, 60, 0.7986, 65, 0.7597)
                capitalY = linearInterpolation(latitude, 60, 0.7346, 65, 0.7903)
            ElseIf (65 <= latitude And latitude < 70) Then
                capitalX = linearInterpolation(latitude, 65, 0.7597, 70, 0.7186)
                capitalY = linearInterpolation(latitude, 65, 0.7903, 70, 0.8435)
            ElseIf (70 <= latitude And latitude < 75) Then
                capitalX = linearInterpolation(latitude, 70, 0.7186, 75, 0.6732)
                capitalY = linearInterpolation(latitude, 70, 0.8435, 75, 0.8936)
            ElseIf (75 <= latitude And latitude < 80) Then
                capitalX = linearInterpolation(latitude, 75, 0.6732, 80, 0.6213)
                capitalY = linearInterpolation(latitude, 75, 0.8936, 80, 0.9394)
            ElseIf (80 <= latitude And latitude < 85) Then
                capitalX = linearInterpolation(latitude, 80, 0.6213, 85, 0.5722)
                capitalY = linearInterpolation(latitude, 80, 0.9394, 85, 0.9761)
            ElseIf (85 <= latitude And latitude <= 90) Then
                capitalX = linearInterpolation(latitude, 85, 0.5722, 90, 0.5322)
                capitalY = linearInterpolation(latitude, 85, 0.9761, 90, 1)
            End If
            Return New Tuple(Of Double, Double)(capitalX, capitalY)
        End Function

        Private Function linearInterpolation(x As Double, x1 As Double, y1 As Double, x2 As Double, y2 As Double) As Double
            Dim deltaX As Double = (x - x1) / (x2 - x1)
            Return (y2 - y1) * deltaX + y1
        End Function

    End Class

End Namespace
