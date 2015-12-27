<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DeSCtrl
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
        Me.txtPS3IP = New System.Windows.Forms.TextBox()
        Me.btnConnect = New System.Windows.Forms.Button()
        Me.btnTake = New System.Windows.Forms.Button()
        Me.btnRestore = New System.Windows.Forms.Button()
        Me.chkHoldO = New System.Windows.Forms.CheckBox()
        Me.txtChat = New System.Windows.Forms.TextBox()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.wb = New System.Windows.Forms.WebBrowser()
        Me.rbCCAPI = New System.Windows.Forms.RadioButton()
        Me.rbTMAPI = New System.Windows.Forms.RadioButton()
        Me.chkCMDPause = New System.Windows.Forms.CheckBox()
        Me.chkHoldL1 = New System.Windows.Forms.CheckBox()
        Me.chkVoting = New System.Windows.Forms.CheckBox()
        Me.chkHoldX = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'txtPS3IP
        '
        Me.txtPS3IP.BackColor = System.Drawing.Color.White
        Me.txtPS3IP.ForeColor = System.Drawing.Color.Black
        Me.txtPS3IP.Location = New System.Drawing.Point(53, 15)
        Me.txtPS3IP.Name = "txtPS3IP"
        Me.txtPS3IP.Size = New System.Drawing.Size(107, 20)
        Me.txtPS3IP.TabIndex = 13
        Me.txtPS3IP.Text = "10.0.0.63"
        '
        'btnConnect
        '
        Me.btnConnect.BackColor = System.Drawing.Color.LightGray
        Me.btnConnect.ForeColor = System.Drawing.Color.Black
        Me.btnConnect.Location = New System.Drawing.Point(166, 12)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(164, 25)
        Me.btnConnect.TabIndex = 12
        Me.btnConnect.Text = "Connect"
        Me.btnConnect.UseVisualStyleBackColor = False
        '
        'btnTake
        '
        Me.btnTake.Location = New System.Drawing.Point(166, 43)
        Me.btnTake.Name = "btnTake"
        Me.btnTake.Size = New System.Drawing.Size(75, 37)
        Me.btnTake.TabIndex = 14
        Me.btnTake.Text = "Take Control"
        Me.btnTake.UseVisualStyleBackColor = True
        '
        'btnRestore
        '
        Me.btnRestore.Location = New System.Drawing.Point(255, 43)
        Me.btnRestore.Name = "btnRestore"
        Me.btnRestore.Size = New System.Drawing.Size(75, 37)
        Me.btnRestore.TabIndex = 15
        Me.btnRestore.Text = "Restore Control"
        Me.btnRestore.UseVisualStyleBackColor = True
        '
        'chkHoldO
        '
        Me.chkHoldO.AutoSize = True
        Me.chkHoldO.Location = New System.Drawing.Point(53, 98)
        Me.chkHoldO.Name = "chkHoldO"
        Me.chkHoldO.Size = New System.Drawing.Size(59, 17)
        Me.chkHoldO.TabIndex = 44
        Me.chkHoldO.Text = "Hold O"
        Me.chkHoldO.UseVisualStyleBackColor = True
        '
        'txtChat
        '
        Me.txtChat.Location = New System.Drawing.Point(36, 167)
        Me.txtChat.Multiline = True
        Me.txtChat.Name = "txtChat"
        Me.txtChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtChat.Size = New System.Drawing.Size(391, 448)
        Me.txtChat.TabIndex = 45
        '
        'btnUpdate
        '
        Me.btnUpdate.Location = New System.Drawing.Point(352, 43)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(75, 37)
        Me.btnUpdate.TabIndex = 46
        Me.btnUpdate.Text = "Update"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'wb
        '
        Me.wb.Location = New System.Drawing.Point(433, 12)
        Me.wb.MinimumSize = New System.Drawing.Size(20, 20)
        Me.wb.Name = "wb"
        Me.wb.ScriptErrorsSuppressed = True
        Me.wb.Size = New System.Drawing.Size(408, 603)
        Me.wb.TabIndex = 47
        '
        'rbCCAPI
        '
        Me.rbCCAPI.AutoSize = True
        Me.rbCCAPI.Location = New System.Drawing.Point(53, 41)
        Me.rbCCAPI.Name = "rbCCAPI"
        Me.rbCCAPI.Size = New System.Drawing.Size(56, 17)
        Me.rbCCAPI.TabIndex = 48
        Me.rbCCAPI.Text = "CCAPI"
        Me.rbCCAPI.UseVisualStyleBackColor = True
        '
        'rbTMAPI
        '
        Me.rbTMAPI.AutoSize = True
        Me.rbTMAPI.Checked = True
        Me.rbTMAPI.Location = New System.Drawing.Point(53, 64)
        Me.rbTMAPI.Name = "rbTMAPI"
        Me.rbTMAPI.Size = New System.Drawing.Size(58, 17)
        Me.rbTMAPI.TabIndex = 49
        Me.rbTMAPI.TabStop = True
        Me.rbTMAPI.Text = "TMAPI"
        Me.rbTMAPI.UseVisualStyleBackColor = True
        '
        'chkCMDPause
        '
        Me.chkCMDPause.AutoSize = True
        Me.chkCMDPause.Location = New System.Drawing.Point(166, 98)
        Me.chkCMDPause.Name = "chkCMDPause"
        Me.chkCMDPause.Size = New System.Drawing.Size(154, 17)
        Me.chkCMDPause.TabIndex = 50
        Me.chkCMDPause.Text = "Pause between commands"
        Me.chkCMDPause.UseVisualStyleBackColor = True
        '
        'chkHoldL1
        '
        Me.chkHoldL1.AutoSize = True
        Me.chkHoldL1.Location = New System.Drawing.Point(53, 121)
        Me.chkHoldL1.Name = "chkHoldL1"
        Me.chkHoldL1.Size = New System.Drawing.Size(63, 17)
        Me.chkHoldL1.TabIndex = 51
        Me.chkHoldL1.Text = "Hold L1"
        Me.chkHoldL1.UseVisualStyleBackColor = True
        '
        'chkVoting
        '
        Me.chkVoting.AutoSize = True
        Me.chkVoting.Location = New System.Drawing.Point(166, 122)
        Me.chkVoting.Name = "chkVoting"
        Me.chkVoting.Size = New System.Drawing.Size(98, 17)
        Me.chkVoting.TabIndex = 52
        Me.chkVoting.Text = "Voting Enabled"
        Me.chkVoting.UseVisualStyleBackColor = True
        '
        'chkHoldX
        '
        Me.chkHoldX.AutoSize = True
        Me.chkHoldX.Location = New System.Drawing.Point(53, 144)
        Me.chkHoldX.Name = "chkHoldX"
        Me.chkHoldX.Size = New System.Drawing.Size(58, 17)
        Me.chkHoldX.TabIndex = 53
        Me.chkHoldX.Text = "Hold X"
        Me.chkHoldX.UseVisualStyleBackColor = True
        '
        'DeSCtrl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(862, 642)
        Me.Controls.Add(Me.chkHoldX)
        Me.Controls.Add(Me.chkVoting)
        Me.Controls.Add(Me.chkHoldL1)
        Me.Controls.Add(Me.chkCMDPause)
        Me.Controls.Add(Me.rbTMAPI)
        Me.Controls.Add(Me.rbCCAPI)
        Me.Controls.Add(Me.wb)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.txtChat)
        Me.Controls.Add(Me.chkHoldO)
        Me.Controls.Add(Me.btnRestore)
        Me.Controls.Add(Me.btnTake)
        Me.Controls.Add(Me.txtPS3IP)
        Me.Controls.Add(Me.btnConnect)
        Me.Name = "DeSCtrl"
        Me.Text = "Wulf's Demon's Souls Remote Control"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtPS3IP As System.Windows.Forms.TextBox
    Friend WithEvents btnConnect As System.Windows.Forms.Button
    Friend WithEvents btnTake As System.Windows.Forms.Button
    Friend WithEvents btnRestore As System.Windows.Forms.Button
    Friend WithEvents chkHoldO As System.Windows.Forms.CheckBox
    Friend WithEvents txtChat As System.Windows.Forms.TextBox
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents wb As System.Windows.Forms.WebBrowser
    Friend WithEvents rbCCAPI As System.Windows.Forms.RadioButton
    Friend WithEvents rbTMAPI As System.Windows.Forms.RadioButton
    Friend WithEvents chkCMDPause As System.Windows.Forms.CheckBox
    Friend WithEvents chkHoldL1 As System.Windows.Forms.CheckBox
    Friend WithEvents chkVoting As System.Windows.Forms.CheckBox
    Friend WithEvents chkHoldX As System.Windows.Forms.CheckBox
    'Friend WithEvents wkb As WebKit.WebKitBrowser

End Class
