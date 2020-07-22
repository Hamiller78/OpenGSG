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

Imports OpenGSGLibrary.GameLogic
Imports OpenGSGLibrary.Map
Imports OpenGSGLibrary.Military
Imports OpenGSGLibrary.WorldData

Namespace WorldData

    ''' <summary>
    ''' Top class to handle all world game data (provinces, countries, units, etc...)
    ''' </summary>
    Public Class WorldDataManager

        ReadOnly Property provinceMap As New ProvinceMap

        ''' <summary>
        ''' Loads game data from game files.
        ''' Should be called at start of program.
        ''' </summary>
        ''' <param name="gamedataPath">String with path to game files.</param>
        Public Sub LoadAll(gamedataPath As String)
            If Not Directory.Exists(gamedataPath) Then
                Throw New DirectoryNotFoundException("Given gamedata directory not found: " & gamedataPath)
            End If

            LoadWorldmap(Path.Combine(gamedataPath, "map"))
            provinceMap.LoadProvinceRGBs(Path.Combine(gamedataPath, "map\definitions.csv"))

        End Sub

        Private Sub LoadWorldmap(filePath As String)
            provinceMap.FromFile(Path.Combine(filePath, "provinces.bmp"))
        End Sub

    End Class

End Namespace