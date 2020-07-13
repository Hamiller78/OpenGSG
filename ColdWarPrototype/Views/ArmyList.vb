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

Imports OpenGSGLibrary.Military
Imports OpenGSGLibrary.WorldData

Namespace MainWindowView

    Public Class ArmyList

        Public Sub New(ByRef motherWindow As MainWindow, ByRef viewedState As WorldState)
            motherWindow_ = motherWindow
            viewedState_ = viewedState
        End Sub

        Private motherWindow_ As MainWindow = Nothing
        Private viewedState_ As WorldState = Nothing
        Private currentProvinceId_ As Integer

        Private isChoosingTarget_ As Boolean = False
        Private armiesInProvince_ As New List(Of Army)
        Private selectedArmies_ As New List(Of Army)

        Private Sub UpdateArmyListBox(mouseProvinceId As Integer)
            armiesInProvince_ = viewedState_.GetArmyManager().GetArmiesInProvince(mouseProvinceId)

            motherWindow_.ArmyListBox.Items.Clear()
            motherWindow_.ArmyListBox.BeginUpdate()
            If armiesInProvince_ IsNot Nothing Then
                For Each Army In armiesInProvince_
                    motherWindow_.ArmyListBox.Items.Add(Army.ToString())
                Next
            End If
            motherWindow_.ArmyListBox.EndUpdate()

        End Sub

    End Class

End Namespace
