Imports System.Data.OleDb
Imports System.IO

Public Class AddForm
    Private connectionString As String = MainForm.connectionString
    Private editId As Integer = -1
    Private currentStock As Integer = 0
    Private oldLoaded As Integer = 0
    Private oldUnloaded As Integer = 0

    Public Sub New()
        InitializeComponent()
        ConfigureUI()
        LoadComboBoxes()

    End Sub

    Public Sub New(id As Integer)
        Me.New()
        editId = id
        LoadExistingData()
    End Sub

    Private Sub ConfigureUI()
        dtpArrival.Format = DateTimePickerFormat.Custom
        dtpArrival.CustomFormat = "dd.MM.yyyy HH:mm"
        dtpDeparture.Format = DateTimePickerFormat.Custom
        dtpDeparture.CustomFormat = "dd.MM.yyyy HH:mm"

        AddHandler rbLoad.CheckedChanged, AddressOf OperationTypeChanged
        AddHandler rbUnload.CheckedChanged, AddressOf OperationTypeChanged
        rbLoad.Checked = True

        OperationTypeChanged(Nothing, Nothing)
    End Sub




    Private Sub OperationTypeChanged(sender As Object, e As EventArgs)
        txtLoaded.Enabled = rbLoad.Checked
        txtUnloaded.Enabled = rbUnload.Checked
        LoadWarehouseComboBox()
    End Sub

    Private Sub LoadExistingData()
        Try
            Using conn As New OleDbConnection(connectionString)
                conn.Open()
                Dim query As String = "SELECT * FROM Incomes WHERE income_id = @id"
                Using cmd As New OleDbCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", editId)
                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            SetComboValue(cmbSuppliers, "supplier_id", reader("supplier_id"))
                            SetComboValue(cmbProducts, "product_id", reader("product_id"))
                            SetComboValue(cmbTrucks, "truck_id", reader("truck_id"))
                            SetComboValue(cmbWarehouses, "warehouse_id", reader("warehouse_id"))
                            dtpArrival.Value = Convert.ToDateTime(reader("arrival_time"))
                            dtpDeparture.Value = Convert.ToDateTime(reader("departure_time"))

                            If Convert.ToInt32(reader("loaded_quantity")) > 0 Then
                                rbLoad.Checked = True
                                txtLoaded.Text = reader("loaded_quantity").ToString()
                            Else
                                rbUnload.Checked = True
                                txtUnloaded.Text = reader("unloaded_quantity").ToString()
                            End If

                            oldLoaded = Convert.ToInt32(reader("loaded_quantity"))
                            oldUnloaded = Convert.ToInt32(reader("unloaded_quantity"))
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Помилка завантаження даних: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadComboBoxes()
        Try
            Using conn As New OleDbConnection(connectionString)
                conn.Open()

                LoadComboBox(cmbSuppliers, conn, "SELECT supplier_id, supplier_name FROM Suppliers", "supplier_id", "supplier_name")
                LoadComboBox(cmbProducts, conn, "SELECT product_id, product_name ,product_image FROM Products", "product_id", "product_name")
                LoadComboBox(cmbTrucks, conn, "SELECT truck_id, truck_plate, truck_capacity, truck_image FROM Trucks", "truck_id", "truck_plate")



                If cmbProducts.Items.Count > 0 Then
                    cmbProducts.SelectedIndex = 0
                    LoadWarehouseComboBox()
                End If
            End Using


        Catch ex As Exception
            MessageBox.Show("Помилка завантаження списків: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadComboBox(combo As ComboBox, conn As OleDbConnection, query As String, valueField As String, displayField As String)
        Using cmd As New OleDbCommand(query, conn)
            Dim dt As New DataTable()
            dt.Load(cmd.ExecuteReader())
            With combo
                .ValueMember = valueField
                .DisplayMember = displayField
                .DataSource = dt
            End With
        End Using
    End Sub

    Private Sub LoadWarehouseComboBox()
        If cmbProducts.SelectedValue Is Nothing Then Return

        Try
            Using conn As New OleDbConnection(connectionString)
                conn.Open()
                Dim query As String =
                    "SELECT w.warehouse_id, w.warehouse_name, IIF(s.quantity IS NULL, 0, s.quantity) AS quantity " &
                    "FROM Warehouses w LEFT JOIN " &
                    "(SELECT warehouse_id, quantity FROM Stock WHERE product_id = @productId) AS s " &
                    "ON w.warehouse_id = s.warehouse_id"

                Using cmd As New OleDbCommand(query, conn)
                    cmd.Parameters.AddWithValue("@productId", Convert.ToInt32(cmbProducts.SelectedValue))
                    Dim dt As New DataTable()
                    dt.Load(cmd.ExecuteReader())
                    cmbWarehouses.ValueMember = "warehouse_id"
                    cmbWarehouses.DisplayMember = "warehouse_name"
                    cmbWarehouses.DataSource = dt
                    If dt.Rows.Count > 0 Then cmbWarehouses.SelectedIndex = 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Помилка оновлення складів: " & ex.Message)
        End Try
    End Sub

    Private Sub cmbTrucks_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTrucks.SelectedIndexChanged
        Try
            If cmbTrucks.SelectedItem IsNot Nothing Then
                Dim row As DataRowView = CType(cmbTrucks.SelectedItem, DataRowView)

                ' Оновлення вантажопідйомності
                lblTruckCapacity.Text = $"Вантажопідйомність: {row("truck_capacity")} кг"

                ' Обробка зображення
                If Not IsDBNull(row("truck_image")) Then
                    Dim imageBytes As Byte() = row("truck_image")

                    ' Видалення OLE-заголовків (якщо потрібно)
                    imageBytes = ExtractImageFromOleData(imageBytes)

                    Using ms As New MemoryStream(imageBytes)
                        picTruck.Image = Image.FromStream(ms)
                    End Using
                Else
                    picTruck.Image = Nothing
                End If
            End If
        Catch ex As Exception
            MessageBox.Show($"Помилка завантаження зображення: {ex.Message}")
            picTruck.Image = Nothing
        End Try
    End Sub


    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not ValidateInputs() Then Return

        Using conn As New OleDbConnection(connectionString)
            conn.Open()
            Using transaction As OleDbTransaction = conn.BeginTransaction()
                Try
                    Dim incomeId = SaveIncome(conn, transaction)
                    UpdateStock(conn, transaction, incomeId)
                    transaction.Commit()
                    MessageBox.Show("Дані збережено успішно!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Me.DialogResult = DialogResult.OK
                    Me.Close()
                Catch ex As Exception
                    transaction.Rollback()
                    MessageBox.Show("Помилка збереження: " & ex.Message)
                End Try
            End Using
        End Using
    End Sub

    Private Function SaveIncome(conn As OleDbConnection, transaction As OleDbTransaction) As Integer
        Dim query As String
        If editId = -1 Then
            ' Додаємо новий запис - НЕ згадуємо income_id
            query = "INSERT INTO Incomes (supplier_id, product_id, truck_id, warehouse_id, arrival_time, departure_time, loaded_quantity, unloaded_quantity) " &
                "VALUES (?, ?, ?, ?, ?, ?, ?, ?)"
        Else
            ' Оновлюємо існуючий запис
            query = "UPDATE Incomes SET supplier_id=?, product_id=?, truck_id=?, warehouse_id=?, arrival_time=?, departure_time=?, loaded_quantity=?, unloaded_quantity=? WHERE income_id=?"
        End If

        Using cmd As New OleDbCommand(query, conn, transaction)
            cmd.Parameters.AddWithValue("@supplier", cmbSuppliers.SelectedValue)
            cmd.Parameters.AddWithValue("@product", cmbProducts.SelectedValue)
            cmd.Parameters.AddWithValue("@truck", cmbTrucks.SelectedValue)
            cmd.Parameters.AddWithValue("@warehouse", cmbWarehouses.SelectedValue)
            cmd.Parameters.AddWithValue("@arrival", dtpArrival.Value)
            cmd.Parameters.AddWithValue("@departure", dtpDeparture.Value)
            cmd.Parameters.AddWithValue("@loaded", If(rbLoad.Checked, Convert.ToInt32(txtLoaded.Text), 0))
            cmd.Parameters.AddWithValue("@unloaded", If(rbUnload.Checked, Convert.ToInt32(txtUnloaded.Text), 0))

            If editId <> -1 Then
                cmd.Parameters.AddWithValue("@id", editId) ' тільки для оновлення
            End If

            cmd.ExecuteNonQuery()

            If editId = -1 Then
                cmd.CommandText = "SELECT @@IDENTITY"
                Return Convert.ToInt32(cmd.ExecuteScalar())
            Else
                Return editId
            End If
        End Using
    End Function





    Private Sub UpdateStock(conn As OleDbConnection, transaction As OleDbTransaction, incomeId As Integer)
        Dim productId = Convert.ToInt32(cmbProducts.SelectedValue)
        Dim warehouseId = Convert.ToInt32(cmbWarehouses.SelectedValue)
        Dim newLoaded = If(String.IsNullOrEmpty(txtLoaded.Text), 0, Convert.ToInt32(txtLoaded.Text))
        Dim newUnloaded = If(String.IsNullOrEmpty(txtUnloaded.Text), 0, Convert.ToInt32(txtUnloaded.Text))

        Dim stockChange As Integer
        If editId = -1 Then
            stockChange = newLoaded - newUnloaded
        Else
            stockChange = (newLoaded - oldLoaded) - (newUnloaded - oldUnloaded)
        End If

        Using checkCmd As New OleDbCommand("SELECT COUNT(*) FROM Stock WHERE product_id=@p AND warehouse_id=@w", conn, transaction)
            checkCmd.Parameters.AddWithValue("@p", productId)
            checkCmd.Parameters.AddWithValue("@w", warehouseId)
            Dim exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0

            If exists Then
                ' Оновлюємо, якщо запис вже є
                Using updateCmd As New OleDbCommand("UPDATE Stock SET quantity = quantity + @delta, last_update = NOW() WHERE product_id=@p AND warehouse_id=@w", conn, transaction)
                    updateCmd.Parameters.AddWithValue("@delta", stockChange)
                    updateCmd.Parameters.AddWithValue("@p", productId)
                    updateCmd.Parameters.AddWithValue("@w", warehouseId)
                    updateCmd.ExecuteNonQuery()
                End Using
            ElseIf stockChange > 0 Then
                ' Вставляємо тільки якщо щось завантажили
                Using insertCmd As New OleDbCommand("INSERT INTO Stock (product_id, warehouse_id, quantity, last_update) VALUES (@p, @w, @qty, NOW())", conn, transaction)
                    insertCmd.Parameters.AddWithValue("@p", productId)
                    insertCmd.Parameters.AddWithValue("@w", warehouseId)
                    insertCmd.Parameters.AddWithValue("@qty", stockChange)
                    insertCmd.ExecuteNonQuery()
                End Using
            End If
        End Using
    End Sub


    Private Function ValidateInputs() As Boolean
        If cmbSuppliers.SelectedIndex = -1 OrElse cmbProducts.SelectedIndex = -1 OrElse cmbTrucks.SelectedIndex = -1 OrElse cmbWarehouses.SelectedIndex = -1 Then
            MessageBox.Show("Заповніть всі обов’язкові поля!", "Увага")
            Return False
        End If

        Dim value As Integer
        If rbLoad.Checked Then
            If Not Integer.TryParse(txtLoaded.Text, value) OrElse value <= 0 Then
                MessageBox.Show("Некоректна кількість для завантаження!")
                Return False
            End If
        Else
            If Not Integer.TryParse(txtUnloaded.Text, value) OrElse value <= 0 Then
                MessageBox.Show("Некоректна кількість для вивантаження!")
                Return False
            End If

            If value > currentStock Then
                MessageBox.Show("Недостатньо товару на складі!")
                Return False
            End If
        End If
        If rbLoad.Checked Then
            If Not Integer.TryParse(txtLoaded.Text, value) OrElse value <= 0 Then
                MessageBox.Show("Некоректна кількість для завантаження!")
                Return False
            End If

            ' Перевірка вантажопідйомності
            If cmbTrucks.SelectedItem IsNot Nothing Then
                Dim truckRow As DataRowView = CType(cmbTrucks.SelectedItem, DataRowView)
                Dim truck_capacity = Convert.ToInt32(truckRow("truck_capacity"))
                If value > truck_capacity Then
                    MessageBox.Show("Кількість перевищує вантажопідйомність обраної машини!")
                    Return False
                End If
            End If
        End If

        Return True
    End Function

    Private Sub SetComboValue(combo As ComboBox, field As String, value As Object)
        For Each item As DataRowView In combo.Items
            If Convert.ToInt32(item(field)) = Convert.ToInt32(value) Then
                combo.SelectedItem = item
                Exit For
            End If
        Next
    End Sub
    Private Function ExtractImageFromOleData(oleBytes As Byte()) As Byte()
        If oleBytes Is Nothing OrElse oleBytes.Length < 100 Then Return oleBytes

        ' Шукаємо сигнатури зображень
        Dim jpegPattern As Byte() = {&HFF, &HD8, &HFF}
        Dim pngPattern As Byte() = {&H89, &H50, &H4E, &H47}

        For i As Integer = 0 To oleBytes.Length - 5
            If oleBytes.Skip(i).Take(3).SequenceEqual(jpegPattern) Then
                Return oleBytes.Skip(i).ToArray()
            ElseIf oleBytes.Skip(i).Take(4).SequenceEqual(pngPattern) Then
                Return oleBytes.Skip(i).ToArray()
            End If
        Next

        Return oleBytes
    End Function

    Private Sub cmbProducts_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbProducts.SelectedIndexChanged
        Try
            If cmbProducts.SelectedItem IsNot Nothing Then
                Dim row As DataRowView = CType(cmbProducts.SelectedItem, DataRowView)

                ' Оновлення складу (якщо потрібно)
                LoadWarehouseComboBox()

                ' Обробка зображення
                If Not IsDBNull(row("product_image")) Then
                    Dim imageBytes As Byte() = row("product_image")

                    ' Видалення OLE-заголовків
                    imageBytes = ExtractImageFromOleData(imageBytes)

                    Using ms As New MemoryStream(imageBytes)
                        picProduct.Image = Image.FromStream(ms)
                    End Using
                Else
                    picProduct.Image = Nothing
                End If
            End If
        Catch ex As Exception
            MessageBox.Show($"Помилка завантаження зображення продукту: {ex.Message}")
            picProduct.Image = Nothing
        End Try
    End Sub

    Private Sub cmbWarehouses_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbWarehouses.SelectedIndexChanged
        If cmbWarehouses.SelectedItem IsNot Nothing Then
            Dim row As DataRowView = CType(cmbWarehouses.SelectedItem, DataRowView)
            currentStock = Convert.ToInt32(row("quantity"))
            lblCurrentStock.Text = $"Поточна кількість: {currentStock} кг"
            txtUnloaded.Enabled = rbUnload.Checked AndAlso currentStock > 0
        End If
    End Sub

    Private Sub txtNumeric_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtLoaded.KeyPress, txtUnloaded.KeyPress
        If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> ControlChars.Back Then e.Handled = True
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class