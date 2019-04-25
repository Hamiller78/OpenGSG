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

Imports OpenGSGLibrary.WorldData

Namespace GameWorld

    ''' <summary>
    ''' Country class for cold war game. Adds specialised properties to base country class.
    ''' </summary>
    Public Class CwpCountry
        Inherits Country

        Public Property longName As String = ""
        Public Property government As String = ""
        Public Property allegiance As String = ""
        Public Property leader As String = ""

        ''' <summary>
        ''' Sets the country properties from the parsed data.
        ''' </summary>
        ''' <param name="fileName">Name of the source file of county object.</param>
        ''' <param name="parsedData">Object with the parsed data from that file.</param>
        Public Overrides Sub SetData(fileName As String, parsedData As Dictionary(Of String, Object))
            MyBase.SetData(fileName, parsedData)

            longName = parsedData("long_name")
            government = parsedData("government")
            If parsedData.ContainsKey("allegiance") Then
                allegiance = parsedData("allegiance")
            Else
                allegiance = ""
            End If
            leader = parsedData("leader")

        End Sub

    End Class

End Namespace