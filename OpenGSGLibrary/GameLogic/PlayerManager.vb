'    OpenSGSGLibrary is an open-source library for Grand Strategy Games
'    Copyright (C) 2020  Torben Kneesch
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
Namespace GameLogic

    Public Class PlayerManager
        Private Shared playerList_ As List(Of Player) = Nothing

        Public Sub CalculateStrategies(currentWorldState As WorldData.WorldState)
            Throw New NotImplementedException()
        End Sub

        ''' <summary>
        ''' Returns a flag whether all players are done with the current tick.
        ''' </summary>
        ''' <returns>Boolean whether everyone is done</returns>
        Public Function IsEverybodyDone() As Boolean
            For Each currentPlayer In playerList_
                If Not currentPlayer.IsTickDone() Then
                    Return False
                End If
            Next

            Return True
        End Function

    End Class

End Namespace