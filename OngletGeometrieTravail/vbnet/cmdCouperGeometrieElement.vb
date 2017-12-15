Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Core.CIM
Imports ArcGIS.Core.Data
Imports ArcGIS.Core.Geometry
Imports ArcGIS.Desktop.Editing
Imports ArcGIS.Desktop.Editing.Attributes
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Mapping

Friend Class cmdCouperGeometrieElement
    Inherits Button

    Protected Overrides Sub OnClick()
        'Définir une nouvelle opération
        Dim op As EditOperation = Nothing

        'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
        Dim identifyResult = QueuedTask.Run(
            Function()
                'Définir les variables de travail
                Dim feature As Feature = Nothing            'Contient un élément d'un FeatureLayer.
                Dim cutLine As Polyline = Nothing           'Contient la polyligne utilisée pour couper les géométries des éléments.

                'Conserver en mémoire les géométries de travail
                For Each geometry In modGeometrieTravail.UnionListeGeometries(modGeometrieTravail.GeometrieTravail)
                    'vérifier si la géométrie est de type ligne
                    If geometry.GeometryType = GeometryType.Polyline Then
                        'Définir la ligne utilisée pour couper
                        cutLine = geometry
                        'Sortir
                        Exit For
                    End If
                Next

                'vérifier si la ligne pour couper est valide
                If cutLine IsNot Nothing Then
                    'Définir une nouvelle opération
                    op = New EditOperation()
                    'Définir le nom de l'opération
                    op.Name = "Couper la géométrie des éléments"

                    'Extraire les FeatureLayers de la map active 
                    Dim layers = MapView.Active.Map.GetLayersAsFlattenedList().OfType(Of FeatureLayer)()

                    'Traiter tous les FeatureLayers
                    For Each layer In layers
                        'Vérifier si on peut modifier le Layer
                        If layer.CanEditData Then
                            'Vérifier si la géométrie du Layer peut être coupée
                            If layer.ShapeType = esriGeometryType.esriGeometryPolyline Or layer.ShapeType = esriGeometryType.esriGeometryPolygon Then
                                'Extraire la sélection du Layer
                                Dim selection = layer.GetSelection

                                'Extraire les éléments sélectionnés
                                Dim rowCursor = selection.Search(Nothing, False)

                                'Traiter tous les éléments
                                Do While rowCursor.MoveNext()
                                    'Extraire l'élément du FeatureLayer
                                    feature = rowCursor.Current()
                                    'Définir la géométrie de l'élément
                                    Dim geometrie = GeometryEngine.Instance.Project(feature.GetShape, MapView.Active.Map.SpatialReference)
                                    'Vérifier si les géométries se croise
                                    If GeometryEngine.Instance.Crosses(geometrie, cutLine) Then
                                        'Vérifier si la géométrie est de type ligne
                                        If geometrie.GeometryType = GeometryType.Polyline Then
                                            'Modifier l'élément
                                            op.Cut(layer, feature.GetObjectID, cutLine)
                                            'Si la géométrie est une surface
                                        ElseIf GeometryEngine.Instance.relate(geometrie, cutLine, "TFT******") Then
                                            'Modifier l'élément
                                            op.Cut(layer, feature.GetObjectID, cutLine)
                                        End If
                                    End If
                                Loop
                            End If
                        End If
                    Next

                    'Si aucune opération
                    If op.IsEmpty Then
                        'annuler l'opération
                        op.Abort()
                        'si une opération a été effectuée
                    Else
                        'Exécuter l'opération
                        op.ExecuteAsync()
                    End If
                End If

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
                If MapView.Active.Map.SelectionCount > 0 Then
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

