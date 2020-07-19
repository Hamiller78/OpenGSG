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

Imports System.IO

Imports OpenGSGLibrary.Tools
Imports ColdWarGameLogic.Simulation

Public Class MainWindow

    ' Global objects
    Public log As Logger = New Logger("CWPLog.log", Directory.GetCurrentDirectory())

    Private gameController_ As New MasterController

    ' Views
    Private provinceInfo_ As MainWindowView.ProvinceInfo = Nothing
    Private countryInfo_ As MainWindowView.CountryInfo = Nothing
    Private armyBox_ As MainWindowView.ArmyList = Nothing
    Private coordinateView_ As MainWindowView.GeoCoordinates = Nothing
    Private worldMapView_ As MainWindowView.WorldMap = Nothing

    ' Controllers
    Private mouseController_ As Gui.MouseController = Nothing

    ' GUI event handlers
    Private Sub MainWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        log.WriteLine(LogLevel.Info, "Session started, TODO: version information")

        gameController_.Init()

        SetupViews()
        SetupControllers()
        SetupEventHandlers()

        log.WriteLine(LogLevel.Info, "Main window loaded")
    End Sub

    Private Sub SetupViews()

        provinceInfo_ = New MainWindowView.ProvinceInfo(Me, gameController_)
        countryInfo_ = New MainWindowView.CountryInfo(Me)
        armyBox_ = New MainWindowView.ArmyList(Me)
        coordinateView_ = New MainWindowView.GeoCoordinates(Me)

        worldMapView_ = New MainWindowView.WorldMap(Me)
        worldMapView_.SetSourceProvinceMap(gameController_.GetWorldManager().provinceMap)  'ugly
    End Sub

    Private Sub SetupControllers()
        mouseController_ = New Gui.MouseController(gameController_)
        Dim sourceMapSize As Size = gameController_.worldData.provinceMap.sourceBitmap.Size
        mouseController_.SetMapScalingFactor(MapPictureBox.Size, sourceMapSize)
    End Sub

    Private Sub SetupEventHandlers()
        AddHandler mouseController_.HoveredProvinceChanged, AddressOf provinceInfo_.HandleProvinceChanged
    End Sub

    Private Sub MapPictureBox_MouseMove(sender As Object, e As MouseEventArgs) Handles MapPictureBox.MouseMove
        mouseController_.HandleMouseMovedOverMap(e)
    End Sub

    '    Private Sub MapPictureBox_MouseClick(sender As Object, e As MouseEventArgs) Handles MapPictureBox.MouseClick
    '        Dim mapX As Integer = e.X * mapScaling_
    '        Dim mapY As Integer = e.Y * mapScaling_
    '
    '        Dim mouseProvinceId As Integer = GetProvinceUnderMouse(mapX, mapY)
    '
    '        If (mouseProvinceId <> -1) Then
    '            If (mouseProvinceId <> currentProvinceId_) Then
    '                currentProvinceId_ = mouseProvinceId
    '            End If
    '            ' move armies if in army move mode
    '            If isChoosingTarget_ Then
    '                MoveSelectedArmies(currentProvinceId_)
    '                isChoosingTarget_ = False
    '            End If
    '            UpdateArmyListBox(mouseProvinceId)
    '        End If
    '
    '    End Sub

    '    Private Sub MapModePolitical_CheckedChanged(sender As Object, e As EventArgs) Handles MapModePolitical.CheckedChanged
    '        SetMapPicture()
    '    End Sub
    '
    '    Private Sub MapModeRaw_CheckedChanged(sender As Object, e As EventArgs) Handles MapModeRaw.CheckedChanged
    '        SetMapPicture()
    '    End Sub

    '    Private Sub DateButton_Click(sender As Object, e As EventArgs) Handles DateButton.Click
    '        tickHandler_.FinishTick()
    '
    '        UpdateDateText()
    '        UpdateCountryInfo(currentCountryTag_)
    '        UpdateProvinceInfo(currentProvinceId_)
    '    End Sub

    '    Private Sub MoveArmiesButton_Click(sender As Object, e As EventArgs) Handles MoveArmiesButton.Click
    '        Dim selectedArmyIndices As ListBox.SelectedIndexCollection = ArmyListBox.SelectedIndices
    '
    '        selectedArmies_.Clear()
    '        For Each listIndex As Integer In selectedArmyIndices
    '            selectedArmies_.Add(armiesInProvince_.ElementAt(listIndex))
    '        Next
    '
    '        If selectedArmies_.Count > 0 Then
    '            isChoosingTarget_ = True
    '        End If
    '
    '    End Sub
    '
    '    ' helper functions
    '
    '
    '    Private Sub MoveSelectedArmies(targetProvinceId As Integer)
    '        For Each movingArmy In selectedArmies_
    '            tickHandler_.GetState().GetArmyManager().MoveArmy(movingArmy, targetProvinceId)
    '        Next
    '    End Sub
    '
    '    Private Sub UpdateDateText()
    '        DateButton.Text = gameDate_.ToString()
    '    End Sub

End Class
