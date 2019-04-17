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

        ReadOnly Property id As Integer
        ReadOnly Property tag As String
        ReadOnly Property name As String
        ReadOnly Property color As Tuple(Of Byte, Byte, Byte)

        Public Sub New(fileName As String, parsedData As Dictionary(Of String, Object))
            Dim fileNameParts As String() = FileManager.ExtractFromFilename(fileName)
            name = Path.GetFileNameWithoutExtension(fileName)

            tag = parsedData("tag")
            Dim colorArray As Integer() = parsedData("color")
            Dim rValue As Byte = colorArray(0)
            Dim gValue As Byte = colorArray(1)
            Dim bValue As Byte = colorArray(2)
            Dim colorCode As Tuple(Of Byte, Byte, Byte) = New Tuple(Of Byte, Byte, Byte)(rValue, gValue, bValue)
            color = colorCode

            parsedData_ = parsedData
        End Sub

        Private parsedData_ = New Dictionary(Of String, Object)

    End Class

End Namespace