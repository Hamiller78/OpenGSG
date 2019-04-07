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
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Kind = OpenGSGLibrary.Parser.Kind
Imports Scanner = OpenGSGLibrary.Parser.Scanner
Imports Token = OpenGSGLibrary.Parser.Token

<TestClass()> Public Class Test_Parser

    <TestMethod()> Public Sub Test_Parse()
        Dim inputStream As IEnumerable(Of Token)
        Dim testParser = New OpenGSGLibrary.Parser.Parser
        Dim testScanner = New Scanner

        Dim inputText As TextReader = New StringReader(
              "state={" _
            & "       id=92" _
            & "       name=STATE_92" _
            & "      }")

        '        testParser.Parse(inputStream())
        testParser.Parse(testScanner.Scan(inputText))
    End Sub

    Private Iterator Function inputStream() As IEnumerator(Of Token)
        Yield Token.FromString("state")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromKind(Kind.LEFTBRACKET)
        Yield Token.FromString("id")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromValue(92)
        Yield Token.FromString("name")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromString("STATE_92")
        Yield Token.FromKind(Kind.RIGHTBRACKET)
        Yield Token.FromKind(Kind.EOF)
        Throw New IndexOutOfRangeException("Tokenstream end reached.")
    End Function

End Class