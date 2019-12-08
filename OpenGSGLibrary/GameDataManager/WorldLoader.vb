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

Imports System.IO

Namespace WorldData

    ''' <summary>
    ''' Class to create a world state from gamedata files.
    ''' The derived classes for provinces, countries, etc. have to be specified when creating an instance of the class.
    ''' </summary>
    ''' <typeparam name="provType"></typeparam>
    ''' <typeparam name="countryType"></typeparam>
    Public Class WorldLoader _
        (Of provType As {New, Province},
            countryType As {New, Country})

        ''' <summary>
        ''' Creates a world state from the data in the game data or mod directories.
        ''' This should be the start state for a game.
        ''' </summary>
        ''' <param name="gamedataPath"></param>
        ''' <returns></returns>
        Public Function CreateState(gamedataPath As String) As WorldState(Of provType, countryType)
            Dim newState As New WorldState(Of provType, countryType)()

            Try
                LoadProvinces(gamedataPath)
                LoadCountries(gamedataPath)
                LoadArmies(gamedataPath)

                newState.SetProvinceTable(provinceTable_)
                newState.SetCountryTable(countryTable_)
                newState.SetArmyManager(armyManager_)

                Return newState
            Catch ex As Exception
                ' TODO: Logging and re-throw
            End Try

        End Function

        Private Sub LoadProvinces(gamedataPath As String)
            Try
                provinceTable_ = GameObjectFactory.FromFolderWithFilenameId _
                                               (Of provType) _
                                               (Path.Combine(gamedataPath, "history\provinces"))
            Catch ex As Exception
                ' TODO: Logging and re-throw
            End Try
        End Sub

        Private Sub LoadCountries(gamedataPath As String)
            Try
                countryTable_ = GameObjectFactory.FromFolder _
                                                  (Of String, countryType) _
                                                  (Path.Combine(gamedataPath, "common\countries"), "tag")
            Catch ex As Exception
                ' TODO: Logging and re-throw
            End Try
        End Sub

        Private Sub LoadArmies(gamedataPath As String)
            Try
                armyManager_.LoadFolder(Path.Combine(gamedataPath, "history\units"))
            Catch ex As Exception
                ' TODO: Logging and re-throw
            End Try
        End Sub

        Private provinceTable_ As Dictionary(Of Integer, provType) = Nothing
        Private countryTable_ As Dictionary(Of String, countryType) = Nothing
        Private armyManager_ As New Military.ArmyManager()

    End Class

End Namespace