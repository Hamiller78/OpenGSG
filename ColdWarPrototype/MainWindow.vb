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

Imports OpenGSGLibrary.Map

Public Class MainWindow

    Private coldWarWorld As GameWorld.WorldData = New GameWorld.WorldData()

    Private Sub MainWindowx_Load(sender As Object, e As EventArgs) Handles Me.Load
        coldWarWorld.LoadWorldmap("..\..\..\TestAssets\provinces.bmp")
        Dim provinceMap As ProvinceMap = coldWarWorld.provinceMap
        MapPictureBox.Image = provinceMap.sourceBitmap
        MapPictureBox.SizeMode = PictureBoxSizeMode.Zoom
        MapPictureBox.Invalidate()
    End Sub

End Class
