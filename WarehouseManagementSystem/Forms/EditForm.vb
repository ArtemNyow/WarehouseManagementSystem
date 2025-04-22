Imports System.Data.OleDb
Imports System.Drawing
Imports System.Windows.Forms

Public Class EditForm
    Inherits Form

    ' Controls
    Private WithEvents ComboBoxTrucks As New ComboBox()
    Private WithEvents ComboBoxProducts As New ComboBox()
    Private WithEvents ComboBoxWarehouses As New ComboBox()
    Private WithEvents DateTimePickerArrival As New DateTimePicker()
    Private WithEvents DateTimePickerDeparture As New DateTimePicker()
    Private WithEvents TextBoxLoadedQuantity As New TextBox()
    Private WithEvents TextBoxUnloadedQuantity As New TextBox()
    Private WithEvents ButtonSave As New Button()
    Private WithEvents ButtonCancel As New Button()

    Private recordIdentifier As Integer = -1

    Public Sub New(Optional identifier As Integer = -1)
        recordIdentifier = identifier
        InitializeComponents()
        LoadComboBoxData()
        If identifier <> -1 Then LoadExistingRecordData()
    End Sub

    Private Sub InitializeComponents()
        Me.SuspendLayout()

        Me.Text = "Редагування транспортної накладної"
        Me.ClientSize = New Size(600, 850)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.BackColor = Color.White

        Dim headerPanel As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 60,
            .BackColor = Color.FromArgb(33, 150, 243)
        }
        Dim titleLabel As New Label With {
            .Text = "РЕДАГУВАННЯ ТРАНСПОРТНОЇ НАКЛАДНОЇ",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = Color.White,
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        headerPanel.Controls.Add(titleLabel)

        Dim footerPanel As New FlowLayoutPanel With {
            .Dock = DockStyle.Bottom,
            .Height = 80,
            .FlowDirection = FlowDirection.RightToLeft,
            .Padding = New Padding(0, 20, 20, 20),
            .BackColor = Color.White
        }
        ConfigureButton(ButtonSave, "Зберегти зміни", Color.FromArgb(76, 175, 80))
        ConfigureButton(ButtonCancel, "Скасувати", Color.FromArgb(244, 67, 54))
        footerPanel.Controls.Add(ButtonSave)
        footerPanel.Controls.Add(ButtonCancel)

        Dim mainPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(25),
            .AutoScroll = True,
            .BackColor = Color.White
        }
        Dim tl As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 3
        }
        tl.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        tl.RowStyles.Add(New RowStyle(SizeType.Absolute, 220))
        tl.RowStyles.Add(New RowStyle(SizeType.Absolute, 160))
        tl.RowStyles.Add(New RowStyle(SizeType.Absolute, 160))
        tl.Controls.Add(CreateTransportGroupBox(), 0, 0)
        tl.Controls.Add(CreateTimeGroupBox(), 0, 1)
        tl.Controls.Add(CreateQuantityGroupBox(), 0, 2)
        mainPanel.Controls.Add(tl)

        Me.Controls.Add(mainPanel)
        Me.Controls.Add(footerPanel)
        Me.Controls.Add(headerPanel)

        Me.ResumeLayout(False)
    End Sub

    Private Sub ConfigureButton(btn As Button, text As String, backColor As Color)
        btn.Text = text
        btn.Size = New Size(160, 40)
        btn.FlatStyle = FlatStyle.Flat
        btn.FlatAppearance.BorderSize = 0
        btn.BackColor = backColor
        btn.ForeColor = Color.White
        btn.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        btn.TextAlign = ContentAlignment.MiddleCenter
        btn.Padding = New Padding(0)
        AddHandler btn.MouseEnter, AddressOf Button_MouseEnter
        AddHandler btn.MouseLeave, AddressOf Button_MouseLeave
        If btn Is ButtonSave Then AddHandler btn.Click, AddressOf ButtonSave_Click
        If btn Is ButtonCancel Then AddHandler btn.Click, AddressOf ButtonCancel_Click
    End Sub

    Private Function CreateTransportGroupBox() As GroupBox
        Dim gb As New GroupBox With {
            .Text = "Транспортні дані",
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 150, 243),
            .BackColor = Color.White,
            .Dock = DockStyle.Fill
        }
        Dim tbl As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .RowCount = 3,
            .Padding = New Padding(10)
        }
        tbl.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 150))
        tbl.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        tbl.RowStyles.Add(New RowStyle(SizeType.Percent, 33))
        tbl.RowStyles.Add(New RowStyle(SizeType.Percent, 33))
        tbl.RowStyles.Add(New RowStyle(SizeType.Percent, 34))

        AddLabel(tbl, 0, 0, "Вантажний автомобіль:")
        ConfigureCombo(ComboBoxTrucks)
        tbl.Controls.Add(ComboBoxTrucks, 1, 0)

        AddLabel(tbl, 0, 1, "Товар:")
        ConfigureCombo(ComboBoxProducts)
        tbl.Controls.Add(ComboBoxProducts, 1, 1)

        AddLabel(tbl, 0, 2, "Склад призначення:")
        ConfigureCombo(ComboBoxWarehouses)
        tbl.Controls.Add(ComboBoxWarehouses, 1, 2)

        gb.Controls.Add(tbl)
        Return gb
    End Function

    Private Function CreateTimeGroupBox() As GroupBox
        ConfigureDateTimePicker(DateTimePickerArrival, "dd.MM.yyyy HH:mm")
        ConfigureDateTimePicker(DateTimePickerDeparture, "dd.MM.yyyy HH:mm")

        Dim gb As New GroupBox With {
            .Text = "Час операцій",
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 150, 243),
            .Dock = DockStyle.Fill
        }
        Dim tbl As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .RowCount = 2,
            .Padding = New Padding(10)
        }
        tbl.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 150))
        tbl.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        tbl.RowStyles.Add(New RowStyle(SizeType.Percent, 50))
        tbl.RowStyles.Add(New RowStyle(SizeType.Percent, 50))

        AddLabel(tbl, 0, 0, "Час прибуття:")
        tbl.Controls.Add(DateTimePickerArrival, 1, 0)

        AddLabel(tbl, 0, 1, "Час відправлення:")
        tbl.Controls.Add(DateTimePickerDeparture, 1, 1)

        gb.Controls.Add(tbl)
        Return gb
    End Function

    Private Function CreateQuantityGroupBox() As GroupBox
        ConfigureTextBox(TextBoxLoadedQuantity)
        ConfigureTextBox(TextBoxUnloadedQuantity)

        Dim gb As New GroupBox With {
            .Text = "Кількісні показники",
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 150, 243),
            .Dock = DockStyle.Fill
        }
        Dim tbl As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .RowCount = 2,
            .Padding = New Padding(10)
        }
        tbl.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 150))
        tbl.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        tbl.RowStyles.Add(New RowStyle(SizeType.Percent, 50))
        tbl.RowStyles.Add(New RowStyle(SizeType.Percent, 50))

        AddLabel(tbl, 0, 0, "Завантажено (кг):")
        tbl.Controls.Add(TextBoxLoadedQuantity, 1, 0)

        AddLabel(tbl, 0, 1, "Розвантажено (кг):")
        tbl.Controls.Add(TextBoxUnloadedQuantity, 1, 1)

        gb.Controls.Add(tbl)
        Return gb
    End Function

    Private Sub ConfigureTextBox(txt As TextBox)
        txt.Dock = DockStyle.Fill
        txt.Font = New Font("Segoe UI", 9)
        txt.BackColor = Color.WhiteSmoke
        txt.BorderStyle = BorderStyle.FixedSingle
        txt.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        txt.Margin = New Padding(0)
    End Sub

    Private Sub ConfigureCombo(cmb As ComboBox)
        cmb.Dock = DockStyle.Fill
        cmb.Font = New Font("Segoe UI", 9)
        cmb.FlatStyle = FlatStyle.Flat
        cmb.BackColor = Color.WhiteSmoke
        cmb.Anchor = AnchorStyles.Left Or AnchorStyles.Right
    End Sub

    Private Sub ConfigureDateTimePicker(dtp As DateTimePicker, fmt As String)
        dtp.Dock = DockStyle.Fill
        dtp.Font = New Font("Segoe UI", 9)
        dtp.Format = DateTimePickerFormat.Custom
        dtp.CustomFormat = fmt
        dtp.CalendarMonthBackground = Color.WhiteSmoke
        dtp.Anchor = AnchorStyles.Left Or AnchorStyles.Right
    End Sub

    Private Sub AddLabel(tbl As TableLayoutPanel, col As Integer, row As Integer, text As String)
        Dim lbl As New Label With {
            .Text = text,
            .Font = New Font("Segoe UI", 9),
            .TextAlign = ContentAlignment.MiddleRight,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0, 6, 10, 6)
        }
        tbl.Controls.Add(lbl, col, row)
    End Sub

    Private Sub LoadComboBoxData()
        Try
            Using conn As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Nyow\Desktop\University\4_course_2\Kursova\WarehouseManagementSystem\WarehouseManagementSystem\DataAccess\WarehouseDB.accdb;Persist Security Info=False;")
                conn.Open()
                FillCombo(conn, "SELECT truck_id, truck_plate FROM Trucks", ComboBoxTrucks, "truck_plate", "truck_id")
                FillCombo(conn, "SELECT product_id, product_name FROM Products", ComboBoxProducts, "product_name", "product_id")
                FillCombo(conn, "SELECT warehouse_id, warehouse_name FROM Warehouses", ComboBoxWarehouses, "warehouse_name", "warehouse_id")
            End Using
        Catch ex As Exception
            MessageBox.Show("Помилка завантаження довідникових даних: " & ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub FillCombo(conn As OleDbConnection, sql As String, cmb As ComboBox, display As String, value As String)
        Using da As New OleDbDataAdapter(sql, conn)
            Dim dt As New DataTable()
            da.Fill(dt)
            cmb.DataSource = dt
            cmb.DisplayMember = display
            cmb.ValueMember = value
        End Using
    End Sub

    Private Sub LoadExistingRecordData()
        Try
            Using conn As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Nyow\Desktop\University\4_course_2\Kursova\WarehouseManagementSystem\WarehouseManagementSystem\DataAccess\WarehouseDB.accdb;Persist Security Info=False;")
                conn.Open()
                Using cmd As New OleDbCommand("SELECT * FROM Incomes WHERE income_id=@id", conn)
                    cmd.Parameters.AddWithValue("@id", recordIdentifier)
                    Using rdr As OleDbDataReader = cmd.ExecuteReader()
                        If rdr.Read() Then
                            ComboBoxTrucks.SelectedValue = rdr("truck_id")
                            ComboBoxProducts.SelectedValue = rdr("product_id")
                            ComboBoxWarehouses.SelectedValue = rdr("warehouse_id")
                            DateTimePickerArrival.Value = Convert.ToDateTime(rdr("arrival_time"))
                            DateTimePickerDeparture.Value = Convert.ToDateTime(rdr("departure_time"))
                            TextBoxLoadedQuantity.Text = If(IsDBNull(rdr("loaded_quantity")), "", rdr("loaded_quantity").ToString())
                            TextBoxUnloadedQuantity.Text = If(IsDBNull(rdr("unloaded_quantity")), "", rdr("unloaded_quantity").ToString())
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Помилка завантаження запису: " & ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ButtonSave_Click(sender As Object, e As EventArgs)
        If Not ValidateInputFields() Then Return

        Try
            Using conn As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Nyow\Desktop\University\4_course_2\Kursova\WarehouseManagementSystem\WarehouseManagementSystem\DataAccess\WarehouseDB.accdb;Persist Security Info=False;")
                conn.Open()
                Dim sql As String = If(recordIdentifier = -1,
                    "INSERT INTO Incomes (truck_id,product_id,warehouse_id,arrival_time,departure_time,loaded_quantity,unloaded_quantity) VALUES (@t,@p,@w,@a,@d,@l,@u)",
                    "UPDATE Incomes SET truck_id=@t,product_id=@p,warehouse_id=@w,arrival_time=@a,departure_time=@d,loaded_quantity=@l,unloaded_quantity=@u WHERE income_id=@id")
                Using cmd As New OleDbCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@t", ComboBoxTrucks.SelectedValue)
                    cmd.Parameters.AddWithValue("@p", ComboBoxProducts.SelectedValue)
                    cmd.Parameters.AddWithValue("@w", ComboBoxWarehouses.SelectedValue)
                    cmd.Parameters.AddWithValue("@a", DateTimePickerArrival.Value)
                    cmd.Parameters.AddWithValue("@d", DateTimePickerDeparture.Value)
                    cmd.Parameters.AddWithValue("@l", Convert.ToDouble(TextBoxLoadedQuantity.Text))
                    cmd.Parameters.AddWithValue("@u", Convert.ToDouble(TextBoxUnloadedQuantity.Text))
                    If recordIdentifier <> -1 Then cmd.Parameters.AddWithValue("@id", recordIdentifier)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Помилка збереження: " & ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function ValidateInputFields() As Boolean
        If String.IsNullOrWhiteSpace(TextBoxLoadedQuantity.Text) OrElse
           String.IsNullOrWhiteSpace(TextBoxUnloadedQuantity.Text) Then
            MessageBox.Show("Будь ласка, заповніть усі поля!", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        If Not IsNumeric(TextBoxLoadedQuantity.Text) OrElse Not IsNumeric(TextBoxUnloadedQuantity.Text) Then
            MessageBox.Show("Кількісні поля повинні бути числами!", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        Return True
    End Function

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub TextBoxNumeric_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBoxLoadedQuantity.KeyPress, TextBoxUnloadedQuantity.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "." Then e.Handled = True
        If e.KeyChar = "." AndAlso DirectCast(sender, TextBox).Text.Contains(".") Then e.Handled = True
    End Sub

    Private Sub Button_MouseEnter(sender As Object, e As EventArgs)
        Dim btn = DirectCast(sender, Button)
        btn.BackColor = If(btn Is ButtonSave, Color.FromArgb(67, 160, 71), Color.FromArgb(229, 57, 53))
    End Sub

    Private Sub Button_MouseLeave(sender As Object, e As EventArgs)
        Dim btn = DirectCast(sender, Button)
        btn.BackColor = If(btn Is ButtonSave, Color.FromArgb(76, 175, 80), Color.FromArgb(244, 67, 54))
    End Sub

End Class
