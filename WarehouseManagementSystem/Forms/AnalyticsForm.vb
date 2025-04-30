Imports System.Data.OleDb
Imports System.Windows.Forms
Imports System.Windows.Forms.DataVisualization.Charting

Public Class AnalyticsForm
    Inherits Form

    Private connectionString As String = MainForm.connectionString

    Private chartDisplay As Chart
    Private numAlpha, numMu, numS, numH, numDays As NumericUpDown
    Private numIncomeMin, numIncomeMax, numOutcomeMin, numOutcomeMax As NumericUpDown
    Private numStep As NumericUpDown
    Private cmbModel As ComboBox
    Private cmbProductFilter As ComboBox
    Private lblSummary As Label
    Private lblProductInfo As Label
    Private lblStats As Label

    Private lblAlpha, lblMu, lblS, lblH, lblDays As Label
    Private lblIncomeRange, lblOutcomeRange, lblStep, lblProductFilterLabel As Label

    Public Sub New()
        MyBase.New()
        Me.Text = "Аналітична панель прогнозу"
        Me.Size = New Size(1200, 850)
        Me.StartPosition = FormStartPosition.CenterScreen

        cmbModel = New ComboBox()
        cmbProductFilter = New ComboBox()
        chartDisplay = New Chart()
        numAlpha = New NumericUpDown()
        numMu = New NumericUpDown()
        numS = New NumericUpDown()
        numH = New NumericUpDown()
        numDays = New NumericUpDown()
        numIncomeMin = New NumericUpDown()
        numIncomeMax = New NumericUpDown()
        numOutcomeMin = New NumericUpDown()
        numOutcomeMax = New NumericUpDown()
        numStep = New NumericUpDown()

        lblAlpha = New Label()
        lblMu = New Label()
        lblS = New Label()
        lblH = New Label()
        lblDays = New Label()
        lblIncomeRange = New Label()
        lblOutcomeRange = New Label()
        lblStep = New Label()
        lblProductFilterLabel = New Label() With {.Text = "Продукт:", .AutoSize = True, .Font = New Font("Segoe UI", 9, FontStyle.Bold), .Margin = New Padding(5, 8, 0, 0)}

        lblSummary = New Label()
        lblProductInfo = New Label()
        lblStats = New Label()

        InitializeLayout()
        AddHandler cmbModel.SelectedIndexChanged, AddressOf OnModelChanged
        LoadAnalytics()
    End Sub

    Private Sub OnModelChanged(sender As Object, e As EventArgs)
        If cmbModel Is Nothing OrElse cmbModel.SelectedItem Is Nothing Then Return

        ' Очистка графіка і текстових полів при зміні моделі
        chartDisplay.Series.Clear()
        chartDisplay.ChartAreas.Clear()
        chartDisplay.ChartAreas.Add(New ChartArea("Default"))
        lblSummary.Text = ""
        lblProductInfo.Text = ""
        lblStats.Text = ""

        Dim model = cmbModel.SelectedItem.ToString()

        numAlpha.Visible = model.Contains("експоненційне")
        lblAlpha.Visible = numAlpha.Visible

        numMu.Visible = model.Contains("M/M/1")
        lblMu.Visible = numMu.Visible

        numS.Visible = model.Contains("EOQ")
        numH.Visible = model.Contains("EOQ")
        lblS.Visible = numS.Visible
        lblH.Visible = numH.Visible

        numIncomeMin.Visible = model.Contains("Імітаційний")
        numIncomeMax.Visible = model.Contains("Імітаційний")
        numOutcomeMin.Visible = model.Contains("Імітаційний")
        numOutcomeMax.Visible = model.Contains("Імітаційний")
        numStep.Visible = model.Contains("Імітаційний")

        lblIncomeRange.Visible = numIncomeMin.Visible
        lblOutcomeRange.Visible = numOutcomeMin.Visible
        lblStep.Visible = numStep.Visible

        numDays.Visible = model.Contains("Імітаційний")
        lblDays.Visible = numDays.Visible

        cmbProductFilter.Visible = model.Contains("Імітаційний")
        lblProductFilterLabel.Visible = cmbProductFilter.Visible

        numAlpha.Enabled = numAlpha.Visible
        numMu.Enabled = numMu.Visible
        numS.Enabled = numS.Visible
        numH.Enabled = numH.Visible
        numIncomeMin.Enabled = numIncomeMin.Visible
        numIncomeMax.Enabled = numIncomeMax.Visible
        numOutcomeMin.Enabled = numOutcomeMin.Visible
        numOutcomeMax.Enabled = numOutcomeMax.Visible
        numStep.Enabled = numStep.Visible
        numDays.Enabled = numDays.Visible

        cmbProductFilter.Enabled = cmbProductFilter.Visible

        LoadAnalytics()
    End Sub


    Private Sub InitializeLayout()
        chartDisplay = New Chart With {.Dock = DockStyle.Fill}
        chartDisplay.ChartAreas.Add(New ChartArea("Default"))

        numAlpha = CreateNumeric(0.01D, 0.99D, 0.3D, 0.01D)
        numMu = CreateNumeric(1, 9999, 10)
        numS = CreateNumeric(1, 10000, 100)
        numH = CreateNumeric(1, 10000, 10)
        numDays = CreateNumeric(1, 365, 30)
        numIncomeMin = CreateNumeric(0, 100, 10)
        numIncomeMax = CreateNumeric(0, 100, 25)
        numOutcomeMin = CreateNumeric(0, 100, 5)
        numOutcomeMax = CreateNumeric(0, 100, 20)
        numStep = CreateNumeric(1, 30, 5)

        lblAlpha = CreateLabel("α:")
        lblMu = CreateLabel("μ:")
        lblS = CreateLabel("S:")
        lblH = CreateLabel("H:")
        lblDays = CreateLabel("Днів:")
        lblIncomeRange = CreateLabel("Надходження (від/до):")
        lblOutcomeRange = CreateLabel("Витрата (від/до):")
        lblStep = CreateLabel("Крок (днів):")

        cmbModel = New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList, .Width = 300, .Font = New Font("Segoe UI", 10)}
        cmbModel.Items.AddRange({
            "Імітаційний прогноз запасів",
            "Прогноз (експоненційне згладжування)",
            "Модель масового обслуговування (M/M/1)",
            "EOQ – оптимальний розмір замовлення"
        })
        cmbModel.SelectedIndex = 0

        cmbProductFilter = New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList, .Width = 200, .Font = New Font("Segoe UI", 10)}

        lblSummary = New Label With {.Dock = DockStyle.Top, .Height = 30, .Font = New Font("Segoe UI", 10, FontStyle.Bold), .ForeColor = Color.MidnightBlue}
        lblProductInfo = New Label With {.Dock = DockStyle.Top, .Height = 25, .Font = New Font("Segoe UI", 9, FontStyle.Italic), .ForeColor = Color.DarkSlateGray}
        lblStats = New Label With {.Dock = DockStyle.Top, .Height = 35, .Font = New Font("Segoe UI", 9, FontStyle.Italic), .ForeColor = Color.DarkGreen, .Text = ""}

        AddHandler cmbModel.SelectedIndexChanged, AddressOf LoadAnalytics
        AddHandler cmbProductFilter.SelectedIndexChanged, AddressOf LoadAnalytics
        AddHandler numAlpha.ValueChanged, AddressOf LoadAnalytics
        AddHandler numMu.ValueChanged, AddressOf LoadAnalytics
        AddHandler numS.ValueChanged, AddressOf LoadAnalytics
        AddHandler numH.ValueChanged, AddressOf LoadAnalytics
        AddHandler numDays.ValueChanged, AddressOf LoadAnalytics
        AddHandler numIncomeMin.ValueChanged, AddressOf LoadAnalytics
        AddHandler numIncomeMax.ValueChanged, AddressOf LoadAnalytics
        AddHandler numOutcomeMin.ValueChanged, AddressOf LoadAnalytics
        AddHandler numOutcomeMax.ValueChanged, AddressOf LoadAnalytics
        AddHandler numStep.ValueChanged, AddressOf LoadAnalytics
        AddHandler cmbModel.SelectedIndexChanged, AddressOf OnModelChanged

        Dim layoutMain As New TableLayoutPanel With {.Dock = DockStyle.Fill, .RowCount = 5, .ColumnCount = 1}
        Dim panelTop As New FlowLayoutPanel With {.Dock = DockStyle.Top, .Padding = New Padding(10), .AutoSize = True, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = True}

        panelTop.Controls.AddRange({
            CreateLabel("Модель:"), cmbModel,
            CreateLabel("Продукт:"), cmbProductFilter,
            lblAlpha, numAlpha,
            lblMu, numMu,
            lblS, numS,
            lblH, numH,
            lblDays, numDays,
            lblIncomeRange, numIncomeMin, numIncomeMax,
            lblOutcomeRange, numOutcomeMin, numOutcomeMax,
            lblStep, numStep
        })

        layoutMain.Controls.Add(panelTop, 0, 0)
        layoutMain.Controls.Add(lblProductInfo, 0, 1)
        layoutMain.Controls.Add(lblSummary, 0, 2)
        layoutMain.Controls.Add(lblStats, 0, 3)
        layoutMain.Controls.Add(chartDisplay, 0, 4)

        Me.Controls.Add(layoutMain)
    End Sub

    Private Function CreateNumeric(min As Decimal, max As Decimal, val As Decimal, Optional inc As Decimal = 1) As NumericUpDown
        Return New NumericUpDown With {
            .Minimum = min,
            .Maximum = max,
            .Value = val,
            .DecimalPlaces = If(inc < 1, 2, 0),
            .Increment = inc,
            .Width = 60,
            .Font = New Font("Segoe UI", 9)
        }
    End Function

    Private Function CreateLabel(text As String) As Label
        Return New Label With {
            .Text = text,
            .AutoSize = True,
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .Margin = New Padding(5, 8, 0, 0)
        }
    End Function

    Private Sub LoadAnalytics()
        chartDisplay.Series.Clear()
        lblSummary.Text = ""
        lblProductInfo.Text = ""

        Using conn As New OleDbConnection(connectionString)
            conn.Open()

            If cmbProductFilter.Items.Count = 0 Then
                Dim cmdFill As New OleDbCommand("SELECT DISTINCT Products.product_name FROM Products INNER JOIN Incomes ON Products.product_id = Incomes.product_id", conn)
                Dim readerFill = cmdFill.ExecuteReader()
                While readerFill.Read()
                    cmbProductFilter.Items.Add(readerFill("product_name").ToString())
                End While
                readerFill.Close()
                If cmbProductFilter.Items.Count > 0 Then cmbProductFilter.SelectedIndex = 0
            End If

            Select Case cmbModel.SelectedItem.ToString()
                Case "Імітаційний прогноз запасів"
                    RunInventoryForecast(conn)
                Case "Прогноз (експоненційне згладжування)"
                    RunExponentialForecast(conn)
                Case "Модель масового обслуговування (M/M/1)"
                    RunQueueingModel(conn)
                Case "EOQ – оптимальний розмір замовлення"
                    RunEOQModel(conn)
            End Select
        End Using
    End Sub

    Private Sub RunExponentialForecast(conn As OleDbConnection)
        Dim area = chartDisplay.ChartAreas("Default")
        area.AxisX.LabelStyle.Format = "dd.MM.yyyy"
        area.AxisX.IntervalType = DateTimeIntervalType.Days
        area.AxisX.LabelStyle.Angle = -45
        area.AxisX.MajorGrid.LineColor = Color.LightGray
        area.AxisY.MajorGrid.LineColor = Color.LightGray
        area.AxisX.Title = "Дата"
        area.AxisY.Title = "Кількість"
        area.AxisX.LabelStyle.IsEndLabelVisible = True
        area.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount

        area.AxisX.IsLabelAutoFit = True
        area.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont Or LabelAutoFitStyles.LabelsAngleStep30

        Dim seriesActual As New Series("Фактичний попит") With {.ChartType = SeriesChartType.Line, .Color = Color.Blue, .XValueType = ChartValueType.DateTime}
        Dim seriesForecast As New Series("Прогноз") With {.ChartType = SeriesChartType.Line, .BorderDashStyle = ChartDashStyle.Dash, .Color = Color.Red, .XValueType = ChartValueType.DateTime}

        Dim dt As New DataTable()
        Dim adapter = New OleDbDataAdapter("SELECT arrival_time, unloaded_quantity FROM Incomes WHERE arrival_time IS NOT NULL ORDER BY arrival_time", conn)
        adapter.Fill(dt)

        Dim times = dt.AsEnumerable().Select(Function(r) CDate(r("arrival_time"))).ToList()
        Dim vals = dt.AsEnumerable().Select(Function(r) CDbl(r("unloaded_quantity"))).ToList()

        Dim alpha = CDbl(numAlpha.Value)
        Dim forecasts As New List(Of Double)
        If vals.Count > 0 Then
            forecasts.Add(vals(0))
            For i = 1 To vals.Count - 1
                forecasts.Add(alpha * vals(i - 1) + (1 - alpha) * forecasts(i - 1))
            Next
        End If

        For i = 0 To vals.Count - 1
            seriesActual.Points.AddXY(times(i), vals(i))
            If i > 0 Then seriesForecast.Points.AddXY(times(i), forecasts(i))
        Next

        chartDisplay.Series.Add(seriesActual)
        chartDisplay.Series.Add(seriesForecast)
    End Sub

    Private Sub RunQueueingModel(conn As OleDbConnection)
        chartDisplay.Series.Clear()
        Dim lambda = CDbl(New OleDbCommand("SELECT AVG(unloaded_quantity) FROM Incomes", conn).ExecuteScalar())
        Dim mu = CDbl(numMu.Value)

        If lambda <= 0 Or mu <= 0 Then
            lblSummary.Text = "Невірні вхідні дані"
            lblStats.Text = ""
            Return
        End If

        Dim rho = lambda / mu

        ' Перевірка стабільності системи
        If rho >= 1 Then
            lblSummary.Text = $"⚠ Система нестабільна: λ = {Math.Round(lambda, 2)} ≥ μ = {mu}. Збільшіть μ або зменшіть навантаження."
            lblStats.Text = ""
            Return
        End If

        ' Основні показники
        Dim L = rho / (1 - rho)        ' середня кількість заявок у системі
        Dim W = 1 / (mu - lambda)      ' середній час перебування заявки в системі
        Dim Lq = (rho ^ 2) / (1 - rho) ' середня кількість в черзі
        Dim Wq = rho / (mu - lambda)   ' середній час у черзі

        lblSummary.Text = $"λ = {Math.Round(lambda, 2)}, μ = {mu}, ρ = {Math.Round(rho, 2)}"
        lblStats.Text = $"L = {Math.Round(L, 2)} | W = {Math.Round(W, 2)} | Lq = {Math.Round(Lq, 2)} | Wq = {Math.Round(Wq, 2)}"

        ' Графік L від ρ
        Dim area = chartDisplay.ChartAreas("Default")
        area.AxisX.Title = "ρ (Рівень завантаження системи)"
        area.AxisY.Title = "L (Сер. кількість заявок у системі)"
        area.AxisX.MajorGrid.LineColor = Color.LightGray
        area.AxisY.MajorGrid.LineColor = Color.LightGray
        area.AxisX.Minimum = 0
        area.AxisX.Maximum = 1
        area.AxisY.Minimum = 0
        area.AxisY.Maximum = 20

        Dim seriesL As New Series("L = ρ / (1 - ρ)") With {
        .ChartType = SeriesChartType.Line,
        .Color = Color.Orange,
        .BorderWidth = 2
    }

        For rhoTest As Double = 0.01 To 0.99 Step 0.01
            Dim Lval = rhoTest / (1 - rhoTest)
            seriesL.Points.AddXY(rhoTest, Lval)
        Next

        chartDisplay.Series.Add(seriesL)

        ' Відмітка поточного ρ на графіку
        Dim seriesPoint As New Series("Поточний ρ") With {
        .ChartType = SeriesChartType.Point,
        .MarkerStyle = MarkerStyle.Cross,
        .MarkerSize = 10,
        .Color = Color.Red
    }
        seriesPoint.Points.AddXY(rho, L)
        chartDisplay.Series.Add(seriesPoint)
    End Sub



    Private Sub RunEOQModel(conn As OleDbConnection)
        Dim D = CDbl(New OleDbCommand("SELECT SUM(unloaded_quantity) FROM Incomes", conn).ExecuteScalar())
        Dim S = CDbl(numS.Value)
        Dim H = CDbl(numH.Value)
        Dim EOQ = Math.Sqrt((2 * D * S) / H)
        lblSummary.Text = $"EOQ = √(2×{D}×{S}/{H}) = {Math.Round(EOQ, 2)} одиниць"
    End Sub

    Private Sub RunInventoryForecast(conn As OleDbConnection)
        Dim rand As New Random()
        Dim days As Integer = CInt(numDays.Value)
        Dim selectedProduct As String = cmbProductFilter.SelectedItem?.ToString()

        Dim area = chartDisplay.ChartAreas("Default")
        area.AxisX.Title = "День"
        area.AxisY.Title = "Запаси"
        area.AxisX.MajorGrid.LineColor = Color.LightGray
        area.AxisY.MajorGrid.LineColor = Color.LightGray
        area.AxisX.LabelStyle.Angle = 0
        area.AxisX.LabelStyle.Format = ""
        area.AxisX.IntervalType = DateTimeIntervalType.Number

        Dim cmd As New OleDbCommand("SELECT DISTINCT Products.product_name FROM Products INNER JOIN Incomes ON Products.product_id = Incomes.product_id", conn)
        Dim reader = cmd.ExecuteReader()
        If Not reader.HasRows Then
            lblProductInfo.Text = "Немає жодного продукту у таблиці Incomes."
            Return
        End If

        While reader.Read()
            Dim productName As String = reader("product_name").ToString()
            If Not String.IsNullOrEmpty(selectedProduct) AndAlso productName <> selectedProduct Then Continue While

            Dim stock As Integer = 100
            Dim series As New Series(productName) With {.ChartType = SeriesChartType.Line, .XValueType = ChartValueType.Int32}
            Dim stepSize As Integer = CInt(numStep.Value)

            For day As Integer = 1 To days Step stepSize
                Dim income = rand.Next(CInt(numIncomeMin.Value), CInt(numIncomeMax.Value) + 1)
                Dim outcome = rand.Next(CInt(numOutcomeMin.Value), CInt(numOutcomeMax.Value) + 1)
                stock += income - outcome
                If stock < 0 Then stock = 0
                series.Points.AddXY(day, stock)
            Next

            If Not chartDisplay.Series.IsUniqueName(series.Name) Then Continue While
            chartDisplay.Series.Add(series)
        End While
        reader.Close()

        lblProductInfo.Text = $"Прогноз запасів для {chartDisplay.Series.Count} продуктів на {days} днів"
        ' Після побудови графіка — аналізуємо точки на адекватність
        Dim minStock As Integer = Integer.MaxValue
        Dim maxStock As Integer = Integer.MinValue
        Dim stablePoints As Integer = 0
        Dim prevValue As Integer? = Nothing

        For Each pt In chartDisplay.Series(0).Points
            Dim yVal = CInt(pt.YValues(0))

            If yVal < minStock Then minStock = yVal
            If yVal > maxStock Then maxStock = yVal
            If prevValue.HasValue AndAlso yVal = prevValue.Value Then
                stablePoints += 1
            End If
            prevValue = yVal
        Next

        Dim resultSummary As String = ""

        If minStock < 0 Then
            resultSummary &= "Увага: Виявлено від’ємні значення запасів. Модель неадекватна. " & vbCrLf
        End If

        If maxStock > 1000 Then
            resultSummary &= "Попередження: Запаси неконтрольовано накопичуються (більше 1000 одиниць). " & vbCrLf
        End If

        If stablePoints > days * 0.5 Then
            resultSummary &= "Попередження: Значна частина прогнозу має незмінне значення. Модель надто згладжена. " & vbCrLf
        End If

        If resultSummary = "" Then
            resultSummary = "Модель пройшла перевірку на адекватність. Поведінка запасів відповідає очікуваному сценарію."
        End If

        lblSummary.Text = resultSummary

        ' Розрахунок середнього та стандартного відхилення
        Dim values As New List(Of Integer)
        For Each pt In chartDisplay.Series(0).Points
            values.Add(CInt(pt.YValues(0)))
        Next

        Dim avg = values.Average()
        Dim stdDev = Math.Sqrt(values.Select(Function(v) Math.Pow(v - avg, 2)).Average())

        lblStats.Text = $"Середній рівень запасів: {Math.Round(avg, 2)} | Стандартне відхилення: {Math.Round(stdDev, 2)}"


    End Sub

    Private Sub PlotQueueingCurve()
        Dim seriesCurve As New Series("L(ρ)") With {
        .ChartType = SeriesChartType.Line,
        .Color = Color.DarkOrange,
        .BorderWidth = 2
    }

        Dim area = chartDisplay.ChartAreas("Default")
        area.AxisX.Title = "Навантаження ρ (λ / μ)"
        area.AxisY.Title = "Середня кількість заявок у системі (L)"
        area.AxisX.MajorGrid.LineColor = Color.LightGray
        area.AxisY.MajorGrid.LineColor = Color.LightGray
        area.AxisX.Minimum = 0
        area.AxisX.Maximum = 0.95
        area.AxisX.Interval = 0.1
        area.AxisY.Minimum = 0
        area.AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount

        For rho = 0.01 To 0.95 Step 0.01
            Dim L = rho / (1 - rho)
            seriesCurve.Points.AddXY(Math.Round(rho, 2), Math.Round(L, 2))
        Next

        chartDisplay.Series.Add(seriesCurve)
    End Sub

End Class
