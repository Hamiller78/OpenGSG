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
        Public Function CreateStartState(gamedataPath As String) As WorldState
            Dim newState As New WorldState()

            Try
                LoadProvinces(gamedataPath)
                LoadCountries(gamedataPath)
                LoadCountryFlags(gamedataPath)
                LoadArmies(gamedataPath)

                newState.SetProvinceTable(provinceTable_)
                newState.SetCountryTable(countryTable_)
                newState.SetArmyManager(armyManager_)

                Return newState
            Catch ex As Exception
                Tools.GlobalLogger.GetInstance().WriteLine(Tools.LogLevel.Fatal, "Could not load initial world state from: " & gamedataPath)
                Throw ex
            End Try

        End Function

        Private Sub LoadProvinces(gamedataPath As String)
            Try
                provinceTable_ = GameObjectFactory.FromFolderWithFilenameId _
                                               (Of Province, provType) _
                                               (Path.Combine(gamedataPath, "history\provinces"))
            Catch ex As Exception
                Tools.GlobalLogger.GetInstance().WriteLine(Tools.LogLevel.Fatal, "Error while loading province data.")
                Throw ex
            End Try
        End Sub

        Private Sub LoadCountries(gamedataPath As String)
            Try
                countryTable_ = GameObjectFactory.FromFolder _
                                                  (Of String, Country, countryType) _
                                                  (Path.Combine(gamedataPath, "common\countries"), "tag")
            Catch ex As Exception
                Tools.GlobalLogger.GetInstance().WriteLine(Tools.LogLevel.Fatal, "Error while loading country data.")
                Throw ex
            End Try
        End Sub

        Private Sub LoadCountryFlags(gamedataPath As String)
            Try
                Dim flagPath As String = Path.Combine(gamedataPath, "gfx\flags")
                For Each country In countryTable_
                    country.Value.LoadFlags(flagPath)
                Next
            Catch ex As Exception
                Tools.GlobalLogger.GetInstance().WriteLine(Tools.LogLevel.Fatal, "Error while loading nation flags.")
                Throw ex
            End Try
        End Sub

        Private Sub LoadArmies(gamedataPath As String)
            Try
                armyManager_.LoadFolder(Path.Combine(gamedataPath, "history\units"))
            Catch ex As Exception
                Tools.GlobalLogger.GetInstance().WriteLine(Tools.LogLevel.Fatal, "Error while loading army data.")
                Throw ex
            End Try
        End Sub

        Private provinceTable_ As IDictionary(Of Integer, Province) = Nothing
        Private countryTable_ As IDictionary(Of String, Country) = Nothing
        Private armyManager_ As New Military.ArmyManager()

    End Class

End Namespace