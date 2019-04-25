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
        ''' <param name="fileName">Name of the source file for the object's data.</param>
        ''' <param name="parsedData">Structure with the parsed data.</param>
        Public Overridable Sub SetData(fileName As String, parsedData As Dictionary(Of String, Object))
            fileName_ = fileName
            parsedData_ = parsedData
        End Sub

        Private fileName_ As String = ""
        Private parsedData_ = New Dictionary(Of String, Object)

    End Class

End Namespace