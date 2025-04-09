Imports System.IO

Public Class MainForm
    Private connectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" &
                                   Path.Combine(Application.StartupPath, "WarehouseDB.accdb")
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class