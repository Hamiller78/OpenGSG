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
Namespace Tools

    ''' <summary>
    ''' Singleton log class using the normal logger class.
    ''' Can be used from anywhere in the program as a global logger.
    ''' </summary>
    Public Class GlobalLogger

        Const DEFAULT_LOGNAME As String = "GSGLib.log"

        Private Shared defLogger_ As Logger = Nothing

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Returns an instance of the default logger.
        ''' If Init() has not been used, the path will be the current directory and the filenamen GSGLib.log
        ''' </summary>
        ''' <returns>Reference to logger object</returns>
        Public Shared Function GetInstance() As Logger
            If defLogger_ Is Nothing Then
                defLogger_ = New Logger(DEFAULT_LOGNAME, CurDir())
            End If
            Return defLogger_
        End Function

        ''' <summary>
        ''' Initializes the internal logger of the singleton with the given path and filename.
        ''' Must be called before the first use of GetInstance().
        ''' </summary>
        ''' <param name="logFile">Strin with file name</param>
        ''' <param name="logPath">String with file path</param>
        Public Shared Sub Init(logFile As String, logPath As String)
            If defLogger_ IsNot Nothing Then
                Throw New ApplicationException("GlobalLogger class can only be initialized once!")
            End If

            defLogger_ = New Logger(logFile, logPath)
        End Sub

    End Class

End Namespace