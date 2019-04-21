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

        Public Function GetProvinceTable() As Dictionary(Of Integer, CwpProvince)
            Return provinceTable_
        End Function

        Public Function GetCountryTable() As Dictionary(Of String, CwpCountry)
            Return countryTable_
        End Function

        ReadOnly Property countryData As New Dictionary(Of String, CwpCountry)

        Public Sub LoadAll(gamedataPath As String)
            If Not Directory.Exists(gamedataPath) Then
                Throw New DirectoryNotFoundException("Given gamedata directory not found: " & gamedataPath)
            End If

            LoadWorldmap(Path.Combine(gamedataPath, "map"))
            provinceMap.LoadProvinceRGBs(Path.Combine(gamedataPath, "map\definitions.csv"))

            provinceTable_ = FileManager.CreateObjectsFromFolder(Of Integer, CwpProvince) _
                                (Path.Combine(gamedataPath, "history\provinces"), "filename_0")
            countryTable_ = FileManager.CreateObjectsFromFolder(Of String, CwpCountry) _
                                (Path.Combine(gamedataPath, "common\countries"), "tag")

        End Sub

        Private provinceTable_ As New Dictionary(Of Integer, CwpProvince)
        Private countryTable_ As New Dictionary(Of String, CwpCountry)

        Private Sub LoadWorldmap(filePath As String)
            provinceMap.FromFile(Path.Combine(filePath, "provinces.bmp"))
        End Sub

    End Class

End Namespace