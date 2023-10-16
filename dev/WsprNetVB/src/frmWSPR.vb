Public Class frmWSPR
    Private Sub cmdEncodeWSPRMessage_Click(sender As Object, e As EventArgs) Handles cmdEncodeWSPRMessage.Click
        Dim WSPR As clsWSPR = New clsWSPR
        WSPR.Encode_Message("W8ZLW", "AA00", 3)
    End Sub
End Class
