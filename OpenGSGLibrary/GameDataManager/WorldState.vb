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

    Public Class WorldState

        Public Sub SetProvinceTable(provinceTable As IDictionary(Of Integer, Province))
            provinceTable_ = provinceTable
        End Sub

        Public Function GetProvinceTable() As IDictionary(Of Integer, Province)
            Return provinceTable_
        End Function

        Public Sub SetCountryTable(countryTable As IDictionary(Of String, Country))
            countryTable_ = countryTable
        End Sub

        Public Function GetCountryTable() As IDictionary(Of String, Country)
            Return countryTable_
        End Function

        Public Sub SetArmyManager(armyManager As Military.ArmyManager)
            armyManager_ = armyManager
        End Sub

        Public Function GetArmyManager() As Military.ArmyManager
            Return armyManager_
        End Function

        Public Function GetOrders() As List(Of Orders.Order)
            Return runningOrders_
        End Function

        Private provinceTable_ As IDictionary(Of Integer, Province) = Nothing
        Private countryTable_ As IDictionary(Of String, Country) = Nothing
        Private armyManager_ As New Military.ArmyManager()

        Private runningOrders_ As New List(Of Orders.Order)

        ' TODO: production
        ' TODO: global modifier
    End Class

End Namespace