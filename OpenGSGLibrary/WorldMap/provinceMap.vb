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

Imports Microsoft.VisualBasic.FileIO

Namespace Map
    ''' <summary>
    ''' Bitmap for province view. RGB values mark provinces, which are translated names and IDs in extra CSV file.
    ''' </summary>
    Public Class ProvinceMap
        Inherits LayerBitmap

        ''' <summary>
        ''' Gets the province number for a RGB value.
        ''' </summary>
        ''' <param name="rgbKey">RGB value as a tuple of three bytes.</param>
        ''' <returns>Province number as integer, -1 if not found.</returns>
        Public Function GetProvinceNumber(rgbKey As Tuple(Of Byte, Byte, Byte)) As Integer
            Dim provinceNumber As Integer
            If rgbProvinceMap_.ContainsKey(rgbKey) Then
                provinceNumber = rgbProvinceMap_.Item(rgbKey)
            Else
                provinceNumber = -1
            End If
            Return provinceNumber
        End Function
        ''' <summary>
        ''' Gets the province name for a RGB value.
        ''' </summary>
        ''' <param name="provinceNumber">Id number of province.</param>
        ''' <returns>Province name as string, empty string if not found.</returns>
        Public Function GetProvinceName(provinceNumber As Integer) As String
            Dim provinceName As String
            Try
                provinceName = provinceNames_.Item(provinceNumber)
            Catch ex As Exception
                provinceName = ""
            End Try
            Return provinceName
        End Function

        ''' <summary>
        ''' Loads province RGB values from a CSV file.
        ''' Expected Format: Number;R;G;B;Name
        ''' Ignores first line (headline).
        ''' </summary>
        ''' <param name="fullFilePath">Full file path of CSV file.</param>
        Public Sub LoadProvinceRGBs(fullFilePath As String)
            Dim csvParser = New TextFieldParser(fullFilePath)
            csvParser.TextFieldType = FieldType.Delimited
            csvParser.SetDelimiters(";")

            Dim newRgbProvinceMap As New Dictionary(Of Tuple(Of Byte, Byte, Byte), Integer)
            Dim newNameMap As New Dictionary(Of Integer, String)
            Dim headline = csvParser.ReadFields()  'ignoring the headline
            Dim currentRow As String()

            While Not csvParser.EndOfData
                Try
                    currentRow = csvParser.ReadFields()
                    Dim provinceColor As Tuple(Of Byte, Byte, Byte) _
                        = Tuple.Create(Of Byte, Byte, Byte)(Val(currentRow(1)), Val(currentRow(2)), Val(currentRow(3)))
                    Dim provinceNumber As Integer = currentRow(0)
                    Dim provinceName As String = currentRow(4)
                    If Not newRgbProvinceMap.ContainsKey(provinceColor) Then
                        newRgbProvinceMap.Add(provinceColor, provinceNumber)
                        newNameMap.Add(provinceNumber, provinceName)
                    End If
                Catch ex As MalformedLineException
                    Console.Write("Line couldn't be parsed: " + ex.ToString())
                Catch ex As IndexOutOfRangeException
                    Console.Write("Line didn't contain enough fields: " + ex.ToString())
                Catch ex As InvalidCastException
                    Console.Write("Cast error for one or more fields: " + ex.ToString())
                End Try
            End While

            rgbProvinceMap_ = newRgbProvinceMap
            provinceNames_ = newNameMap
        End Sub

        Private rgbProvinceMap_ As New Dictionary(Of Tuple(Of Byte, Byte, Byte), Integer)
        Private provinceNames_ As New Dictionary(Of Integer, String)

    End Class

End Namespace