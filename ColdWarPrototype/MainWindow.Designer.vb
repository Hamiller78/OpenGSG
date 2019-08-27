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
        Me.ProvinceBox = New System.Windows.Forms.GroupBox()
        Me.ProvinceTerrain = New System.Windows.Forms.Label()
        Me.ProvinceProduction = New System.Windows.Forms.Label()
        Me.ProvinceEducation = New System.Windows.Forms.Label()
        Me.ProvinceOwner = New System.Windows.Forms.Label()
        Me.ProvinceController = New System.Windows.Forms.Label()
        Me.ProvinceIndustrialization = New System.Windows.Forms.Label()
        Me.ProvincePopulation = New System.Windows.Forms.Label()
        Me.ProvinceName = New System.Windows.Forms.Label()
        Me.CountryBox = New System.Windows.Forms.GroupBox()
        Me.FlagPictureBox = New System.Windows.Forms.PictureBox()
        Me.CountryProduction = New System.Windows.Forms.Label()
        Me.CountryAllegiance = New System.Windows.Forms.Label()
        Me.CountryGovernment = New System.Windows.Forms.Label()
        Me.CountryLeader = New System.Windows.Forms.Label()
        Me.CountryName = New System.Windows.Forms.Label()
        Me.DateButton = New System.Windows.Forms.Button()
        Me.CoordinateDisplay = New System.Windows.Forms.Label()
        Me.ArmyListBox = New System.Windows.Forms.ListBox()
        Me.MoveArmiesButton = New System.Windows.Forms.Button()
        CType(Me.MapPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MapModeGroup.SuspendLayout()
        Me.ProvinceBox.SuspendLayout()
        Me.CountryBox.SuspendLayout()
        CType(Me.FlagPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'ProvinceBox
        '
        Me.ProvinceBox.Controls.Add(Me.ProvinceTerrain)
        Me.ProvinceBox.Controls.Add(Me.ProvinceProduction)
        Me.ProvinceBox.Controls.Add(Me.ProvinceEducation)
        Me.ProvinceBox.Controls.Add(Me.ProvinceOwner)
        Me.ProvinceBox.Controls.Add(Me.ProvinceController)
        Me.ProvinceBox.Controls.Add(Me.ProvinceIndustrialization)
        Me.ProvinceBox.Controls.Add(Me.ProvincePopulation)
        Me.ProvinceBox.Controls.Add(Me.ProvinceName)
        Me.ProvinceBox.Location = New System.Drawing.Point(4, 77)
        Me.ProvinceBox.Name = "ProvinceBox"
        Me.ProvinceBox.Size = New System.Drawing.Size(224, 156)
        Me.ProvinceBox.TabIndex = 4
        Me.ProvinceBox.TabStop = False
        Me.ProvinceBox.Text = "Province"
        '
        'ProvinceTerrain
        '
        Me.ProvinceTerrain.AutoSize = True
        Me.ProvinceTerrain.Location = New System.Drawing.Point(8, 128)
        Me.ProvinceTerrain.Name = "ProvinceTerrain"
        Me.ProvinceTerrain.Size = New System.Drawing.Size(10, 13)
        Me.ProvinceTerrain.TabIndex = 8
        Me.ProvinceTerrain.Text = "-"
        '
        'ProvinceProduction
        '
        Me.ProvinceProduction.AutoSize = True
        Me.ProvinceProduction.Location = New System.Drawing.Point(8, 115)
        Me.ProvinceProduction.Name = "ProvinceProduction"
        Me.ProvinceProduction.Size = New System.Drawing.Size(10, 13)
        Me.ProvinceProduction.TabIndex = 7
        Me.ProvinceProduction.Text = "-"
        '
        'ProvinceEducation
        '
        Me.ProvinceEducation.AutoSize = True
        Me.ProvinceEducation.Location = New System.Drawing.Point(8, 70)
        Me.ProvinceEducation.Name = "ProvinceEducation"
        Me.ProvinceEducation.Size = New System.Drawing.Size(10, 13)
        Me.ProvinceEducation.TabIndex = 5
        Me.ProvinceEducation.Text = "-"
        '
        'ProvinceOwner
        '
        Me.ProvinceOwner.AutoSize = True
        Me.ProvinceOwner.Location = New System.Drawing.Point(8, 83)
        Me.ProvinceOwner.Name = "ProvinceOwner"
        Me.ProvinceOwner.Size = New System.Drawing.Size(10, 13)
        Me.ProvinceOwner.TabIndex = 6
        Me.ProvinceOwner.Text = "-"
        '
        'ProvinceController
        '
        Me.ProvinceController.AutoSize = True
        Me.ProvinceController.Location = New System.Drawing.Point(8, 96)
        Me.ProvinceController.Name = "ProvinceController"
        Me.ProvinceController.Size = New System.Drawing.Size(10, 13)
        Me.ProvinceController.TabIndex = 3
        Me.ProvinceController.Text = "-"
        '
        'ProvinceIndustrialization
        '
        Me.ProvinceIndustrialization.AutoSize = True
        Me.ProvinceIndustrialization.Location = New System.Drawing.Point(8, 57)
        Me.ProvinceIndustrialization.Name = "ProvinceIndustrialization"
        Me.ProvinceIndustrialization.Size = New System.Drawing.Size(10, 13)
        Me.ProvinceIndustrialization.TabIndex = 2
        Me.ProvinceIndustrialization.Text = "-"
        '
        'ProvincePopulation
        '
        Me.ProvincePopulation.AutoSize = True
        Me.ProvincePopulation.Location = New System.Drawing.Point(8, 38)
        Me.ProvincePopulation.Name = "ProvincePopulation"
        Me.ProvincePopulation.Size = New System.Drawing.Size(13, 13)
        Me.ProvincePopulation.TabIndex = 1
        Me.ProvincePopulation.Text = "0"
        '
        'ProvinceName
        '
        Me.ProvinceName.AutoSize = True
        Me.ProvinceName.Location = New System.Drawing.Point(8, 18)
        Me.ProvinceName.Name = "ProvinceName"
        Me.ProvinceName.Size = New System.Drawing.Size(10, 13)
        Me.ProvinceName.TabIndex = 0
        Me.ProvinceName.Text = "-"
        '
        'CountryBox
        '
        Me.CountryBox.Controls.Add(Me.FlagPictureBox)
        Me.CountryBox.Controls.Add(Me.CountryProduction)
        Me.CountryBox.Controls.Add(Me.CountryAllegiance)
        Me.CountryBox.Controls.Add(Me.CountryGovernment)
        Me.CountryBox.Controls.Add(Me.CountryLeader)
        Me.CountryBox.Controls.Add(Me.CountryName)
        Me.CountryBox.Location = New System.Drawing.Point(4, 239)
        Me.CountryBox.Name = "CountryBox"
        Me.CountryBox.Size = New System.Drawing.Size(224, 93)
        Me.CountryBox.TabIndex = 5
        Me.CountryBox.TabStop = False
        Me.CountryBox.Text = "Country"
        '
        'FlagPictureBox
        '
        Me.FlagPictureBox.Location = New System.Drawing.Point(174, 16)
        Me.FlagPictureBox.Name = "FlagPictureBox"
        Me.FlagPictureBox.Size = New System.Drawing.Size(45, 30)
        Me.FlagPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.FlagPictureBox.TabIndex = 5
        Me.FlagPictureBox.TabStop = False
        '
        'CountryProduction
        '
        Me.CountryProduction.AutoSize = True
        Me.CountryProduction.Location = New System.Drawing.Point(8, 73)
        Me.CountryProduction.Name = "CountryProduction"
        Me.CountryProduction.Size = New System.Drawing.Size(10, 13)
        Me.CountryProduction.TabIndex = 4
        Me.CountryProduction.Text = "-"
        '
        'CountryAllegiance
        '
        Me.CountryAllegiance.AutoSize = True
        Me.CountryAllegiance.Location = New System.Drawing.Point(8, 55)
        Me.CountryAllegiance.Name = "CountryAllegiance"
        Me.CountryAllegiance.Size = New System.Drawing.Size(10, 13)
        Me.CountryAllegiance.TabIndex = 3
        Me.CountryAllegiance.Text = "-"
        '
        'CountryGovernment
        '
        Me.CountryGovernment.AutoSize = True
        Me.CountryGovernment.Location = New System.Drawing.Point(8, 42)
        Me.CountryGovernment.Name = "CountryGovernment"
        Me.CountryGovernment.Size = New System.Drawing.Size(10, 13)
        Me.CountryGovernment.TabIndex = 2
        Me.CountryGovernment.Text = "-"
        '
        'CountryLeader
        '
        Me.CountryLeader.AutoSize = True
        Me.CountryLeader.Location = New System.Drawing.Point(8, 29)
        Me.CountryLeader.Name = "CountryLeader"
        Me.CountryLeader.Size = New System.Drawing.Size(10, 13)
        Me.CountryLeader.TabIndex = 1
        Me.CountryLeader.Text = "-"
        '
        'CountryName
        '
        Me.CountryName.AutoSize = True
        Me.CountryName.Location = New System.Drawing.Point(8, 16)
        Me.CountryName.Name = "CountryName"
        Me.CountryName.Size = New System.Drawing.Size(10, 13)
        Me.CountryName.TabIndex = 0
        Me.CountryName.Text = "-"
        '
        'DateButton
        '
        Me.DateButton.Location = New System.Drawing.Point(4, 952)
        Me.DateButton.Name = "DateButton"
        Me.DateButton.Size = New System.Drawing.Size(216, 37)
        Me.DateButton.TabIndex = 6
        Me.DateButton.Text = "DD-MM-YYYY"
        Me.DateButton.UseVisualStyleBackColor = True
        '
        'CoordinateDisplay
        '
        Me.CoordinateDisplay.AutoSize = True
        Me.CoordinateDisplay.Location = New System.Drawing.Point(3, 936)
        Me.CoordinateDisplay.Name = "CoordinateDisplay"
        Me.CoordinateDisplay.Size = New System.Drawing.Size(22, 13)
        Me.CoordinateDisplay.TabIndex = 7
        Me.CoordinateDisplay.Text = "- , -"
        '
        'ArmyListBox
        '
        Me.ArmyListBox.FormattingEnabled = True
        Me.ArmyListBox.Location = New System.Drawing.Point(4, 341)
        Me.ArmyListBox.Name = "ArmyListBox"
        Me.ArmyListBox.Size = New System.Drawing.Size(223, 199)
        Me.ArmyListBox.TabIndex = 8
        '
        'MoveArmiesButton
        '
        Me.MoveArmiesButton.Location = New System.Drawing.Point(6, 547)
        Me.MoveArmiesButton.Name = "MoveArmiesButton"
        Me.MoveArmiesButton.Size = New System.Drawing.Size(220, 33)
        Me.MoveArmiesButton.TabIndex = 9
        Me.MoveArmiesButton.Text = "Move Armies"
        Me.MoveArmiesButton.UseVisualStyleBackColor = True
        '
        'MainWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1884, 1001)
        Me.Controls.Add(Me.MoveArmiesButton)
        Me.Controls.Add(Me.ArmyListBox)
        Me.Controls.Add(Me.CoordinateDisplay)
        Me.Controls.Add(Me.DateButton)
        Me.Controls.Add(Me.CountryBox)
        Me.Controls.Add(Me.ProvinceBox)
        Me.Controls.Add(Me.MapModeGroup)
        Me.Controls.Add(Me.MapPictureBox)
        Me.Name = "MainWindow"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Cold War Strategy Prototype"
        CType(Me.MapPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MapModeGroup.ResumeLayout(False)
        Me.MapModeGroup.PerformLayout()
        Me.ProvinceBox.ResumeLayout(False)
        Me.ProvinceBox.PerformLayout()
        Me.CountryBox.ResumeLayout(False)
        Me.CountryBox.PerformLayout()
        CType(Me.FlagPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MapPictureBox As PictureBox
    Friend WithEvents MapModeRaw As RadioButton
    Friend WithEvents MapModePolitical As RadioButton
    Friend WithEvents MapModeGroup As GroupBox
    Friend WithEvents ProvinceBox As GroupBox
    Friend WithEvents ProvinceName As Label
    Friend WithEvents ProvinceEducation As Label
    Friend WithEvents ProvinceOwner As Label
    Friend WithEvents ProvinceController As Label
    Friend WithEvents ProvinceIndustrialization As Label
    Friend WithEvents ProvincePopulation As Label
    Friend WithEvents CountryBox As GroupBox
    Friend WithEvents CountryAllegiance As Label
    Friend WithEvents CountryGovernment As Label
    Friend WithEvents CountryLeader As Label
    Friend WithEvents CountryName As Label
    Friend WithEvents DateButton As Button
    Friend WithEvents ProvinceProduction As Label
    Friend WithEvents CountryProduction As Label
    Friend WithEvents CoordinateDisplay As Label
    Friend WithEvents FlagPictureBox As PictureBox
    Friend WithEvents ProvinceTerrain As Label
    Friend WithEvents ArmyListBox As ListBox
    Friend WithEvents MoveArmiesButton As Button
End Class
