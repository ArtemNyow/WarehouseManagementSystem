Imports System.Data.OleDb

Public Class MainForm
    ' Зверніть увагу на правильний шлях до бази даних та перевірте властивості файлу в проекті
    Public Shared connectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=WarehouseDB.accdb;Persist Security Info=False;"

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadData()
        LoadStockData()
    End Sub

    Private Sub LoadData()
        Try
            Using conn As New OleDbConnection(connectionString)
                conn.Open()
                Dim query As String =
    "SELECT " &
    "s.supplier_name AS [Назва постачальника], " &
    "p.product_name AS [Назва продукта], " &
    "t.truck_plate AS [Машина], " &
    "w.warehouse_name AS [Назва складу], " &
    "i.arrival_time AS [Час прибуття], " &
    "i.departure_time AS [Час відправки] " &
    "FROM (((Incomes i " &
    "INNER JOIN Suppliers s ON i.supplier_id = s.supplier_id) " &
    "INNER JOIN Products p ON i.product_id = p.product_id) " &
    "INNER JOIN Trucks t ON i.truck_id = t.truck_id) " &
    "INNER JOIN Warehouses w ON i.warehouse_id = w.warehouse_id"

                Dim da As New OleDbDataAdapter(query, conn)
                Dim dt As New DataTable()
                da.Fill(dt)
                dgvIncomes.DataSource = dt
            End Using
        Catch ex As Exception
            MessageBox.Show("Помилка завантаження даних: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadStockData()
        Try
            Using conn As New OleDbConnection(connectionString)
                conn.Open()
                Dim query As String =
                    "SELECT p.product_name AS [Товар], " &
                    "w.warehouse_name AS [Склад], " &
                    "s.quantity AS [Кількість], " &
                    "s.last_update AS [Оновлено] " &
                    "FROM ((Stock s " &
                    "INNER JOIN Products p ON s.product_id = p.product_id) " &
                    "INNER JOIN Warehouses w ON s.warehouse_id = w.warehouse_id)"
                Dim da As New OleDbDataAdapter(query, conn)
                Dim dt As New DataTable()
                da.Fill(dt)
                If dt.Rows.Count = 0 Then
                    MessageBox.Show("Дані відсутні в таблиці Stock.")
                Else
                    dgvStock.DataSource = dt
                    dgvStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Помилка завантаження Stock: " & ex.Message)
        End Try
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Using addForm As New AddForm()
            If addForm.ShowDialog() = DialogResult.OK Then
                ' Повне перезавантаження даних
                dgvIncomes.DataSource = Nothing
                dgvStock.DataSource = Nothing
                LoadData()
                LoadStockData()

                ' Примусове оновлення відображення
                dgvIncomes.Update()
                dgvStock.Update()
                Refresh()
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        ' Перевірка, чи вибрано рядок у DataGridView
        If dgvIncomes.SelectedRows.Count = 0 Then Return

        Dim incomeId As Integer = CInt(dgvIncomes.SelectedRows(0).Cells("income_id").Value)
        Dim editForm As New AddForm(incomeId)
        If editForm.ShowDialog() = DialogResult.OK Then
            LoadData()
            LoadStockData()
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If dgvIncomes.SelectedRows.Count = 0 Then Return

        Dim incomeId As Integer = CInt(dgvIncomes.SelectedRows(0).Cells("income_id").Value)
        If MessageBox.Show("Видалити запис?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Using conn As New OleDbConnection(connectionString)
                    conn.Open()
                    Dim query As String = "DELETE FROM Incomes WHERE income_id = @id"
                    Using cmd As New OleDbCommand(query, conn)
                        cmd.Parameters.AddWithValue("@id", incomeId)
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
                LoadData()
                LoadStockData()
            Catch ex As Exception
                MessageBox.Show("Помилка видалення: " & ex.Message)
            End Try
        End If
    End Sub

End Class
