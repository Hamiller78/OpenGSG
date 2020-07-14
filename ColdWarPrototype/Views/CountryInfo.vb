'    OpenSGSGLibrary is an open-source library for Grand Strategy Games
'    Copyright (C) 2019-2020  Torben Kneesch
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

Imports ColdWarGameLogic.WorldData
Imports OpenGSGLibrary.WorldData

Namespace MainWindowView

    Public Class CountryInfo

        Public Sub New(ByRef motherWindow As MainWindow)
            motherWindow_ = motherWindow
        End Sub

        Public Sub UpdateCountryInfo(ByRef currentState As WorldState, ByRef countryTag As String)
            currentCountryTag_ = countryTag
            Dim currentCountry As CwpCountry = currentState.GetCountryTable(countryTag)
            motherWindow_.CountryName.Text = currentCountry.longName
            motherWindow_.CountryLeader.Text = currentCountry.leader
            motherWindow_.CountryGovernment.Text = currentCountry.government
            motherWindow_.CountryAllegiance.Text = currentCountry.allegiance
            '        CountryProduction.Text = tickHandler_.GetState().GetCountryProduction(currentCountryTag_)
            motherWindow_.FlagPictureBox.Image = currentCountry.flag
        End Sub

        Private motherWindow_ As MainWindow = Nothing
        Private currentCountryTag_ As String

    End Class

End Namespace