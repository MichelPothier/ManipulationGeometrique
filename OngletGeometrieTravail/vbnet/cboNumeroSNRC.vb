Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Core.Geometry
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Mapping

''' <summary>
''' Represents the ComboBox
''' </summary>
''' <remarks></remarks>
Public Class cboNumeroSNRC
    Inherits ComboBox

    Private _isInitialized As Boolean

    '''<summary>
    ''' Combo Box constructor
    '''</summary>
    Public Sub New()
        'Mettre à jour le comboBox
        UpdateCombo()
    End Sub

    '''<summary>
    ''' 'Mettre à jour le comboBox
    '''</summary>

    Private Sub UpdateCombo()
        'Vérifier s'il a été initialisé
        If _isInitialized Then
            'Sélectionner le premier item ou celui par défaut
            SelectedItem = ItemCollection.FirstOrDefault() 'set the default item in the comboBox
        End If

        'Vérifier s'il n'a pas été initialisé
        If Not _isInitialized Then
            'Vider le contenu du ComboBox
            Clear()
            'Indiquer que l'initialisation a été effectuée
            _isInitialized = True
        End If

        Enabled = True 'Activer le ComboBox
        SelectedItem = ItemCollection.FirstOrDefault() 'set the default item in the comboBox
    End Sub

    ''' <summary>
    ''' The on comboBox selection change event. 
    ''' </summary>
    ''' <param name="item">The newly selected combo box item</param>
    Protected Overrides Sub OnSelectionChange(item As ComboBoxItem)
        If (item Is Nothing) Then
            Return
        End If

        If (String.IsNullOrEmpty(item.Text)) Then
            Return
        End If

        'Appel de la routine pour traiter le Snrc entré
        Call OnEnter()
    End Sub

    Protected Overrides Sub OnEnter()
        'Définir le menu des géométries de travail
        Dim dckGeometrieTravail = dckGeometrieTravailViewModel.Current

        'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
        Dim identifyResult = QueuedTask.Run(
            Function()
                'Définir le numéro SNRC
                Dim snrc = New clsSNRC(Me.Text)

                'Vérifier si le SNRC est valide
                If snrc.EstValide Then
                    'Redéfinir le numéro valide
                    Me.Text = snrc.Numero

                    'Vérifier si le numéro est absent
                    If Not Me.ItemCollection.Any(Function(t) (TryCast(t, ComboBoxItem)).Text = snrc.Numero) Then
                        'Ajouter le numéro SNRC dans la liste
                        Add(New ComboBoxItem(snrc.Numero))
                    End If

                    'Créer le polygone géographique à 1 minute d'intervalle
                    Dim polygonGeo = snrc.PolygoneGeo(1)

                    'Transformation du système de coordonnées selon la vue active
                    Dim polygon = GeometryEngine.Instance.Project(polygonGeo, MapView.Active.Map.SpatialReference)

                    'Vérifier si on doit détruire les géométries existantes
                    If modGeometrieTravail.ActiverDetruireGeometrie Then modGeometrieTravail.GeometrieTravail.Clear()

                    'Ajouter le polygone dans les géométries de travail
                    modGeometrieTravail.GeometrieTravail.Add(polygon)
                End If

                'Afficher l'information sur le SNRC
                dckGeometrieTravail.Information = snrc.Information

                'Zoomer et dessiner la liste des géométries de travail
                Call modGeometrieTravail.DessinerListeGeometries(modGeometrieTravail.GeometrieTravail, True, True)

                'Retourner le succès du traitement
                Return True
            End Function)

        'Définir le TreeView
        Dim treview = modGeometrieTravail.TreeViewGeometrieTravail
        'définir l'item des géométries de travail
        Dim item As TreeViewItem = treview.Items.Item(0)
        'Vider l'item
        item.Items.Clear()
        'Fermer le contenu de l'item
        item.IsExpanded = False

        'Activer le DockPane des géométries de travail
        dckGeometrieTravail.Activate()
    End Sub

    Protected Overrides Sub OnUpdate()
        'Désactiver la commande par défaut
        Enabled = False
        'Définir le DockPane des géométries de travail
        Dim dckGeometrieTravail = dckGeometrieTravailViewModel.Current
        'Vérifier si le DockPane est valide
        If dckGeometrieTravail IsNot Nothing And MapView.Active IsNot Nothing Then
            'Vérifier si le DockPane est visible
            If Not (dckGeometrieTravail.DockState = DockPaneState.Hidden Or dckGeometrieTravail.DockState = DockPaneState.None) Then
                'Activer la commande
                Enabled = True
            End If
        End If
    End Sub
End Class
