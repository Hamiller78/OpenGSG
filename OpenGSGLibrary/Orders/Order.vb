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
Namespace Orders

    ''' <summary>
    ''' Base class for orders.
    ''' Orders asre build orders, army move orders, etc.
    ''' They usually take a number of ticks to complete.
    ''' </summary>
    Public MustInherit Class Order

        ''' <summary>
        ''' Returns the tick in whose beginning the order will be completed.
        ''' </summary>
        ''' <returns>Tick number as long.</returns>
        Public Function GetCompletionTick() As Long
            Return completionTick_
        End Function

        ''' <summary>
        ''' Returns the progress of the order in percent.
        ''' </summary>
        ''' <param name="currentTick">Long with tick to calclulate the progress for.</param>
        ''' <returns>Single value with order progress in percent.</returns>
        Public Function GetProgressInPercent(currentTick As Long) As Single
            If completionTick_ > startTick_ Then
                Return (currentTick - startTick_) / (completionTick_ - startTick_)
            Else
                Return 100.0
            End If
        End Function

        ''' <summary>
        ''' Virtual method for derived classes.
        ''' Executes the order when it reaches its completion tick.
        ''' </summary>
        ''' <param name="currentWorld">Current world state to which the order will be applied.</param>
        Public MustOverride Sub FinalizeOrder(currentWorld As WorldData.WorldState)

        Protected startTick_ As New Long
        Protected completionTick_ As New Long

        Protected type_ As New OrderType

    End Class

End Namespace