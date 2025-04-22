<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AddForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AddForm))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.cmbSuppliers = New System.Windows.Forms.ComboBox()
        Me.cmbProducts = New System.Windows.Forms.ComboBox()
        Me.cmbTrucks = New System.Windows.Forms.ComboBox()
        Me.dtpArrival = New System.Windows.Forms.DateTimePicker()
        Me.dtpDeparture = New System.Windows.Forms.DateTimePicker()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.txtUnloaded = New System.Windows.Forms.TextBox()
        Me.txtLoaded = New System.Windows.Forms.TextBox()
        Me.lblTruckCapacity = New System.Windows.Forms.Label()
        Me.cmbWarehouses = New System.Windows.Forms.ComboBox()
        Me.lblCurrentStock = New System.Windows.Forms.Label()
        Me.rbLoad = New System.Windows.Forms.RadioButton()
        Me.rbUnload = New System.Windows.Forms.RadioButton()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.picProduct = New System.Windows.Forms.PictureBox()
        Me.picTruck = New System.Windows.Forms.PictureBox()
        Me.Panel1.SuspendLayout()
        CType(Me.picProduct, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picTruck, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(31, 92)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(82, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Постачальник:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(31, 125)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(41, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Товар:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(31, 254)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(68, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Вантажівка:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(31, 392)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(41, 13)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Склад:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(31, 22)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(81, 13)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Час прибуття: "
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(31, 54)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(74, 13)
        Me.Label6.TabIndex = 5
        Me.Label6.Text = "Час відбуття:"
        '
        'cmbSuppliers
        '
        Me.cmbSuppliers.FormattingEnabled = True
        Me.cmbSuppliers.Location = New System.Drawing.Point(118, 92)
        Me.cmbSuppliers.Name = "cmbSuppliers"
        Me.cmbSuppliers.Size = New System.Drawing.Size(227, 21)
        Me.cmbSuppliers.TabIndex = 8
        '
        'cmbProducts
        '
        Me.cmbProducts.FormattingEnabled = True
        Me.cmbProducts.Location = New System.Drawing.Point(118, 125)
        Me.cmbProducts.Name = "cmbProducts"
        Me.cmbProducts.Size = New System.Drawing.Size(227, 21)
        Me.cmbProducts.TabIndex = 9
        '
        'cmbTrucks
        '
        Me.cmbTrucks.FormattingEnabled = True
        Me.cmbTrucks.Location = New System.Drawing.Point(118, 251)
        Me.cmbTrucks.Name = "cmbTrucks"
        Me.cmbTrucks.Size = New System.Drawing.Size(227, 21)
        Me.cmbTrucks.TabIndex = 10
        '
        'dtpArrival
        '
        Me.dtpArrival.CustomFormat = """Час прибуття: ""dd.MM.yyyy HH:mm"
        Me.dtpArrival.Location = New System.Drawing.Point(118, 22)
        Me.dtpArrival.Name = "dtpArrival"
        Me.dtpArrival.Size = New System.Drawing.Size(227, 20)
        Me.dtpArrival.TabIndex = 12
        '
        'dtpDeparture
        '
        Me.dtpDeparture.Location = New System.Drawing.Point(118, 54)
        Me.dtpDeparture.Name = "dtpDeparture"
        Me.dtpDeparture.Size = New System.Drawing.Size(227, 20)
        Me.dtpDeparture.TabIndex = 13
        Me.dtpDeparture.Value = New Date(2025, 4, 14, 19, 10, 0, 0)
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(12, 522)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(187, 63)
        Me.btnSave.TabIndex = 16
        Me.btnSave.Text = "Зберегти "
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(205, 522)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(183, 63)
        Me.btnCancel.TabIndex = 17
        Me.btnCancel.Text = "Скасувати"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'txtUnloaded
        '
        Me.txtUnloaded.Location = New System.Drawing.Point(151, 484)
        Me.txtUnloaded.Name = "txtUnloaded"
        Me.txtUnloaded.Size = New System.Drawing.Size(80, 20)
        Me.txtUnloaded.TabIndex = 18
        '
        'txtLoaded
        '
        Me.txtLoaded.Location = New System.Drawing.Point(151, 450)
        Me.txtLoaded.Name = "txtLoaded"
        Me.txtLoaded.Size = New System.Drawing.Size(80, 20)
        Me.txtLoaded.TabIndex = 19
        '
        'lblTruckCapacity
        '
        Me.lblTruckCapacity.AutoSize = True
        Me.lblTruckCapacity.Location = New System.Drawing.Point(237, 487)
        Me.lblTruckCapacity.Name = "lblTruckCapacity"
        Me.lblTruckCapacity.Size = New System.Drawing.Size(186, 13)
        Me.lblTruckCapacity.TabIndex = 20
        Me.lblTruckCapacity.Text = "Поточна кількість: {currentStock} кг"
        '
        'cmbWarehouses
        '
        Me.cmbWarehouses.FormattingEnabled = True
        Me.cmbWarehouses.Location = New System.Drawing.Point(118, 389)
        Me.cmbWarehouses.Name = "cmbWarehouses"
        Me.cmbWarehouses.Size = New System.Drawing.Size(227, 21)
        Me.cmbWarehouses.TabIndex = 21
        '
        'lblCurrentStock
        '
        Me.lblCurrentStock.AutoSize = True
        Me.lblCurrentStock.Location = New System.Drawing.Point(237, 453)
        Me.lblCurrentStock.Name = "lblCurrentStock"
        Me.lblCurrentStock.Size = New System.Drawing.Size(186, 13)
        Me.lblCurrentStock.TabIndex = 22
        Me.lblCurrentStock.Text = "Поточна кількість: {currentStock} кг"
        '
        'rbLoad
        '
        Me.rbLoad.AutoSize = True
        Me.rbLoad.Checked = True
        Me.rbLoad.Location = New System.Drawing.Point(3, 4)
        Me.rbLoad.Margin = New System.Windows.Forms.Padding(0)
        Me.rbLoad.Name = "rbLoad"
        Me.rbLoad.Size = New System.Drawing.Size(116, 17)
        Me.rbLoad.TabIndex = 23
        Me.rbLoad.TabStop = True
        Me.rbLoad.Text = "Завантажено (кг):"
        Me.rbLoad.UseVisualStyleBackColor = True
        '
        'rbUnload
        '
        Me.rbUnload.AutoSize = True
        Me.rbUnload.Location = New System.Drawing.Point(3, 38)
        Me.rbUnload.Margin = New System.Windows.Forms.Padding(0)
        Me.rbUnload.Name = "rbUnload"
        Me.rbUnload.Size = New System.Drawing.Size(116, 17)
        Me.rbUnload.TabIndex = 24
        Me.rbUnload.Text = "Вивантажено (кг):"
        Me.rbUnload.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.rbLoad)
        Me.Panel1.Controls.Add(Me.rbUnload)
        Me.Panel1.Location = New System.Drawing.Point(22, 446)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(119, 58)
        Me.Panel1.TabIndex = 26
        '
        'picProduct
        '
        Me.picProduct.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picProduct.Location = New System.Drawing.Point(118, 152)
        Me.picProduct.Name = "picProduct"
        Me.picProduct.Size = New System.Drawing.Size(227, 93)
        Me.picProduct.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picProduct.TabIndex = 27
        Me.picProduct.TabStop = False
        '
        'picTruck
        '
        Me.picTruck.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picTruck.Location = New System.Drawing.Point(118, 279)
        Me.picTruck.Name = "picTruck"
        Me.picTruck.Size = New System.Drawing.Size(227, 92)
        Me.picTruck.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picTruck.TabIndex = 28
        Me.picTruck.TabStop = False
        '
        'AddForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(400, 597)
        Me.Controls.Add(Me.picTruck)
        Me.Controls.Add(Me.picProduct)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.lblCurrentStock)
        Me.Controls.Add(Me.cmbWarehouses)
        Me.Controls.Add(Me.lblTruckCapacity)
        Me.Controls.Add(Me.txtLoaded)
        Me.Controls.Add(Me.txtUnloaded)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.dtpDeparture)
        Me.Controls.Add(Me.dtpArrival)
        Me.Controls.Add(Me.cmbTrucks)
        Me.Controls.Add(Me.cmbProducts)
        Me.Controls.Add(Me.cmbSuppliers)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "AddForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Нове надхоження"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.picProduct, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picTruck, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents cmbSuppliers As ComboBox
    Friend WithEvents cmbProducts As ComboBox
    Friend WithEvents cmbTrucks As ComboBox
    Friend WithEvents dtpArrival As DateTimePicker
    Friend WithEvents dtpDeparture As DateTimePicker
    Friend WithEvents btnSave As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents txtUnloaded As TextBox
    Friend WithEvents txtLoaded As TextBox
    Friend WithEvents lblTruckCapacity As Label
    Friend WithEvents cmbWarehouses As ComboBox
    Friend WithEvents lblCurrentStock As Label
    Friend WithEvents rbLoad As RadioButton
    Friend WithEvents rbUnload As RadioButton
    Friend WithEvents Panel1 As Panel
    Friend WithEvents picProduct As PictureBox
    Friend WithEvents picTruck As PictureBox
End Class
