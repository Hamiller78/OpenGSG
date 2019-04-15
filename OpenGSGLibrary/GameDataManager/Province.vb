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

Namespace WorldData

    Public Class Province

        ReadOnly Property id As Integer
        ReadOnly Property name As String
        ReadOnly Property controller As String
        ReadOnly Property owner As String


        Public Sub New(fileName As String, parsedData As Dictionary(Of String, Object))
            Dim fileNameParts As String() = FileManager.ExtractFromFilename(fileName)
            id = Val(fileNameParts(0))
            name = fileNameParts(1)

            controller = parsedData("controller")
            owner = parsedData("owner")
        End Sub

    End Class

End Namespace