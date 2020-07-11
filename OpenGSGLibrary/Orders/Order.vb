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
Namespace Orders

    Public MustInherit Class Order

        Public Function GetCompletionTick() As Integer
            Return completeTick_
        End Function

        Public MustOverride Sub FinalizeOrder(ByRef currentWorld As WorldData.WorldState)

        Private startTick_ As New Long
        Private completeTick_ As New Long

        Private type_ As New OrderType

    End Class

End Namespace