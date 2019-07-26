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

        testResult = testParser.Parse(InputStream())

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

    Private Iterator Function InputStream() As IEnumerator(Of Token)
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


    <TestMethod()> Public Sub Test_ParseEmptyCollection()
        Dim testParser = New OpenGSGLibrary.Parser.Parser
        Dim testResult = New Dictionary(Of String, Object)

        testResult = testParser.Parse(InputStreamWithEmptyCollection())

        Assert.AreEqual(vbObject, VarType(testResult("country")))
        Dim countryProps As Dictionary(Of String, Object) = testResult("country")

        Dim tag As String = countryProps("tag")
        Assert.AreEqual("FRG", tag)

        Dim name As String = countryProps("name")
        Assert.AreEqual("Germany", name)

        Assert.AreEqual(False, countryProps.ContainsKey("resources"))

    End Sub

    Private Iterator Function InputStreamWithEmptyCollection() As IEnumerator(Of Token)
        Yield Token.FromString("country")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromKind(Kind.LEFTBRACKET)

        Yield Token.FromString("tag")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromString("FRG")

        Yield Token.FromString("name")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromString("Germany")

        Yield Token.FromString("resources")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromKind(Kind.LEFTBRACKET)
        Yield Token.FromKind(Kind.RIGHTBRACKET)

        Yield Token.FromKind(Kind.RIGHTBRACKET)
        Yield Token.FromKind(Kind.EOF)
        Throw New IndexOutOfRangeException("Tokenstream end reached.")
    End Function

    <TestMethod()> Public Sub Test_ParseDuplicateKeyCollection()
        Dim testParser = New OpenGSGLibrary.Parser.Parser
        Dim testResult = New Dictionary(Of String, Object)

        testResult = testParser.Parse(InputStreamWithDuplicateKey())

        Assert.AreEqual(vbObject, VarType(testResult("army")))
        Dim armyProps As Lookup(Of String, Object) = testResult("army")

        Assert.AreEqual(1, armyProps("name").Count)
        Dim name As String = armyProps("name").First()
        Assert.AreEqual("Some army name", name)

        Assert.AreEqual(1, armyProps("name").Count)
        Dim location As Integer = armyProps("location").First()
        Assert.AreEqual(4, location)

        Assert.AreEqual(2, armyProps("division").Count)

        Dim division As Lookup(Of String, Object) = armyProps("division").ElementAt(0)
        Assert.AreEqual("Division 1", division("name").Single())
        Assert.AreEqual("Infantry", division("type").Single())
        Assert.AreEqual(10000, division("size").Single())

        division = armyProps("division").ElementAt(1)
        Assert.AreEqual("Division 2", division("name").Single())
        Assert.AreEqual("Armor", division("type").Single())
        Assert.AreEqual(300, division("size").Single())

    End Sub

    Private Iterator Function InputStreamWithDuplicateKey() As IEnumerator(Of Token)
        Yield Token.FromString("army")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromKind(Kind.LEFTBRACKET)

        Yield Token.FromString("name")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromString("Some army name")

        Yield Token.FromString("location")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromValue(4)

        Yield Token.FromString("division")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromKind(Kind.LEFTBRACKET)

        Yield Token.FromString("name")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromString("Division 1")

        Yield Token.FromString("type")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromString("Infantry")

        Yield Token.FromString("size")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromValue(10000)

        Yield Token.FromKind(Kind.RIGHTBRACKET)
        Yield Token.FromString("division")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromKind(Kind.LEFTBRACKET)

        Yield Token.FromString("name")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromString("Division 2")

        Yield Token.FromString("type")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromString("Armor")

        Yield Token.FromString("size")
        Yield Token.FromKind(Kind.EQUAL)
        Yield Token.FromValue(300)

        Yield Token.FromKind(Kind.RIGHTBRACKET)

        Yield Token.FromKind(Kind.RIGHTBRACKET)
        Yield Token.FromKind(Kind.EOF)
        Throw New IndexOutOfRangeException("Tokenstream end reached.")
    End Function

End Class