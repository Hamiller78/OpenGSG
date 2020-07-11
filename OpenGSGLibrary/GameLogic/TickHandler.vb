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
Imports OpenGSGLibrary.WorldData

Namespace GameLogic

    Public Class TickHandler

        Public Event TickDone As EventHandler

        Private playerManager_ As New PlayerManager()
        Private currentWorldState_ As WorldState
        Private currentTick_ As Integer = 0

        ''' <summary>
        ''' Connects world state to tick handler which includes setting the state in TickHandlers
        ''' and associating event handlers. 
        ''' </summary>
        ''' <param name="newState">WorldState to set in TickHandler.</param>
        Public Sub ConnectState(newState As WorldState)
            currentWorldState_ = newState

            Dim provinceDict As IDictionary(Of Integer, Province) = currentWorldState_.GetProvinceTable()
            For Each province In provinceDict.Values
                AddHandler TickDone, AddressOf province.OnTickDone
            Next
        End Sub

        ''' <summary>
        ''' Returns the world state used by the tick handler
        ''' </summary>
        ''' <returns>WorldState object for the current tick which will be modified each tick.</returns>
        Public Function GetState() As WorldState
            Return currentWorldState_
        End Function

        ''' <summary>
        ''' Method to do stuff when an new tick starts.
        ''' </summary>
        Public Sub BeginNewTick()
            ' do timed game events
            ' launch AI threads
            playerManager_.CalculateStrategies(currentWorldState_)
            ' collect GUI input
        End Sub

        ''' <summary>
        ''' Is the current tick complete?
        ''' </summary>
        ''' <returns>Boolean whether the current tick is complete</returns>
        Public Function IsTickComplete() As Boolean
            ' TODO: tick time complete?

            If playerManager_.IsEverybodyDone() = False Then
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' Method to do stuff when a tick is completed and the next WorldState is calculated.
        ''' </summary>
        Public Sub FinishTick()
            ' lock GUI input
            ' calculate world in next tick
            currentTick_ += 1
            ' notify all interested classes
            RaiseEvent TickDone(Me, New TickEventArgs(currentTick_))

        End Sub

    End Class

    ''' <summary>
    ''' Helper class passed with the TickDone event.
    ''' Extra argument is the current tick number.
    ''' </summary>
    Public Class TickEventArgs
        Inherits EventArgs

        Public Property tick As Integer

        Public Sub New(tickPar As Integer)
            tick = tickPar
        End Sub

    End Class

End Namespace