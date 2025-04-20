Imports System.Data.OleDb
Imports System.Windows.Forms
Imports System.Windows.Forms.DataVisualization.Charting

Public Class AnalyticsForm
    Inherits Form

    Private connectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=WarehouseDB.accdb;"

    ' Елементи керування
    Private chartDemand As Chart
    Private chartAccuracy As Chart
    Private dgvReorder As DataGridView
    Private dgvTruckEfficiency As DataGridView
    Private lblTurnover As Label
    Private lblAvgTruckEff As Label
    Private numAlpha As NumericUpDown
    Private numOrderCost As NumericUpDown
    Private numHoldingCost As NumericUpDown

    Public Sub New()
        Me.Text = "Аналітика складу"
        Me.Size = New Size(1300, 900)
        Me.StartPosition = FormStartPosition.CenterScreen
        InitializeLayout()
        LoadAnalyticsData()
    End Sub

    Private Sub InitializeLayout()
        Dim layout As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .RowCount = 4,
            .ColumnCount = 2,
            .CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
        }

        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 40))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 25))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 25))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 10))
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))

        ' --- Графік попиту ---
        chartDemand = CreateChart("Прогноз попиту (Exponential Smoothing)", SeriesChartType.Line)
        layout.Controls.Add(WrapInGroupBox(chartDemand, "Прогноз попиту"), 0, 0)
        layout.SetColumnSpan(chartDemand.Parent, 2)

        ' --- Таблиця товарів ---
        dgvReorder = CreateGrid()
        layout.Controls.Add(WrapInGroupBox(dgvReorder, "Товари для замовлення"), 0, 1)

        ' --- Таблиця ефективності вантажівок ---
        dgvTruckEfficiency = CreateGrid()
        layout.Controls.Add(WrapInGroupBox(dgvTruckEfficiency, "Ефективність вантажівок"), 1, 1)

        ' --- Графік MAPE ---
        chartAccuracy = CreateChart("Точність прогнозу", SeriesChartType.Column)
        layout.Controls.Add(WrapInGroupBox(chartAccuracy, "MAPE по днях"), 0, 2)
        layout.SetColumnSpan(chartAccuracy.Parent, 2)

        ' --- Параметри ---
        Dim pnlParams As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .Padding = New Padding(10)}

        pnlParams.Controls.Add(New Label With {.Text = "Коефіцієнт згладжування (α):", .AutoSize = True})
        numAlpha = New NumericUpDown With {.Minimum = 0.1D, .Maximum = 1D, .DecimalPlaces = 2, .Increment = 0.05D, .Value = 0.3D}
        AddHandler numAlpha.ValueChanged, AddressOf OnParamChanged
        pnlParams.Controls.Add(numAlpha)

        pnlParams.Controls.Add(New Label With {.Text = "Вартість замовлення (S):", .AutoSize = True, .Margin = New Padding(30, 0, 0, 0)})
        numOrderCost = New NumericUpDown With {.Minimum = 1, .Maximum = 10000, .DecimalPlaces = 2, .Increment = 10, .Value = 100}
        AddHandler numOrderCost.ValueChanged, AddressOf OnParamChanged
        pnlParams.Controls.Add(numOrderCost)

        pnlParams.Controls.Add(New Label With {.Text = "Витрати на зберігання (H):", .AutoSize = True, .Margin = New Padding(30, 0, 0, 0)})
        numHoldingCost = New NumericUpDown With {.Minimum = 0.1D, .Maximum = 1000, .DecimalPlaces = 2, .Increment = 1, .Value = 1}
        AddHandler numHoldingCost.ValueChanged, AddressOf OnParamChanged
        pnlParams.Controls.Add(numHoldingCost)

        lblTurnover = New Label With {.AutoSize = True, .Font = New Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold), .Margin = New Padding(30, 0, 0, 0)}
        lblAvgTruckEff = New Label With {.AutoSize = True, .Font = New Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold), .Margin = New Padding(30, 0, 0, 0)}
        pnlParams.Controls.Add(lblTurnover)
        pnlParams.Controls.Add(lblAvgTruckEff)

        layout.Controls.Add(pnlParams, 0, 3)
        layout.SetColumnSpan(pnlParams, 2)

        Me.Controls.Add(layout)
    End Sub

    Private Function WrapInGroupBox(ctrl As Control, title As String) As GroupBox
        Dim groupBox As New GroupBox With {
        .Text = title,
        .Dock = DockStyle.Fill
    }
        groupBox.Controls.Add(ctrl)
        Return groupBox
    End Function

    Private Function CreateGrid() As DataGridView
        Return New DataGridView With {.Dock = DockStyle.Fill, .ReadOnly = True, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill}
    End Function

    Private Function CreateChart(title As String, type As SeriesChartType) As Chart
        Dim chart As New Chart With {.Dock = DockStyle.Fill}
        chart.ChartAreas.Add(New ChartArea("Main"))
        chart.Legends.Add(New Legend())
        Return chart
    End Function

    Private Sub OnParamChanged(sender As Object, e As EventArgs)
        LoadAnalyticsData()
    End Sub

    Private Sub LoadAnalyticsData()
        Try
            Using conn As New OleDbConnection(connectionString)
                conn.Open()
                LoadDemandForecast(conn)
                LoadReorderTable(conn)
                LoadTruckEfficiency(conn)
                LoadSummaryStats(conn)
            End Using
        Catch ex As Exception
            MessageBox.Show("Помилка аналітики: " & ex.Message)
        End Try
    End Sub

    ' === Прогноз попиту ===
    Private Sub LoadDemandForecast(conn As OleDbConnection)
        chartDemand.Series.Clear()
        chartDemand.Titles.Clear()
        Dim actual As New Series("Фактичний попит") With {
        .ChartType = SeriesChartType.Line,
        .Color = Color.Blue,
        .BorderWidth = 2,
        .MarkerStyle = MarkerStyle.Circle
    }
        Dim forecast As New Series("Прогноз") With {.ChartType = SeriesChartType.Line, .BorderDashStyle = ChartDashStyle.Dash}

        Dim dt As New DataTable()
        Dim cmd = New OleDbDataAdapter("SELECT arrival_time, unloaded_quantity FROM Incomes ORDER BY arrival_time", conn)
        cmd.Fill(dt)

        Dim times = dt.AsEnumerable().Select(Function(r) CDate(r("arrival_time"))).ToList()
        Dim values = dt.AsEnumerable().Select(Function(r) CDbl(r("unloaded_quantity"))).ToList()

        Dim alpha = CDbl(numAlpha.Value)
        Dim forecasts As New List(Of Double)
        If values.Count > 0 Then forecasts.Add(values(0))
        For i = 1 To values.Count - 1
            forecasts.Add(alpha * values(i - 1) + (1 - alpha) * forecasts(i - 1))
        Next

        For i = 0 To values.Count - 1
            actual.Points.AddXY(times(i), values(i))
            If i > 0 Then forecast.Points.AddXY(times(i), forecasts(i))
        Next

        chartDemand.Series.Add(actual)
        chartDemand.Series.Add(forecast)

        LoadForecastAccuracy(times, values, forecasts)
    End Sub

    Private Sub LoadForecastAccuracy(times As List(Of Date), actuals As List(Of Double), forecasts As List(Of Double))
        chartAccuracy.Series.Clear()
        Dim mape As New Series("MAPE") With {.ChartType = SeriesChartType.Column}
        For i = 1 To actuals.Count - 1
            Dim err = Math.Abs((actuals(i) - forecasts(i)) / actuals(i)) * 100
            mape.Points.AddXY(times(i), err)
        Next
        chartAccuracy.Series.Add(mape)
    End Sub

    ' === EOQ та ROP ===
    Private Sub LoadReorderTable(conn As OleDbConnection)
        Dim query = "SELECT p.product_name, s.quantity, " &
                "(SELECT AVG(unloaded_quantity) FROM Incomes WHERE product_id = p.product_id) AS середньодобовий_попит " &  ' Український аліас
                "FROM Products p INNER JOIN Stock s ON p.product_id = s.product_id"

        Dim dt As New DataTable()
        Dim adapter As New OleDbDataAdapter(query, conn)
        adapter.Fill(dt)

        dt.Columns.Add("ROP", GetType(Double))
        dt.Columns.Add("EOQ", GetType(Double))

        Dim S = CDec(numOrderCost.Value)
        Dim H = CDec(numHoldingCost.Value)

        For Each row As DataRow In dt.Rows
            ' Звертаємось до стовпця з українською назвою через квадратні дужки та лапки
            Dim avg = If(IsDBNull(row("середньодобовий_попит")), 0, CDbl(row("середньодобовий_попит")))
            Dim D = avg * 365
            Dim leadTime = 7
            row("ROP") = Math.Round(avg * leadTime + avg * 0.5, 1)  ' Зменшено до 1 знаку після коми
            row("EOQ") = If(H > 0, Math.Round(Math.Sqrt(2 * D * S / H), 0), 0)
        Next

        dgvReorder.DataSource = dt
        dgvReorder.Columns("product_name").HeaderText = "Товар"
        dgvReorder.Columns("quantity").HeaderText = "Залишок"

        dgvReorder.Columns("середньодобовий_попит").HeaderText = "Серед. попит"
        dgvReorder.Columns("середньодобовий_попит").DefaultCellStyle.Format = "N1" ' Красиве відображення
        dgvReorder.Columns("ROP").HeaderText = "Точка замовлення"
        dgvReorder.Columns("EOQ").HeaderText = "ЕОQ"
    End Sub

    ' === Ефективність вантажівок ===
    Private Sub LoadTruckEfficiency(conn As OleDbConnection)
        Dim dt As New DataTable()
        Dim query = "SELECT t.truck_plate, " &
                    "SUM(i.loaded_quantity)/t.truck_capacity*100 AS efficiency " &
                    "FROM Incomes i INNER JOIN Trucks t ON i.truck_id = t.truck_id " &
                    "GROUP BY t.truck_plate, t.truck_capacity"

        Dim adapter As New OleDbDataAdapter(query, conn)
        adapter.Fill(dt)

        dgvTruckEfficiency.DataSource = dt
        dgvTruckEfficiency.Columns("truck_plate").HeaderText = "Номер"
        dgvTruckEfficiency.Columns("efficiency").DefaultCellStyle.Format = "N1" '
        dgvTruckEfficiency.Columns("efficiency").HeaderText = "Ефективність (%)"
    End Sub

    ' === Підсумкова статистика ===
    Private Sub LoadSummaryStats(conn As OleDbConnection)
        Dim turnover = New OleDbCommand("SELECT SUM(unloaded_quantity) / AVG(quantity) FROM Incomes i INNER JOIN Stock s ON i.product_id = s.product_id", conn).ExecuteScalar()
        lblTurnover.Text = "Оборотність: " & Math.Round(CDec(turnover), 2)

        Dim eff = New OleDbCommand("SELECT AVG(i.loaded_quantity / t.truck_capacity) * 100 FROM Incomes i INNER JOIN Trucks t ON i.truck_id = t.truck_id", conn).ExecuteScalar()
        lblAvgTruckEff.Text = "Середня ефективність: " & Math.Round(CDec(eff), 1) & " %"
    End Sub
End Class
