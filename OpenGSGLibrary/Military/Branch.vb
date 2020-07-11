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
Public Class Branch

    Public Enum Type
        Army
        Navy
        Airforce
        Space
    End Enum

    Private Property type_

    Public Sub New(type As Branch.Type)
        type_ = type
    End Sub

    Public Sub New(typeName As String)
        Select Case typeName.ToLower()
            Case "army"
                type_ = Type.Army
            Case "navy"
                type_ = Type.Navy
            Case "airforce"
                type_ = Type.Airforce
            Case "space"
                type_ = Type.Space
            Case Else
                Throw New ApplicationException("Unknown type for military branch: " & typeName)
        End Select
    End Sub

End Class
