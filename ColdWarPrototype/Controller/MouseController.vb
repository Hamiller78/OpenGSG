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
Imports ColdWarGameLogic.Simulation

Namespace Gui

    Public Class MouseController

        Public Event HoveredProvinceChanged As EventHandler
        Public Event SelectedProvinceChanged As EventHandler
        Public Event HoveredCountryChanged As EventHandler
        Public Event SelectedCountryChanged As EventHandler

        Public Sub New(ByRef gameController As MasterController)
            gameController_ = gameController
            provinceMap_ = gameController.GetWorldManager().provinceMap
        End Sub

        Public Sub SetMapScalingFactor(guiViewSize As Size, mapSize As Size)
            Dim xFactor As Double = mapSize.Width / guiViewSize.Width
            Dim yFactor As Double = mapSize.Height / guiViewSize.Height
            mapScaling_ = Math.Max(xFactor, yFactor)
        End Sub

        Public Sub HandleMouseMovedOverMap(e As MouseEventArgs)
            Dim mapX As Integer = e.X * mapScaling_
            Dim mapY As Integer = e.Y * mapScaling_

            Dim mouseProvinceId As Integer = GetProvinceUnderMouse(mapX, mapY)
            If (mouseProvinceId <> -1) And (mouseProvinceId <> currentProvinceId_) Then
                RaiseEvent HoveredProvinceChanged(Me, New ProvinceEventArgs(mouseProvinceId))
                CheckChangedCountry(mouseProvinceId)
                currentProvinceId_ = mouseProvinceId
            End If

        End Sub

        Private Function GetProvinceUnderMouse(mapX As Integer, mapY As Integer) As Integer
            Dim pixelTuple As Tuple(Of Byte, Byte, Byte) = provinceMap_.GetPixelRgb(mapX, mapY)
            Dim mouseProvinceId As Integer = provinceMap_.GetProvinceNumber(pixelTuple)
            Return mouseProvinceId
        End Function

        Private Function GetCountryTagFromProvinceId(provinceId As Integer) As String
            Dim provTable As IDictionary(Of Integer, Province) = GetCurrentProvinceTable()
            If provinceId < 0 Or provinceId > provTable.Count Then
                Return String.Empty
            End If
            Dim prov As Province = provTable(provinceId)
            Return prov.GetOwner()
        End Function

        Private Function GetCurrentProvinceTable() As IDictionary(Of Integer, Province)
            Return gameController_.tickHandler.GetState().GetProvinceTable()  ' TODO: Rethink architecture
        End Function

        Private Sub CheckChangedCountry(ProvinceId As Integer)
            Dim mouseCountryTag As String = GetCountryTagFromProvinceId(ProvinceId)
            If mouseCountryTag <> currentCountryTag_ Then
                RaiseEvent HoveredCountryChanged(Me, New CountryEventArgs(mouseCountryTag))
            End If
        End Sub

        Private gameController_ As MasterController = Nothing
        Private provinceMap_ As ProvinceMap = Nothing

        Private currentProvinceId_ As Integer = -1
        Private currentCountryTag_ As String = ""
        Private mapScaling_ As Double = 0.0


    End Class

    ''' <summary>
    ''' Event argument class for a province ID.
    ''' </summary>
    Public Class ProvinceEventArgs
        Inherits EventArgs

        Public Property provinceId As Integer

        Public Sub New(provinceIdPar As Integer)
            provinceId = provinceIdPar
        End Sub

    End Class

    ''' <summary>
    ''' Event argument class for a country tag.
    ''' </summary>
    Public Class CountryEventArgs
        Inherits EventArgs

        Public Property countryTag As String

        Public Sub New(countryTagPar As String)
            countryTag = countryTagPar
        End Sub

    End Class

End Namespace