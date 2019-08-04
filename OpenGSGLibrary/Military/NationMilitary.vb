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

Namespace Military

    Public Class NationMilitary
        Inherits WorldData.GameObject

        Public Overrides Sub SetData(fileName As String, parsedData As Lookup(Of String, Object))
            MyBase.SetData(fileName, parsedData)

            owner_ = parsedData("tag").Single()
            armies_ = WorldData.GameObjectFactory.ListFromLookup(Of Army)(parsedData, "army")
            For Each army In armies_
                army.SetOwner(owner_)
            Next
        End Sub

        Public Function GetArmiesList() As List(Of Army)
            Return armies_
        End Function

        Private owner_ As String = ""
        Private armies_ As List(Of Army)
    End Class

End Namespace