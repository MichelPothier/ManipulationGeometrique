<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dckMenuUniteTravail
    Inherits System.Windows.Forms.UserControl

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.tblBarreUniteTravail = New System.Windows.Forms.TabControl()
        Me.pgeInformation = New System.Windows.Forms.TabPage()
        Me.cboNumero = New System.Windows.Forms.ComboBox()
        Me.lblDescription = New System.Windows.Forms.Label()
        Me.lblUniteTravail = New System.Windows.Forms.Label()
        Me.rtxDescription = New System.Windows.Forms.RichTextBox()
        Me.pgeParametres = New System.Windows.Forms.TabPage()
        Me.chkGeometriePrecise = New System.Windows.Forms.CheckBox()
        Me.btnStyle = New System.Windows.Forms.Button()
        Me.chkZoom = New System.Windows.Forms.CheckBox()
        Me.chkDessiner = New System.Windows.Forms.CheckBox()
        Me.btnInitialiser = New System.Windows.Forms.Button()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.chkPrecision = New System.Windows.Forms.CheckBox()
        Me.tblBarreUniteTravail.SuspendLayout()
        Me.pgeInformation.SuspendLayout()
        Me.pgeParametres.SuspendLayout()
        Me.SuspendLayout()
        '
        'tblBarreUniteTravail
        '
        Me.tblBarreUniteTravail.Controls.Add(Me.pgeInformation)
        Me.tblBarreUniteTravail.Controls.Add(Me.pgeParametres)
        Me.tblBarreUniteTravail.Location = New System.Drawing.Point(3, 0)
        Me.tblBarreUniteTravail.Name = "tblBarreUniteTravail"
        Me.tblBarreUniteTravail.SelectedIndex = 0
        Me.tblBarreUniteTravail.Size = New System.Drawing.Size(297, 267)
        Me.tblBarreUniteTravail.TabIndex = 0
        '
        'pgeInformation
        '
        Me.pgeInformation.Controls.Add(Me.cboNumero)
        Me.pgeInformation.Controls.Add(Me.lblDescription)
        Me.pgeInformation.Controls.Add(Me.lblUniteTravail)
        Me.pgeInformation.Controls.Add(Me.rtxDescription)
        Me.pgeInformation.Location = New System.Drawing.Point(4, 22)
        Me.pgeInformation.Name = "pgeInformation"
        Me.pgeInformation.Padding = New System.Windows.Forms.Padding(3)
        Me.pgeInformation.Size = New System.Drawing.Size(289, 241)
        Me.pgeInformation.TabIndex = 0
        Me.pgeInformation.Text = "Information"
        Me.pgeInformation.UseVisualStyleBackColor = True
        '
        'cboNumero
        '
        Me.cboNumero.FormattingEnabled = True
        Me.cboNumero.Location = New System.Drawing.Point(6, 25)
        Me.cboNumero.Name = "cboNumero"
        Me.cboNumero.Size = New System.Drawing.Size(277, 21)
        Me.cboNumero.TabIndex = 4
        '
        'lblDescription
        '
        Me.lblDescription.AutoSize = True
        Me.lblDescription.Location = New System.Drawing.Point(3, 53)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(151, 13)
        Me.lblDescription.TabIndex = 3
        Me.lblDescription.Text = "Description de l'unité de travail"
        '
        'lblUniteTravail
        '
        Me.lblUniteTravail.AutoSize = True
        Me.lblUniteTravail.Location = New System.Drawing.Point(3, 8)
        Me.lblUniteTravail.Name = "lblUniteTravail"
        Me.lblUniteTravail.Size = New System.Drawing.Size(135, 13)
        Me.lblUniteTravail.TabIndex = 2
        Me.lblUniteTravail.Text = "Numéro de l'unité de travail"
        '
        'rtxDescription
        '
        Me.rtxDescription.Location = New System.Drawing.Point(6, 70)
        Me.rtxDescription.Name = "rtxDescription"
        Me.rtxDescription.Size = New System.Drawing.Size(277, 165)
        Me.rtxDescription.TabIndex = 1
        Me.rtxDescription.Text = ""
        '
        'pgeParametres
        '
        Me.pgeParametres.Controls.Add(Me.chkGeometriePrecise)
        Me.pgeParametres.Controls.Add(Me.btnStyle)
        Me.pgeParametres.Controls.Add(Me.chkZoom)
        Me.pgeParametres.Controls.Add(Me.chkDessiner)
        Me.pgeParametres.Location = New System.Drawing.Point(4, 22)
        Me.pgeParametres.Name = "pgeParametres"
        Me.pgeParametres.Padding = New System.Windows.Forms.Padding(3)
        Me.pgeParametres.Size = New System.Drawing.Size(289, 241)
        Me.pgeParametres.TabIndex = 1
        Me.pgeParametres.Text = "Paramètres"
        Me.pgeParametres.UseVisualStyleBackColor = True
        '
        'chkGeometriePrecise
        '
        Me.chkGeometriePrecise.AutoSize = True
        Me.chkGeometriePrecise.Location = New System.Drawing.Point(10, 81)
        Me.chkGeometriePrecise.Name = "chkGeometriePrecise"
        Me.chkGeometriePrecise.Size = New System.Drawing.Size(133, 17)
        Me.chkGeometriePrecise.TabIndex = 4
        Me.chkGeometriePrecise.Text = "Géométrie plus précise"
        Me.chkGeometriePrecise.UseVisualStyleBackColor = True
        '
        'btnStyle
        '
        Me.btnStyle.Location = New System.Drawing.Point(10, 114)
        Me.btnStyle.Name = "btnStyle"
        Me.btnStyle.Size = New System.Drawing.Size(144, 24)
        Me.btnStyle.TabIndex = 3
        Me.btnStyle.Text = "Style de la géométrie"
        Me.btnStyle.UseVisualStyleBackColor = True
        '
        'chkZoom
        '
        Me.chkZoom.AutoSize = True
        Me.chkZoom.Location = New System.Drawing.Point(10, 46)
        Me.chkZoom.Name = "chkZoom"
        Me.chkZoom.Size = New System.Drawing.Size(144, 17)
        Me.chkZoom.TabIndex = 1
        Me.chkZoom.Text = "Zoom selon la géométrie "
        Me.chkZoom.UseVisualStyleBackColor = True
        '
        'chkDessiner
        '
        Me.chkDessiner.AutoSize = True
        Me.chkDessiner.Location = New System.Drawing.Point(10, 12)
        Me.chkDessiner.Name = "chkDessiner"
        Me.chkDessiner.Size = New System.Drawing.Size(143, 17)
        Me.chkDessiner.TabIndex = 0
        Me.chkDessiner.Text = "Dessiner l'unité de travail"
        Me.chkDessiner.UseVisualStyleBackColor = True
        '
        'btnInitialiser
        '
        Me.btnInitialiser.Location = New System.Drawing.Point(3, 273)
        Me.btnInitialiser.Name = "btnInitialiser"
        Me.btnInitialiser.Size = New System.Drawing.Size(75, 24)
        Me.btnInitialiser.TabIndex = 1
        Me.btnInitialiser.Text = "Initialiser"
        Me.btnInitialiser.UseVisualStyleBackColor = True
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(14, 28)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(182, 21)
        Me.ComboBox1.TabIndex = 0
        '
        'chkPrecision
        '
        Me.chkPrecision.AutoSize = True
        Me.chkPrecision.Location = New System.Drawing.Point(17, 101)
        Me.chkPrecision.Name = "chkPrecision"
        Me.chkPrecision.Size = New System.Drawing.Size(133, 17)
        Me.chkPrecision.TabIndex = 2
        Me.chkPrecision.Text = "Géométrie plus précise"
        Me.chkPrecision.UseVisualStyleBackColor = True
        '
        'dckMenuUniteTravail
        '
        Me.Controls.Add(Me.btnInitialiser)
        Me.Controls.Add(Me.tblBarreUniteTravail)
        Me.Name = "dckMenuUniteTravail"
        Me.Size = New System.Drawing.Size(300, 300)
        Me.tblBarreUniteTravail.ResumeLayout(False)
        Me.pgeInformation.ResumeLayout(False)
        Me.pgeInformation.PerformLayout()
        Me.pgeParametres.ResumeLayout(False)
        Me.pgeParametres.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tblBarreUniteTravail As System.Windows.Forms.TabControl
    Friend WithEvents pgeInformation As System.Windows.Forms.TabPage
    Friend WithEvents pgeParametres As System.Windows.Forms.TabPage
    Friend WithEvents lblDescription As System.Windows.Forms.Label
    Friend WithEvents lblUniteTravail As System.Windows.Forms.Label
    Friend WithEvents rtxDescription As System.Windows.Forms.RichTextBox
    Friend WithEvents btnInitialiser As System.Windows.Forms.Button
    Friend WithEvents btnStyle As System.Windows.Forms.Button
    Friend WithEvents chkZoom As System.Windows.Forms.CheckBox
    Friend WithEvents chkDessiner As System.Windows.Forms.CheckBox
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Friend WithEvents chkPrecision As System.Windows.Forms.CheckBox
    Friend WithEvents chkGeometriePrecise As System.Windows.Forms.CheckBox
    Friend WithEvents cboNumero As System.Windows.Forms.ComboBox

End Class
