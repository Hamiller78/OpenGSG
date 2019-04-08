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

Imports System.Drawing

Namespace Map

    Public Class LayerBitmap

        Public Sub FromFile(filePathAndName As String)
            sourceBitmap_ = Image.FromFile(filePathAndName)
        End Sub

        Public Function GetPixelRgb(x As Integer, y As Integer) As Tuple(Of Byte, Byte, Byte)
            Try
                Dim color As Color = sourceBitmap_.GetPixel(x, y)
                Return New Tuple(Of Byte, Byte, Byte)(color.R, color.G, color.B)
            Catch ex As ArgumentOutOfRangeException
                Return New Tuple(Of Byte, Byte, Byte)(0, 0, 0)
            End Try
        End Function

        Private Property sourceBitmap_ As Bitmap
    End Class

End Namespace