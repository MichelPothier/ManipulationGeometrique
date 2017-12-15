Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Core.Data
Imports ArcGIS.Core.Geometry
Imports ArcGIS.Desktop.Editing
Imports ArcGIS.Desktop.Editing.Attributes
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Mapping

Friend Class cmdRemplacerGeometrieElement
    Inherits Button

    Protected Overrides Sub OnClick()
        'Définir une nouvelle opération
        Dim op As EditOperation = Nothing

        'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
        Dim identifyResult = QueuedTask.Run(
            Function()
                'Définir les variables de travail
                Dim activeMapView = MapView.Active          'Contient la Map active.
                Dim feature As Feature = Nothing            'Contient un élément d'un FeatureLayer.
                Dim geometrie As Geometry = Nothing         'Contient une géométrie d'un élément.
                Dim modifyInspector As Inspector = Nothing  'Contient la classe pour modifier un élément.

                'Conserver en mémoire les géométries de travail
                Dim geometries = modGeometrieTravail.GeometrieTravail

                'Extraire les FeatureLayers de la map active 
                Dim layers = activeMapView.Map.GetLayersAsFlattenedList().OfType(Of FeatureLayer)()

                'Traiter tous les FeatureLayers
                For Each layer In layers
                    'Vérifier si on peut modifier le Layer
                    If layer.CanEditData Then
                        'Extraire la sélection du Layer
                        Dim selection = layer.GetSelection

                        'Extraire les éléments sélectionnés
                        Dim rowCursor = selection.Search(Nothing, False)

                        'Traiter tous les éléments
                        Do While rowCursor.MoveNext()
                            'Extraire l'élément du FeatureLayer
                            feature = rowCursor.Current()
                            'Traiter toutes les géométries
                            For Each geometrie In geometries
                                'Vérifier si le type de géométrie est le même que celui de l'élément
                                If geometrie.GeometryType = feature.GetShape.GeometryType Then
                                    'Sélectionner le Layer
                                    Dim lyrs = MapView.Active.Map.FindLayers(layer.Name)
                                    MapView.Active.SelectLayers(lyrs)

                                    'Définir une nouvelle opération
                                    op = New EditOperation()
                                    'Définir le nom de l'opération
                                    op.Name = "Remplacer la géométrie d'un élément"

                                    'Classe pour modifier un élément
                                    modifyInspector = New Inspector()
                                    'Définir l'élément à modifier
                                    modifyInspector.LoadAsync(layer, feature.GetObjectID)
                                    'Remplacer la géométrie
                                    modifyInspector(layer.GetFeatureClass.GetDefinition.GetShapeField) = geometrie
                                    'Modifier l'élément
                                    op.Modify(modifyInspector)
                                    'Exécuter l'opération
                                    op.ExecuteAsync()
                                    'Sortir de la fonction
                                    Return True
                                End If
                            Next
                        Loop
                    End If
                Next

                'Retourner le succès du traitement
                Return True
            End Function)
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
                'Vérifier si un seul élément est sélectionné
                If MapView.Active.Map.SelectionCount = 1 Then
                    'Vérifier la présence des géométries de travail
                    If modGeometrieTravail.GeometrieTravail.Count > 0 Then
                        'Activer la commande
                        Enabled = True
                    End If
                End If
            End If
        End If
    End Sub
End Class

