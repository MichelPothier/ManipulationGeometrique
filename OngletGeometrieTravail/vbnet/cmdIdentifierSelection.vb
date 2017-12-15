Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Core.Data
Imports ArcGIS.Core.Geometry
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Mapping

Friend Class cmdIdentifierSelection
    Inherits Button

    Protected Overrides Sub OnClick()
        'Définir le menu des géométries de travail
        Dim dckGeometrieTravail = dckGeometrieTravailViewModel.Current

        'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
        Dim identifyResult = QueuedTask.Run(
            Function()
                'Définir les variables de travail
                Dim activeMapView = MapView.Active          'Contient la Map active.
                Dim feature As Feature = Nothing            'Contient un élément d'un FeatureLayer.
                Dim shape As Geometry = Nothing             'Contient une géométrie d'un élément.

                'Conserver en mémoire les géométries de travail
                Dim geometries = modGeometrieTravail.GeometrieTravail

                'Vérifier si on doit détruire les géométries existantes
                If modGeometrieTravail.ActiverDetruireGeometrie Then geometries.Clear()

                'Extraire les FeatureLayers de la map active 
                Dim layers = activeMapView.Map.GetLayersAsFlattenedList().OfType(Of FeatureLayer)()

                'Traiter tous les FeatureLayers
                For Each layer In layers
                    'Extraire la sélection du Layer
                    Dim selection = layer.GetSelection

                    'Extraire les éléments sélectionnés
                    Dim rowCursor = selection.Search(Nothing, False)

                    'Traiter tous les éléments
                    Do While rowCursor.MoveNext()
                        'Extraire l'élément du FeatureLayer
                        feature = rowCursor.Current()
                        'Extraire la géométrie de l'élément
                        shape = feature.GetShape
                        'Projeter la géométrie
                        shape = GeometryEngine.Instance.Project(shape, activeMapView.Map.SpatialReference)
                        'Ajouter la géométrie dans la liste des géométries
                        geometries.Add(shape)
                    Loop
                Next

                'Afficher l'information sur les géométries de travail
                dckGeometrieTravail.Information = modGeometrieTravail.Current.InfoListeGeometries(modGeometrieTravail.GeometrieTravail)

                'Afficher la liste des géométries de travail
                Call modGeometrieTravail.DessinerListeGeometries(modGeometrieTravail.GeometrieTravail)

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
                'Vérifier si au moins un élément est sélectionné
                If MapView.Active.Map.SelectionCount > 0 Then
                    'Activer la commande
                    Enabled = True
                End If
            End If
        End If
    End Sub
End Class

