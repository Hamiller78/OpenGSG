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

Public Class ProvinceInfo

    Public Sub New(ByRef motherWindow As MainWindow, ByRef viewedState As WorldState)
        motherWindow_ = motherWindow
        viewedState_ = viewedState
    End Sub

    Private motherWindow_ As MainWindow = Nothing
    Private viewedState_ As WorldState = Nothing
    Private currentProvinceId_ As Integer

    Private Sub UpdateProvinceInfo(mouseProvinceId As Integer)
        currentProvinceId_ = mouseProvinceId
        Dim currentProvince As CwpProvince = viewedState_.GetProvinceTable(currentProvinceId_)

        motherWindow_.ProvinceName.Text = currentProvince.GetName()
        motherWindow_.ProvincePopulation.Text = Trim(Str(currentProvince.population))
        motherWindow_.ProvinceIndustrialization.Text = Trim(Str(currentProvince.industrialization))
        motherWindow_.ProvinceEducation.Text = Trim(Str(currentProvince.education))
        motherWindow_.ProvinceProduction.Text = currentProvince.production
        motherWindow_.ProvinceTerrain.Text = currentProvince.terrain
        motherWindow_.ProvinceOwner.Text = currentProvince.GetOwner()
        motherWindow_.ProvinceController.Text = currentProvince.GetController()

    End Sub

End Class
