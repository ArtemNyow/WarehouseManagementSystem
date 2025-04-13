<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
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
        Me.dgvIncomes = New System.Windows.Forms.DataGridView()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.btnEdit = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.dgvStock = New System.Windows.Forms.DataGridView()
        CType(Me.dgvIncomes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FlowLayoutPanel1.SuspendLayout()
        CType(Me.dgvStock, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvIncomes
        '
        Me.dgvIncomes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvIncomes.Dock = System.Windows.Forms.DockStyle.Top
        Me.dgvIncomes.Location = New System.Drawing.Point(0, 58)
        Me.dgvIncomes.Name = "dgvIncomes"
        Me.dgvIncomes.Size = New System.Drawing.Size(852, 244)
        Me.dgvIncomes.TabIndex = 0
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(3, 3)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(178, 52)
        Me.btnAdd.TabIndex = 1
        Me.btnAdd.Text = "Додати "
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'btnEdit
        '
        Me.btnEdit.Location = New System.Drawing.Point(187, 3)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(189, 52)
        Me.btnEdit.TabIndex = 2
        Me.btnEdit.Text = "Редагувати "
        Me.btnEdit.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.Location = New System.Drawing.Point(382, 3)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(195, 52)
        Me.btnDelete.TabIndex = 3
        Me.btnDelete.Text = "Видалити "
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.Controls.Add(Me.btnAdd)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnEdit)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnDelete)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnRefresh)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(852, 58)
        Me.FlowLayoutPanel1.TabIndex = 4
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(583, 3)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(178, 52)
        Me.btnRefresh.TabIndex = 4
        Me.btnRefresh.Text = "Оновити "
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'dgvStock
        '
        Me.dgvStock.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.dgvStock.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvStock.Dock = System.Windows.Forms.DockStyle.Top
        Me.dgvStock.Location = New System.Drawing.Point(0, 302)
        Me.dgvStock.Name = "dgvStock"
        Me.dgvStock.Size = New System.Drawing.Size(852, 150)
        Me.dgvStock.TabIndex = 5
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(852, 610)
        Me.Controls.Add(Me.dgvStock)
        Me.Controls.Add(Me.dgvIncomes)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Name = "MainForm"
        Me.Text = "Warehouse Management System"
        CType(Me.dgvIncomes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        CType(Me.dgvStock, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgvIncomes As DataGridView
    Friend WithEvents btnAdd As Button
    Friend WithEvents btnEdit As Button
    Friend WithEvents btnDelete As Button
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents dgvStock As DataGridView
    Friend WithEvents btnRefresh As Button
End Class
