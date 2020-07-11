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
Imports System.IO

Namespace WorldData

    ''' <summary>
    ''' Class to generate game objects from game files
    ''' </summary>
    Public Class GameObjectFactory

        ''' <summary>
        ''' Creates a table of generated objects from the parsed files in one directory.
        ''' </summary>
        ''' <typeparam name="baseType">Base variable type of the generated objects (e.g. Province) used for the returned dictionary</typeparam>
        ''' <typeparam name="derivedType">Specialized variable type of the generated objects (e.g. CwpProvince) used for the generated objects.</typeparam>
        ''' <param name="folderPath">String with the directory path.</param>
        ''' <returns>Dictionary with generated objects using file names without extensions as key.</returns>
        Public Shared Function FromFolderWithFilenameId(Of baseType As {New, GameObject}, derivedType As {New, baseType}) _
            (folderPath As String) As Dictionary(Of Integer, baseType)

            If Not Directory.Exists(folderPath) Then
                Throw New DirectoryNotFoundException("Given game data directory not found: " & folderPath)
            End If

            Dim objectTable = New Dictionary(Of Integer, baseType)

            Dim parsedObjectData As Dictionary(Of String, Object) = ParseFolder(folderPath)
            For Each singleObjectData In parsedObjectData
                Dim newObject = New derivedType()
                newObject.SetData(singleObjectData.Key, singleObjectData.Value)
                Dim key As String = ""
                Dim filenameParts As String() = ExtractFromFilename(singleObjectData.Key)
                Try
                    key = Convert.ToInt32(filenameParts(0))
                    objectTable.Add(key, newObject)
                Catch ex As Exception
                    Tools.GlobalLogger.GetInstance().WriteLine(
                        Tools.LogLevel.Fatal, "Error while parsing file: " & singleObjectData.Key)
                End Try
            Next

            Return objectTable

        End Function

        ''' <summary>
        ''' Creates a table of generated objects from the parsed files in one directory.
        ''' </summary>
        ''' <typeparam name="idtype">Variable type of the id (e.g. Integer, String)</typeparam>
        ''' <typeparam name="baseType">Base variable type of the generated objects (e.g. Province) used for the returned dictionary</typeparam>
        ''' <typeparam name="derivedType">Specialized variable type of the generated objects (e.g. CwpProvince) used for the generated objects.</typeparam>
        ''' <param name="folderPath">String with the directory path.</param>
        ''' <param name="keyField">Name of the field in the parsed file which value is used as key in the return value.</param>
        ''' <returns>Dictionary with generated objects.</returns>
        Public Shared Function FromFolder(Of idtype, baseType As {New, GameObject}, derivedType As {New, baseType}) _
            (folderPath As String, keyField As String) As Dictionary(Of idtype, baseType)

            If Not Directory.Exists(folderPath) Then
                Throw New DirectoryNotFoundException("Given game data directory not found: " & folderPath)
            End If

            Dim objectTable = New Dictionary(Of idtype, baseType)

            Dim parsedObjectData As Dictionary(Of String, Object) = ParseFolder(folderPath)
            For Each singleObjectData In parsedObjectData
                Dim countryData As Lookup(Of String, Object) = singleObjectData.Value
                Dim newObject = New derivedType()
                newObject.SetData(singleObjectData.Key, countryData)
                Dim key As idtype = countryData(keyField).Single()
                objectTable.Add(key, newObject)
            Next

            Return objectTable

        End Function

        ''' <summary>
        ''' Creates a list of GameObject objects from a Lookup structure.
        ''' </summary>
        ''' <typeparam name="type">Class inheriting from gamneObject class</typeparam>
        ''' <param name="parsedData">Parser data as a Lookup(Of String, object)</param>
        ''' <param name="objectId">Field id in the top level of the parser data for the 'type' objects</param>
        ''' <returns></returns>
        Public Shared Function ListFromLookup(Of type As {New, GameObject}) _
            (parsedData As Lookup(Of String, Object), objectId As String) As List(Of type)

            Dim generatedList As New List(Of type)

            For Each objParserData In parsedData(objectId)
                Dim newObj As New type()
                newObj.SetData("", objParserData)
                generatedList.Add(newObj)
            Next

            Return generatedList
        End Function

        ''' <summary>
        ''' Parses all files in a folder with txt extension.
        ''' </summary>
        ''' <param name="gamedataPath">String with the path of the folder.</param>
        ''' <returns>Dictionary with a file name as key (without extension) and that file's parsed data as object.</returns>
        Public Shared Function ParseFolder(gamedataPath As String) As Dictionary(Of String, Object)
            Dim dictionaryOfParsedData = New Dictionary(Of String, Object)

            Dim txtFilesInDir As String() = Directory.GetFiles(gamedataPath, "*.txt", SearchOption.AllDirectories)

            Dim scanner = New Parser.Scanner()
            Dim parser = New Parser.Parser()
            For Each textFile As String In txtFilesInDir
                Try
                    Dim rawFile As TextReader = File.OpenText(textFile)
                    Dim nextParseData As Lookup(Of String, Object) = parser.Parse(scanner.Scan(rawFile))
                    If Not IsNothing(nextParseData) Then
                        dictionaryOfParsedData.Add(Path.GetFileNameWithoutExtension(textFile), nextParseData)
                    End If
                Catch ex As Exception
                    'TODO: Need a logger to throw a warning
                End Try
            Next

            Return dictionaryOfParsedData

        End Function

        ''' <summary>
        ''' Returns parts of a file name separated by - without path and extension 
        ''' </summary>
        ''' <param name="filePath">File name with extension and possibly path.</param>
        ''' <returns>Array of strings with the parts of the file names.</returns>
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