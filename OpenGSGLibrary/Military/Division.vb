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
Namespace Military

    ''' <summary>
    ''' Class to describe divisions which together form an army.
    ''' Each division consists of one unit type (e.g. one model of tanks)
    ''' </summary>
    Public Class Division
        Inherits WorldData.GameObject

        Private name_ As String = ""
        Private type_ As String = ""
        Private size_ As Integer = 0
        Private owner_ As String = ""

        ''' <summary>
        ''' Sets the properties of a division from the parser data of the unit file.
        ''' This method handles name, unit type and size of division.
        ''' Should be inherited to handle more game-specific properties.
        ''' </summary>
        ''' <param name="fileName">Name of file with data (unused).</param>
        ''' <param name="parsedData">Lookup object with parser data of division.</param>
        Public Overrides Sub SetData(fileName As String, parsedData As Lookup(Of String, Object))
            name_ = parsedData("name").Single()
            type_ = parsedData("type").Single()
            size_ = parsedData("size").Single()
        End Sub

        ''' <summary>
        ''' Sets the owner of the division.
        ''' </summary>
        ''' <param name="tag">String with natio tag e.g. "USA".</param>
        Public Sub SetOwner(tag As String)
            owner_ = tag
        End Sub

    End Class

End Namespace