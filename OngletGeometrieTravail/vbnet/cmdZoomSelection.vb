Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Mapping

Friend Class cmdZoomSelection
    Inherits Button

    Protected Overrides Sub OnClick()
        'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
        Dim identifyResult = QueuedTask.Run(
            Function()
                'Obtenir la sélection de la map active
                MapView.Active.ZoomToSelected()
                'Retourner le succès du traitement
                Return True
            End Function)
    End Sub

    Protected Overrides Sub OnUpdate()
        'Désactiver la commande par défaut
        Enabled = False
        'Vérifier si vue est active
        If MapView.Active IsNot Nothing Then
            'Vérifier si au moins un élément est sélectionné
            If MapView.Active.Map.SelectionCount > 0 Then
                'Activer la commande
                Enabled = True
            End If
        End If
    End Sub
End Class

