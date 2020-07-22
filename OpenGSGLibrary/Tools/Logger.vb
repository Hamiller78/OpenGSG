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
Imports System.Globalization
Imports System.IO

Namespace Tools

    Public Enum LogLevel
        Info
        Warning
        Err
        Fatal
    End Enum

    Public Class Logger

        Private logFile_ As StreamWriter = Nothing

        ''' <summary>
        ''' Constructor for logger.
        ''' Checks if log folder exists and takes care of archiving of old log files if necessary,
        ''' so that we are good to go.
        ''' </summary>
        ''' <param name="logName">Name of log file</param>
        ''' <param name="logPath">Path for main log file, old versions are archived in subfolder "LogArchive"</param>
        Public Sub New(logName As String, logPath As String)

            If Not Directory.Exists(logPath) Then
                Throw New DirectoryNotFoundException("Log directory not found: " & logPath)
            End If

            ' archive old log file if necessary
            Dim fullLogFilePath As String = Path.Combine(logPath, logName)
            If File.Exists(fullLogFilePath) Then
                ArchiveLogFile(fullLogFilePath)
            End If

            ' write header of new log file
            logFile_ = New StreamWriter(fullLogFilePath, FileMode.Append)
        End Sub

        ''' <summary>
        ''' Immediately writes one line of output into log file.
        ''' System time in milliseconds and log level will be written in front of message.
        ''' </summary>
        ''' <param name="logLevel">LogLevel from LogLevel enumeration, will be noted as I|W|E|F.</param>
        ''' <param name="message">Message to write into log.</param>
        Public Sub WriteLine(logLevel As LogLevel, message As String)
            Dim outputLine As String = ""
            outputLine = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) & vbTab

            Select Case logLevel
                Case LogLevel.Info
                    outputLine = outputLine & "I" & vbTab
                Case LogLevel.Warning
                    outputLine = outputLine & "W " & vbTab
                Case LogLevel.Err
                    outputLine = outputLine & "E " & vbTab
                Case LogLevel.Fatal
                    outputLine = outputLine & "F " & vbTab
            End Select

            outputLine = outputLine & message

            logFile_.WriteLine(outputLine)
            logFile_.Flush()
        End Sub

        Private Sub ArchiveLogFile(fullFilePath As String)
            ' make sure LogArchive subfolder exists
            Dim logFileName As String = Path.GetFileName(fullFilePath)
            Dim archivePath As String = Path.Combine(Path.GetDirectoryName(fullFilePath), "LogArchive")
            If Not Directory.Exists(archivePath) Then
                Directory.CreateDirectory(archivePath)
            End If

            ' make name of archived log
            Dim archiveFilePath As String = GetFreeLogFilePath(archivePath, logFileName)

            ' move the old log file
            File.Move(fullFilePath, archiveFilePath)
        End Sub

        Private Function GetFreeLogFilePath(archivePath As String, logFileName As String) As String
            Dim coreName As String = Path.GetFileNameWithoutExtension(logFileName) & "_" & Date.Today.ToString("yyyyMMdd")

            Dim proposedName As String = coreName & ".log"
            Dim i As Integer = 0
            While File.Exists(Path.Combine(archivePath, proposedName))
                i += 1
                proposedName = coreName & "_" & Trim(Str(i)) & ".log"
            End While

            Return Path.Combine(archivePath, proposedName)
        End Function

    End Class

End Namespace