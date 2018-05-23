Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Core.Geometry
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Mapping
Imports ArcGIS.Desktop.Framework.Threading.Tasks

Public Class tooIdentifierSnrc50K
    Inherits MapTool

    Public Sub New()
        IsSketchTool = True
        UseSnapping = False
        SketchType = SketchGeometryType.Rectangle
        SketchOutputMode = SketchOutputMode.Map
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
                Dim shape As Geometry = Nothing             'Contient une géométrie d'un élément.
                Dim polygonGeo As Polygon = Nothing         'Contient le polygone en coodonnées géographique.
                Dim polygon As Polygon = Nothing            'Contient le polygone projeté selon la map active.
                Dim envelop As Envelope = Nothing           'Contient l'enveloppe.
                Dim snrc As clsSNRC = Nothing               'Contient l'information d'un SNRC.
                Dim arreter As Boolean = False              'Indiquer si on doit arrêter.

                'Conserver en mémoire les géométries de travail
                Dim geometries = modGeometrieTravail.GeometrieTravail

                'Vérifier si on doit détruire les géométries existantes
                If modGeometrieTravail.ActiverDetruireGeometrie Then geometries.Clear()

                'Définir le rectangle à partir de la géométrie
                Dim rectangle As Polygon = geometry
                'Vérifier si la superficie du polygone est nulle
                If rectangle.Area = 0 Then
                    'Définir l'enveloppe du point
                    envelop = rectangle.Points.First.Extent
                Else
                    'Définir l'enveloppe du polygone
                    envelop = rectangle.Extent
                End If

                'Projeter l'enveloppe en Géographique
                envelop = GeometryEngine.Instance.Project(envelop, SpatialReferenceBuilder.CreateSpatialReference(4617))

                'Initialiser le point de départ
                Dim point As MapPoint = MapPointBuilder.CreateMapPoint(envelop.XMin, envelop.YMax, envelop.SpatialReference)

                'Traiter tous les SNRC pour toutes les lignes
                Do
                    'Traiter tous les SNRCs sur une ligne
                    Do
                        'Définir le numéro SNRC à partir d'un point et de l'échelle 50K
                        snrc = New clsSNRC(point, 50000)

                        'Vérifier si le SNRC est valide
                        If snrc.EstValide Then
                            'Créer le polygone géographique à 1 minute d'intervalle
                            polygonGeo = snrc.PolygoneGeo(1)

                            'Transformation du système de coordonnées selon la vue active
                            polygon = GeometryEngine.Instance.Project(polygonGeo, MapView.Active.Map.SpatialReference)

                            'Ajouter le polygone dans les géométries de travail
                            modGeometrieTravail.GeometrieTravail.Add(polygon)

                            'Indiquer si on veut arreter
                            arreter = polygonGeo.Extent.XMax > envelop.XMax
                        Else
                            'Indiquer qu'on veut arreter
                            arreter = True
                        End If

                        'Définir le point suivant pour traiter le prochain Snrc sur la ligne
                        point = MapPointBuilder.CreateMapPoint(point.X + snrc.DeltaLongitude(point.Y, 50000), point.Y, envelop.SpatialReference)

                    Loop Until point.X > envelop.XMax And arreter


                    'Vérifier si le polygone est invalide
                    If polygon Is Nothing Then
                        'Indiquer si on veut arreter
                        arreter = True
                    Else
                        'Indiquer si on veut arreter
                        arreter = polygonGeo.Extent.YMin < envelop.YMin
                    End If

                    'Réinitialiser le point en X et Y pour traiter une nouvelle ligne
                    point = MapPointBuilder.CreateMapPoint(envelop.XMin, point.Y - snrc.DeltaLatitude(point.Y, 50000), envelop.SpatialReference)
                Loop Until point.Y < envelop.YMin And arreter

                'Afficher l'information sur les géométries de travail
                dckGeometrieTravail.Information = modGeometrieTravail.Current.InfoListeGeometries(modGeometrieTravail.GeometrieTravail)
                'dckGeometrieTravail.Information = dckGeometrieTravail.Information & vbCrLf & snrc.Information

                'Afficher la liste des géométries de travail
                Call modGeometrieTravail.DessinerListeGeometries(modGeometrieTravail.GeometrieTravail)

                'Définir le numéro SNRC dans le ComboBox
                modGeometrieTravail.cboSNRC.Text = snrc.Numero

                'Afficher l'information sur le SNRC
                dckGeometrieTravail.Information = snrc.Information

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
