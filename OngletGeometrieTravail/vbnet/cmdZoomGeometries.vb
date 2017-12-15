Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Mapping

Friend Class cmdZoomGeometries
    Inherits Button

    Protected Overrides Sub OnClick()
        'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
        Dim identifyResult = QueuedTask.Run(
            Function()
                'Définir la map active
                Dim activeMapView As MapView = MapView.Active
                'Définir les géométries
                Dim geometries = modGeometrieTravail.GeometrieTravail
                'Définir l'enveloppe de la liste des géométries de travail
                Dim envelope = modGeometrieTravail.CalculerEnveloppe(geometries)
                'Agrandir l'envelope
                envelope = envelope.Expand(1.5, 1.5, True)
                'Zoom selon l'envelope agrandit
                activeMapView.ZoomTo(envelope)
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

