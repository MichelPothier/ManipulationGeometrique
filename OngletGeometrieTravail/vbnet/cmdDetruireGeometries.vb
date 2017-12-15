Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Mapping

Friend Class cmdDetruireGeometries
    Inherits Button

    Protected Overrides Sub OnClick()
        'Définir le TreeView des géométries
        Dim treeView As TreeView = modGeometrieTravail.TreeViewGeometrieTravail

        'Définir l'item du TreeView Sélectionné
        Dim treeViewSelected As TreeViewItem = treeView.Items.Item(0)

        'Définir l'item du TreeView Sélectionné
        treeViewSelected.IsSelected = True

        'Redéfinir la nouvelle liste des géométries de travail
        modGeometrieTravail.GeometrieTravail.RemoveRange(0, modGeometrieTravail.GeometrieTravail.Count)

        'Définir le menu des géométries de travail
        Dim dckGeometrieTravail = dckGeometrieTravailViewModel.Current

        'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
        Dim identifyResult = QueuedTask.Run(
        Function()
            'Afficher l'information sur les géométries de travail
            dckGeometrieTravail.Information = modGeometrieTravail.Current.InfoListeGeometries(modGeometrieTravail.GeometrieTravail)

            'Afficher la liste des géométries de travail
            Call modGeometrieTravail.DetruireAffichage()

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
                'Vérifier la présence des géométries de travail
                If modGeometrieTravail.GeometrieTravail.Count > 0 Then
                    'Activer la commande
                    Enabled = True
                End If
            End If
        End If
    End Sub
End Class

