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

    Public Class ProvinceManager

        Public Sub LoadAllProvinceFiles(provincePath As String)
            If Not Directory.Exists(provincePath) Then
                Throw New DirectoryNotFoundException("Given gamedata directory not found: " & provincePath)
            End If

            Dim parsedProvinceData As Dictionary(Of String, Object) = FileManager.LoadFolder(provincePath)
            For Each singleProvinceData In parsedProvinceData
                Dim newProvince = New Province(singleProvinceData.Key, singleProvinceData.Value)
                provinceTable_.Add(newProvince.id, newProvince)
            Next

        End Sub

        Public Function GetProvince(id As Integer) As Province
            Return provinceTable_(id)
        End Function

        Private provinceTable_ = New Dictionary(Of Integer, Province)

    End Class

End Namespace