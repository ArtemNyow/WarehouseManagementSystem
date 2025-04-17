Imports System.Data.OleDb

Public Class EditForm
    Inherits Form

    Private WithEvents ComboBoxTrucks As New ComboBox()
    Private WithEvents ComboBoxProducts As New ComboBox()
    Private WithEvents ComboBoxWarehouses As New ComboBox()
    Private WithEvents DateTimePickerArrival As New DateTimePicker()
    Private WithEvents DateTimePickerDeparture As New DateTimePicker()
    Private WithEvents TextBoxQuantity As New TextBox()
    Private WithEvents ButtonSave As New Button()
    Private recordId As Integer = -1
    Private WithEvents ButtonCancel As New Button()

    Public Sub New()
        InitializeComponents()
        LoadComboBoxes()
    End Sub

    Private Sub InitializeComponents()
        Me.Text = "Додати новий запис"
        Me.Size = New Size(400, 350)
        Me.StartPosition = FormStartPosition.CenterParent

        ' Налаштування ComboBox для вантажівок
        ComboBoxTrucks.Location = New Point(20, 20)
        ComboBoxTrucks.Width = 350
        Me.Controls.Add(ComboBoxTrucks)

        ' Налаштування ComboBox для продуктів
        ComboBoxProducts.Location = New Point(20, 60)
        ComboBoxProducts.Width = 350
        Me.Controls.Add(ComboBoxProducts)

        ' Налаштування ComboBox для складів
        ComboBoxWarehouses.Location = New Point(20, 100)
        ComboBoxWarehouses.Width = 350
        Me.Controls.Add(ComboBoxWarehouses)

        ' Налаштування DateTimePicker для часу прибуття
        DateTimePickerArrival.Format = DateTimePickerFormat.Custom
        DateTimePickerArrival.CustomFormat = "dd.MM.yyyy HH:mm"
        DateTimePickerArrival.Location = New Point(20, 140)
        DateTimePickerArrival.Width = 350
        Me.Controls.Add(DateTimePickerArrival)

        ' Налаштування DateTimePicker для часу відправлення
        DateTimePickerDeparture.Format = DateTimePickerFormat.Custom
        DateTimePickerDeparture.CustomFormat = "dd.MM.yyyy HH:mm"
        DateTimePickerDeparture.Location = New Point(20, 180)
        DateTimePickerDeparture.Width = 350
        Me.Controls.Add(DateTimePickerDeparture)

        ' Налаштування TextBox для кількості
        TextBoxQuantity.Location = New Point(20, 220)
        TextBoxQuantity.Width = 350
        TextBoxQuantity.Text  = "Кількість (кг)"
        Me.Controls.Add(TextBoxQuantity)

        ' Налаштування кнопки збереження
        ButtonSave.Text = "Зберегти"
        ButtonSave.Location = New Point(20, 260)
        ButtonSave.DialogResult = DialogResult.OK
        Me.Controls.Add(ButtonSave)

        ' Налаштування кнопки скасування
        ButtonCancel.Text = "Скасувати"
        ButtonCancel.Location = New Point(140, 260)
        ButtonCancel.DialogResult = DialogResult.Cancel
        Me.Controls.Add(ButtonCancel)
    End Sub

    Private Sub LoadComboBoxes()
        Try
            Using Connection As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=WarehouseDB.accdb;")
                Connection.Open()

                ' Завантаження вантажівок
                Dim TrucksTable As New DataTable()
                Using Command As New OleDbCommand("SELECT truck_id, truck_plate FROM Trucks", Connection)
                    TrucksTable.Load(Command.ExecuteReader())
                    ComboBoxTrucks.DataSource = TrucksTable
                    ComboBoxTrucks.DisplayMember = "truck_plate"
                    ComboBoxTrucks.ValueMember = "truck_id"
                End Using

                ' Завантаження продуктів
                Dim ProductsTable As New DataTable()
                Using Command As New OleDbCommand("SELECT product_id, product_name FROM Products", Connection)
                    ProductsTable.Load(Command.ExecuteReader())
                    ComboBoxProducts.DataSource = ProductsTable
                    ComboBoxProducts.DisplayMember = "product_name"
                    ComboBoxProducts.ValueMember = "product_id"
                End Using

                ' Завантаження складів
                Dim WarehousesTable As New DataTable()
                Using Command As New OleDbCommand("SELECT warehouse_id, warehouse_name FROM Warehouses", Connection)
                    WarehousesTable.Load(Command.ExecuteReader())
                    ComboBoxWarehouses.DataSource = WarehousesTable
                    ComboBoxWarehouses.DisplayMember = "warehouse_name"
                    ComboBoxWarehouses.ValueMember = "warehouse_id"
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Помилка завантаження списків: " & ex.Message)
        End Try
    End Sub
    Public Sub New(Optional id As Integer = -1)
        InitializeComponents()
        recordId = id
        LoadComboBoxes()
        If id <> -1 Then LoadExistingData()
    End Sub

    Private Sub LoadExistingData()
        Try
            Using conn As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=WarehouseDB.accdb;")
                conn.Open()
                Dim query = "SELECT * FROM Incomes WHERE income_id = @id"
                Using cmd As New OleDbCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", recordId)
                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            ComboBoxTrucks.SelectedValue = reader("truck_id")
                            ComboBoxProducts.SelectedValue = reader("product_id")
                            ComboBoxWarehouses.SelectedValue = reader("warehouse_id")
                            DateTimePickerArrival.Value = Convert.ToDateTime(reader("arrival_time"))
                            DateTimePickerDeparture.Value = Convert.ToDateTime(reader("departure_time"))
                            TextBoxQuantity.Text = If(IsDBNull(reader("quantity")), "", reader("quantity").ToString())
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Помилка завантаження даних: " & ex.Message)
        End Try
    End Sub

    Private Sub ButtonSave_Click(sender As Object, e As EventArgs) Handles ButtonSave.Click
        If MessageBox.Show("Зберегти зміни?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        Try
            Using conn As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=WarehouseDB.accdb;")
                conn.Open()
                Dim query As String = If(recordId = -1,
                    "INSERT INTO Incomes (truck_id, product_id, warehouse_id, arrival_time, departure_time, quantity) VALUES (?,?,?,?,?,?)",
                    "UPDATE Incomes SET truck_id=?, product_id=?, warehouse_id=?, arrival_time=?, departure_time=?, quantity=? WHERE income_id=?")

                Using cmd As New OleDbCommand(query, conn)
                    cmd.Parameters.AddWithValue("@truck", ComboBoxTrucks.SelectedValue)
                    cmd.Parameters.AddWithValue("@product", ComboBoxProducts.SelectedValue)
                    cmd.Parameters.AddWithValue("@warehouse", ComboBoxWarehouses.SelectedValue)
                    cmd.Parameters.AddWithValue("@arrival", DateTimePickerArrival.Value)
                    cmd.Parameters.AddWithValue("@departure", DateTimePickerDeparture.Value)
                    cmd.Parameters.AddWithValue("@quantity", CInt(TextBoxQuantity.Text))

                    If recordId <> -1 Then
                        cmd.Parameters.AddWithValue("@id", recordId)
                    End If

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("Помилка збереження: " & ex.Message)
        End Try
    End Sub


End Class