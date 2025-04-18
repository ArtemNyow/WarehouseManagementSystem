Imports System.Data.OleDb
Imports System.Windows.Forms
Imports System.Windows.Forms.DataVisualization.Charting

Public Class AnalyticsForm
    Inherits Form

    Private connectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=WarehouseDB.accdb;"

    ' Елементи управління
    Private chartDemand As Chart
    Private chartAccuracy As Chart
    Private dgvLowStock As DataGridView
    Private dgvTruckEfficiency As DataGridView
    Private lblTurnover As Label
    Private lblAvgEfficiency As Label
    Private numAlpha As NumericUpDown

    Public Sub New()
        InitializeComponents()
        LoadAnalyticsData()
    End Sub

    Private Sub InitializeComponents()
        Me.Text = "Аналітика та оптимізація"
        Me.Size = New Size(1300, 900)
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Основний контейнер: 2 колонки, 4 рядки
        Dim mainLayout As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .RowCount = 4,
            .CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
        }
        mainLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        mainLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        mainLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 40)) ' Графік попиту
        mainLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 25)) ' Таблиці
        mainLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 25)) ' Графік точності
        mainLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 10)) ' Показники

        ' === Графік попиту та прогнозу ===
        Dim gbDemand As New GroupBox With {
            .Text = "Прогноз попиту (Exponential Smoothing)",
            .Dock = DockStyle.Fill
        }
        chartDemand = New Chart()
        chartDemand.Dock = DockStyle.Fill
        chartDemand.ChartAreas.Add(New ChartArea("DemandArea"))
        chartDemand.Legends.Add(New Legend())
        gbDemand.Controls.Add(chartDemand)
        mainLayout.Controls.Add(gbDemand, 0, 0)
        mainLayout.SetColumnSpan(gbDemand, 2)

        ' === Таблиці: низькі запаси та ефективність вантажівок ===
        Dim gbLowStock As New GroupBox With {.Text = "Товари для повторного замовлення", .Dock = DockStyle.Fill}
        dgvLowStock = New DataGridView With {.Dock = DockStyle.Fill, .ReadOnly = True, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill}
        gbLowStock.Controls.Add(dgvLowStock)
        mainLayout.Controls.Add(gbLowStock, 0, 1)

        Dim gbTrucks As New GroupBox With {.Text = "Ефективність вантажівок (%)", .Dock = DockStyle.Fill}
        dgvTruckEfficiency = New DataGridView With {.Dock = DockStyle.Fill, .ReadOnly = True, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill}
        gbTrucks.Controls.Add(dgvTruckEfficiency)
        mainLayout.Controls.Add(gbTrucks, 1, 1)

        ' === Графік точності прогнозу ===
        Dim gbAccuracy As New GroupBox With {.Text = "Точність прогнозу (MAPE)", .Dock = DockStyle.Fill}
        chartAccuracy = New Chart()
        chartAccuracy.Dock = DockStyle.Fill
        chartAccuracy.ChartAreas.Add(New ChartArea("AccuracyArea"))
        chartAccuracy.Legends.Add(New Legend())
        gbAccuracy.Controls.Add(chartAccuracy)
        mainLayout.Controls.Add(gbAccuracy, 0, 2)
        mainLayout.SetColumnSpan(gbAccuracy, 2)

        ' === Параметр моделі: альфа експоненціального згладжування ===
        Dim pnlParams As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight, .Padding = New Padding(10)}
        pnlParams.Controls.Add(New Label With {.Text = "Alpha: ", .AutoSize = True})
        numAlpha = New NumericUpDown With {.Minimum = 0.1D, .Maximum = 1D, .DecimalPlaces = 2, .Increment = 0.05D, .Value = 0.3D}
        AddHandler numAlpha.ValueChanged, AddressOf OnParamChanged
        pnlParams.Controls.Add(numAlpha)
        mainLayout.Controls.Add(pnlParams, 0, 3)
        mainLayout.SetColumnSpan(pnlParams, 2)

        ' Метки загальних показників
        lblTurnover = New Label With {.AutoSize = True, .Font = New Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold), .Margin = New Padding(20, 5, 5, 5)}
        lblAvgEfficiency = New Label With {.AutoSize = True, .Font = New Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold), .Margin = New Padding(20, 5, 5, 5)}
        pnlParams.Controls.Add(lblTurnover)
        pnlParams.Controls.Add(lblAvgEfficiency)

        Me.Controls.Add(mainLayout)
    End Sub

    Private Sub OnParamChanged(sender As Object, e As EventArgs)
        LoadAnalyticsData()
    End Sub

    Private Sub LoadAnalyticsData()
        Try
            Using conn As New OleDbConnection(connectionString)
                conn.Open()
                LoadDemandForecast(conn)
                LoadReorderPoints(conn)
                LoadTruckEfficiency(conn)
                LoadGeneralStats(conn)
            End Using
        Catch ex As Exception
            MessageBox.Show("Помилка завантаження аналітики: " & ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadDemandForecast(conn As OleDbConnection)
        chartDemand.Series.Clear()
        Dim seriesActual As New Series("Фактичний попит") With {.ChartType = SeriesChartType.Line}
        Dim seriesForecast As New Series("Прогноз") With {.ChartType = SeriesChartType.Line, .BorderDashStyle = ChartDashStyle.Dash}

        Dim cmdText = "SELECT arrival_time, unloaded_quantity FROM Incomes ORDER BY arrival_time"
        Dim adapter As New OleDbDataAdapter(cmdText, conn)
        Dim dt As New DataTable()
        adapter.Fill(dt)

        ' Збираємо дані
        Dim times As New List(Of Date)()
        Dim actuals As New List(Of Double)()
        For Each row As DataRow In dt.Rows
            times.Add(CDate(row("arrival_time")))
            actuals.Add(CDbl(row("unloaded_quantity")))
        Next

        ' Обчислюємо експоненціальне згладжування
        Dim alpha As Double = CDbl(numAlpha.Value)
        Dim forecasts As New List(Of Double)
        If actuals.Count > 0 Then forecasts.Add(actuals(0)) ' Ініціалізація
        For i As Integer = 1 To actuals.Count - 1
            Dim f = alpha * actuals(i - 1) + (1 - alpha) * forecasts(i - 1)
            forecasts.Add(f)
        Next

        ' Додаємо точки серій
        For i As Integer = 0 To times.Count - 1
            seriesActual.Points.AddXY(times(i), actuals(i))
            If i > 0 Then seriesForecast.Points.AddXY(times(i), forecasts(i))
        Next

        chartDemand.Series.Add(seriesActual)
        chartDemand.Series.Add(seriesForecast)

        ' Побудова точності (MAPE)
        LoadForecastAccuracy(times, actuals, forecasts)
    End Sub

    Private Sub LoadForecastAccuracy(times As List(Of Date), actuals As List(Of Double), forecasts As List(Of Double))
        chartAccuracy.Series.Clear()
        Dim seriesMAPE As New Series("MAPE") With {.ChartType = SeriesChartType.Column}

        ' Обчислюємо помилку по точках
        For i As Integer = 1 To actuals.Count - 1
            Dim errorPct = Math.Abs((actuals(i) - forecasts(i)) / actuals(i)) * 100
            seriesMAPE.Points.AddXY(times(i), errorPct)
        Next

        chartAccuracy.Series.Add(seriesMAPE)
    End Sub

    Private Sub LoadReorderPoints(conn As OleDbConnection)
        ' без змін
        Dim query = "SELECT p.product_name, s.quantity, " &
                    "(SELECT AVG(unloaded_quantity) FROM Incomes WHERE product_id = p.product_id) AS avg_demand " &
                    "FROM Stock s INNER JOIN Products p ON s.product_id = p.product_id"
        Dim adapter As New OleDbDataAdapter(query, conn)
        Dim dt As New DataTable()
        adapter.Fill(dt)

        dt.Columns.Add("ROP", GetType(Double))
        For Each row As DataRow In dt.Rows
            Dim avgDemand = If(IsDBNull(row("avg_demand")), 0, CDbl(row("avg_demand")))
            Dim leadTime = 7
            Dim safetyStock = avgDemand * 0.5
            row("ROP") = avgDemand * leadTime + safetyStock
        Next

        dgvLowStock.DataSource = dt
        dgvLowStock.Columns("product_name").HeaderText = "Товар"
        dgvLowStock.Columns("quantity").HeaderText = "На складі"
        dgvLowStock.Columns("ROP").HeaderText = "Точка замовлення"
    End Sub

    Private Sub LoadTruckEfficiency(conn As OleDbConnection)
        ' без змін
        Dim query = "SELECT t.truck_plate, " &
                    "SUM(i.loaded_quantity) / t.truck_capacity * 100 AS efficiency " &
                    "FROM Incomes i INNER JOIN Trucks t ON i.truck_id = t.truck_id " &
                    "GROUP BY t.truck_plate, t.truck_capacity"
        Dim adapter As New OleDbDataAdapter(query, conn)
        Dim dt As New DataTable()
        adapter.Fill(dt)

        dgvTruckEfficiency.DataSource = dt
        dgvTruckEfficiency.Columns("truck_plate").HeaderText = "Номер машини"
        dgvTruckEfficiency.Columns("efficiency").HeaderText = "Ефективність (%)"
    End Sub

    Private Sub LoadGeneralStats(conn As OleDbConnection)
        ' без змін
        Dim cmdTurnover = New OleDbCommand(
            "SELECT SUM(unloaded_quantity) / AVG(quantity) FROM Incomes i " &
            "INNER JOIN Stock s ON i.product_id = s.product_id", conn)
        Dim turnover = cmdTurnover.ExecuteScalar()
        lblTurnover.Text = "Середня оборотність: " & Math.Round(CDec(turnover), 2).ToString()

        Dim cmdEfficiency = New OleDbCommand(
            "SELECT AVG(i.loaded_quantity / t.truck_capacity) * 100 FROM Incomes i " &
            "INNER JOIN Trucks t ON i.truck_id = t.truck_id", conn)
        Dim avgEff = cmdEfficiency.ExecuteScalar()
        lblAvgEfficiency.Text = "Середня ефективність: " & Math.Round(CDec(avgEff), 1).ToString() & "%"
    End Sub
End Class
