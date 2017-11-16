<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dckMenuGeometrieTravail
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
        Me.tabGeometrieTravail = New System.Windows.Forms.TabControl()
        Me.pgeInformation = New System.Windows.Forms.TabPage()
        Me.rtxInformation = New System.Windows.Forms.RichTextBox()
        Me.pgeGeometrie = New System.Windows.Forms.TabPage()
        Me.treGeometrie = New System.Windows.Forms.TreeView()
        Me.pgeParametres = New System.Windows.Forms.TabPage()
        Me.lblStyle = New System.Windows.Forms.Label()
        Me.btnTexte = New System.Windows.Forms.Button()
        Me.btnSommet = New System.Windows.Forms.Button()
        Me.btnSurface = New System.Windows.Forms.Button()
        Me.btnLigne = New System.Windows.Forms.Button()
        Me.btnPoint = New System.Windows.Forms.Button()
        Me.chkZoomGeometrie = New System.Windows.Forms.CheckBox()
        Me.chkDessinerSommet = New System.Windows.Forms.CheckBox()
        Me.chkDessinerGeometrie = New System.Windows.Forms.CheckBox()
        Me.chkDessinerNumero = New System.Windows.Forms.CheckBox()
        Me.btnInitialiser = New System.Windows.Forms.Button()
        Me.tabGeometrieTravail.SuspendLayout()
        Me.pgeInformation.SuspendLayout()
        Me.pgeGeometrie.SuspendLayout()
        Me.pgeParametres.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabGeometrieTravail
        '
        Me.tabGeometrieTravail.Controls.Add(Me.pgeInformation)
        Me.tabGeometrieTravail.Controls.Add(Me.pgeGeometrie)
        Me.tabGeometrieTravail.Controls.Add(Me.pgeParametres)
        Me.tabGeometrieTravail.Location = New System.Drawing.Point(0, 0)
        Me.tabGeometrieTravail.Name = "tabGeometrieTravail"
        Me.tabGeometrieTravail.SelectedIndex = 0
        Me.tabGeometrieTravail.Size = New System.Drawing.Size(300, 261)
        Me.tabGeometrieTravail.TabIndex = 0
        '
        'pgeInformation
        '
        Me.pgeInformation.Controls.Add(Me.rtxInformation)
        Me.pgeInformation.Location = New System.Drawing.Point(4, 22)
        Me.pgeInformation.Name = "pgeInformation"
        Me.pgeInformation.Padding = New System.Windows.Forms.Padding(3)
        Me.pgeInformation.Size = New System.Drawing.Size(292, 235)
        Me.pgeInformation.TabIndex = 0
        Me.pgeInformation.Text = "Information"
        Me.pgeInformation.UseVisualStyleBackColor = True
        '
        'rtxInformation
        '
        Me.rtxInformation.Location = New System.Drawing.Point(3, 3)
        Me.rtxInformation.Name = "rtxInformation"
        Me.rtxInformation.Size = New System.Drawing.Size(286, 223)
        Me.rtxInformation.TabIndex = 0
        Me.rtxInformation.Text = ""
        '
        'pgeGeometrie
        '
        Me.pgeGeometrie.Controls.Add(Me.treGeometrie)
        Me.pgeGeometrie.Location = New System.Drawing.Point(4, 22)
        Me.pgeGeometrie.Name = "pgeGeometrie"
        Me.pgeGeometrie.Size = New System.Drawing.Size(292, 229)
        Me.pgeGeometrie.TabIndex = 2
        Me.pgeGeometrie.Text = "Géometrie"
        Me.pgeGeometrie.UseVisualStyleBackColor = True
        '
        'treGeometrie
        '
        Me.treGeometrie.Dock = System.Windows.Forms.DockStyle.Fill
        Me.treGeometrie.Location = New System.Drawing.Point(0, 0)
        Me.treGeometrie.Name = "treGeometrie"
        Me.treGeometrie.Size = New System.Drawing.Size(292, 229)
        Me.treGeometrie.TabIndex = 0
        '
        'pgeParametres
        '
        Me.pgeParametres.Controls.Add(Me.lblStyle)
        Me.pgeParametres.Controls.Add(Me.btnTexte)
        Me.pgeParametres.Controls.Add(Me.btnSommet)
        Me.pgeParametres.Controls.Add(Me.btnSurface)
        Me.pgeParametres.Controls.Add(Me.btnLigne)
        Me.pgeParametres.Controls.Add(Me.btnPoint)
        Me.pgeParametres.Controls.Add(Me.chkZoomGeometrie)
        Me.pgeParametres.Controls.Add(Me.chkDessinerSommet)
        Me.pgeParametres.Controls.Add(Me.chkDessinerGeometrie)
        Me.pgeParametres.Controls.Add(Me.chkDessinerNumero)
        Me.pgeParametres.Location = New System.Drawing.Point(4, 22)
        Me.pgeParametres.Name = "pgeParametres"
        Me.pgeParametres.Padding = New System.Windows.Forms.Padding(3)
        Me.pgeParametres.Size = New System.Drawing.Size(292, 229)
        Me.pgeParametres.TabIndex = 1
        Me.pgeParametres.Text = "Paramètres"
        Me.pgeParametres.UseVisualStyleBackColor = True
        '
        'lblStyle
        '
        Me.lblStyle.AutoSize = True
        Me.lblStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStyle.Location = New System.Drawing.Point(61, 107)
        Me.lblStyle.Name = "lblStyle"
        Me.lblStyle.Size = New System.Drawing.Size(121, 16)
        Me.lblStyle.TabIndex = 9
        Me.lblStyle.Text = "Choix des styles"
        '
        'btnTexte
        '
        Me.btnTexte.Location = New System.Drawing.Point(133, 168)
        Me.btnTexte.Name = "btnTexte"
        Me.btnTexte.Size = New System.Drawing.Size(118, 25)
        Me.btnTexte.TabIndex = 8
        Me.btnTexte.Text = "Texte"
        Me.btnTexte.UseVisualStyleBackColor = True
        '
        'btnSommet
        '
        Me.btnSommet.Location = New System.Drawing.Point(133, 137)
        Me.btnSommet.Name = "btnSommet"
        Me.btnSommet.Size = New System.Drawing.Size(118, 25)
        Me.btnSommet.TabIndex = 7
        Me.btnSommet.Text = "Sommet"
        Me.btnSommet.UseVisualStyleBackColor = True
        '
        'btnSurface
        '
        Me.btnSurface.Location = New System.Drawing.Point(9, 199)
        Me.btnSurface.Name = "btnSurface"
        Me.btnSurface.Size = New System.Drawing.Size(118, 25)
        Me.btnSurface.TabIndex = 6
        Me.btnSurface.Text = "Surface"
        Me.btnSurface.UseVisualStyleBackColor = True
        '
        'btnLigne
        '
        Me.btnLigne.Location = New System.Drawing.Point(9, 168)
        Me.btnLigne.Name = "btnLigne"
        Me.btnLigne.Size = New System.Drawing.Size(118, 25)
        Me.btnLigne.TabIndex = 5
        Me.btnLigne.Text = "Ligne"
        Me.btnLigne.UseVisualStyleBackColor = True
        '
        'btnPoint
        '
        Me.btnPoint.Location = New System.Drawing.Point(9, 137)
        Me.btnPoint.Name = "btnPoint"
        Me.btnPoint.Size = New System.Drawing.Size(118, 25)
        Me.btnPoint.TabIndex = 4
        Me.btnPoint.Text = "Point"
        Me.btnPoint.UseVisualStyleBackColor = True
        '
        'chkZoomGeometrie
        '
        Me.chkZoomGeometrie.AutoSize = True
        Me.chkZoomGeometrie.Checked = True
        Me.chkZoomGeometrie.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkZoomGeometrie.Location = New System.Drawing.Point(9, 81)
        Me.chkZoomGeometrie.Name = "chkZoomGeometrie"
        Me.chkZoomGeometrie.Size = New System.Drawing.Size(197, 17)
        Me.chkZoomGeometrie.TabIndex = 3
        Me.chkZoomGeometrie.Text = "Zoom selon les géométries de travail"
        Me.chkZoomGeometrie.UseVisualStyleBackColor = True
        '
        'chkDessinerSommet
        '
        Me.chkDessinerSommet.AutoSize = True
        Me.chkDessinerSommet.Checked = True
        Me.chkDessinerSommet.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDessinerSommet.Location = New System.Drawing.Point(9, 58)
        Me.chkDessinerSommet.Name = "chkDessinerSommet"
        Me.chkDessinerSommet.Size = New System.Drawing.Size(201, 17)
        Me.chkDessinerSommet.TabIndex = 2
        Me.chkDessinerSommet.Text = "Dessiner les sommets des géométries"
        Me.chkDessinerSommet.UseVisualStyleBackColor = True
        '
        'chkDessinerGeometrie
        '
        Me.chkDessinerGeometrie.AutoSize = True
        Me.chkDessinerGeometrie.Checked = True
        Me.chkDessinerGeometrie.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDessinerGeometrie.Location = New System.Drawing.Point(9, 35)
        Me.chkDessinerGeometrie.Name = "chkDessinerGeometrie"
        Me.chkDessinerGeometrie.Size = New System.Drawing.Size(183, 17)
        Me.chkDessinerGeometrie.TabIndex = 1
        Me.chkDessinerGeometrie.Text = "Dessiner les géométries de travail"
        Me.chkDessinerGeometrie.UseVisualStyleBackColor = True
        '
        'chkDessinerNumero
        '
        Me.chkDessinerNumero.AutoSize = True
        Me.chkDessinerNumero.Checked = True
        Me.chkDessinerNumero.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDessinerNumero.Location = New System.Drawing.Point(9, 12)
        Me.chkDessinerNumero.Name = "chkDessinerNumero"
        Me.chkDessinerNumero.Size = New System.Drawing.Size(195, 17)
        Me.chkDessinerNumero.TabIndex = 0
        Me.chkDessinerNumero.Text = "Dessiner les numéros de géométries"
        Me.chkDessinerNumero.UseVisualStyleBackColor = True
        '
        'btnInitialiser
        '
        Me.btnInitialiser.Location = New System.Drawing.Point(3, 267)
        Me.btnInitialiser.Name = "btnInitialiser"
        Me.btnInitialiser.Size = New System.Drawing.Size(100, 26)
        Me.btnInitialiser.TabIndex = 1
        Me.btnInitialiser.Text = "Initialiser"
        Me.btnInitialiser.UseVisualStyleBackColor = True
        '
        'dckMenuGeometrieTravail
        '
        Me.Controls.Add(Me.btnInitialiser)
        Me.Controls.Add(Me.tabGeometrieTravail)
        Me.Name = "dckMenuGeometrieTravail"
        Me.Size = New System.Drawing.Size(300, 300)
        Me.tabGeometrieTravail.ResumeLayout(False)
        Me.pgeInformation.ResumeLayout(False)
        Me.pgeGeometrie.ResumeLayout(False)
        Me.pgeParametres.ResumeLayout(False)
        Me.pgeParametres.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tabGeometrieTravail As System.Windows.Forms.TabControl
    Friend WithEvents pgeInformation As System.Windows.Forms.TabPage
    Friend WithEvents pgeParametres As System.Windows.Forms.TabPage
    Friend WithEvents btnInitialiser As System.Windows.Forms.Button
    Friend WithEvents rtxInformation As System.Windows.Forms.RichTextBox
    Friend WithEvents lblStyle As System.Windows.Forms.Label
    Friend WithEvents btnTexte As System.Windows.Forms.Button
    Friend WithEvents btnSommet As System.Windows.Forms.Button
    Friend WithEvents btnSurface As System.Windows.Forms.Button
    Friend WithEvents btnLigne As System.Windows.Forms.Button
    Friend WithEvents btnPoint As System.Windows.Forms.Button
    Friend WithEvents chkZoomGeometrie As System.Windows.Forms.CheckBox
    Friend WithEvents chkDessinerSommet As System.Windows.Forms.CheckBox
    Friend WithEvents chkDessinerGeometrie As System.Windows.Forms.CheckBox
    Friend WithEvents chkDessinerNumero As System.Windows.Forms.CheckBox
    Friend WithEvents pgeGeometrie As System.Windows.Forms.TabPage
    Friend WithEvents treGeometrie As System.Windows.Forms.TreeView

End Class
