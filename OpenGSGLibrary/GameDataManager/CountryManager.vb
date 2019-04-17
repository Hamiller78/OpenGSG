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

    Public Class CountryManager

        Public Sub LoadAllProvinceFiles(countryPath As String)
            If Not Directory.Exists(countryPath) Then
                Throw New DirectoryNotFoundException("Given gamedata directory not found: " & countryPath)
            End If

            Dim parsedCountryData As Dictionary(Of String, Object) = FileManager.LoadFolder(countryPath)
            For Each singleCountryData In parsedCountryData
                Dim newCountry = New Country(singleCountryData.Key, singleCountryData.Value)
                countryTable_.Add(newCountry.id, newCountry)
                countryTagTable_.Add(newCountry.tag, newCountry)
            Next

        End Sub

        Public Function GetCountry(id As Integer) As Country
            Return countryTable_.Item(id)
        End Function

        Public Function GetCountry(tag As String) As Country
            Return countryTagTable_.Item(tag)
        End Function

        Private countryTable_ As Dictionary(Of Integer, Country) = New Dictionary(Of Integer, Country)
        Private countryTagTable_ As Dictionary(Of String, Country) = New Dictionary(Of String, Country)

    End Class

End Namespace