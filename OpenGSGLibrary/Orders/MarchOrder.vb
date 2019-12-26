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
Namespace Orders

    Public Class MarchOrder
        Inherits Order

        Public Sub New(completionTick As Long, army As Military.Army, targetProvince As Integer)
            type_ = OrderType.MarchOrder
            completionTick_ = completionTick
            army_ = army
            targetProvince_ = targetProvince
        End Sub

        Public Overrides Sub FinalizeOrder(currentWorld As WorldData.WorldState)

            Dim armyManager As Military.ArmyManager = currentWorld.GetArmyManager()
            armyManager.MoveArmy(army_, targetProvince_)

        End Sub

        Private army_ As Military.Army = Nothing
        Private targetProvince_ As Integer = 0

    End Class

End Namespace