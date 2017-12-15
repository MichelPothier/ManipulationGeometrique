Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Core.Data
Imports ArcGIS.Core.Geometry
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Mapping
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Framework.Dialogs

Public Class tooIdentifierGeometrie
    Inherits MapTool

    Public Sub New()
        IsSketchTool = True
        UseSnapping = False
        SketchType = SketchGeometryType.Rectangle
        SketchOutputMode = SketchOutputMode.Screen
    End Sub

    Protected Overrides Function OnToolActivateAsync(active As Boolean) As Task
        Return MyBase.OnToolActivateAsync(active)
    End Function

    Protected Overrides Function OnSketchCompleteAsync(geometry As Geometry) As Task(Of Boolean)
        'Vérifier si la géométrie est invalide, retourner une erreur d'exécution
        If geometry Is Nothing Then Return Task.FromResult(False)

        'Vérifier si la géométrie est vide, retourner une erreur d'exécution
        If geometry.IsEmpty Then Return Task.FromResult(False)

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

                'Définir le polygone à partir de la géométrie
                Dim polygon As Polygon = geometry
                'Vérifier si la superficie du polygone est nulle
                If polygon.Area = 0 Then
                    'Définir la géométrie comme étant un point
                    geometry = polygon.Points.First
                End If

                'Extraire tous les éléments sélectionnés
                Dim pfeatures = activeMapView.GetFeatures(geometry)

                'Extraire les FeatureLayers de la map active 
                Dim layers = activeMapView.Map.GetLayersAsFlattenedList().OfType(Of FeatureLayer)()

                'Traiter tous les FeatureLayers
                For Each layer In layers
                    'Enlever la sélection du Layer
                    layer.ClearSelection()

                    'Si le Layer contient au moins un élément
                    If pfeatures.ContainsKey(layer) Then
                        'Extraire la listes des Oids
                        Dim oids = pfeatures(layer)
                        'Extraire la sélection du Layer
                        Dim selection = layer.GetSelection

                        'Sélectionner les éléments à partir d'une liste de oid
                        selection.Add(oids)
                        'Changer la sélection du Layer
                        layer.SetSelection(selection)

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
                    End If
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

        'Retourner le résultat de l'exécution du traitement
        Return identifyResult
    End Function

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
