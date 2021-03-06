﻿'    OpenSGSGLibrary is an open-source library for Grand Strategy Games
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
Imports System.Linq

Imports OpenGSGLibrary.WorldData

Namespace WorldData

    ''' <summary>
    ''' Country class for cold war game. Adds specialised properties to base country class.
    ''' </summary>
    Public Class CwpCountry
        Inherits Country

        Public Property longName As String = ""
        Public Property government As String = ""
        Public Property allegiance As String = ""
        Public Property leader As String = ""

        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Sets the country properties from the parsed data.
        ''' </summary>
        ''' <param name="fileName">Name of the source file of country object.</param>
        ''' <param name="parsedData">Object with the parsed data from that file.</param>
        Public Overrides Sub SetData(fileName As String, parsedData As Lookup(Of String, Object))
            MyBase.SetData(fileName, parsedData)

            longName = parsedData("long_name").Single()
            government = parsedData("government").Single()
            If parsedData.Contains("allegiance") Then
                allegiance = parsedData("allegiance").Single()
            Else
                allegiance = ""
            End If
            leader = parsedData("leader").Single()

        End Sub

    End Class

End Namespace