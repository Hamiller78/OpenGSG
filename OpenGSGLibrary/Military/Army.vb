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

Namespace Military

    Public Class Army
        Inherits WorldData.GameObject

        Public Property units As List(Of Division)

        Private owner_ As String = ""
        Private name_ As String = ""
        Private location_ As Integer = 0
        Private divisions_ As List(Of Division) = Nothing

        Public Overrides Sub SetData(fileName As String, parsedData As Lookup(Of String, Object))
            name_ = parsedData("name").Single()
            location_ = Val(parsedData("location").Single())
            divisions_ = WorldData.GameObjectFactory.ListFromLookup(Of Division)(parsedData, "division")
        End Sub

        Public Sub SetOwner(tag As String)
            owner_ = tag
            If divisions_ IsNot Nothing Then
                For Each division In divisions_
                    division.SetOwner(tag)
                Next
            End If
        End Sub

        Public Function GetLocation() As Integer
            Return location_
        End Function

    End Class

End Namespace