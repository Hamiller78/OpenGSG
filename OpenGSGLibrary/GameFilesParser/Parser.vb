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
Namespace Parser

    Public Class Parser

        ''' <summary>
        ''' Parse a token stream.
        ''' Will return a nested structure with the following properties:
        ''' A Lookup is a list of assignments with a keyword string and a VariantType object with a contained type
        ''' depending on the represented content.
        ''' Strings and numbers are String and Integer. Nested lists of assignments are Lookup objects.
        ''' A list of numbers is a list of integers.
        ''' </summary>
        ''' <param name="tokenStream">Token stream.</param>
        ''' <returns>Returns a Lookup with a structure repressenting the stream content.</returns>
        Public Function Parse(tokenStream As IEnumerator(Of Token)) As ILookup(Of String, Object)
            tokenStream.MoveNext()
            Return ParseAssignmentList(tokenStream)
        End Function

        ''' <summary>
        ''' Parses any type of collection (stuff enclosed in curly brackets).
        ''' Token stream should be set to the left bracket and will be moved to the first token after the right bracket.
        ''' </summary>
        ''' <param name="tokenStream">Token stream. Must be set to an element behind a left bracket.</param>
        ''' <returns>Returns an object containing either a Lookup, a list of integers or another object.</returns>
        Private Function ParseCollection(tokenStream As IEnumerator(Of Token)) As Object
            Dim collectionObj = New Object()

            tokenStream.MoveNext()
            Dim currentToken As Token = tokenStream.Current
            If currentToken.kind = Kind.NUMBER Then
                collectionObj = ParseValueList(tokenStream)
            ElseIf currentToken.kind = Kind.KEYWORD Then
                collectionObj = ParseAssignmentList(tokenStream)
            ElseIf Kind.RIGHTBRACKET Then
                collectionObj = Nothing
            Else
                Throw New ApplicationException("Error while parsing: {, } or keyword expected,
                                                        got " & currentToken.ToString() & " instead.")
            End If
            tokenStream.MoveNext()

            Return collectionObj
        End Function

        ''' <summary>
        ''' Parses a list of assignment. Also works on the top level, where the list is terminated by EOF.
        ''' Returns with the token stream on the RIGHTBRACKET or EOF.
        ''' </summary>
        ''' <param name="tokenStream">Token stream swet on the first keyword.</param>
        ''' <returns>Returns a Lookup with keywords as keys and objects as value.</returns>
        Private Function ParseAssignmentList(tokenStream As IEnumerator(Of Token)) As Lookup(Of String, Object)
            Dim parsedList As New List(Of KeyValuePair(Of String, Object))
            Dim currentToken As Token = tokenStream.Current

            While (currentToken.kind <> Kind.RIGHTBRACKET) And (currentToken.kind <> Kind.EOF)
                Dim newEntry As KeyValuePair(Of String, Object) = ParseAssignment(tokenStream)
                If Not IsNothing(newEntry.Value) Then
                    parsedList.Add(newEntry)
                End If
                currentToken = tokenStream.Current
            End While

            Dim returnLookup As Lookup(Of String, Object) = parsedList.ToLookup(Function(kvPair) kvPair.Key,
                                                                                Function(kvPair) kvPair.Value)
            Return returnLookup
        End Function

        ''' <summary>
        ''' Parses an assignment. Token stream will be moved to the token after the rhs of the assignment.
        ''' </summary>
        ''' <param name="tokenStream">Token stream must be set on a KEYWORD token.</param>
        ''' <returns>Returns a KeyValuePair with the keyword and a general object.</returns>
        Private Function ParseAssignment(tokenStream As IEnumerator(Of Token)) As KeyValuePair(Of String, Object)
            Dim currentToken As Token = tokenStream.Current
            If currentToken.kind <> Kind.KEYWORD Then
                Throw New ApplicationException("Error while parsing: Keyword expected, got " & currentToken.ToString() & " instead.")
            End If
            Dim keyword As String = currentToken.text

            tokenStream.MoveNext()
            currentToken = tokenStream.Current
            If currentToken.kind <> Kind.EQUAL Then
                Throw New ApplicationException("Error while parsing: = expected, got " & currentToken.ToString() & " instead.")
            End If
            Dim assignedObject = ParseRhs(tokenStream)

            Return New KeyValuePair(Of String, Object)(keyword, assignedObject)
        End Function

        ''' <summary>
        ''' Parses right hand side of an assigment.
        ''' Token stream must be set to the EQUAL token and will be moved behind the assignment.
        ''' </summary>
        ''' <param name="tokenStream">Token stream set to an EQUAL token.</param>
        ''' <returns>Returns an object containing either a Lookup, a list of integers or a nested object.</returns>
        Private Function ParseRhs(tokenStream As IEnumerator(Of Token)) As Object
            Dim returnObj = New Object

            tokenStream.MoveNext()
            Dim currentToken As Token = tokenStream.Current
            Select Case currentToken.kind
                Case Kind.LEFTBRACKET
                    returnObj = ParseCollection(tokenStream)
                Case Kind.NUMBER
                    returnObj = ParseValue(tokenStream)
                Case Kind.KEYWORD
                    returnObj = ParseText(tokenStream)
                Case Else
                    Throw New ApplicationException("Error while parsing: {, } or assignable value expected,
                                                    got " & currentToken.ToString() & " instead.")
            End Select

            Return returnObj
        End Function

        ''' <summary>
        ''' Parses a list of numeric values.
        ''' Token stream should be set on first element and will be moved to the right bracket which ends the list.
        ''' </summary>
        ''' <param name="tokenStream">Token stream. Should be set to the first number after a left bracket.</param>
        ''' <returns>List of the values as integers.</returns>
        Private Function ParseValueList(tokenStream As IEnumerator(Of Token)) As List(Of Integer)
            Dim returnList = New List(Of Integer)
            Dim currentToken As Token = tokenStream.Current
            While currentToken.kind <> Kind.RIGHTBRACKET
                If currentToken.kind <> Kind.NUMBER Then
                    Throw New ApplicationException("Error while parsing value list: Number expected,
                                                        got " & currentToken.ToString() & " instead.")
                End If
                returnList.Add(currentToken.value)
                tokenStream.MoveNext()
                currentToken = tokenStream.Current
            End While

            Return returnList
        End Function

        ''' <summary>
        ''' Parses the numeric value of the current token and moves the iterator one element forward.
        ''' </summary>
        ''' <param name="tokenStream">Token stream. Must be set to a value element.</param>
        ''' <returns>Returns the value as an integer.</returns>
        Private Function ParseValue(tokenStream As IEnumerator(Of Token)) As Integer
            Dim currentToken As Token = tokenStream.Current
            tokenStream.MoveNext()
            Return currentToken.value
        End Function

        ''' <summary>
        ''' Parses the text of the current token and moves the iterator one element forward.
        ''' </summary>
        ''' <param name="tokenStream">Token stream. Must be set to a keyword element.</param>
        ''' <returns>Returns the text of the current token as a string</returns>
        Private Function ParseText(tokenStream As IEnumerator(Of Token)) As String
            Dim currentToken As Token = tokenStream.Current
            tokenStream.MoveNext()
            Return currentToken.text
        End Function

    End Class

End Namespace