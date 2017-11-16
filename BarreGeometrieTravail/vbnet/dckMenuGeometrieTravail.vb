Imports System.Windows.Forms
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.DisplayUI
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Display

'**
'Nom de la composante : dckMenuGeometrieTravail 
'
'''<summary>
'''Menu qui permet d'afficher l'information sur les géométries de travail en mémoire.
'''
'''Le menu d’information affiche le nombre total de géométries de travail présentent 
'''en mémoire, le nombre de points, de lignes et de surfaces intérieures et extérieures. 
'''De plus, la longueur totale des lignes et la superficie totale des surfaces.
'''
'''Le menu de paramètres permet d’indiquer si les numéros des géométries et les 
'''géométries doivent être dessinées et si un Zoom doit également être fait 
'''lors de la création d’une géométrie. De plus, on peut changer le symbole 
'''des géométries de type point, ligne et surface ainsi que celui du  texte.
'''</summary>
''' 
'''<remarks>
'''Auteur : Michel Pothier
'''</remarks>
Public Class dckMenuGeometrieTravail
    'Déclarer les variables de travail
    Private gqGeometry As New Collection    'Contient les géométries détaillées
    Private m_Click As Integer = 0          'Nombre de click effectué

    Public Sub New(ByVal hook As Object)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Hook = hook

        'Définir l'application
        m_Application = CType(hook, IApplication)

        'Définir le document
        m_MxDocument = CType(m_Application.Document, IMxDocument)

        'Initialiser le menu
        Me.Init()

        'Conserver le menu en mémoire
        m_MenuGeometrieTravail = Me
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

    Protected Overrides Sub Finalize()
        gqGeometry = Nothing
        MyBase.Finalize()
    End Sub

    ''' <summary>
    ''' Implementation class of the dockable window add-in. It is responsible for
    ''' creating and disposing the user interface class for the dockable window.
    ''' </summary>
    Public Class AddinImpl
        Inherits ESRI.ArcGIS.Desktop.AddIns.DockableWindow

        Private m_windowUI As dckMenuGeometrieTravail

        Protected Overrides Function OnCreateChild() As System.IntPtr
            m_windowUI = New dckMenuGeometrieTravail(Me.Hook)
            Return m_windowUI.Handle
        End Function

        Protected Overrides Sub Dispose(ByVal Param As Boolean)
            If m_windowUI IsNot Nothing Then
                m_windowUI.Dispose(Param)
            End If

            MyBase.Dispose(Param)
        End Sub

    End Class

#Region "Routines et fonctions privées"
    Private Sub btnPoint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPoint.Click
        'Déclarer les variables de travail
        Dim pSymbolSelector As ISymbolSelector = Nothing 'Interface qui permet de changer la symbologie d'un symbol

        Try
            'Définir un nouvel interface de sélection de symboles
            pSymbolSelector = New SymbolSelector

            'Ajouter le symbole présent
            If pSymbolSelector.AddSymbol(modGeometrieTravail.mpSymbolePoint) Then
                'Sélectionner un nouveau symbole
                If pSymbolSelector.SelectSymbol(modGeometrieTravail.m_Application.hWnd) Then
                    'Conserver le nouveau symbole
                    modGeometrieTravail.mpSymbolePoint = pSymbolSelector.GetSymbolAt(0)
                End If
            End If

        Finally
            'Vider la mémoire
            pSymbolSelector = Nothing
        End Try
    End Sub

    Private Sub btnLigne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLigne.Click
        'Déclarer les variables de travail
        Dim pSymbolSelector As ISymbolSelector = Nothing 'Interface qui permet de changer la symbologie d'un symbol

        Try
            'Définir un nouvel interface de sélection de symboles
            pSymbolSelector = New SymbolSelector

            'Ajouter le symbole présent
            If pSymbolSelector.AddSymbol(modGeometrieTravail.mpSymboleLigne) Then
                'Sélectionner un nouveau symbole
                If pSymbolSelector.SelectSymbol(modGeometrieTravail.m_Application.hWnd) Then
                    'Conserver le nouveau symbole
                    modGeometrieTravail.mpSymboleLigne = pSymbolSelector.GetSymbolAt(0)
                End If
            End If

        Finally
            'Vider la mémoire
            pSymbolSelector = Nothing
        End Try
    End Sub

    Private Sub btnSurface_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSurface.Click
        'Déclarer les variables de travail
        Dim pSymbolSelector As ISymbolSelector = Nothing 'Interface qui permet de changer la symbologie d'un symbol

        Try
            'Définir un nouvel interface de sélection de symboles
            pSymbolSelector = New SymbolSelector

            'Ajouter le symbole présent
            If pSymbolSelector.AddSymbol(modGeometrieTravail.mpSymboleSurface) Then
                'Sélectionner un nouveau symbole
                If pSymbolSelector.SelectSymbol(modGeometrieTravail.m_Application.hWnd) Then
                    'Conserver le nouveau symbole
                    modGeometrieTravail.mpSymboleSurface = pSymbolSelector.GetSymbolAt(0)
                End If
            End If

        Finally
            'Vider la mémoire
            pSymbolSelector = Nothing
        End Try
    End Sub

    Private Sub btnSommet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSommet.Click
        'Déclarer les variables de travail
        Dim pSymbolSelector As ISymbolSelector = Nothing 'Interface qui permet de changer la symbologie d'un symbol

        Try
            'Définir un nouvel interface de sélection de symboles
            pSymbolSelector = New SymbolSelector

            'Ajouter le symbole présent
            If pSymbolSelector.AddSymbol(modGeometrieTravail.mpSymboleSommet) Then
                'Sélectionner un nouveau symbole
                If pSymbolSelector.SelectSymbol(modGeometrieTravail.m_Application.hWnd) Then
                    'Conserver le nouveau symbole
                    modGeometrieTravail.mpSymboleSommet = pSymbolSelector.GetSymbolAt(0)
                End If
            End If

        Finally
            'Vider la mémoire
            pSymbolSelector = Nothing
        End Try
    End Sub

    Private Sub btnTexte_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTexte.Click
        'Déclarer les variables de travail
        Dim pSymbolSelector As ISymbolSelector = Nothing 'Interface qui permet de changer la symbologie d'un symbol

        Try
            'Définir un nouvel interface de sélection de symboles
            pSymbolSelector = New SymbolSelector

            'Ajouter le symbole présent
            If pSymbolSelector.AddSymbol(modGeometrieTravail.mpSymboleTexte) Then
                'Sélectionner un nouveau symbole
                If pSymbolSelector.SelectSymbol(modGeometrieTravail.m_Application.hWnd) Then
                    'Conserver le nouveau symbole
                    modGeometrieTravail.mpSymboleTexte = pSymbolSelector.GetSymbolAt(0)
                End If
            End If

        Finally
            'Vider la mémoire
            pSymbolSelector = Nothing
        End Try
    End Sub

    Private Sub btnInitialiser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInitialiser.Click
        'Ce bouton permet d'effectuer l'initialisation du menu et des variables.
        Try
            'Initialiser le menu
            Me.Init()

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        End Try
    End Sub

    Private Sub dckMenuGeometrieTravail_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        'Déclarer les variables de travail
        Dim iDeltaHeight As Integer
        Dim iDeltaWidth As Integer

        Try
            'Calculer les deltas
            iDeltaHeight = Me.Height - m_Height
            iDeltaWidth = Me.Width - m_Width

            'Redimensionner les objets du menu
            tabGeometrieTravail.Height = tabGeometrieTravail.Height + iDeltaHeight
            tabGeometrieTravail.Width = tabGeometrieTravail.Width + iDeltaWidth
            rtxInformation.Height = rtxInformation.Height + iDeltaHeight
            rtxInformation.Width = rtxInformation.Width + iDeltaWidth
            btnInitialiser.Top = btnInitialiser.Top + iDeltaHeight

            'Initialiser les variables de dimension
            m_Height = Me.Height
            m_Width = Me.Width

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        End Try
    End Sub



    '''<summary>
    ''' Routine appelée lorsqu'un noeud doit être ouvert.
    ''' On n'ouvre pas le noeud s'il s'agit d'un double-click.
    '''</summary>
    ''' 
    Private Sub treGeometrie_BeforeExpand(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewCancelEventArgs) Handles treGeometrie.BeforeExpand
        'Vérifier si c'est un Double-Click
        If m_Click = 2 Then
            'Annuler l'ouverture du Node
            e.Cancel = True
        End If
    End Sub

    '''<summary>
    ''' Routine appelée lorsqu'un noeud doit être fermé.
    ''' On ne ferme pas le noeud s'il s'agit d'un double-click.
    '''</summary>
    ''' 
    Private Sub treGeometrie_BeforeCollapse(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewCancelEventArgs) Handles treGeometrie.BeforeCollapse
        'Vérifier si c'est un Double-Click
        If m_Click = 2 Then
            'Annuler l'ouverture du Node
            e.Cancel = True
        End If
    End Sub

    '''<summary>
    ''' Routine appelée lorsqu'un click de souris est effectué.
    ''' On conserve le nombre de clicks afin de savoir si c'est un double-click qui est effectué.
    '''</summary>
    ''' 
    Private Sub treGeometrie_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles treGeometrie.MouseDown
        'Conserver le nombre de click
        m_Click = e.Clicks
    End Sub

    Private Sub treGeometrie_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles treGeometrie.NodeMouseClick
        'Déclarer les variables de travail
        Dim pGeometry As IGeometry = Nothing            'Interface contenant la géométrie détaillée.
        Dim pTrackCancel As ITrackCancel = Nothing      'Interface qui permet d'annuler la sélection avec la touche ESC.

        Try
            'Vérifier si le node est déjà traité
            If e.Node Is Nothing Then Return

            'Vérifier si le Node contient un nom
            If gqGeometry.Contains(e.Node.Name) Then
                'Définir la géométrie à afficher
                pGeometry = CType(gqGeometry.Item(e.Node.Name), IGeometry)

                'Vérifier si on doit dessiner
                If chkZoomGeometrie.Checked Then
                    'Dessiner la géométrie de la géométrie de travail
                    Call ZoomGeometrieTravail(pGeometry)
                End If

                'Permettre d'annuler la sélection avec la touche ESC
                pTrackCancel = New CancelTracker
                pTrackCancel.CancelOnKeyPress = True
                pTrackCancel.CancelOnClick = False

                'Dessiner la géométrie de la géométrie de travail
                Call bDessinerGeometrie(pGeometry, True, chkDessinerGeometrie.Checked, chkDessinerSommet.Checked, chkDessinerNumero.Checked, pTrackCancel)
            End If

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        Finally
            'Vider la mémoire
            pGeometry = Nothing
            pTrackCancel = Nothing
        End Try
    End Sub

    Private Sub treGeometrie_NodeMouseDoubleClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles treGeometrie.NodeMouseDoubleClick
        'Déclarer les variables de travail
        Dim pGeometry As IGeometry = Nothing            'Interface contenant la géométrie détaillée.
        Dim pTrackCancel As ITrackCancel = Nothing      'Interface qui permet d'annuler la sélection avec la touche ESC.

        Try
            'Vérifier si le node est déjà traité
            If e.Node Is Nothing Then Return

            'Vérifier si le Node contient un nom
            If gqGeometry.Contains(e.Node.Name) Then
                'Définir la géométrie à afficher
                pGeometry = CType(gqGeometry.Item(e.Node.Name), IGeometry)

                'Vérifier si la géométrie n'a pas été détaillée
                If e.Node.Nodes.Count = 0 Then
                    'Détailler les géométries de travail dans un TreeView
                    DetailGeometrieTravail(pGeometry, e.Node, e.Node.Nodes)
                    'Indiquer qu'il ne s'agit plus d'un double-click
                    m_Click = 1
                    'Expand Node
                    e.Node.Expand()
                End If

                'Dessiner la géométrie de la géométrie de travail
                Call ZoomGeometrieTravail(pGeometry)

                'Permettre d'annuler la sélection avec la touche ESC
                pTrackCancel = New CancelTracker
                pTrackCancel.CancelOnKeyPress = True
                pTrackCancel.CancelOnClick = False

                'Dessiner la géométrie de la géométrie de travail
                Call bDessinerGeometrie(pGeometry, True, chkDessinerGeometrie.Checked, chkDessinerSommet.Checked, chkDessinerNumero.Checked, pTrackCancel)
            End If

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        Finally
            'Vider la mémoire
            pGeometry = Nothing
            pTrackCancel = Nothing
        End Try
    End Sub
#End Region

#Region "Routines et fonctions publiques"
    '''<summary>
    ''' Permet d'initialiser le menu, les symboles, d'afficher de l'information dans le richtextbox et dessiner les géométries.
    '''</summary>
    '''
    Public Sub Init()
        Try
            'Définir les valeurs par défaut
            chkZoomGeometrie.Checked = False
            chkDessinerGeometrie.Checked = True
            chkDessinerSommet.Checked = True
            chkDessinerNumero.Checked = False

            'Initialiser la mémoire
            mpGeometrieTravail = CType(New GeometryBag, IGeometryCollection)
            mpSymboleTexte = Nothing
            mpSymboleSommet = Nothing
            mpSymbolePoint = Nothing
            mpSymboleLigne = Nothing
            mpSymboleSurface = Nothing

            'Initialiser les couleurs
            Call modGeometrieTravail.InitSymbole()

            'Afficher l'information du numéro de la géométrie de travail
            Call Me.InfoGeometrieTravail()

            'Rafraîchir le menu
            Me.Refresh()

        Catch erreur As Exception
            Throw erreur
        End Try
    End Sub

    '''<summary>
    '''Permet d'afficher de l'information sur les géométries de travail et de les dessiner si spécifié.
    '''</summary>
    '''
    Public Sub InfoGeometrieTravail()
        'Déclarer les variables de travail
        Dim pTrackCancel As ITrackCancel = Nothing      'Interface qui permet d'annuler la sélection avec la touche ESC.
        Dim pGeometry As IGeometry = Nothing            'Interface contenant la géométrie traitée.
        Dim qNode As TreeNode = Nothing                 'Interface contenant un Node de géométrie.

        Try
            'Vérifier si une unité de travail est présente
            If Not mpGeometrieTravail Is Nothing Then
                'Initialiser les nodes contenant chaque géométrie de travail
                treGeometrie.Nodes.Clear()

                'Initialiser la collection des géométries détaillées
                gqGeometry = New Collection

                'Définir l'information pour les géométries de travail
                rtxInformation.Text = InfoGeometryBag()

                'Interface contenant la géométrie traitée
                pGeometry = CType(mpGeometrieTravail, IGeometry)

                'Ajouter le Node pour décrire la géométrie
                qNode = treGeometrie.Nodes.Add(gqGeometry.Count.ToString, Str(treGeometrie.Nodes.Count + 1) & ": " & pGeometry.GeometryType.ToString)
                'Ajouter la géométrie dans la liste avec sa clé de recherche
                gqGeometry.Add(pGeometry, gqGeometry.Count.ToString)

                'Vérifier si on doit dessiner
                If chkZoomGeometrie.Checked Then
                    'Dessiner la géométrie de la géométrie de travail
                    Call ZoomGeometrieTravail(CType(mpGeometrieTravail, IGeometry))
                End If

                'Vérifier si le menu est définie
                If m_MenuGeometrieTravail IsNot Nothing Then
                    'Permettre d'annuler la sélection avec la touche ESC
                    pTrackCancel = New CancelTracker
                    pTrackCancel.CancelOnKeyPress = True
                    pTrackCancel.CancelOnClick = False

                    'Dessiner la géométrie de la géométrie de travail
                    bDessinerGeometrie(CType(mpGeometrieTravail, IGeometry), True,
                                       m_MenuGeometrieTravail.chkDessinerGeometrie.Checked, _
                                       m_MenuGeometrieTravail.chkDessinerSommet.Checked, _
                                       m_MenuGeometrieTravail.chkDessinerNumero.Checked,
                                       pTrackCancel)
                End If

                'Si aucune géométrie de travail spécifié
            Else
                'Vider le contenu du texte
                rtxInformation.Text = ""
            End If

            'Rafraîchir le menu
            Me.Refresh()

        Catch erreur As Exception
            'Message d'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pTrackCancel = Nothing
            pGeometry = Nothing
            qNode = Nothing
        End Try
    End Sub

    '''<summary>
    '''Permet d'afficher de détailler le contenu de chaque géométrie de travail dans une TreeView.
    '''</summary>
    ''' 
    '''<param name=" pGeometry "> Interface contenant la géométrie à détailler dans le TreeView.</param>
    '''<param name=" qNode "> Interface contenant un Node de géométrie.</param>
    '''<param name=" qNodes "> Objet contenant la collection de Nodes dans lequel on y ajoute des Nodes du TreeView.</param>
    '''
    Public Sub DetailGeometrieTravail(ByVal pGeometry As IGeometry, ByVal qNode As TreeNode, ByRef qNodes As TreeNodeCollection, Optional ByVal bDetail As Boolean = True)
        'Déclarer les variables de travail
        Dim pGeometryColl As IGeometryCollection = Nothing  'Interface contenant une géométrie de travail.
        Dim pPolyline As IPolyline = Nothing                'Interface pour extraire la longueur d'une ligne.
        Dim pPath As IPath = Nothing                        'Interface pour extraire la longueur du Path.
        Dim pPolygon As IPolygon4 = Nothing                 'Interface ESRI contenant une surface.
        Dim pRing As IRing = Nothing                        'Interface pour extraire le type d'anneau.
        Dim pArea As IArea = Nothing                        'Interface pour extraire la superficie.
        Dim pPointColl As IPointCollection = Nothing        'Interface permettant d'extraire les sommets.
        Dim pPoint As IPoint = Nothing                      'Interface contenant un sommet.
        Dim qNodePoint As TreeNode = Nothing                'Interface contenant un Node de point.

        Try
            'Vérifier si la géométrie est valide
            If Not pGeometry Is Nothing Then
                'Vérifier si la géométrie est un Bag ou un Multipoint
                If pGeometry.GeometryType = esriGeometryType.esriGeometryBag _
                Or pGeometry.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                    'Vérifier si le noeud est absent
                    If qNode Is Nothing Then
                        'Ajouter le Node pour décrire la géométrie
                        qNode = qNodes.Add(gqGeometry.Count.ToString, Str(qNodes.Count + 1) & ": " & pGeometry.GeometryType.ToString)
                        'Ajouter la géométrie dans la liste avec sa clé de recherche
                        gqGeometry.Add(pGeometry, gqGeometry.Count.ToString)
                    End If

                    'Vérifier si on doit détaillé
                    If bDetail Then
                        'Interface pour extraire les géométries du BAG 
                        pGeometryColl = CType(pGeometry, IGeometryCollection)

                        'Traiter toutes les géométries de travail
                        For i = 0 To pGeometryColl.GeometryCount - 1
                            'Détailler la géométrie de travail
                            DetailGeometrieTravail(pGeometryColl.Geometry(i), Nothing, qNode.Nodes, False)
                        Next
                    End If

                    'Si la géométrie est un Polyline
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then
                    'Vérifier si le noeud est absent
                    If qNode Is Nothing Then
                        'Interface pour extraire la lingueur de la ligne
                        pPolyline = CType(pGeometry, IPolyline)

                        'Ajouter le Node pour décrire la géométrie
                        qNode = qNodes.Add(gqGeometry.Count.ToString, Str(qNodes.Count + 1) & ": " & pGeometry.GeometryType.ToString & ": " & pPolyline.Length.ToString("F2"))
                        'Ajouter la géométrie dans la liste avec sa clé de recherche
                        gqGeometry.Add(pGeometry, gqGeometry.Count.ToString)
                    End If

                    'Vérifier si on doit détaillé
                    If bDetail Then
                        'Interface pour extraire les composantes du Polyline
                        pGeometryColl = CType(pGeometry, IGeometryCollection)

                        'Traiter toutes les composantes du Polyline
                        For i = 0 To pGeometryColl.GeometryCount - 1
                            'Détailler la géométrie de travail
                            DetailGeometrieTravail(pGeometryColl.Geometry(i), Nothing, qNode.Nodes, False)
                        Next
                    End If

                    'Si la géométrie est un Polygon
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolygon Then
                    'Vérifier si le noeud est absent
                    If qNode Is Nothing Then
                        'Interface pour extraire la superficie
                        pArea = CType(pGeometry, IArea)

                        'Ajouter le Node pour décrire la géométrie
                        qNode = qNodes.Add(gqGeometry.Count.ToString, Str(qNodes.Count + 1) & ": " & pGeometry.GeometryType.ToString & ": " & pArea.Area.ToString("F2"))
                        'Ajouter la géométrie dans la liste avec sa clé de recherche
                        gqGeometry.Add(pGeometry, gqGeometry.Count.ToString)
                    End If

                    'Vérifier si on doit détaillé
                    If bDetail Then
                        'Interface pour extraire les composantes du Polygon
                        pGeometryColl = CType(pGeometry, IGeometryCollection)

                        'Traiter toutes les composantes du polygon
                        For i = 0 To pGeometryColl.GeometryCount - 1
                            'Détailler la géométrie de travail
                            DetailGeometrieTravail(pGeometryColl.Geometry(i), Nothing, qNode.Nodes, False)
                        Next
                    End If

                    'Si la géométrie est un Path
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPath Then
                    'Vérifier si le noeud est absent
                    If qNode Is Nothing Then
                        'Interface pour extraire la longueur du Path
                        pPath = CType(pGeometry, IPath)

                        'Ajouter le Node pour décrire la géométrie
                        qNode = qNodes.Add(gqGeometry.Count.ToString, Str(qNodes.Count + 1) & ": " & pGeometry.GeometryType.ToString & ": " & pPath.Length.ToString("F2"))
                        'Ajouter la géométrie dans la liste avec sa clé de recherche
                        gqGeometry.Add(pGeometry, gqGeometry.Count.ToString)
                    End If

                    'Vérifier si on doit détaillé
                    If bDetail Then
                        'Interface pour extraire les sommets
                        pPointColl = CType(pGeometry, IPointCollection)

                        'Traiter tous les sommets
                        For i = 0 To pPointColl.PointCount - 1
                            'Détailler la géométrie de travail
                            DetailGeometrieTravail(pPointColl.Point(i), Nothing, qNode.Nodes, False)
                        Next
                    End If

                    'Si la géométrie est un Ring
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryRing Then
                    'Vérifier si le noeud est absent
                    If qNode Is Nothing Then
                        'Interface pour extraire le type d'anneau
                        pRing = CType(pGeometry, IRing)
                        'Interface pour extraire la superficie
                        pArea = CType(pGeometry, IArea)

                        'Vérifier si c'est un anneau extérieur
                        If pRing.IsExterior Then
                            'Ajouter le Node pour décrire la géométrie
                            qNode = qNodes.Add(gqGeometry.Count.ToString, Str(qNodes.Count + 1) & ": " & pGeometry.GeometryType.ToString & "-E: " & pArea.Area.ToString("F2"))
                            'Si c'est un anneau intérieur
                        Else
                            'Ajouter le Node pour décrire la géométrie
                            qNode = qNodes.Add(gqGeometry.Count.ToString, Str(qNodes.Count + 1) & ": " & pGeometry.GeometryType.ToString & "-I: " & pArea.Area.ToString("F2"))
                        End If
                        'Ajouter la géométrie dans la liste avec sa clé de recherche
                        gqGeometry.Add(pGeometry, gqGeometry.Count.ToString)
                    End If

                    'Vérifier si on doit détaillé
                    If bDetail Then
                        'Interface pour extraire les sommets
                        pPointColl = CType(pGeometry, IPointCollection)

                        'Traiter tous les sommets
                        For i = 0 To pPointColl.PointCount - 1
                            'Détailler la géométrie de travail
                            DetailGeometrieTravail(pPointColl.Point(i), Nothing, qNode.Nodes, False)
                        Next
                    End If

                    'Si la géométrie est un Point
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                    'Vérifier si le noeud est absent
                    If qNode Is Nothing Then
                        'Interface contenant le sommet traité
                        pPoint = CType(pGeometry, IPoint)

                        'Ajouter le Node pour décrire la géométrie
                        qNodePoint = qNodes.Add(gqGeometry.Count.ToString, Str(qNodes.Count + 1) & ": " & pPoint.GeometryType.ToString)

                        'Ajouter le Node pour décrire la géométrie
                        qNodePoint.Nodes.Add(gqGeometry.Count.ToString, "ID:" & pPoint.ID.ToString _
                                             & ", X:" & pPoint.X.ToString("F9") & ", Y:" & pPoint.Y.ToString("F9") _
                                             & ", Z:" & pPoint.Z.ToString("F9") & ", M:" & pPoint.M.ToString("F9"))

                        'Ajouter la géométrie dans la liste avec sa clé de recherche
                        gqGeometry.Add(pGeometry, gqGeometry.Count.ToString)
                    End If
                End If
            End If

        Catch erreur As Exception
            'Afficher le message d'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pGeometryColl = Nothing
            pPolyline = Nothing
            pPolygon = Nothing
            pPointColl = Nothing
            pPath = Nothing
            pRing = Nothing
            pArea = Nothing
            pPoint = Nothing
            qNodePoint = Nothing
            GC.Collect()
        End Try
    End Sub
#End Region
End Class