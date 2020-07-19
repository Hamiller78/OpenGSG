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
Imports System.Drawing

Imports OpenGSGLibrary.WorldData
Imports OpenGSGLibrary.GameLogic
Imports OpenGSGLibrary.Map
Imports ColdWarGameLogic.WorldData

Namespace Simulation

    Public Class MasterController

        Private Const GAMEDATA_PATH As String = "..\..\..\ColdWarPrototype\GameData"

        Public ReadOnly worldData As New WorldDataManager
        Public ReadOnly tickHandler As New TickHandler

        Public Sub Init()
            Dim startState As OpenGSGLibrary.WorldData.WorldState =
              worldLoader_.CreateStartState(GAMEDATA_PATH)
            tickHandler.ConnectProvinceEventHandlers(startState)  'TODO: Set state in separate method

            worldData.LoadAll(GAMEDATA_PATH) ' Only map views are still in WorldDataManager

        End Sub

        Public Function GetWorldManager() As WorldDataManager  'TODO: is public property now, this is unnecessary
            Return worldData
        End Function

        Private worldLoader_ As New OpenGSGLibrary.WorldData.WorldLoader(Of CwpProvince, CwpCountry)

    End Class

End Namespace