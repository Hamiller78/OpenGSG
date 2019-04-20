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

    Public Class Country
        Inherits GameObject

        Public Function GetTag() As String
            Return tag_
        End Function

        Public Function GetName() As String
            Return name_
        End Function

        Public Function GetColor() As Tuple(Of Byte, Byte, Byte)
            Return color_
        End Function

        Public Overloads Sub SetData(fileName As String, parsedData As Dictionary(Of String, Object))
            MyBase.SetData(fileName, parsedData)

            Dim fileNameParts As String() = FileManager.ExtractFromFilename(fileName)
            name_ = Path.GetFileNameWithoutExtension(fileName)

            tag_ = parsedData("tag")
            Dim colorList As List(Of Integer) = parsedData("color")
            Dim rValue As Byte = colorList(0)
            Dim gValue As Byte = colorList(1)
            Dim bValue As Byte = colorList(2)
            Dim colorCode As Tuple(Of Byte, Byte, Byte) = New Tuple(Of Byte, Byte, Byte)(rValue, gValue, bValue)
            color_ = colorCode
        End Sub

        Private tag_ As String
        Private name_ As String
        Private color_ As Tuple(Of Byte, Byte, Byte)

    End Class

End Namespace