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
Imports System.Drawing

Imports OpenGSGLibrary.WorldData

Namespace Map

    ''' <summary>
    ''' Base class for making maps for different view modes (e.g. political, economical).
    ''' Each derived class should correspond to a map mode in the game.
    ''' Maps are stored as System.Drawing.Image objects.
    ''' </summary>
    Public MustInherit Class ModeMapMaker

        Public Sub New(sourceMap As LayerBitmap)
            sourceMap_ = sourceMap
        End Sub

        Public MustOverride Function MakeMap(ByRef sourceState As WorldState) As Image

        Protected sourceMap_ As LayerBitmap

    End Class

End Namespace