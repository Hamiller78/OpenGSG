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

    Public Class Parser

        Public Sub Parser(tokenStream As IEnumerator(Of Token))
            tokenStream.MoveNext()
            While tokenStream.Current.kind <> Kind.EOF
                ParseAssignment(tokenStream)
            End While
        End Sub

        Private Sub ParseAssignment(tokenStream As IEnumerator(Of Token))
            Dim currentToken As Token = tokenStream.Current
            If currentToken.kind <> Kind.KEYWORD Then
                Throw New ApplicationException("Error while parsing: Keyword expected, got " & currentToken.ToString() & " instead.")
            End If
            Dim keyword As String = currentToken.text 'TODO: Save the text

            tokenStream.MoveNext()
            currentToken = tokenStream.Current
            If currentToken.kind <> Kind.EQUAL Then
                Throw New ApplicationException("Error while parsing: = expected, got " & currentToken.ToString() & " instead.")
            End If

            tokenStream.MoveNext()
            ParseRhs(tokenStream)
        End Sub

        Private Sub ParseRhs(tokenStream As IEnumerator(Of Token))
            Dim currentToken As Token = tokenStream.Current
            Select Case currentToken.kind
                Case Kind.LEFTBRACKET
                    tokenStream.MoveNext()
                    ParseCollection(tokenStream)
                Case Kind.NUMBER
                    tokenStream.MoveNext()
                    ParseValue(tokenStream)
                Case Kind.KEYWORD
                    tokenStream.MoveNext()
                    ParseText(tokenStream)
                Case Else
                    Throw New ApplicationException("Error while parsing: { or assignable value expected,
                                                    got " & currentToken.ToString() & " instead.")
            End Select
        End Sub

        Private Sub ParseCollection(tokenStream As IEnumerator(Of Token))
            Dim currentToken As Token = tokenStream.Current
            While currentToken.kind <> Kind.RIGHTBRACKET
                Select Case currentToken.kind
                    Case Kind.NUMBER
                        ParseValue(tokenStream)
                    Case Kind.KEYWORD
                        ParseAssignment(tokenStream)
                    Case Kind.LEFTBRACKET
                        ParseCollection(tokenStream)
                    Case Else
                        Throw New ApplicationException("Error while parsing: { or keyword expected,
                                                        got " & currentToken.ToString() & " instead.")
                End Select
                tokenStream.MoveNext()
            End While

        End Sub

        Private Sub ParseValue(tokenStream As IEnumerator(Of Token))
            Dim currentToken As Token = tokenStream.Current
            Dim value As Integer = currentToken.value 'TODO: Save the value
        End Sub

        Private Sub ParseText(tokenStream As IEnumerator(Of Token))
            Dim currentToken As Token = tokenStream.Current
            Dim text As String = currentToken.text 'TODO: Save the text
        End Sub
    End Class

End Namespace