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

Namespace Military

    ''' <summary>
    ''' Manages all units in the game, e.g. uits per province, per country, etc...
    ''' </summary>
    Public Class ArmyManager
        '
        '       Public Sub LoadFolder(unitsPath As String)
        '           If Not Directory.Exists(unitsPath) Then
        '               Throw New DirectoryNotFoundException("Given gamedata directory not found: " & unitsPath)
        '           End If
        '
        '           Dim countryArmies As List(Of Army)
        '           countryToArmiesTable_ = WorldData.GameObjectFactory.ListsFromFolder(Of String, Army) _
        '                                     (unitsPath, "filename_0")
        '
        '           ' TODO: Update provinceIdToArmiesTable with data drom countryToArmiesTable_
        '
        '       End Sub

        Private provinceIdToArmiesTable_ As Dictionary(Of Integer, List(Of Army))
        Private countryToArmiesTable_ As Dictionary(Of String, List(Of Army))

    End Class

End Namespace