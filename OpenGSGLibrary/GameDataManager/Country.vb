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

Imports System.IO

Namespace WorldData

    ''' <summary>
    ''' Base class for countries. Handles the most basic country properties.
    ''' </summary>
    Public MustInherit Class Country
        Inherits GameObject

        Public Sub New()
        End Sub

        Public Function GetTag() As String
            Return tag_
        End Function

        Public Function GetName() As String
            Return name_
        End Function

        Public Function GetColor() As Tuple(Of Byte, Byte, Byte)
            Return color_
        End Function

        ''' <summary>
        ''' Sets the properties of a country from the parser data of the country file.
        ''' This method handles country tag, name and the color tuple.
        ''' Should be inherited to handle more game-specific properties.
        ''' </summary>
        ''' <param name="fileName"></param>
        ''' <param name="parsedData"></param>
        Public Overrides Sub SetData(fileName As String, parsedData As Lookup(Of String, Object))
            MyBase.SetData(fileName, parsedData)

            Dim fileNameParts As String() = GameObjectFactory.ExtractFromFilename(fileName)
            name_ = Path.GetFileNameWithoutExtension(fileName)

            tag_ = parsedData("tag").Single()
            Dim colorList As List(Of Integer) = parsedData("color").Single()
            Dim rValue As Byte = colorList(0)
            Dim gValue As Byte = colorList(1)
            Dim bValue As Byte = colorList(2)
            Dim colorCode As Tuple(Of Byte, Byte, Byte) = New Tuple(Of Byte, Byte, Byte)(rValue, gValue, bValue)
            color_ = colorCode
        End Sub

        ''' <summary>
        ''' Loads all flag images for the country from files.
        ''' The exact implementation is game specific, e.g. HOI4 has variations for ideology
        ''' </summary>
        ''' <param name="flagPath">String with path to the folder containing the flag image files.</param>
        Public MustOverride Sub LoadFlags(flagPath As String)

        Private tag_ As String
        Private name_ As String
        Private color_ As Tuple(Of Byte, Byte, Byte)

    End Class

End Namespace