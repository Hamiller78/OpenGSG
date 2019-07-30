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

Namespace Military

    ''' <summary>
    ''' Manages all units in the game, e.g. units per province, per country, etc...
    ''' </summary>
    Public Class ArmyManager

        Public Sub LoadFolder(unitsPath As String)
            If Not Directory.Exists(unitsPath) Then
                Throw New DirectoryNotFoundException("Given game data directory not found: " & unitsPath)
            End If

            Dim nationMilitaryTable_ = WorldData.GameObjectFactory.FromFolder(Of String, NationMilitary)(unitsPath, "tag")

            ' TODO: Update provinceIdToArmiesTable with data drom countryToArmiesTable_

        End Sub

        Private provinceIdToArmiesTable_ As Dictionary(Of Integer, List(Of Army))
        Private nationMilitaryTable_ As Dictionary(Of String, NationMilitary)

    End Class

End Namespace