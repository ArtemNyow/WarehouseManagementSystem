Imports System.Data.OleDb

Public Class AddForm
    Private connectionString As String = MainForm.connectionString
    Private editId As Integer = -1
    Private currentStock As Integer = 0
    Private oldLoaded As Integer = 0
    Private oldUnloaded As Integer = 0

    ' Конструктори
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

    ' Налаштування елементів управління
    Private Sub ConfigureUI()
        dtpArrival.Format = DateTimePickerFormat.Custom
        dtpArrival.CustomFormat = "dd.MM.yyyy HH:mm"
        dtpDeparture.Format = DateTimePickerFormat.Custom
        dtpDeparture.CustomFormat = "dd.MM.yyyy HH:mm"
    End Sub

    ' Завантаження існуючих даних для редагування
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
                            txtLoaded.Text = reader("loaded_quantity").ToString()
                            txtUnloaded.Text = reader("unloaded_quantity").ToString()
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

    ' Ініціалізація комбобоксів
    Private Sub LoadComboBoxes()
        Try
            Using conn As New OleDbConnection(connectionString)
                conn.Open()

                ' Постачальники
                Using cmd As New OleDbCommand("SELECT supplier_id, supplier_name FROM Suppliers", conn)
                    Dim dt As New DataTable()
                    dt.Load(cmd.ExecuteReader())
                    With cmbSuppliers
                        .ValueMember = "supplier_id"
                        .DisplayMember = "supplier_name"
                        .DataSource = dt
                    End With
                End Using

                ' Товари
                Using cmd As New OleDbCommand("SELECT product_id, product_name FROM Products", conn)
                    Dim dt As New DataTable()
                    dt.Load(cmd.ExecuteReader())
                    With cmbProducts
                        .ValueMember = "product_id"
                        .DisplayMember = "product_name"
                        .DataSource = dt
                        If .Items.Count > 0 Then .SelectedIndex = 0
                    End With
                End Using

                ' Вантажівки
                Using cmd As New OleDbCommand("SELECT truck_id, truck_plate, truck_capacity FROM Trucks", conn)
                    Dim dt As New DataTable()
                    dt.Load(cmd.ExecuteReader())
                    With cmbTrucks
                        .ValueMember = "truck_id"
                        .DisplayMember = "truck_plate"
                        .DataSource = dt
                    End With
                End Using
            End Using

            LoadWarehouseComboBox()
        Catch ex As Exception
            MessageBox.Show("Помилка завантаження списків: " & ex.Message)
        End Try
    End Sub

    ' Завантаження складів для обраного товару
    Private Sub LoadWarehouseComboBox()
        Try
            If cmbProducts.SelectedValue Is Nothing Then
                MessageBox.Show("Оберіть товар зі списку!")
                Return
            End If

            Using conn As New OleDbConnection(connectionString)
                conn.Open()
                Using cmd As New OleDbCommand(
                    "SELECT w.warehouse_id, w.warehouse_name, s.quantity " &
                    "FROM (Warehouses w INNER JOIN Stock s ON w.warehouse_id = s.warehouse_id) " &
                    "WHERE s.product_id = @productId", conn)

                    cmd.Parameters.Add("@productId", OleDbType.Integer).Value = Convert.ToInt32(cmbProducts.SelectedValue)

                    Dim dt As New DataTable()
                    dt.Load(cmd.ExecuteReader())

                    With cmbWarehouses
                        .ValueMember = "warehouse_id"
                        .DisplayMember = "warehouse_name"
                        .DataSource = dt
                        If .Items.Count > 0 Then .SelectedIndex = 0
                    End With
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Помилка оновлення складів: " & ex.Message)
        End Try
    End Sub

    ' Обробники подій
    Private Sub cmbProducts_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbProducts.SelectedIndexChanged
        LoadWarehouseComboBox()
    End Sub

    Private Sub cmbWarehouses_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbWarehouses.SelectedIndexChanged
        If cmbWarehouses.SelectedItem IsNot Nothing Then
            Dim selectedRow As DataRowView = DirectCast(cmbWarehouses.SelectedItem, DataRowView)
            currentStock = Convert.ToInt32(selectedRow("quantity"))
            lblCurrentStock.Text = $"Поточна кількість: {currentStock} кг"
        End If
    End Sub

    Private Sub cmbTrucks_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTrucks.SelectedIndexChanged
        If cmbTrucks.SelectedItem IsNot Nothing Then
            Dim selectedTruck As DataRowView = DirectCast(cmbTrucks.SelectedItem, DataRowView)
            lblTruckCapacity.Text = $"Місткість: {selectedTruck("truck_capacity")} кг"
        End If
    End Sub

    ' Збереження даних
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not ValidateInputs() Then Return

        Using conn As New OleDbConnection(connectionString)
            conn.Open()
            Using transaction As OleDbTransaction = conn.BeginTransaction()
                Try
                    Dim incomeId As Integer = SaveIncome(conn, transaction)
                    UpdateStock(conn, transaction, incomeId)
                    transaction.Commit()
                    MessageBox.Show("Дані збережено успішно!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Me.DialogResult = DialogResult.OK
                    Me.Close()
                Catch ex As Exception
                    transaction.Rollback()
                    MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
    End Sub

    ' Допоміжні методи
    Private Function SaveIncome(conn As OleDbConnection, transaction As OleDbTransaction) As Integer
        Dim query As String = If(editId = -1,
            "INSERT INTO Incomes (supplier_id, product_id, truck_id, warehouse_id, arrival_time, departure_time, loaded_quantity, unloaded_quantity) " &
            "VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
            "UPDATE Incomes SET supplier_id=?, product_id=?, truck_id=?, warehouse_id=?, arrival_time=?, departure_time=?, loaded_quantity=?, unloaded_quantity=? " &
            "WHERE income_id=?")

        Using cmd As New OleDbCommand(query, conn, transaction)
            ' Отримання значень з комбобоксів
            Dim supplierId As Integer = Convert.ToInt32(cmbSuppliers.SelectedValue)
            Dim productId As Integer = Convert.ToInt32(cmbProducts.SelectedValue)
            Dim truckId As Integer = Convert.ToInt32(cmbTrucks.SelectedValue)
            Dim warehouseId As Integer = Convert.ToInt32(cmbWarehouses.SelectedValue)

            ' Додавання параметрів
            cmd.Parameters.AddWithValue("@sup", supplierId)
            cmd.Parameters.AddWithValue("@prod", productId)
            cmd.Parameters.AddWithValue("@truck", truckId)
            cmd.Parameters.AddWithValue("@wh", warehouseId)
            cmd.Parameters.AddWithValue("@arrival", dtpArrival.Value)
            cmd.Parameters.AddWithValue("@departure", dtpDeparture.Value)
            cmd.Parameters.AddWithValue("@loaded", Convert.ToInt32(txtLoaded.Text))
            cmd.Parameters.AddWithValue("@unloaded", Convert.ToInt32(txtUnloaded.Text))

            If editId <> -1 Then
                cmd.Parameters.AddWithValue("@id", editId)
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
        Dim newLoaded = Convert.ToInt32(txtLoaded.Text)
        Dim newUnloaded = Convert.ToInt32(txtUnloaded.Text)
        Dim warehouseId = Convert.ToInt32(cmbWarehouses.SelectedValue)
        Dim productId = Convert.ToInt32(cmbProducts.SelectedValue)

        Dim stockChange = (newUnloaded - oldUnloaded) - (newLoaded - oldLoaded)

        Using cmd As New OleDbCommand(
            "UPDATE Stock SET quantity = quantity + @change WHERE product_id = @prodId AND warehouse_id = @whId",
            conn, transaction)

            cmd.Parameters.AddWithValue("@change", stockChange)
            cmd.Parameters.AddWithValue("@prodId", productId)
            cmd.Parameters.AddWithValue("@whId", warehouseId)
            cmd.ExecuteNonQuery()
        End Using

        Using cmd As New OleDbCommand(
            "UPDATE Stock SET last_update = NOW() WHERE product_id = @prodId AND warehouse_id = @whId",
            conn, transaction)

            cmd.Parameters.AddWithValue("@prodId", productId)
            cmd.Parameters.AddWithValue("@whId", warehouseId)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Function ValidateInputs() As Boolean
        If cmbSuppliers.SelectedIndex = -1 OrElse
           cmbProducts.SelectedIndex = -1 OrElse
           cmbTrucks.SelectedIndex = -1 OrElse
           cmbWarehouses.SelectedIndex = -1 Then
            MessageBox.Show("Заповніть всі обов'язкові поля!", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If Not Integer.TryParse(txtLoaded.Text, Nothing) OrElse
           Not Integer.TryParse(txtUnloaded.Text, Nothing) Then
            MessageBox.Show("Некоректні числові значення!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Dim loadedQty = Convert.ToInt32(txtLoaded.Text)
        Dim truckCapacity = Convert.ToInt32(DirectCast(cmbTrucks.SelectedItem, DataRowView)("truck_capacity"))

        If loadedQty > truckCapacity Then
            MessageBox.Show($"Перевищено місткість вантажівки! Максимум: {truckCapacity} кг", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        If loadedQty > currentStock Then
            MessageBox.Show($"Недостатньо товару на складі! Доступно: {currentStock} кг", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True
    End Function

    Private Sub SetComboValue(combo As ComboBox, fieldName As String, value As Object)
        For Each item As DataRowView In combo.Items
            If Convert.ToInt32(item(fieldName)) = Convert.ToInt32(value) Then
                combo.SelectedItem = item
                Exit Sub
            End If
        Next
    End Sub

    Private Sub txtLoaded_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtLoaded.KeyPress, txtUnloaded.KeyPress
        If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> ControlChars.Back Then
            e.Handled = True
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class