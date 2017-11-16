Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.DisplayUI
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI
'**
'Nom de la composante : dckMenuUniteTravail.vb
'
'''<summary>
'''Menu qui permet de faire l'interface avec l'usager afin de définir une Unité de travail.
'''Ce menu est un dockable window.
'''</summary>
'''
'''<remarks>
'''Auteur : Michel Pothier
'''</remarks>
''' 
Public Class dckMenuUniteTravail

    Public Sub New(ByVal hook As Object)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        'Initialiser le formulaire
        Call Initialiser()

        ' Add any initialization after the InitializeComponent() call.
        m_Application = CType(hook, IApplication)
        Me.Hook = hook
        'Définir le document
        m_MxDocument = CType(m_Application.Document, IMxDocument)

        'Conserver le menu en mémoire
        m_MenuUniteTravail = Me
    End Sub

    Private m_hook As Object
    ''' <summary>
    ''' Host object of the dockable window
    ''' </summary> 
    Public Property Hook() As Object
        Get
            Return m_hook
        End Get
        Set(ByVal value As Object)
            m_hook = value
        End Set
    End Property

    ''' <summary>
    ''' Implementation class of the dockable window add-in. It is responsible for
    ''' creating and disposing the user interface class for the dockable window.
    ''' </summary>
    Public Class AddinImpl
        Inherits ESRI.ArcGIS.Desktop.AddIns.DockableWindow

        Private m_windowUI As dckMenuUniteTravail

        Protected Overrides Function OnCreateChild() As System.IntPtr
            m_windowUI = New dckMenuUniteTravail(Me.Hook)
            Return m_windowUI.Handle
        End Function

        Protected Overrides Sub Dispose(ByVal Param As Boolean)
            If m_windowUI IsNot Nothing Then
                m_windowUI.Dispose(Param)
            End If

            MyBase.Dispose(Param)
        End Sub

    End Class

#Region " Commandes du formulaire "
    Private Sub cboNumero_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles cboNumero.KeyDown
        'Vérifier si un Return est entrée
        If e.KeyValue = 13 Then
            'Affichier l'information de l'unité de travail
            Call InfoUniteTravail()
            'vérifier si l'unité est valide
            If modUniteTravail.m_UniteTravail.EstValide Then
                'Ajouter l'unité de travail dans le combobox
                cboNumero.Items.Add(cboNumero.Text)
            End If
        End If
    End Sub

    Private Sub cboNumero_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboNumero.SelectedIndexChanged
        'Affichier l'information de l'unité de travail
        Call InfoUniteTravail()
    End Sub

    Private Sub chkGeometriePrecise_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkGeometriePrecise.CheckedChanged
        'Conserver l'information sur la création d'une géométrie précise
        modUniteTravail.m_GeometriePrecise = chkGeometriePrecise.Checked
    End Sub

    Private Sub btnStyle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStyle.Click
        'Déclarer les variables de travail
        Dim pSymbolSelector As ISymbolSelector = Nothing 'Interface qui permet de changer la symbologie d'un symbol

        Try
            'Définir un nouvel interface de sélection de symboles
            pSymbolSelector = New SymbolSelector

            'Ajouter le symbole présent
            If pSymbolSelector.AddSymbol(modUniteTravail.m_SymboleSurface) Then
                'Sélectionner un nouveau symbole
                If pSymbolSelector.SelectSymbol(modUniteTravail.m_Application.hWnd) Then
                    'Conserver le nouveau symbole
                    modUniteTravail.m_SymboleSurface = pSymbolSelector.GetSymbolAt(0)
                End If
            End If

        Finally
            'Vider la mémoire
            pSymbolSelector = Nothing
        End Try
    End Sub

    Private Sub btnInitialiser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInitialiser.Click
        Call Initialiser()
    End Sub
#End Region

#Region " Routines et fonctions privées "

    '''<summary>
    '''Routine qui permet d'initialiser le formulaire. 
    '''</summary>
    '''
    Private Sub Initialiser()
        'Déclarer les variables de travail
        Dim pFillSymbol As ISimpleFillSymbol = Nothing  'Interface ESRI contenant le symbol de surface par défaut.
        Dim pLineSymbol As ISimpleLineSymbol = Nothing  'Interface ESRI contenant le symbole de ligne par défaut.
        Dim pRgbColor As IRgbColor = Nothing            'Interface ESRI contenant la couleur du symbole par défaut.
        Dim pTextSymbol As ITextSymbol = Nothing            'Interface ESRI contenant un symbole de texte.
        Dim pMarkerSymbol As ISimpleMarkerSymbol = Nothing  'Interface ESRI contenant un symbole de point.

        Try
            'Définir les valeurs par défaut
            cboNumero.Text = ""
            cboNumero.Items.Clear()
            rtxDescription.Text = ""
            chkGeometriePrecise.Checked = False
            chkDessiner.Checked = False
            chkZoom.Checked = False

            'Définir la couleur de la symbologie
            pRgbColor = New RgbColor
            pRgbColor.Red = 255
            'Conserver le symbole en mémoire
            modUniteTravail.m_RgbColor = pRgbColor

            'Définir la symbologie pour le texte
            pTextSymbol = New TextSymbol
            pTextSymbol.Color = pRgbColor
            pTextSymbol.Font.Bold = True
            pTextSymbol.Size = 10
            pTextSymbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHAFull
            'Conserver le symbole en mémoire
            modUniteTravail.m_SymboleTexte = CType(pTextSymbol, ISymbol)

            'Définir la symbologie pour le point
            pMarkerSymbol = New SimpleMarkerSymbol
            pMarkerSymbol.Color = pRgbColor
            pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle
            pMarkerSymbol.Size = 3
            'Conserver le symbole en mémoire
            modUniteTravail.m_SymbolePoint = CType(pMarkerSymbol, ISymbol)

            'Définir la symbologie
            pLineSymbol = New SimpleLineSymbol
            pLineSymbol.Color = pRgbColor
            pFillSymbol = New SimpleFillSymbol
            pFillSymbol.Color = pRgbColor
            pFillSymbol.Outline = pLineSymbol
            pFillSymbol.Style = esriSimpleFillStyle.esriSFSBackwardDiagonal
            'Conserver le symbole en mémoire
            modUniteTravail.m_SymboleSurface = CType(pFillSymbol, ISymbol)

            'Créer l'unité de travail par défaut
            modUniteTravail.m_UniteTravail = New clsUniteTravail

            'Vérifier si une unité de travail est présente
            If Not modUniteTravail.m_UniteTravail Is Nothing Then
                'Ajouter le numéro spécifié dans la liste
                cboNumero.Text = modUniteTravail.m_UniteTravail.Numero
                'Ajouter le numéro spécifié dans la liste
                cboNumero.Items.Add(modUniteTravail.m_UniteTravail.Numero)
                'Affichier l'information de l'unité de travail
                Call InfoUniteTravail()
            End If

            'Remettre les valeurs par défaut
            chkGeometriePrecise.Checked = True
            chkDessiner.Checked = True
            chkZoom.Checked = True

        Finally
            'Vider la mémoire
            pFillSymbol = Nothing
            pLineSymbol = Nothing
            pRgbColor = Nothing
            pTextSymbol = Nothing
            pMarkerSymbol = Nothing
        End Try
    End Sub

    '''<summary>
    '''Routine qui permet d'afficher l'information et de dessiner la géométrie selon un numéro de l'unité
    '''de travail en mémoire. 
    '''</summary>
    '''
    Private Sub InfoUniteTravail()
        'Déclarer les variables de travail
        Dim oSnrc As clsSNRC = Nothing 'Objet contenant l'information d'un SNRC

        Try
            'Créer une nouvelle classe selon l'unité de travail
            modUniteTravail.m_UniteTravail = modUniteTravail.CreerUniteTravail(cboNumero.Text)

            'Vérifier si une unité de travail est présente
            If Not modUniteTravail.m_UniteTravail Is Nothing Then
                'Vérifier la catégorie de l'unité de travail
                If modUniteTravail.m_UniteTravail.Categorie = "CANADA" Then
                    'Définir l'information du numéro de l'unité de travail
                    rtxDescription.Text = modUniteTravail.m_UniteTravail.Information
                    'Si c'est un SNRC
                Else
                    'Interface pour afficher l'information sur le SNRC
                    oSnrc = CType(modUniteTravail.m_UniteTravail, clsSNRC)

                    'Définir l'information du SNRC
                    rtxDescription.Text = oSnrc.Information
                End If

                'Vérifier si l'unité de travail est valide
                If modUniteTravail.m_UniteTravail.EstValide Then
                    'Initialiser le numéro de l'unité de travail
                    cboNumero.Text = modUniteTravail.m_UniteTravail.Numero

                    'Vérifier si on doit dessiner
                    If chkZoom.Checked Then
                        'Dessiner la géométrie de l'unité de travail
                        Call modUniteTravail.ZoomGeometrieUniteTravail()
                    End If

                    'Vérifier si on doit dessiner
                    If chkDessiner.Checked Then
                        'Dessiner la géométrie de l'unité de travail
                        Call modUniteTravail.DessinerGeometrieUniteTravail()
                    End If
                End If

                'Si aucune unité de travail spécifié
            Else
                'Initialiser le numéro de l'unité de travail
                cboNumero.Text = ""

                'Vider le contenu du texte
                rtxDescription.Text = ""
            End If

        Finally
            'Vider la mémoire
            oSnrc = Nothing
        End Try
    End Sub

    Private Sub dckMenuUniteTravail_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        'Déclarer les variables de travail
        Dim iDeltaHeight As Integer
        Dim iDeltaWidth As Integer

        'Calculer les deltas
        iDeltaHeight = Me.Height - m_Height
        iDeltaWidth = Me.Width - m_Width

        'Vérifier si le menu a été initialisé
        If m_MenuUniteTravail IsNot Nothing Then
            'Vérifier la hauteur du menu
            If m_MenuUniteTravail.Size.Height > 200 Then
                'Redimensionner les objets du menu
                tblBarreUniteTravail.Height = tblBarreUniteTravail.Height + iDeltaHeight
                rtxDescription.Height = rtxDescription.Height + iDeltaHeight
                btnInitialiser.Top = btnInitialiser.Top + iDeltaHeight
                'Initialiser les variables de dimension
                m_Height = Me.Height
            End If
            'Vérifier la largeur du menu
            If m_MenuUniteTravail.Size.Width > 200 Then
                'Redimensionner les objets du menu
                tblBarreUniteTravail.Width = tblBarreUniteTravail.Width + iDeltaWidth
                cboNumero.Width = cboNumero.Width + iDeltaWidth
                rtxDescription.Width = rtxDescription.Width + iDeltaWidth
                'Initialiser les variables de dimension
                m_Width = Me.Width
            End If
        End If
    End Sub
#End Region
End Class