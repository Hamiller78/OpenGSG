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

Imports System.IO

Imports OpenGSGLibrary.Map

Namespace GameWorld

    Public Class WorldData

        ReadOnly Property provinceMap
            Get
                Return provinceMap_
            End Get
        End Property

        Public Sub LoadAll(gamedataPath As String)
            If Not Directory.Exists(gamedataPath) Then
                Throw New DirectoryNotFoundException("Given gamedata directory not found: " & gamedataPath)
            End If

            LoadWorldmap(Path.Combine(gamedataPath, "map"))
            provinceMap_.LoadProvinceRGBs(Path.Combine(gamedataPath, "map\definitions.csv"))

        End Sub

        Private Sub LoadWorldmap(filePath As String)
            provinceMap_.FromFile(Path.Combine(filePath, "provinces.bmp"))
        End Sub

        Private gameDataDir As Directory
        Private provinceMap_ As New ProvinceMap()

    End Class

End Namespace