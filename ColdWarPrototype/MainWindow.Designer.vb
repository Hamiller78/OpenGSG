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
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainWindow
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.MapPictureBox = New System.Windows.Forms.PictureBox()
        Me.MapModeRaw = New System.Windows.Forms.RadioButton()
        Me.MapModePolitical = New System.Windows.Forms.RadioButton()
        Me.MapModeGroup = New System.Windows.Forms.GroupBox()
        CType(Me.MapPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MapModeGroup.SuspendLayout()
        Me.SuspendLayout()
        '
        'MapPictureBox
        '
        Me.MapPictureBox.Location = New System.Drawing.Point(230, 0)
        Me.MapPictureBox.Name = "MapPictureBox"
        Me.MapPictureBox.Size = New System.Drawing.Size(1654, 1000)
        Me.MapPictureBox.TabIndex = 0
        Me.MapPictureBox.TabStop = False
        '
        'MapModeRaw
        '
        Me.MapModeRaw.AutoSize = True
        Me.MapModeRaw.Checked = True
        Me.MapModeRaw.Location = New System.Drawing.Point(8, 19)
        Me.MapModeRaw.Name = "MapModeRaw"
        Me.MapModeRaw.Size = New System.Drawing.Size(47, 17)
        Me.MapModeRaw.TabIndex = 1
        Me.MapModeRaw.TabStop = True
        Me.MapModeRaw.Text = "Raw"
        Me.MapModeRaw.UseVisualStyleBackColor = True
        '
        'MapModePolitical
        '
        Me.MapModePolitical.AutoSize = True
        Me.MapModePolitical.Location = New System.Drawing.Point(8, 42)
        Me.MapModePolitical.Name = "MapModePolitical"
        Me.MapModePolitical.Size = New System.Drawing.Size(61, 17)
        Me.MapModePolitical.TabIndex = 2
        Me.MapModePolitical.Text = "Political"
        Me.MapModePolitical.UseVisualStyleBackColor = True
        '
        'MapModeGroup
        '
        Me.MapModeGroup.Controls.Add(Me.MapModePolitical)
        Me.MapModeGroup.Controls.Add(Me.MapModeRaw)
        Me.MapModeGroup.Location = New System.Drawing.Point(4, 3)
        Me.MapModeGroup.Name = "MapModeGroup"
        Me.MapModeGroup.Size = New System.Drawing.Size(220, 66)
        Me.MapModeGroup.TabIndex = 3
        Me.MapModeGroup.TabStop = False
        Me.MapModeGroup.Text = "Map Modes"
        '
        'MainWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1884, 1001)
        Me.Controls.Add(Me.MapModeGroup)
        Me.Controls.Add(Me.MapPictureBox)
        Me.Name = "MainWindow"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Cold War Strategy Prototype"
        CType(Me.MapPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MapModeGroup.ResumeLayout(False)
        Me.MapModeGroup.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents MapPictureBox As PictureBox
    Friend WithEvents MapModeRaw As RadioButton
    Friend WithEvents MapModePolitical As RadioButton
    Friend WithEvents MapModeGroup As GroupBox
End Class
