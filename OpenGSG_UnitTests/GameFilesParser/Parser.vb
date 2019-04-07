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
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Kind = OpenGSGLibrary.Parser.Kind
Imports Token = OpenGSGLibrary.Parser.Token

<TestClass()> Public Class Test_Parser

    <TestMethod()> Public Sub Test_Parse()
        Dim testParser = New OpenGSGLibrary.Parser.Parser
        Dim testResult = New Dictionary(Of String, Object)

        testResult = testParser.Parse(inputStream())

        Assert.AreEqual(vbObject, VarType(testResult("state")))
        Dim stateProps As Dictionary(Of String, Object) = testResult("state")

        Dim id As Integer = stateProps("id")
        Assert.AreEqual(92, id)

        Dim name As String = stateProps("name")
        Assert.AreEqual("STATE_92", name)

        Dim provinceList As List(Of Integer) = stateProps("provinces")
        Assert.AreEqual(13, provinceList(0))
        Assert.AreEqual(13423, provinceList(1))
        Assert.AreEqual(908, provinceList(2))

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

        Yield Token.FromString("provinces")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromKind(Kind.LEFTBRACKET)
        Yield Token.FromValue(13)
        Yield Token.FromValue(13423)
        Yield Token.FromValue(908)
        Yield Token.FromKind(Kind.RIGHTBRACKET)

        Yield Token.FromKind(Kind.RIGHTBRACKET)
        Yield Token.FromKind(Kind.EOF)
        Throw New IndexOutOfRangeException("Tokenstream end reached.")
    End Function

End Class