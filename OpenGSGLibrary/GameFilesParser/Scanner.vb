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

Imports System.Collections.Generic
Imports System.IO
Imports System.Text

Namespace Parser

    ''' <summary>
    ''' Prepares a text for parsing by converting it into a stream of tokens representing control characters,
    ''' keywords and values. Unnecessary characters are removed in the rpocess.
    ''' </summary>
    Public Class Scanner

        ''' <summary>
        ''' Main Scan method of the Scanner class.
        ''' </summary>
        ''' <param name="reader">Text in the form of an implementation of TextReader i.e. StringBuilder</param>
        ''' <returns></returns>
        Public Iterator Function Scan(reader As TextReader) As IEnumerator(Of Token)
            While reader.Peek() <> -1
                Dim nextChar As Char = Chr(reader.Peek())
                If Char.IsWhiteSpace(nextChar) Or Char.IsControl(nextChar) Then
                    reader.Read()
                ElseIf nextChar = "#" Then
                    ' "#" is comment, skip rest of line
                    reader.ReadLine()
                ElseIf Char.IsLetterOrDigit(nextChar) Then
                    Dim sval As String = ScanName(reader)
                    If IsNumeric(sval) Then
                        Yield Token.FromValue(Val(sval))
                    Else
                        Yield Token.FromString(sval)
                    End If
                ElseIf Char.IsSymbol(nextChar) Then
                    Yield ScanSymbol(reader)
                Else
                    Yield ScanOther(reader)
                End If
            End While
            Yield Token.FromKind(Kind.EOF)
        End Function

        Private Function ScanName(reader As TextReader) As String
            Dim sb = New StringBuilder()
            Dim nextCharCode = reader.Peek()
            While (Char.IsLetterOrDigit(Chr(nextCharCode)) Or Chr(nextCharCode) = "_")
                sb.Append(Chr(reader.Read()))
                nextCharCode = reader.Peek()
                If nextCharCode = -1 Then Return sb.ToString()
            End While
            Return sb.ToString()
        End Function

        Private Function ScanSymbol(reader As TextReader) As Token
            Dim currentChar As Char = Chr(reader.Read())
            Select Case currentChar
                Case "="
                    Return Token.FromKind(Kind.EQUAL)
                Case Else
                    Return Token.FromKind(Kind.UNKNOWN)
            End Select
        End Function

        Private Function ScanOther(reader As TextReader) As Token
            Dim currentChar As Char = Chr(reader.Read())
            Select Case currentChar
                Case "{"
                    Return Token.FromKind(Kind.LEFTBRACKET)
                Case "}"
                    Return Token.FromKind(Kind.RIGHTBRACKET)
                Case Else
                    Return Token.FromKind(Kind.UNKNOWN)
            End Select
        End Function

    End Class

End Namespace