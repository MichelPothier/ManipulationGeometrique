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

Friend Class cmdDifferenceInverseGeometrieElement
    Inherits Button

    Protected Overrides Sub OnClick()
        'Définir une nouvelle opération
        Dim op As EditOperation = Nothing

        'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
        Dim identifyResult = QueuedTask.Run(
            Function()
                'Définir les variables de travail
                Dim feature As Feature = Nothing            'Contient un élément d'un FeatureLayer.
                Dim multipoint As Multipoint = Nothing      'Contient le multipoint.
                Dim polyline As Polyline = Nothing          'Contient la polyligne.
                Dim polygon As Polygon = Nothing            'Contient le polygon.
                Dim geometrie As Geometry = Nothing         'Contient la géométrie de l'élément.
                Dim resultat As Geometry = Nothing          'Contient la géométrie résultante.
                Dim modifyInspector As Inspector = New Inspector()  'Contient la classe pour modifier un élément.

                'vérifier si la ligne pour couper est valide
                If modGeometrieTravail.GeometrieTravail.Count > 0 Then
                    'Conserver en mémoire les géométries de travail
                    For Each geometry In modGeometrieTravail.UnionListeGeometries(modGeometrieTravail.GeometrieTravail)
                        'Vérifier si la géométrie est de type point
                        If geometry.GeometryType = GeometryType.Multipoint Then
                            'Définir le multipoint
                            multipoint = geometry
                            'Vérifier si la géométrie est de type ligne
                        ElseIf geometry.GeometryType = GeometryType.Polyline Then
                            'Définir la polyligne
                            polyline = geometry
                            'Vérifier si la géométrie est de type surface
                        ElseIf geometry.GeometryType = GeometryType.Polygon Then
                            'Définir le polygon
                            polygon = geometry
                        End If
                    Next

                    'Définir une nouvelle opération
                    op = New EditOperation()
                    'Définir le nom de l'opération
                    op.Name = "Différence inverse éléments"

                    'Extraire les FeatureLayers de la map active 
                    Dim layers = MapView.Active.Map.GetLayersAsFlattenedList().OfType(Of FeatureLayer)()

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
                                'Projeter la géométrie
                                geometrie = GeometryEngine.Instance.Project(feature.GetShape, MapView.Active.Map.SpatialReference)
                                'Initialiser le résultat
                                resultat = geometrie.Clone

                                'Si le Multipoint est défini
                                If multipoint IsNot Nothing AndAlso resultat.GeometryType = GeometryType.Multipoint Then
                                    'Vérifier si les géométries s'intersectent
                                    If GeometryEngine.Instance.Intersects(resultat, multipoint) Then
                                        'Calculer le résultat de la différence
                                        resultat = GeometryEngine.Instance.Difference(multipoint, resultat)
                                    End If
                                End If

                                'Si la Polyline est définie
                                If polyline IsNot Nothing AndAlso resultat.GeometryType = GeometryType.Polyline Then
                                    'Vérifier si les géométries s'intersectent
                                    If GeometryEngine.Instance.Intersects(resultat, polyline) Then
                                        'Calculer le résultat de la différence
                                        resultat = GeometryEngine.Instance.Difference(polyline, resultat)
                                    End If
                                End If

                                'Si le Polygon est défini
                                If polygon IsNot Nothing AndAlso resultat.GeometryType = GeometryType.Polygon Then
                                    'Vérifier si les géométries s'intersectent
                                    If GeometryEngine.Instance.Intersects(resultat, polygon) Then
                                        'Calculer le résultat de la différence
                                        resultat = GeometryEngine.Instance.Difference(polygon, resultat)
                                    End If
                                End If

                                'Vérifier si le résultat est différent de l'originale
                                If Not GeometryEngine.Instance.Equals(geometrie, resultat) Then
                                    'Vérifier si un résultat n'est pas vide
                                    If resultat.IsEmpty Then
                                        'Détruire l'élément
                                        op.Delete(layer, feature.GetObjectID)

                                        'Si un résultat n'est pas vide
                                    Else
                                        'Définir l'élément à modifier
                                        modifyInspector.LoadAsync(layer, feature.GetObjectID)
                                        'Remplacer la géométrie
                                        modifyInspector(layer.GetFeatureClass.GetDefinition.GetShapeField) = resultat
                                        'Modifier l'élément
                                        op.Modify(modifyInspector)
                                    End If
                                End If
                            Loop
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

