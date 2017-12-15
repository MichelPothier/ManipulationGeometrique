Imports System.Windows.Controls
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Mapping
Imports ArcGIS.Core.Geometry

Public Class dckGeometrieTravailView
    Inherits System.Windows.Controls.UserControl

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    Public Sub New()
        'Initialisation de la composante
        InitializeComponent()

        'Définir le TreeView géométries de travail
        modGeometrieTravail.TreeViewGeometrieTravail = Me.treGeometrieTravail
    End Sub

    Public Sub cboAction_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        'Définir le ComboBox
        Dim combobox As ComboBox = sender
        'Définir l'action de visualisation
        modGeometrieTravail.ActionVisualisation = combobox.SelectedIndex
    End Sub

    Public Sub dckGeometrieTravailView_SelectedItemChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Object))
        'Déclarer les variables de travail
        Dim treeView As TreeView = sender                               'Contient le TreeView contenant toutes les géométries de travail.
        Dim treeViewList As TreeViewItem = treeView.Items.Item(0)       'Contient l'item du TreeView contenant toutes les géométries de travail.
        Dim treeViewSelected As TreeViewItem = treeView.SelectedItem    'Contient l'item sélectionné du TreeView des géométries de travail.
        Dim activeMapView As MapView = MapView.Active                   'Contient la Map active.
        Dim dckGeometrieTravail = dckGeometrieTravailViewModel.Current  'Contient le PaneView des géométries de travail.

        'Vérifier si l'item sélectionné est une liste
        If treeViewSelected.Tag = "LIST" Then
            'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
            Dim identifyResult = QueuedTask.Run(
                Function()
                    'Définir la liste des géométries
                    Dim geometries = modGeometrieTravail.GeometrieTravail

                    'Afficher l'information sur les géométries de travail
                    dckGeometrieTravail.Information = modGeometrieTravail.Current.InfoListeGeometries(geometries)

                    'Dessiner la liste des géométries
                    Call modGeometrieTravail.DessinerListeGeometries(geometries)

                    'Retourner le succès du traitement
                    Return True
                End Function)

            'Si l'item sélectionné est une géométrie
        ElseIf treeViewSelected.Tag = "GEOMETRY" Then
            'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
            Dim identifyResult = QueuedTask.Run(
                Function()
                    'Définir le numéro de l'item
                    Dim no As Integer = treeViewList.Items.IndexOf(treeViewSelected)
                    'Définir la géométrie de l'item
                    Dim geometrie = modGeometrieTravail.GeometrieTravail.Item(no)

                    'Afficher l'information de la géométrie de travail
                    dckGeometrieTravail.Information = modGeometrieTravail.Current.InfoGeometrie(geometrie)

                    'Dessiner la géométrie
                    Call modGeometrieTravail.DessinerGeometrie(geometrie)

                    'Retourner le succès du traitement
                    Return True
                End Function)

        ElseIf treeViewSelected.Tag = "RING" Then
            'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
            Dim identifyResult = QueuedTask.Run(
                Function()
                    'Définir l'item parent
                    Dim treeViewParent As TreeViewItem = treeViewSelected.Parent
                    'Définir le numéro de l'item sélectionné du  TreeView
                    Dim n1 As Integer = treeViewList.Items.IndexOf(treeViewParent)
                    'Définir la géométrie correspondant à l'item sélectionné
                    Dim polygone As Polygon = modGeometrieTravail.GeometrieTravail.Item(n1)
                    'Définir le numéro de l'item sélectionné du  TreeView
                    Dim n2 As Integer = treeViewParent.Items.IndexOf(treeViewSelected)
                    'Définir l'envelope du segment
                    Dim ring = PolylineBuilder.CreatePolyline(polygone.Parts.Item(n2))
                    'Définir l'enveloppe de la ligne
                    Dim envelope = ring.Extent

                    'Afficher l'information de la composante
                    dckGeometrieTravail.Information = modGeometrieTravail.Current.InfoComposante(polygone, n2)

                    'Dessiner la géométrie
                    Call modGeometrieTravail.DessinerGeometrie(ring)

                    'Retourner le succès du traitement
                    Return True
                End Function)

        ElseIf treeViewSelected.Tag = "LINE" Then
            'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
            Dim identifyResult = QueuedTask.Run(
                Function()
                    'Définir l'item parent
                    Dim treeViewParent As TreeViewItem = treeViewSelected.Parent
                    'Définir le numéro de l'item sélectionné du  TreeView
                    Dim n1 As Integer = treeViewList.Items.IndexOf(treeViewParent)
                    'Définir la géométrie correspondant à l'item sélectionné
                    Dim polyligne As Polyline = modGeometrieTravail.GeometrieTravail.Item(n1)
                    'Définir le numéro de l'item sélectionné du  TreeView
                    Dim n2 As Integer = treeViewParent.Items.IndexOf(treeViewSelected)
                    'Définir l'envelope du segment
                    Dim ligne = PolylineBuilder.CreatePolyline(polyligne.Parts.Item(n2))

                    'Afficher l'information de la composante
                    dckGeometrieTravail.Information = modGeometrieTravail.Current.InfoComposante(polyligne, n2)

                    'Dessiner la géométrie
                    Call modGeometrieTravail.DessinerGeometrie(ligne)

                    'Retourner le succès du traitement
                    Return True
                End Function)

        ElseIf treeViewSelected.Tag = "SEGMENT" Then
            'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
            Dim identifyResult = QueuedTask.Run(
                Function()
                    'Définir l'item parent
                    Dim treeViewParent1 As TreeViewItem = treeViewSelected.Parent
                    'Définir l'item GrandParent
                    Dim treeViewParent2 As TreeViewItem = treeViewParent1.Parent
                    'Définir le numéro de l'item sélectionné du TreeView
                    Dim n1 As Integer = treeViewList.Items.IndexOf(treeViewParent2)
                    'Définir la géométrie
                    Dim multipart As Multipart = modGeometrieTravail.GeometrieTravail.Item(n1)
                    'Définir le numéro de la partie du TreeView
                    Dim n2 As Integer = treeViewParent2.Items.IndexOf(treeViewParent1)
                    'Définir la partie
                    Dim segments As ReadOnlySegmentCollection = multipart.Parts.Item(n2)
                    'Définir le numéro de l'item sélectionné du TreeView
                    Dim n3 As Integer = treeViewParent1.Items.IndexOf(treeViewSelected)
                    'Définir le segment
                    Dim segment As Segment = multipart.Parts.Item(n2).Item(n3)
                    'Définir la polyligne
                    Dim polyline = PolylineBuilder.CreatePolyline(segment)

                    'Afficher l'information du segment de la composante
                    dckGeometrieTravail.Information = modGeometrieTravail.Current.InfoSegment(multipart, n2, n3)

                    'Dessiner la géométrie
                    Call modGeometrieTravail.DessinerGeometrie(polyline)

                    'Retourner le succès du traitement
                    Return True
                End Function)

            'Si l'item sélectionné est un point
        ElseIf treeViewSelected.Tag = "POINT" Then
            'Définir l'item parent
            Dim treeViewParent1 As TreeViewItem = treeViewSelected.Parent()
            'Définir le point à traiter
            Dim point As MapPoint = Nothing
            'Si la géométrie est un multipoint
            If treeViewParent1.Tag = "GEOMETRY" Then
                'Définir le numéro de l'item sélectionné du  TreeView
                Dim n1 As Integer = treeViewList.Items.IndexOf(treeViewParent1)
                'Définir la géométrie correspondant à l'item sélectionné
                Dim multipoint As Multipoint = modGeometrieTravail.GeometrieTravail.Item(n1)
                'Définir le numéro de l'item sélectionné du TreeView
                Dim n2 As Integer = treeViewParent1.Items.IndexOf(treeViewSelected)
                'Définir le point à traiter
                point = multipoint.Points.Item(n2)
                'Afficher l'information de la composante
                dckGeometrieTravail.Information = modGeometrieTravail.Current.InfoComposante(multipoint, n2)

                'Dessiner la géométrie
                Call modGeometrieTravail.DessinerGeometrie(point)

                'Si la géométrie n'est pas un multipoint
            Else
                'Définir l'item GrandParent
                Dim treeViewParent2 As TreeViewItem = treeViewParent1.Parent
                'Définir l'item ArriereGrandParent
                Dim treeViewParent3 As TreeViewItem = treeViewParent2.Parent
                'Définir le numéro de l'item sélectionné du TreeView
                Dim n1 As Integer = treeViewList.Items.IndexOf(treeViewParent3)
                'Définir la géométrie
                Dim multipart As Multipart = modGeometrieTravail.GeometrieTravail.Item(n1)
                'Définir le numéro de la partie du TreeView
                Dim n2 As Integer = treeViewParent3.Items.IndexOf(treeViewParent2)
                'Définir la partie
                Dim segments As ReadOnlySegmentCollection = multipart.Parts.Item(n2)
                'Définir le numéro de l'item sélectionné du TreeView
                Dim n3 As Integer = treeViewParent2.Items.IndexOf(treeViewParent1)
                'Définir le segment
                Dim segment = segments.Item(n3)
                'Définir le numéro de l'item sélectionné du TreeView
                Dim n4 As Integer = treeViewParent1.Items.IndexOf(treeViewSelected)
                'Si c'est le premier point
                If n4 = 0 Then
                    'Définir le point à traiter
                    point = segment.StartPoint
                    'Si c'est le dernier point
                Else
                    'Définir le point à traiter
                    point = segment.EndPoint
                End If

                'Afficher l'information de la composante
                dckGeometrieTravail.Information = modGeometrieTravail.Current.InfoGeometrie(point)

                'Dessiner la géométrie
                Call modGeometrieTravail.DessinerGeometrie(point)
            End If
        End If
    End Sub

    Private Sub dckGeometrieTravailView_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        'Définir les variables de travail
        Dim treeViewList As TreeViewItem = sender
        Dim treeViewSelected As TreeViewItem = treGeometrieTravail.SelectedItem

        'Vérifier si l'item n'a pas été traité
        If treeViewSelected.Items.Count = 0 Then
            'Définir les géométries de travail
            Dim geometries = modGeometrieTravail.GeometrieTravail

            If treeViewSelected.Tag = "LIST" Then
                'Remplir le TreeViewItem selon le type de géométrie
                Call modGeometrieTravail.RemplirTreeViewItemGeometrie(geometries, treeViewSelected)

            ElseIf treeViewSelected.Tag = "GEOMETRY" Then
                'Définir le numéro de l'item sélectionné du  TreeView
                Dim no As Integer = treeViewList.Items.IndexOf(treeViewSelected)
                'Définir la géométrie correspondant à l'item sélectionné
                Dim geometrie = modGeometrieTravail.GeometrieTravail.Item(no)
                'Remplir le TreeViewItem selon le type de géométrie
                Call modGeometrieTravail.RemplirTreeViewItemGeometrie(geometrie, treeViewSelected)

            ElseIf treeViewSelected.Tag = "RING" Then
                'Définir l'item parent
                Dim treeViewParent As TreeViewItem = treeViewSelected.Parent
                'Définir le numéro de l'item sélectionné du  TreeView
                Dim n1 As Integer = treeViewList.Items.IndexOf(treeViewParent)
                'Définir la géométrie correspondant à l'item sélectionné
                Dim polygone As Polygon = modGeometrieTravail.GeometrieTravail.Item(n1)
                'Définir le numéro de l'item sélectionné du  TreeView
                Dim n2 As Integer = treeViewParent.Items.IndexOf(treeViewSelected)
                'Définir l'anneau du polygone
                Dim anneau = polygone.Parts.Item(n2)
                'Remplir le TreeViewItem selon le type de géométrie
                Call modGeometrieTravail.RemplirTreeViewItemGeometrie(anneau, treeViewSelected)

            ElseIf treeViewSelected.Tag = "LINE" Then
                'Définir l'item parent
                Dim treeViewParent As TreeViewItem = treeViewSelected.Parent
                'Définir le numéro de l'item sélectionné du  TreeView
                Dim n1 As Integer = treeViewList.Items.IndexOf(treeViewParent)
                'Définir la géométrie correspondant à l'item sélectionné
                Dim polyligne As Polyline = modGeometrieTravail.GeometrieTravail.Item(n1)
                'Définir le numéro de l'item sélectionné du  TreeView
                Dim n2 As Integer = treeViewParent.Items.IndexOf(treeViewSelected)
                'Définir la ligne de la polyligne
                Dim ligne = polyligne.Parts.Item(n2)
                'Remplir le TreeViewItem selon le type de géométrie
                Call modGeometrieTravail.RemplirTreeViewItemGeometrie(ligne, treeViewSelected)

            ElseIf treeViewSelected.Tag = "SEGMENT" Then
                'Définir l'item parent
                Dim treeViewParent1 As TreeViewItem = treeViewSelected.Parent
                'Définir l'item GrandParent
                Dim treeViewParent2 As TreeViewItem = treeViewParent1.Parent
                'Définir le numéro de l'item sélectionné du TreeView
                Dim n1 As Integer = treeViewList.Items.IndexOf(treeViewParent2)
                'Définir la géométrie
                Dim multipart As Multipart = modGeometrieTravail.GeometrieTravail.Item(n1)
                'Définir le numéro de la partie du TreeView
                Dim n2 As Integer = treeViewParent2.Items.IndexOf(treeViewParent1)
                'Définir la partie
                Dim segments As ReadOnlySegmentCollection = multipart.Parts.Item(n2)
                'Définir le numéro de l'item sélectionné du TreeView
                Dim n3 As Integer = treeViewParent1.Items.IndexOf(treeViewSelected)
                'Définir le segment
                Dim segment = segments.Item(n3)
                'Remplir le TreeViewItem selon le type de géométrie
                Call modGeometrieTravail.RemplirTreeViewItemGeometrie(segment, treeViewSelected)
            End If
        End If
    End Sub
End Class
