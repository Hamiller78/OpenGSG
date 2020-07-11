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

    Public MustInherit Class Player

        Private tickDone_ As Boolean = False

        Private playerIndex_ As Integer
        Private country_ As WorldData.Country

        ''' <summary>
        ''' Flag whether player is done for the turn.
        ''' For human players can be set to false to pause (e.g. when an event pops up)
        ''' </summary>
        ''' <returns>Boolean whether player is done for this tick</returns>
        Public Function IsTickDone() As Boolean
            Return tickDone_
        End Function

    End Class

End Namespace