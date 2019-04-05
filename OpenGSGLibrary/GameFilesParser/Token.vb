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
Namespace Parser

    ''' <summary>
    ''' Enumeration for the different Kinds of Tokens
    ''' </summary>
    Public Enum Kind
        EOF
        EQUAL
        LEFTBRACKET
        RIGHTBRACKET
        KEYWORD
        NUMBER
        UNKNOWN
    End Enum

    ''' <summary>
    ''' Token class to represent the elements in game files.
    ''' A whole file will be represented in a compact stream of Token objects by the Scanner
    ''' </summary>
    Public Class Token

        Public ReadOnly kind As Kind
        Public ReadOnly value As Integer
        Public ReadOnly text As String

        ''' <summary>
        ''' Creates a Token object of a given Kind without a value
        ''' </summary>
        ''' <param name="k">The Kind of the new Token</param>
        Private Sub New(k As Kind)
            kind = k
            value = 0
        End Sub

        ''' <summary>
        ''' Creates a Token object of the NUMBER Kind with the given value
        ''' </summary>
        ''' <param name="i">Number to be assigned to the Token</param>
        Private Sub New(i As Integer)
            kind = Kind.NUMBER
            value = i
        End Sub

        ''' <summary>
        ''' Creates a Token object of the KEYWORD Kind with the given text
        ''' </summary>
        ''' <param name="s">Text to be assigned to the Token</param>
        Private Sub New(s As String)
            kind = Kind.KEYWORD
            text = s
        End Sub

        ''' <summary>
        ''' Returns a string representation of the Token depending on its Kind
        ''' </summary>
        ''' <returns>Text for KEYWORD Tokens, value for NUMBER Tokens, Kind as text for other</returns>
        Public Overrides Function ToString() As String
            If kind = Kind.NUMBER Then
                Return "NUMBER(" & CStr(value) & ")"
            ElseIf kind = Kind.KEYWORD Then
                Return text
            Else
                Return kind.ToString()
            End If
        End Function

        ''' <summary>
        ''' Static factory method to generate a Token of the given Kind
        ''' </summary>
        ''' <param name="k">Kind of the generated Token</param>
        ''' <returns>Generated Token</returns>
        Public Shared Function FromKind(k As Kind) As Token
            Return New Token(k)
        End Function

        ''' <summary>
        ''' Static factory method to generate a NUMBER Token
        ''' </summary>
        ''' <param name="i">Value of the generated Token</param>
        ''' <returns>Generated Token</returns>
        Public Shared Function FromValue(i As Integer) As Token
            Return New Token(i)
        End Function

        ''' <summary>
        ''' Static factory method to generate a KEYWORD Token
        ''' </summary>
        ''' <param name="s">Text of the generated Token</param>
        ''' <returns>Generated Token</returns>
        Public Shared Function FromString(s As String) As Token
            Return New Token(s)
        End Function

    End Class

End Namespace