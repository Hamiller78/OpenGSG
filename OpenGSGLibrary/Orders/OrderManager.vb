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

    Public Class OrderManager

        Private orderList_ As New List(Of Order)
        Private currentWorldState_ As WorldData.WorldState = Nothing

        ''' <summary>
        ''' Sets the WorldState to apply orders to.
        ''' </summary>
        ''' <param name="newWorldState">WorldState object to use for orders.</param>
        Public Sub SetWorldState(newWorldState As WorldData.WorldState)
            currentWorldState_ = newWorldState
        End Sub

        ''' <summary>
        ''' Adds an Order object to the list of open orders.
        ''' </summary>
        ''' <param name="newOrder">Order object.</param>
        Public Sub AddOrder(newOrder As Order)
            orderList_.Add(newOrder)
        End Sub

        ''' <summary>
        ''' Handler for game ticks.
        ''' Checks if orders should be finalized and removed from the list.
        ''' </summary>
        ''' <param name="sender">sender TickHandler</param>
        ''' <param name="e">TickEventArgs containing current game time</param>
        Public Sub OnTickDone(sender As Object, e As GameLogic.TickEventArgs)

            ' TODO: Optimize this for performance?
            For Each dueOrder As Order In orderList_.Where(Function(ord) ord.GetCompletionTick() = e.tick)
                dueOrder.FinalizeOrder(currentWorldState_)
            Next
            orderList_.RemoveAll(Function(ord) ord.GetCompletionTick() = e.tick)

        End Sub

    End Class

End Namespace