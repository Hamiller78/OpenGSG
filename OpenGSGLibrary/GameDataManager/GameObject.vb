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

Namespace WorldData

    ''' <summary>
    ''' Base class for game object classes like provinces, countries, armies, etc...
    ''' Always stores its parser data
    ''' </summary>
    Public Class GameObject

        ''' <summary>
        ''' Sets file name and parser data of the object.
        ''' This doesn't work in the constructor with generic types, hence we use an extra method.
        ''' </summary>
        ''' <param name="fileName">Name of the source file for the object's data without extension.</param>
        ''' <param name="parsedData">Structure with the parsed data.</param>
        Public Overridable Sub SetData(fileName As String, parsedData As Lookup(Of String, Object))
            fileName_ = fileName
            parsedData_ = parsedData
        End Sub

        ''' <summary>
        ''' Handler for tick done events by TickHandler.
        ''' Should be reimplemented for all game elements which require an update with each game tick.
        ''' E.g. provinces can have population growth, armies replenish their morale, etc.
        ''' </summary>
        ''' <param name="sender">sender object of the event</param>
        ''' <param name="e">TickEventArgs, contain the new tick number</param>
        Public Overridable Sub OnTickDone(sender As Object, e As GameLogic.TickEventArgs)
        End Sub

        Private fileName_ As String = ""
        Private parsedData_ As Lookup(Of String, Object) = Nothing

    End Class

End Namespace