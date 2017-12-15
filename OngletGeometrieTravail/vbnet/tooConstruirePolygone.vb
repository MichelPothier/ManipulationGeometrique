Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports ArcGIS.Desktop.Editing
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Core.Geometry
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Mapping
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Framework.Dialogs

Friend Class tooConstruirePolygone
    Inherits MapTool

    Public Sub New()
        Snapping.IsEnabled = True
        IsSketchTool = True
        UseSnapping = True
        ' Select the type of construction tool you wish to implement.  
        ' Make sure that the tool is correctly registered with the correct component category type in the daml
        ' SketchType = SketchGeomeTryType.Point
        ' SketchType = SketchGeometryType.Line
        SketchType = SketchGeometryType.Polygon
    End Sub

    ''' <summary>
    ''' Called when the sketch finishes. This is where we will create the sketch operation and then execute it.
    ''' </summary>
    ''' <param name="geometry">The geometry created by the sketch.</param>
    ''' <returns>A Task returning a Boolean indicating if the sketch complete event was successfully handled.</returns>
    Protected Overrides Function OnSketchCompleteAsync(ByVal geometry As Geometry) As Task(Of Boolean)
        'Vérifier si la géométrie est invalide, retourner une erreur d'exécution
        If geometry Is Nothing Then Return Task.FromResult(False)

        'Vérifier si la géométrie est vide, retourner une erreur d'exécution
        If geometry.IsEmpty Then Return Task.FromResult(False)

        'Définir le menu des géométries de travail
        Dim dckGeometrieTravail = dckGeometrieTravailViewModel.Current

        'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
        Dim identifyResult = QueuedTask.Run(
            Function()
                'Conserver en mémoire les géométries de travail
                Dim geometries = modGeometrieTravail.GeometrieTravail

                'Vérifier si on doit détruire les géométries existantes
                If modGeometrieTravail.ActiverDetruireGeometrie Then geometries.Clear()

                'Ajouter la géométrie dans la liste
                geometries.Add(geometry)

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

