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

    Public Class WorldState _
        (Of provType As {New, Province},
            countryType As {New, Country})

        Public Sub SetProvinceTable(provinceTable As Dictionary(Of Integer, provType))
            provinceTable_ = provinceTable
        End Sub

        Public Function GetProvinceTable() As Dictionary(Of Integer, provType)
            Return provinceTable_
        End Function

        Public Sub SetCountryTable(countryTable As Dictionary(Of String, countryType))
            countryTable_ = countryTable
        End Sub

        Public Function GetCountryTable() As Dictionary(Of String, countryType)
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

        Private provinceTable_ As New Dictionary(Of Integer, provType)
        Private countryTable_ As New Dictionary(Of String, countryType)
        Private armyManager_ As New Military.ArmyManager()

        Private runningOrders_ As New List(Of Orders.Order)

        ' TODO: production
        ' TODO: pending events
        ' TODO: global modifier
    End Class

End Namespace