'    OpenSGSGLibrary is an open-source library for Grand Strategy Games
'    Copyright (C) 2020  Torben Kneesch
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

        Public Sub New(army As Military.Army, targetProvince As Integer)
            army_ = army
            targetProvince_ = targetProvince
        End Sub

        Public Overrides Sub FinalizeOrder(ByRef currentWorld As WorldData.WorldState)

            Dim armyManager As Military.ArmyManager = currentWorld.GetArmyManager()
            armyManager.MoveArmy(army_, targetProvince_)

        End Sub

        Private army_ As Military.Army
        Private targetProvince_ As Integer

    End Class

End Namespace