Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Mapping

Friend Class cmdBufferGeometries
    Inherits Button

    Protected Overrides Sub OnClick()
        'Définir la distance du buffer par défaut en mètre
        Dim dist As Double = 5.0
        'Vérifier si la référence spatiale est géographique
        If MapView.Active.Map.SpatialReference.IsGeographic Then
            'définir la valeur par défaut en degrés
            dist = 0.0000003
        End If

        'Demander la valeur de la distance
        Dim reponse = InputBox("Distance du tampon (buffer) ? ", "Créer des tampons (buffer)", dist.ToString)

        'Vérifier le résultat de la valeur du tampon
        If IsNumeric(reponse) Then
            'Définir la distance
            dist = CDbl(reponse)
            'Définir le TreeView des géométries
            Dim treeView As TreeView = modGeometrieTravail.TreeViewGeometrieTravail

            'Définir l'item du TreeView Sélectionné
            Dim treeViewSelected As TreeViewItem = treeView.Items.Item(0)

            'Définir l'item du TreeView Sélectionné
            treeViewSelected.IsSelected = True

            'Définir le menu des géométries de travail
            Dim dckGeometrieTravail = dckGeometrieTravailViewModel.Current

            'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
            Dim identifyResult = QueuedTask.Run(
                Function()
                    'Buffer de la liste des géométries de travail
                    modGeometrieTravail.GeometrieTravail = modGeometrieTravail.BufferListeGeometries(modGeometrieTravail.GeometrieTravail, dist)

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
        End If
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

    'Protected Overrides Sub OnClick()
    '    Dim maxSteps As UInteger = 10
    '    Dim pd = New ArcGIS.Desktop.Framework.Threading.Tasks.ProgressDialog("Exécution en cours ...", "Annuler", maxSteps, False)
    '    Call RunCancelableProgress(New CancelableProgressorSource(pd), maxSteps)
    'End Sub

    'Public Shared Function RunCancelableProgress(ByVal cps As CancelableProgressorSource, ByVal howLongInSeconds As UInteger) As Task
    '    Return QueuedTask.Run(Function()
    '                              cps.Progressor.Max = CUInt(howLongInSeconds)
    '                              While Not cps.Progressor.CancellationToken.IsCancellationRequested
    '                                  cps.Progressor.Value += 1
    '                                  cps.Progressor.Status = (cps.Progressor.Value * 100 / cps.Progressor.Max) & " % Completed"
    '                                  cps.Progressor.Message = "Message " & cps.Progressor.Value
    '                                  If System.Diagnostics.Debugger.IsAttached Then
    '                                      System.Diagnostics.Debug.WriteLine(String.Format("RunCancelableProgress Loop{0}", cps.Progressor.Value))
    '                                  End If

    '                                  Task.Delay(1000).Wait()
    '                                  If cps.Progressor.Value = cps.Progressor.Max Then Exit While
    '                              End While

    '                              System.Diagnostics.Debug.WriteLine(String.Format("RunCancelableProgress: Canceled {0}", cps.Progressor.CancellationToken.IsCancellationRequested))
    '                              Return True
    '                          End Function, cps.Progressor)
    'End Function
End Class

