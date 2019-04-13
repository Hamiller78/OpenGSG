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

        ' Load province map
        coldWarWorld.LoadAll("..\..\..\ColdWarPrototype\GameData")
        provinceMap_ = coldWarWorld.provinceMap

        ' Resize map for screen output
        Dim sourceSize As Size = provinceMap_.sourceBitmap.Size
        SetMapScalingFactor(MapPictureBox.Size, sourceSize)
        Dim newSize As Size = New Size(sourceSize.Width / mapScaling_, sourceSize.Height / mapScaling_)
        Dim resizedBitmap As Bitmap = New Bitmap(provinceMap_.sourceBitmap, newSize)

        ' Set the resized map in PictureBox
        MapPictureBox.Image = resizedBitmap
        MapPictureBox.Invalidate()
    End Sub

    Private Sub MapPictureBox_MouseClick(sender As Object, e As MouseEventArgs) Handles MapPictureBox.MouseClick
        Dim mouseX As Integer = e.X
        Dim mouseY As Integer = e.Y

        Dim mapX As Integer = e.X * mapScaling_
        Dim mapY As Integer = e.Y * mapScaling_

        Dim pixelTuple As Tuple(Of Byte, Byte, Byte) = provinceMap_.GetPixelRgb(mapX, mapY)
        Dim provinceId As Integer = provinceMap_.GetProvinceNumber(pixelTuple)
        If provinceId <> -1 Then
            Dim provinceName As String = provinceMap_.GetProvinceName(provinceId)
            MsgBox(provinceName)
        End If

    End Sub

    Private provinceMap_ As ProvinceMap
    Private mapScaling_ As Double = 0.0

    Private Sub SetMapScalingFactor(newSize As Size, originalSize As Size)
        Dim xFactor As Double = originalSize.Width / newSize.Width
        Dim yFactor As Double = originalSize.Height / newSize.Height
        mapScaling_ = Math.Max(xFactor, yFactor)
    End Sub

End Class
