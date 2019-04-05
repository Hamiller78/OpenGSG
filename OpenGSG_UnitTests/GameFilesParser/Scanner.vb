﻿'    OpenSGSGLibrary is an open-source library for Grand Strategy Games
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

Imports Kind = OpenGSGLibrary.Parser.Kind
Imports Token = OpenGSGLibrary.Parser.Token

Namespace Parser

    <TestClass()> Public Class Test_Scanner

        <TestMethod()> Public Sub Test_Scan_StringReader()
            Dim testScanner = New OpenGSGLibrary.Parser.Scanner
            Dim inputText As TextReader = New StringReader(
              "state={" _
            & "       id=92" _
            & "       name=STATE_92" _
            & "      }")
            Dim outputStream As IEnumerator(Of Token)
            outputStream = testScanner.Scan(inputText)

            outputStream.MoveNext()
            Assert.AreEqual(Kind.KEYWORD, outputStream.Current.kind)
            Assert.AreEqual("state", outputStream.Current.ToString())

            outputStream.MoveNext()
            Assert.AreEqual(Kind.EQUAL, outputStream.Current.kind)

            outputStream.MoveNext()
            Assert.AreEqual(Kind.LEFTBRACKET, outputStream.Current.kind)

            outputStream.MoveNext()
            Assert.AreEqual(Kind.KEYWORD, outputStream.Current.kind)
            Assert.AreEqual("id", outputStream.Current.ToString())

            outputStream.MoveNext()
            Assert.AreEqual(Kind.EQUAL, outputStream.Current.kind)

            outputStream.MoveNext()
            Assert.AreEqual(Kind.NUMBER, outputStream.Current.kind)
            Assert.AreEqual("NUMBER(92)", outputStream.Current.ToString())

            outputStream.MoveNext()
            Assert.AreEqual(Kind.KEYWORD, outputStream.Current.kind)
            Assert.AreEqual("name", outputStream.Current.ToString())

            outputStream.MoveNext()
            Assert.AreEqual(Kind.EQUAL, outputStream.Current.kind)

            outputStream.MoveNext()
            Assert.AreEqual(Kind.KEYWORD, outputStream.Current.kind)
            Assert.AreEqual("STATE_92", outputStream.Current.ToString())

            outputStream.MoveNext()
            Assert.AreEqual(Kind.RIGHTBRACKET, outputStream.Current.kind)

        End Sub

    End Class

End Namespace