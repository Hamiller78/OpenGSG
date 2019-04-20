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

Namespace WorldData

    Public Class FileManager

        Public Shared Function CreateObjectsFromFolder(Of idtype, type As {New, GameObject}) _
            (folderPath As String, keyField As String) As Dictionary(Of idtype, type)

            If Not Directory.Exists(folderPath) Then
                Throw New DirectoryNotFoundException("Given game data directory not found: " & folderPath)
            End If

            Dim objectTable = New Dictionary(Of idtype, type)

            Dim parsedObjectData As Dictionary(Of String, Object) = ParseFolder(folderPath)
            For Each singleObjectData In parsedObjectData
                Dim newObject = New type()
                newObject.SetData(singleObjectData.Key, singleObjectData.Value)
                Dim key As idtype = CType(singleObjectData.Value("keyField"), idtype)
                objectTable.Add(key, newObject)
            Next

            Return objectTable

        End Function

        Public Shared Function ParseFolder(gamedataPath As String) As Dictionary(Of String, Object)
            Dim dictionaryOfParsedData = New Dictionary(Of String, Object)

            Dim fileArray As String() = Directory.GetFiles(gamedataPath, "*.txt", SearchOption.AllDirectories)

            Dim scanner = New Parser.Scanner()
            Dim parser = New Parser.Parser()
            For Each textFile As String In fileArray
                Try
                    Dim rawFile As TextReader = File.OpenText(textFile)
                    Dim nextParseData As Dictionary(Of String, Object) = parser.Parse(scanner.Scan(rawFile))
                    If Not IsNothing(nextParseData) Then
                        dictionaryOfParsedData.Add(Path.GetFileName(textFile), nextParseData)
                    End If
                Catch ex As Exception
                    'TODO: Need a logger to throw a warning
                End Try
            Next

            Return dictionaryOfParsedData

        End Function

        Public Shared Function ExtractFromFilename(filePath As String) As String()
            Dim filename As String = Path.GetFileNameWithoutExtension(filePath)
            If filename = "" Then
                Throw New ApplicationException("No file name found in path: " + filePath)
            End If

            Dim nameParts As String() = filename.Split("-")
            For i = 0 To UBound(nameParts)
                nameParts(i) = nameParts(i).Trim()
            Next

            Return nameParts
        End Function

    End Class

End Namespace