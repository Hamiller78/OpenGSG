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
Imports System.Drawing

Namespace Map

    ''' <summary>
    ''' Base class for bitmaps of world map.
    ''' </summary>
    Public Class LayerBitmap

        Public ReadOnly Property sourceBitmap
            Get
                Return sourceBitmap_
            End Get
        End Property

        ''' <summary>
        ''' Sets the contained bitmap from a file.
        ''' </summary>
        ''' <param name="filePathAndName">String with full file path of image file.</param>
        Public Sub FromFile(filePathAndName As String)
            sourceBitmap_ = Image.FromFile(filePathAndName)
            sourceBitmap_ = New Bitmap(sourceBitmap_)
        End Sub

        ''' <summary>
        ''' Gets the RGB value of a pixel in the bitmap.
        ''' </summary>
        ''' <param name="x">x coordinate of target pixel in original image coordinates.</param>
        ''' <param name="y">y coordinate of target pixel in original image coordinates.</param>
        ''' <returns>RGB value as tuple of 3 bytes.</returns>
        Public Function GetPixelRgb(x As Integer, y As Integer) As Tuple(Of Byte, Byte, Byte)
            Try
                Dim color As Color = sourceBitmap_.GetPixel(x, y)
                Return New Tuple(Of Byte, Byte, Byte)(color.R, color.G, color.B)
            Catch ex As ArgumentOutOfRangeException
                Return New Tuple(Of Byte, Byte, Byte)(0, 0, 0)
            End Try
        End Function

        Private sourceBitmap_ As Bitmap
    End Class

End Namespace