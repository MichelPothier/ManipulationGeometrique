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

Friend Class cmdIdentifierSurfaceLigne
    Inherits Button

    Protected Overrides Sub OnClick()
        'Définir le menu des géométries de travail
        Dim dckGeometrieTravail = dckGeometrieTravailViewModel.Current

        'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
        Dim identifyResult = QueuedTask.Run(
            Function()
                'Définir les variables de travail
                Dim feature As Feature = Nothing            'Contient un élément d'un FeatureLayer.
                Dim polyline As Polyline = Nothing          'Contient une géométrie d'un élément.

                'Définir la référence spatiale
                Dim spatialRef As SpatialReference = MapView.Active.Map.SpatialReference
                'Classe pour construire une ligne
                Dim polylineBuilder = New PolylineBuilder(spatialRef)
                'Classe pour construire une surface
                Dim polygonBuilder = New PolygonBuilder(spatialRef)

                'Conserver en mémoire les géométries de travail
                Dim geometries = modGeometrieTravail.GeometrieTravail

                'Vérifier si on doit détruire les géométries existantes
                If modGeometrieTravail.ActiverDetruireGeometrie Then geometries.Clear()

                'Extraire les FeatureLayers de la map active 
                Dim layers = MapView.Active.Map.GetLayersAsFlattenedList().OfType(Of FeatureLayer)()

                'Traiter tous les FeatureLayers
                For Each layer In layers
                    'Vérifier si le Layer est de type Polyline
                    If layer.ShapeType = ArcGIS.Core.CIM.esriGeometryType.esriGeometryPolyline Then
                        'Extraire la sélection du Layer
                        Dim selection = layer.GetSelection

                        'Extraire les éléments sélectionnés
                        Dim rowCursor = selection.Search(Nothing, False)
                        'Traiter tous les éléments
                        Do While rowCursor.MoveNext()
                            'Extraire l'élément du FeatureLayer
                            feature = rowCursor.Current()
                            'Extraire la géométrie de l'élément
                            polyline = feature.GetShape
                            'Projeter la géométrie
                            polyline = GeometryEngine.Instance.Project(polyline, spatialRef)
                            'Ajouter les parties de la polyline
                            polylineBuilder.AddParts(polyline.Parts)
                        Loop
                    End If
                Next

                'Simplifier la géométrie
                polyline = polylineBuilder.ToGeometry

                'Simplifier la géométrie
                polyline = GeometryEngine.Instance.SimplifyPolyline(polyline, SimplifyType.Planar, True)

                'Traiter toutes les lignes de la polylignes
                For Each part In polyline.Parts
                    'Vérifier si la ligne est fermée
                    If part.First.StartPoint.IsEqual(part.Last.EndPoint) Then
                        'Ajouter la ligne fermée au polygone
                        polygonBuilder.AddPart(part)
                    End If
                Next

                'Créer le polygone
                Dim polygon = polygonBuilder.ToGeometry
                'Vérifier si le polygone n'est pas vide
                If Not polygon.IsEmpty Then
                    'Ajouter la géométrie dans la liste des géométries
                    geometries.Add(polygon)
                End If

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

