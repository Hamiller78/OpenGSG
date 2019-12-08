﻿'    OpenSGSGLibrary is an open-source library for Grand Strategy Games
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

        Public Function GetProvinceTable() As Dictionary(Of Integer, CwpProvince)
            Return provinceTable_
        End Function

        Public Function GetCountryTable() As Dictionary(Of String, CwpCountry)
            Return countryTable_
        End Function

        Public Function GetArmyManager() As ArmyManager
            Return armyManager_
        End Function

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

            provinceTable_ = GameObjectFactory.FromFolderWithFilenameId(Of CwpProvince) _
                                (Path.Combine(gamedataPath, "history\provinces"))
            countryTable_ = GameObjectFactory.FromFolder(Of String, CwpCountry) _
                                (Path.Combine(gamedataPath, "common\countries"), "tag")
            LoadCountryFlags(Path.Combine(gamedataPath, "gfx\flags"))

            armyManager_.LoadFolder(Path.Combine(gamedataPath, "history\units"))

        End Sub

        ''' <summary>
        ''' Sets the ickDone handler for all provinces
        ''' </summary>
        ''' <param name="tickHandler"></param>
        Public Sub SetAllProvinceHandlers(ByRef tickHandler As TickHandler)
            For Each provinceEntry In provinceTable_
                AddHandler tickHandler.TickDone, AddressOf provinceEntry.Value.OnTickDone
            Next
        End Sub


        ''' <summary>
        ''' Getter for a countrys' production.
        ''' Calculates the value by adding the province values of that country.
        ''' </summary>
        ''' <param name="countryTag">String with the country tag (e.g. "GDR").</param>
        ''' <returns></returns>
        Public Function GetCountryProduction(countryTag As String) As Long
            Dim countryProduction As Long = 0
            For Each province In provinceTable_
                If province.Value.GetController() = countryTag Then
                    countryProduction += province.Value.production
                End If
            Next
            Return countryProduction
        End Function

        Private provinceTable_ As New Dictionary(Of Integer, CwpProvince)
        Private countryTable_ As New Dictionary(Of String, CwpCountry)
        Private armyManager_ As New ArmyManager()

        Private Sub LoadWorldmap(filePath As String)
            provinceMap.FromFile(Path.Combine(filePath, "provinces.bmp"))
        End Sub

        Private Sub LoadCountryFlags(flagPath As String)
            For Each country In countryTable_
                country.Value.LoadFlags(flagPath)
            Next
        End Sub

    End Class

End Namespace