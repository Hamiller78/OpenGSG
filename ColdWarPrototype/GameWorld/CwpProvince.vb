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

Imports OpenGSGLibrary.WorldData

Namespace GameWorld

    ''' <summary>
    ''' Province class for cold war game. Adds specialised properties to base province class.
    ''' </summary>
    Public Class CwpProvince
        Inherits Province

        Public Property population As Long = 0
        Public Property industrialization As Long = 0
        Public Property education As Long = 0

        Public ReadOnly Property production As Long
            Get
                CalculateProduction()
                Return production_
            End Get
        End Property
        Private production_ As Long = 0

        ''' <summary>
        ''' Sets the province properties from the parsed data.
        ''' </summary>
        ''' <param name="fileName">Name of the source file of province object.</param>
        ''' <param name="parsedData">Object with the parsed data from that file.</param>
        Public Overrides Sub SetData(fileName As String, parsedData As Dictionary(Of String, Object))
            MyBase.SetData(fileName, parsedData)

            population = parsedData("population")
            industrialization = parsedData("industrialization")
            education = parsedData("education")

        End Sub

        Public Sub UpdateDaily()
            ' Change in population number, just something small for a test
            population = population * 1.00003
            ' Change in industrialization
            ' Change of education
        End Sub

        Public Sub CalculateProduction()
            production_ = population * industrialization / 100 * education / 100
        End Sub

    End Class

End Namespace