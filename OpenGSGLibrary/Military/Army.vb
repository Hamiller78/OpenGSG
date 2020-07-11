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
    ''' Class to describe one army.
    ''' An army is moved on the map as an unit.
    ''' It contains an arbitrary number of divisions.
    ''' </summary>
    Public Class Army
        Inherits WorldData.GameObject

        Public Property units As List(Of Division)

        Private owner_ As String = ""
        Private name_ As String = ""
        Private location_ As Integer = 0
        Private divisions_ As List(Of Division) = Nothing

        ''' <summary>
        ''' Sets the properties of a army from the parser data of the unit file.
        ''' This method handles army name, location and contained divisions.
        ''' Should be inherited to handle more game-specific properties.
        ''' </summary>
        ''' <param name="fileName"></param>
        ''' <param name="parsedData"></param>
        Public Overrides Sub SetData(fileName As String, parsedData As Lookup(Of String, Object))
            name_ = parsedData("name").Single()
            location_ = Val(parsedData("location").Single())
            divisions_ = WorldData.GameObjectFactory.ListFromLookup(Of Division)(parsedData, "division")
        End Sub

        ''' <summary>
        ''' Sets the nation tag of the army.
        ''' </summary>
        ''' <param name="tag">String with nation tag, e.g. "USA"</param>
        Public Sub SetOwner(tag As String)
            owner_ = tag
            If divisions_ IsNot Nothing Then
                For Each division In divisions_
                    division.SetOwner(tag)
                Next
            End If
        End Sub

        ''' <summary>
        ''' Returns the location id where the army is.
        ''' </summary>
        ''' <returns>Iteger with province id.</returns>
        Public Function GetLocation() As Integer
            Return location_
        End Function

        ''' <summary>
        ''' Sets the location of the army.
        ''' </summary>
        ''' <param name="provinceId">Integer with province id.</param>
        Public Sub SetLocation(provinceId As Integer)
            location_ = provinceId
        End Sub

        ''' <summary>
        ''' Returns a string with name of army.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return name_
        End Function

    End Class

End Namespace