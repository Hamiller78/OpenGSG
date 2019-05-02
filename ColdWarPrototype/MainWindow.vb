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
Imports System.IO

Imports OpenGSGLibrary.Map
Imports OpenGSGLibrary.Tools

Public Class MainWindow

    ' Global objects
    Public log As Logger = New Logger("CWPLog.log", Directory.GetCurrentDirectory())

    ' General game data
    Private coldWarWorld_ As GameWorld.WorldDataManager = New GameWorld.WorldDataManager()
    Private gameDate_ = New DateTime(1950, 1, 1)

    ' GUI related members
    Private currentProvinceId_ As Integer = -1
    Private currentCountryTag_ As String = ""

    ' Map related members
    Private provinceMap_ As ProvinceMap
    Private countryMap_ As Bitmap
    Private mapScaling_ As Double = 0.0
    Private mapProjection_ As RobinsonProjection = New RobinsonProjection

    ' GUI event handlers
    Private Sub MainWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        log.WriteLine(LogLevel.Info, "Session started, TODO: version information")

        ' Load province map
        coldWarWorld_.LoadAll("..\..\..\ColdWarPrototype\GameData")
        provinceMap_ = coldWarWorld_.provinceMap

        ' Render maps
        Dim MapRenderer = New CountryMapRenderer(Of GameWorld.CwpProvince, GameWorld.CwpCountry)(provinceMap_)
        MapRenderer.SetDataTables(coldWarWorld_.GetProvinceTable, coldWarWorld_.GetCountryTable)
        countryMap_ = MapRenderer.RenderMap()

        ' Set map
        SetMapPicture()
        mapProjection_.SetCapitalR(MapPictureBox.Width / 2)

        ' Set text of date button
        UpdateDateText()

        log.WriteLine(LogLevel.Info, "Main window loaded")
    End Sub

    Private Sub MapPictureBox_MouseMove(sender As Object, e As MouseEventArgs) Handles MapPictureBox.MouseMove
        Dim mouseX As Integer = e.X
        Dim mouseY As Integer = e.Y

        Dim mapX As Integer = e.X * mapScaling_
        Dim mapY As Integer = e.Y * mapScaling_

        Dim pixelTuple As Tuple(Of Byte, Byte, Byte) = provinceMap_.GetPixelRgb(mapX, mapY)
        Dim mouseProvinceId As Integer = provinceMap_.GetProvinceNumber(pixelTuple)
        If (mouseProvinceId <> -1) And (mouseProvinceId <> currentProvinceId_) Then
            UpdateProvinceInfo(mouseProvinceId)
        End If

        Dim mapCoords = New Tuple(Of Double, Double)(mouseX - MapPictureBox.Width / 2D + 82, mouseY - MapPictureBox.Height / 2D + 82)
        Dim mouseGeoCoord As GeoCoordinate = mapProjection_.getGlobeCoordinates(mapCoords)
        CoordinateDisplay.Text = mouseGeoCoord.ToString()
        'CoordinateDisplay.Text = mapCoords.ToString()

    End Sub

    Private Sub MapModePolitical_CheckedChanged(sender As Object, e As EventArgs) Handles MapModePolitical.CheckedChanged
        SetMapPicture()
    End Sub

    Private Sub MapModeRaw_CheckedChanged(sender As Object, e As EventArgs) Handles MapModeRaw.CheckedChanged
        SetMapPicture()
    End Sub

    Private Sub DateButton_Click(sender As Object, e As EventArgs) Handles DateButton.Click
        coldWarWorld_.UpdateEverythingDaily()
        gameDate_ = gameDate_.Add(TimeSpan.FromDays(1))

        UpdateDateText()
        UpdateCountryInfo(currentCountryTag_)
        UpdateProvinceInfo(currentProvinceId_)
    End Sub

    ' helper functions
    Private Sub SetMapPicture()
        Dim renderedBitmap As Bitmap

        ' check if the required stuff is loaded
        If IsNothing(provinceMap_) Then Return

        ' Set new map
        If MapModePolitical.Checked Then
            renderedBitmap = countryMap_
        Else
            renderedBitmap = provinceMap_.sourceBitmap
        End If

        ' Resize map for screen output
        Dim sourceSize As Size = provinceMap_.sourceBitmap.Size
        SetMapScalingFactor(MapPictureBox.Size, sourceSize)
        Dim newSize As Size = New Size(sourceSize.Width / mapScaling_, sourceSize.Height / mapScaling_)
        Dim resizedBitmap As Bitmap = New Bitmap(renderedBitmap, newSize)

        ' Set the resized map in PictureBox
        MapPictureBox.Image = resizedBitmap
        MapPictureBox.Invalidate()
    End Sub

    Private Sub SetMapScalingFactor(newSize As Size, originalSize As Size)
        Dim xFactor As Double = originalSize.Width / newSize.Width
        Dim yFactor As Double = originalSize.Height / newSize.Height
        mapScaling_ = Math.Max(xFactor, yFactor)
    End Sub

    Private Sub UpdateProvinceInfo(mouseProvinceId As Integer)
        currentProvinceId_ = mouseProvinceId
        ProvinceName.Text = provinceMap_.GetProvinceName(currentProvinceId_)
        Dim currentProvince As GameWorld.CwpProvince = coldWarWorld_.GetProvinceTable(currentProvinceId_)
        ProvincePopulation.Text = Trim(Str(currentProvince.population))
        ProvinceIndustrialization.Text = Trim(Str(currentProvince.industrialization))
        ProvinceEducation.Text = Trim(Str(currentProvince.education))
        ProvinceProduction.Text = currentProvince.production

        Dim mouseCountryTag As String = currentProvince.GetOwner()
        ProvinceController.Text = currentProvince.GetController()

        If mouseCountryTag <> currentCountryTag_ Then
            UpdateCountryInfo(mouseCountryTag)
            ProvinceOwner.Text = currentCountryTag_
        End If
    End Sub

    Private Sub UpdateCountryInfo(mouseCountryTag As String)
        currentCountryTag_ = mouseCountryTag
        Dim currentCountry As GameWorld.CwpCountry = coldWarWorld_.GetCountryTable(mouseCountryTag)
        CountryName.Text = currentCountry.longName
        CountryLeader.Text = currentCountry.leader
        CountryGovernment.Text = currentCountry.government
        CountryAllegiance.Text = currentCountry.allegiance
        CountryProduction.Text = coldWarWorld_.GetCountryProduction(currentCountryTag_)
    End Sub

    Private Sub UpdateDateText()
        DateButton.Text = gameDate_.ToString()
    End Sub

End Class
