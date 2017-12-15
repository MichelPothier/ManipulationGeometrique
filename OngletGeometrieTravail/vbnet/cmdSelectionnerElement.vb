Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Desktop.Mapping
Imports ArcGIS.Desktop.Framework.Threading.Tasks

Friend Class cmdSelectionnerElement
    Inherits Button

    Protected Overrides Sub OnClick()
        'Déclarer les variables de travail
        Dim activeMapView As MapView = MapView.Active   'Contient la Map active.
        'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
        Dim identifyResult = QueuedTask.Run(
            Function()
                'Obtenir la sélection de la map active
                Dim selection = MapView.Active.Map.GetSelection()
                'Effacer la sélection
                selection.Clear()
                'Changer la sélection de la map active
                MapView.Active.Map.SetSelection(selection)
                'Traiter toutes les géométries
                For Each geometrie In modGeometrieTravail.GeometrieTravail
                    'Sélectionner selon la géométrie
                    activeMapView.SelectFeatures(geometrie, SelectionCombinationMethod.Add)
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
                'Vérifier la présence des géométries de travail
                If modGeometrieTravail.GeometrieTravail.Count > 0 Then
                    'Activer la commande
                    Enabled = True
                End If
            End If
        End If
    End Sub
End Class

