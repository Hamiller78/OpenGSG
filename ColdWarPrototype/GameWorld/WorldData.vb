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
Imports OpenGSGLibrary.WorldData

Namespace GameWorld

    Public Class WorldData

        ReadOnly Property provinceMap As New ProvinceMap
        ReadOnly Property provinceData As New ProvinceManager
        ReadOnly Property countryData As New CountryManager

        Public Sub LoadAll(gamedataPath As String)
            If Not Directory.Exists(gamedataPath) Then
                Throw New DirectoryNotFoundException("Given gamedata directory not found: " & gamedataPath)
            End If

            LoadWorldmap(Path.Combine(gamedataPath, "map"))
            provinceMap.LoadProvinceRGBs(Path.Combine(gamedataPath, "map\definitions.csv"))
            provinceData.LoadAllProvinceFiles(Path.Combine(gamedataPath, "history\provinces"))
            countryData.LoadAllCountryFiles(Path.Combine(gamedataPath, "common\countries"))

        End Sub

        Private Sub LoadWorldmap(filePath As String)
            provinceMap.FromFile(Path.Combine(filePath, "provinces.bmp"))
        End Sub

    End Class

End Namespace