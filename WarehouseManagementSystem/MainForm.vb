Imports System.Data.OleDb
Imports System.Threading
Imports System.Windows.Forms

Public Class MainForm
    Inherits Form

    ' Controls
    Private dgvIncomes As DataGridView
    Private btnAdd As Button
    Private btnEdit As Button
    Private btnDelete As Button
    Private btnAnalytics As Button
    Private txtSearch As TextBox
    Private cmbFilterSupplier As ComboBox
    Private statusStrip As StatusStrip
    Private toolStripStatusLabel As ToolStripStatusLabel

    Public connectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=WarehouseDB.accdb;"
    Private dataTable As New DataTable()



    Public Sub New()
        InitializeComponents()
        LoadSuppliersFilter()
        LoadData()
    End Sub

    Private Sub InitializeComponents()
        Me.Text = "Управління складом"
        Me.Size = New Size(1024, 600)
        Me.MinimumSize = New Size(800, 600)
        Me.StartPosition = FormStartPosition.CenterScreen


        ' === Controls ===
        dgvIncomes = New DataGridView()
        btnAdd = New Button()
        btnEdit = New Button()
        btnDelete = New Button()
        btnAnalytics = New Button()
        txtSearch = New TextBox()
        cmbFilterSupplier = New ComboBox()
        statusStrip = New StatusStrip()
        toolStripStatusLabel = New ToolStripStatusLabel("Готово")

        ' === DataGridView ===
        dgvIncomes.Dock = DockStyle.Fill
        dgvIncomes.AutoGenerateColumns = False
        dgvIncomes.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvIncomes.ReadOnly = True
        dgvIncomes.AllowUserToAddRows = False

        ' === Створення колонок ===
        dgvIncomes.Columns.Clear()
        dgvIncomes.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "ID", .DataPropertyName = "income_id", .Name = "income_id"})
        dgvIncomes.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Постачальник", .DataPropertyName = "supplier_name", .Name = "supplier_name"})
        dgvIncomes.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Продукт", .DataPropertyName = "product_name", .Name = "product_name"})
        dgvIncomes.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Машина", .DataPropertyName = "truck_plate", .Name = "truck_plate"})
        dgvIncomes.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Склад", .DataPropertyName = "warehouse_name", .Name = "warehouse_name"})
        dgvIncomes.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Прибуття", .DataPropertyName = "arrival_time", .Name = "arrival_time"})
        dgvIncomes.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Відбуття", .DataPropertyName = "departure_time", .Name = "departure_time"})
        dgvIncomes.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Завантажено", .DataPropertyName = "loaded_quantity", .Name = "loaded_quantity"})
        dgvIncomes.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Розвантажено", .DataPropertyName = "unloaded_quantity", .Name = "unloaded_quantity"})

        ' === Top Panel (пошук + фільтр) ===
        Dim topPanel As New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .Height = 40,
            .FlowDirection = FlowDirection.LeftToRight
        }
        txtSearch.Width = 200
        txtSearch.Text = ""
        cmbFilterSupplier.Width = 200
        topPanel.Controls.AddRange({txtSearch, cmbFilterSupplier})

        ' === Button Panel ===
        Dim buttonPanel As New FlowLayoutPanel With {
            .Dock = DockStyle.Bottom,
            .Height = 45,
            .FlowDirection = FlowDirection.LeftToRight,
            .Padding = New Padding(10, 5, 0, 5)
        }
        StyleButton(btnAdd, "Додати")
        StyleButton(btnEdit, "Редагувати")
        StyleButton(btnDelete, "Видалити")
        StyleButton(btnAnalytics, "Аналітика")
        buttonPanel.Controls.AddRange({btnAdd, btnEdit, btnDelete, btnAnalytics})









        ' === StatusStrip ===
        statusStrip.Items.Add(toolStripStatusLabel)
        statusStrip.Dock = DockStyle.Bottom

        ' === Add controls ===
        Me.Controls.AddRange({dgvIncomes, topPanel, buttonPanel, statusStrip})

        ' === Events ===
        AddHandler txtSearch.TextChanged, AddressOf LoadData
        AddHandler cmbFilterSupplier.SelectedIndexChanged, AddressOf LoadData
        AddHandler btnAdd.Click, AddressOf btnAdd_Click
        AddHandler btnEdit.Click, AddressOf btnEdit_Click
        AddHandler btnDelete.Click, AddressOf btnDelete_Click
        AddHandler btnAnalytics.Click, AddressOf btnAnalytics_Click



    End Sub

    Private Sub StyleButton(btn As Button, text As String)
        btn.Text = text
        btn.Width = 100
        btn.Height = 30
        btn.FlatStyle = FlatStyle.Flat
        btn.BackColor = Color.DodgerBlue
        btn.ForeColor = Color.White
        btn.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        btn.Cursor = Cursors.Hand
        AddHandler btn.MouseEnter, Sub() btn.BackColor = Color.RoyalBlue
        AddHandler btn.MouseLeave, Sub() btn.BackColor = Color.DodgerBlue
    End Sub

    Private Sub LoadSuppliersFilter()
        Try
            Using conn As New OleDbConnection(connectionString)
                conn.Open()
                Dim cmd As New OleDbCommand("SELECT supplier_id, supplier_name FROM Suppliers", conn)
                Dim reader = cmd.ExecuteReader()
                Dim dt As New DataTable()
                dt.Load(reader)

                Dim dr As DataRow = dt.NewRow()
                dr("supplier_id") = 0
                dr("supplier_name") = "Всі постачальники"
                dt.Rows.InsertAt(dr, 0)

                cmbFilterSupplier.DisplayMember = "supplier_name"
                cmbFilterSupplier.ValueMember = "supplier_id"
                cmbFilterSupplier.DataSource = dt
            End Using
        Catch ex As Exception
            MessageBox.Show("Не вдалося завантажити список постачальників: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadData()
        Try
            Using conn As New OleDbConnection(connectionString)
                conn.Open()

                Dim baseQuery As String =
                "SELECT i.income_id, s.supplier_name, p.product_name, t.truck_plate, w.warehouse_name, " &
                "i.arrival_time, i.departure_time, i.loaded_quantity, i.unloaded_quantity " &
                "FROM (((Incomes AS i " &
                "INNER JOIN Suppliers AS s ON i.supplier_id = s.supplier_id) " &
                "INNER JOIN Products AS p ON i.product_id = p.product_id) " &
                "INNER JOIN Trucks AS t ON i.truck_id = t.truck_id) " &
                "INNER JOIN Warehouses AS w ON i.warehouse_id = w.warehouse_id"

                Dim whereClauses As New List(Of String)

                ' Пошук
                Dim searchText As String = txtSearch.Text.Trim()
                If Not String.IsNullOrEmpty(searchText) AndAlso searchText <> "Пошук..." Then
                    searchText = searchText.Replace("'", "''") ' екранізація апострофів
                    whereClauses.Add(
                    "(s.supplier_name LIKE '%" & searchText & "%' OR " &
                    "p.product_name LIKE '%" & searchText & "%' OR " &
                    "t.truck_plate LIKE '%" & searchText & "%' OR " &
                    "w.warehouse_name LIKE '%" & searchText & "%')"
                )
                End If

                ' Фільтр постачальника
                If cmbFilterSupplier.SelectedIndex > 0 Then
                    Dim selectedId = CInt(cmbFilterSupplier.SelectedValue)
                    whereClauses.Add("i.supplier_id = " & selectedId)
                End If

                ' Збір фінального запиту
                If whereClauses.Count > 0 Then
                    baseQuery &= " WHERE " & String.Join(" AND ", whereClauses)
                End If

                ' Завантаження
                Dim adapter As New OleDbDataAdapter(baseQuery, conn)
                dataTable.Clear()
                adapter.Fill(dataTable)
                dgvIncomes.DataSource = dataTable
            End Using
        Catch ex As Exception
            MessageBox.Show("Помилка завантаження: " & ex.Message)
        End Try
    End Sub



    Private Sub btnAdd_Click(sender As Object, e As EventArgs)
        Using addForm As New AddForm()
            If addForm.ShowDialog() = DialogResult.OK Then LoadData()
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs)
        If dgvIncomes.SelectedRows.Count = 0 Then
            MessageBox.Show("Виберіть запис для редагування", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim selectedId As Integer = CInt(dgvIncomes.SelectedRows(0).Cells("income_id").Value)
        Using editForm As New EditForm(selectedId)
            If editForm.ShowDialog() = DialogResult.OK Then LoadData()
        End Using
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs)
        If dgvIncomes.SelectedRows.Count = 0 Then
            MessageBox.Show("Виберіть запис для видалення", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If MessageBox.Show("Видалити обраний запис?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Dim selectedId As Integer = CInt(dgvIncomes.SelectedRows(0).Cells("income_id").Value)
            Try
                Using conn As New OleDbConnection(connectionString)
                    conn.Open()
                    Dim cmd As New OleDbCommand("DELETE FROM Incomes WHERE income_id = ?", conn)
                    cmd.Parameters.AddWithValue("@id", selectedId)
                    cmd.ExecuteNonQuery()
                End Using
                LoadData()
            Catch ex As Exception
                MessageBox.Show("Помилка видалення: " & ex.Message)
            End Try
        End If
    End Sub
    Private Sub btnAnalytics_Click(sender As Object, e As EventArgs)
        Using analyticsForm As New AnalyticsForm()
            analyticsForm.ShowDialog()
        End Using
    End Sub
End Class
