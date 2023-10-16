<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWSPR
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
        Me.cmdEncodeWSPRMessage = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'cmdEncodeWSPRMessage
        '
        Me.cmdEncodeWSPRMessage.Location = New System.Drawing.Point(91, 46)
        Me.cmdEncodeWSPRMessage.Name = "cmdEncodeWSPRMessage"
        Me.cmdEncodeWSPRMessage.Size = New System.Drawing.Size(225, 27)
        Me.cmdEncodeWSPRMessage.TabIndex = 0
        Me.cmdEncodeWSPRMessage.Text = "Encode WSPR Message"
        Me.cmdEncodeWSPRMessage.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(407, 120)
        Me.Controls.Add(Me.cmdEncodeWSPRMessage)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "WSPRNET"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents cmdEncodeWSPRMessage As Button
End Class
