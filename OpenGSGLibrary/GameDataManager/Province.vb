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

    ''' <summary>
    ''' Base class for provinces. Handles the most basic province properties.
    ''' </summary>
    Public Class Province
        Inherits GameObject

        Public Function GetId() As Integer
            Return id_
        End Function

        Public Function GetName() As String
            Return name_
        End Function

        Public Function GetController() As String
            Return controller_
        End Function

        Public Function GetOwner() As String
            Return owner_
        End Function

        ''' <summary>
        ''' Sets the properties of a province from the parser data of the province file.
        ''' This method handles province id, name, controller's country tag and owner's country tag.
        ''' Should be inherited to handle more game-specific properties.
        ''' </summary>
        ''' <param name="fileName"></param>
        ''' <param name="parsedData"></param>
        Public Overrides Sub SetData(fileName As String, parsedData As Dictionary(Of String, Object))
            MyBase.SetData(fileName, parsedData)
            Dim fileNameParts As String() = GameObjectFactory.ExtractFromFilename(fileName)
            id_ = Val(fileNameParts(0))
            name_ = fileNameParts(1)

            controller_ = parsedData("controller")
            owner_ = parsedData("owner")
        End Sub

        Private id_ As Integer
        Private name_ As String
        Private controller_ As String
        Private owner_ As String

    End Class

End Namespace