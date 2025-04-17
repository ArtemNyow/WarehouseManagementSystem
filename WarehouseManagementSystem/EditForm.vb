Imports System.Data.OleDb
Imports System.Drawing
Imports System.Windows.Forms

Public Class EditForm
    Inherits Form

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
        If identifier <> -1 Then
            LoadExistingRecordData()
        End If
    End Sub

    Private Sub InitializeComponents()
        ConfigureMainFormSettings()
        CreateHeaderPanel()
        CreateMainContentPanel()
        CreateButtonsPanel()
    End Sub

    Private Sub ConfigureMainFormSettings()
        Me.Text = "Редагування запису"
        Me.Size = New Size(600, 850)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.BackColor = Color.White
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
    End Sub

    Private Sub CreateHeaderPanel()
        Dim headerPanel As New Panel()
        headerPanel.Dock = DockStyle.Top
        headerPanel.Height = 60
        headerPanel.BackColor = Color.FromArgb(33, 150, 243)

        Dim titleLabel As New Label()
        titleLabel.Text = "РЕДАГУВАННЯ ТРАНСПОРТНОЇ НАКЛАДНОЇ"
        titleLabel.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        titleLabel.ForeColor = Color.White
        titleLabel.Dock = DockStyle.Fill
        titleLabel.TextAlign = ContentAlignment.MiddleCenter

        headerPanel.Controls.Add(titleLabel)
        Me.Controls.Add(headerPanel)
    End Sub

    Private Sub CreateMainContentPanel()
        Dim mainPanel As New Panel()
        mainPanel.Dock = DockStyle.Fill
        mainPanel.BackColor = Color.White
        mainPanel.Padding = New Padding(25)
        mainPanel.AutoScroll = True

        mainPanel.Controls.Add(CreateTransportGroupBox())
        mainPanel.Controls.Add(CreateTimeGroupBox())
        mainPanel.Controls.Add(CreateQuantityGroupBox())

        Me.Controls.Add(mainPanel)
    End Sub

    Private Function CreateTransportGroupBox() As GroupBox
        Dim groupBoxTransport As New GroupBox()
        groupBoxTransport.Text = "Транспортні дані"
        groupBoxTransport.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        groupBoxTransport.ForeColor = Color.FromArgb(33, 150, 243)
        groupBoxTransport.Dock = DockStyle.Top
        groupBoxTransport.Padding = New Padding(15)
        groupBoxTransport.Height = 220

        groupBoxTransport.Controls.Add(CreateTruckControl())
        groupBoxTransport.Controls.Add(CreateProductControl())
        groupBoxTransport.Controls.Add(CreateWarehouseControl())

        Return groupBoxTransport
    End Function

    Private Function CreateTruckControl() As Panel
        Dim panelTruck As New Panel()
        panelTruck.Dock = DockStyle.Top
        panelTruck.Height = 55
        panelTruck.Padding = New Padding(0, 5, 0, 5)

        Dim labelTruck As New Label()
        labelTruck.Text = "Вантажний автомобіль:"
        labelTruck.Font = New Font("Segoe UI", 9)
        labelTruck.Dock = DockStyle.Left
        labelTruck.Width = 150
        labelTruck.TextAlign = ContentAlignment.MiddleLeft

        ComboBoxTrucks.Dock = DockStyle.Fill
        ComboBoxTrucks.Font = New Font("Segoe UI", 9)
        ComboBoxTrucks.FlatStyle = FlatStyle.Flat
        ComboBoxTrucks.BackColor = Color.WhiteSmoke
        ComboBoxTrucks.DropDownStyle = ComboBoxStyle.DropDownList

        panelTruck.Controls.Add(ComboBoxTrucks)
        panelTruck.Controls.Add(labelTruck)

        Return panelTruck
    End Function

    Private Function CreateProductControl() As Panel
        Dim panelProduct As New Panel()
        panelProduct.Dock = DockStyle.Top
        panelProduct.Height = 55
        panelProduct.Padding = New Padding(0, 5, 0, 5)

        Dim labelProduct As New Label()
        labelProduct.Text = "Товар:"
        labelProduct.Font = New Font("Segoe UI", 9)
        labelProduct.Dock = DockStyle.Left
        labelProduct.Width = 150
        labelProduct.TextAlign = ContentAlignment.MiddleLeft

        ComboBoxProducts.Dock = DockStyle.Fill
        ComboBoxProducts.Font = New Font("Segoe UI", 9)
        ComboBoxProducts.FlatStyle = FlatStyle.Flat
        ComboBoxProducts.BackColor = Color.WhiteSmoke
        ComboBoxProducts.DropDownStyle = ComboBoxStyle.DropDownList

        panelProduct.Controls.Add(ComboBoxProducts)
        panelProduct.Controls.Add(labelProduct)

        Return panelProduct
    End Function

    Private Function CreateWarehouseControl() As Panel
        Dim panelWarehouse As New Panel()
        panelWarehouse.Dock = DockStyle.Top
        panelWarehouse.Height = 55
        panelWarehouse.Padding = New Padding(0, 5, 0, 5)

        Dim labelWarehouse As New Label()
        labelWarehouse.Text = "Склад призначення:"
        labelWarehouse.Font = New Font("Segoe UI", 9)
        labelWarehouse.Dock = DockStyle.Left
        labelWarehouse.Width = 150
        labelWarehouse.TextAlign = ContentAlignment.MiddleLeft

        ComboBoxWarehouses.Dock = DockStyle.Fill
        ComboBoxWarehouses.Font = New Font("Segoe UI", 9)
        ComboBoxWarehouses.FlatStyle = FlatStyle.Flat
        ComboBoxWarehouses.BackColor = Color.WhiteSmoke
        ComboBoxWarehouses.DropDownStyle = ComboBoxStyle.DropDownList

        panelWarehouse.Controls.Add(ComboBoxWarehouses)
        panelWarehouse.Controls.Add(labelWarehouse)

        Return panelWarehouse
    End Function

    Private Function CreateTimeGroupBox() As GroupBox
        Dim groupBoxTime As New GroupBox()
        groupBoxTime.Text = "Час операцій"
        groupBoxTime.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        groupBoxTime.ForeColor = Color.FromArgb(33, 150, 243)
        groupBoxTime.Dock = DockStyle.Top
        groupBoxTime.Padding = New Padding(15)
        groupBoxTime.Height = 160

        groupBoxTime.Controls.Add(CreateArrivalTimeControl())
        groupBoxTime.Controls.Add(CreateDepartureTimeControl())

        Return groupBoxTime
    End Function

    Private Function CreateArrivalTimeControl() As Panel
        Dim panelArrival As New Panel()
        panelArrival.Dock = DockStyle.Top
         panelArrival.Height = 65 ' Збільшено висоту
        panelArrival.Padding = New Padding(0, 10, 0, 10)

        Dim labelArrival As New Label()
        labelArrival.Text = "Час прибуття:"
        labelArrival.Font = New Font("Segoe UI", 9)
        labelArrival.Dock = DockStyle.Left
        labelArrival.Width = 150
        labelArrival.TextAlign = ContentAlignment.MiddleLeft

        DateTimePickerArrival.CustomFormat = "dd.MM.yyyy HH:mm"
        DateTimePickerArrival.Format = DateTimePickerFormat.Custom
        DateTimePickerArrival.Dock = DockStyle.Fill
        DateTimePickerArrival.Font = New Font("Segoe UI", 9)
        DateTimePickerArrival.CalendarMonthBackground = Color.WhiteSmoke

        panelArrival.Controls.Add(DateTimePickerArrival)
        panelArrival.Controls.Add(labelArrival)

        Return panelArrival
    End Function

    Private Function CreateDepartureTimeControl() As Panel
        Dim panelDeparture As New Panel()
        panelDeparture.Dock = DockStyle.Top
        panelDeparture.Height = 55
        panelDeparture.Padding = New Padding(0, 5, 0, 5)

        Dim labelDeparture As New Label()
        labelDeparture.Text = "Час відправлення:"
        labelDeparture.Font = New Font("Segoe UI", 9)
        labelDeparture.Dock = DockStyle.Left
        labelDeparture.Width = 150
        labelDeparture.TextAlign = ContentAlignment.MiddleLeft

        DateTimePickerDeparture.CustomFormat = "dd.MM.yyyy HH:mm"
        DateTimePickerDeparture.Format = DateTimePickerFormat.Custom
        DateTimePickerDeparture.Dock = DockStyle.Fill
        DateTimePickerDeparture.Font = New Font("Segoe UI", 9)
        DateTimePickerDeparture.CalendarMonthBackground = Color.WhiteSmoke

        panelDeparture.Controls.Add(DateTimePickerDeparture)
        panelDeparture.Controls.Add(labelDeparture)

        Return panelDeparture
    End Function

    Private Function CreateQuantityGroupBox() As GroupBox
        Dim groupBoxQuantity As New GroupBox()
        groupBoxQuantity.Text = "Кількісні показники"
        groupBoxQuantity.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        groupBoxQuantity.ForeColor = Color.FromArgb(33, 150, 243)
        groupBoxQuantity.Dock = DockStyle.Top
        groupBoxQuantity.Padding = New Padding(15)
        groupBoxQuantity.Height = 160

        groupBoxQuantity.Controls.Add(CreateLoadedQuantityControl())
        groupBoxQuantity.Controls.Add(CreateUnloadedQuantityControl())

        Return groupBoxQuantity
    End Function

    Private Function CreateLoadedQuantityControl() As Panel
        Dim panelLoaded As New Panel()
        panelLoaded.Dock = DockStyle.Top
        panelLoaded.Height = 55
        panelLoaded.Padding = New Padding(0, 5, 0, 5)

        Dim labelLoaded As New Label()
        labelLoaded.Text = "Завантажено (кг):"
        labelLoaded.Font = New Font("Segoe UI", 9)
        labelLoaded.Dock = DockStyle.Left
        labelLoaded.Width = 150
        labelLoaded.TextAlign = ContentAlignment.MiddleLeft

        TextBoxLoadedQuantity.Dock = DockStyle.Fill
        TextBoxLoadedQuantity.Font = New Font("Segoe UI", 9)
        TextBoxLoadedQuantity.BackColor = Color.WhiteSmoke
        TextBoxLoadedQuantity.BorderStyle = BorderStyle.None
        TextBoxLoadedQuantity.Padding = New Padding(5)

        panelLoaded.Controls.Add(TextBoxLoadedQuantity)
        panelLoaded.Controls.Add(labelLoaded)

        Return panelLoaded
    End Function

    Private Function CreateUnloadedQuantityControl() As Panel
        Dim panelUnloaded As New Panel()
        panelUnloaded.Dock = DockStyle.Top
        panelUnloaded.Height = 55
        panelUnloaded.Padding = New Padding(0, 5, 0, 5)

        Dim labelUnloaded As New Label()
        labelUnloaded.Text = "Розвантажено (кг):"
        labelUnloaded.Font = New Font("Segoe UI", 9)
        labelUnloaded.Dock = DockStyle.Left
        labelUnloaded.Width = 150
        labelUnloaded.TextAlign = ContentAlignment.MiddleLeft

        TextBoxUnloadedQuantity.Dock = DockStyle.Fill
        TextBoxUnloadedQuantity.Font = New Font("Segoe UI", 9)
        TextBoxUnloadedQuantity.BackColor = Color.WhiteSmoke
        TextBoxUnloadedQuantity.BorderStyle = BorderStyle.None
        TextBoxUnloadedQuantity.Padding = New Padding(5)

        panelUnloaded.Controls.Add(TextBoxUnloadedQuantity)
        panelUnloaded.Controls.Add(labelUnloaded)

        Return panelUnloaded
    End Function

    Private Sub CreateButtonsPanel()
        Dim panelButtons As New Panel()
        panelButtons.Dock = DockStyle.Bottom
        panelButtons.Height = 80
        panelButtons.BackColor = Color.White
        panelButtons.Padding = New Padding(25)

        ConfigureSaveButton(panelButtons)
        ConfigureCancelButton(panelButtons)

        Me.Controls.Add(panelButtons)
    End Sub

    Private Sub ConfigureSaveButton(container As Panel)
        ButtonSave.Text = "Зберегти зміни"
        ButtonSave.Size = New Size(160, 40)
        ButtonSave.FlatStyle = FlatStyle.Flat
        ButtonSave.FlatAppearance.BorderSize = 0
        ButtonSave.BackColor = Color.FromArgb(76, 175, 80)
        ButtonSave.ForeColor = Color.White
        ButtonSave.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        ButtonSave.Anchor = AnchorStyles.Right
        ButtonSave.Left = container.Width - 330
        container.Controls.Add(ButtonSave)
    End Sub

    Private Sub ConfigureCancelButton(container As Panel)
        ButtonCancel.Text = "Скасувати редагування"
        ButtonCancel.Size = New Size(160, 40)
        ButtonCancel.FlatStyle = FlatStyle.Flat
        ButtonCancel.FlatAppearance.BorderSize = 0
        ButtonCancel.BackColor = Color.FromArgb(244, 67, 54)
        ButtonCancel.ForeColor = Color.White
        ButtonCancel.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        ButtonCancel.Anchor = AnchorStyles.Right
        ButtonCancel.Left = container.Width - 160
        container.Controls.Add(ButtonCancel)
    End Sub

    Private Sub LoadComboBoxData()
        Try
            Using databaseConnection As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=WarehouseDB.accdb;")
                databaseConnection.Open()

                LoadComboBoxDataFromTable(databaseConnection, "SELECT truck_id, truck_plate FROM Trucks", ComboBoxTrucks, "truck_plate", "truck_id")
                LoadComboBoxDataFromTable(databaseConnection, "SELECT product_id, product_name FROM Products", ComboBoxProducts, "product_name", "product_id")
                LoadComboBoxDataFromTable(databaseConnection, "SELECT warehouse_id, warehouse_name FROM Warehouses", ComboBoxWarehouses, "warehouse_name", "warehouse_id")
            End Using
        Catch exception As Exception
            ShowErrorMessage("Помилка завантаження довідникових даних: " & exception.Message)
        End Try
    End Sub

    Private Sub LoadComboBoxDataFromTable(connection As OleDbConnection, query As String, comboBox As ComboBox, displayMember As String, valueMember As String)
        Using dataAdapter As New OleDbDataAdapter(query, connection)
            Dim dataTable As New DataTable()
            dataAdapter.Fill(dataTable)
            comboBox.DataSource = dataTable
            comboBox.DisplayMember = displayMember
            comboBox.ValueMember = valueMember
        End Using
    End Sub

    Private Sub LoadExistingRecordData()
        Try
            Using databaseConnection As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=WarehouseDB.accdb;")
                databaseConnection.Open()
                Using databaseCommand As New OleDbCommand("SELECT * FROM Incomes WHERE income_id = @identifier", databaseConnection)
                    databaseCommand.Parameters.AddWithValue("@identifier", recordIdentifier)
                    Using dataReader As OleDbDataReader = databaseCommand.ExecuteReader()
                        If dataReader.Read() Then
                            ComboBoxTrucks.SelectedValue = dataReader("truck_id")
                            ComboBoxProducts.SelectedValue = dataReader("product_id")
                            ComboBoxWarehouses.SelectedValue = dataReader("warehouse_id")
                            DateTimePickerArrival.Value = Convert.ToDateTime(dataReader("arrival_time"))
                            DateTimePickerDeparture.Value = Convert.ToDateTime(dataReader("departure_time"))
                            TextBoxLoadedQuantity.Text = If(IsDBNull(dataReader("loaded_quantity")), "", dataReader("loaded_quantity").ToString())
                            TextBoxUnloadedQuantity.Text = If(IsDBNull(dataReader("unloaded_quantity")), "", dataReader("unloaded_quantity").ToString())
                        End If
                    End Using
                End Using
            End Using
        Catch exception As Exception
            ShowErrorMessage("Помилка завантаження існуючого запису: " & exception.Message)
        End Try
    End Sub

    Private Sub ButtonSave_Click(sender As Object, eventArgs As EventArgs) Handles ButtonSave.Click
        If Not ValidateInputFields() Then
            Return
        End If

        Try
            Using databaseConnection As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=WarehouseDB.accdb;")
                databaseConnection.Open()
                Dim databaseQuery As String = If(recordIdentifier = -1,
                    "INSERT INTO Incomes (truck_id, product_id, warehouse_id, arrival_time, departure_time, loaded_quantity, unloaded_quantity) " &
                    "VALUES (@truckIdentifier, @productIdentifier, @warehouseIdentifier, @arrivalTime, @departureTime, @loadedQuantity, @unloadedQuantity)",
                    "UPDATE Incomes SET truck_id = @truckIdentifier, product_id = @productIdentifier, warehouse_id = @warehouseIdentifier, " &
                    "arrival_time = @arrivalTime, departure_time = @departureTime, loaded_quantity = @loadedQuantity, " &
                    "unloaded_quantity = @unloadedQuantity WHERE income_id = @recordIdentifier")

                Using databaseCommand As New OleDbCommand(databaseQuery, databaseConnection)
                    databaseCommand.Parameters.AddWithValue("@truckIdentifier", ComboBoxTrucks.SelectedValue)
                    databaseCommand.Parameters.AddWithValue("@productIdentifier", ComboBoxProducts.SelectedValue)
                    databaseCommand.Parameters.AddWithValue("@warehouseIdentifier", ComboBoxWarehouses.SelectedValue)
                    databaseCommand.Parameters.AddWithValue("@arrivalTime", DateTimePickerArrival.Value)
                    databaseCommand.Parameters.AddWithValue("@departureTime", DateTimePickerDeparture.Value)
                    databaseCommand.Parameters.AddWithValue("@loadedQuantity", Convert.ToDouble(TextBoxLoadedQuantity.Text))
                    databaseCommand.Parameters.AddWithValue("@unloadedQuantity", Convert.ToDouble(TextBoxUnloadedQuantity.Text))

                    If recordIdentifier <> -1 Then
                        databaseCommand.Parameters.AddWithValue("@recordIdentifier", recordIdentifier)
                    End If

                    databaseCommand.ExecuteNonQuery()
                End Using
            End Using

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch exception As Exception
            ShowErrorMessage("Помилка збереження змін: " & exception.Message)
        End Try
    End Sub

    Private Function ValidateInputFields() As Boolean
        If String.IsNullOrWhiteSpace(TextBoxLoadedQuantity.Text) OrElse
           String.IsNullOrWhiteSpace(TextBoxUnloadedQuantity.Text) Then
            ShowWarningMessage("Будь ласка, заповніть всі обов'язкові поля!")
            Return False
        End If

        If Not IsNumeric(TextBoxLoadedQuantity.Text) OrElse
           Not IsNumeric(TextBoxUnloadedQuantity.Text) Then
            ShowWarningMessage("Кількісні поля повинні містити числові значення!")
            Return False
        End If

        Return True
    End Function

    Private Sub ButtonCancel_Click(sender As Object, eventArgs As EventArgs) Handles ButtonCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub TextBoxNumeric_KeyPress(sender As Object, eventArgs As KeyPressEventArgs) Handles TextBoxLoadedQuantity.KeyPress, TextBoxUnloadedQuantity.KeyPress
        If Not Char.IsControl(eventArgs.KeyChar) AndAlso Not Char.IsDigit(eventArgs.KeyChar) AndAlso eventArgs.KeyChar <> "." Then
            eventArgs.Handled = True
        End If

        If eventArgs.KeyChar = "." AndAlso DirectCast(sender, TextBox).Text.IndexOf(".") > -1 Then
            eventArgs.Handled = True
        End If
    End Sub

    Private Sub ShowErrorMessage(message As String)
        MessageBox.Show(message, "Помилка операції", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub ShowWarningMessage(message As String)
        MessageBox.Show(message, "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub

    Private Sub Button_MouseEnter(sender As Object, eventArgs As EventArgs) Handles ButtonSave.MouseEnter, ButtonCancel.MouseEnter
        Dim targetButton As Button = DirectCast(sender, Button)
        If targetButton Is ButtonSave Then
            targetButton.BackColor = Color.FromArgb(67, 160, 71)
        Else
            targetButton.BackColor = Color.FromArgb(229, 57, 53)
        End If
    End Sub

    Private Sub Button_MouseLeave(sender As Object, eventArgs As EventArgs) Handles ButtonSave.MouseLeave, ButtonCancel.MouseLeave
        Dim targetButton As Button = DirectCast(sender, Button)
        If targetButton Is ButtonSave Then
            targetButton.BackColor = Color.FromArgb(76, 175, 80)
        Else
            targetButton.BackColor = Color.FromArgb(244, 67, 54)
        End If
    End Sub
End Class