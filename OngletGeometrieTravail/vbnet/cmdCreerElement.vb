Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Core.Data
Imports ArcGIS.Desktop.Editing
Imports ArcGIS.Desktop.Editing.Templates
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Mapping

Friend Class cmdCreerElement
    Inherits Button

    Protected Overrides Sub OnClick()
        'Définir une nouvelle opération
        Dim op As EditOperation = Nothing

        'Exécuter la fonction qui permet de sélectionner les éléments et d'identifier les géométries
        Dim identifyResult = QueuedTask.Run(
        Function()
            'Définir le FeatureLayer
            Dim featureLayer As FeatureLayer = Nothing

            'Définir le FeatureLayer à partir de la sélection d'un FeatureLayer
            If MapView.Active.GetSelectedLayers.Count = 1 Then
                'Extraire le Layer sélectionné
                Dim Layer = MapView.Active.GetSelectedLayers.Item(0)
                'Vérifier si c'est un FeatureLayer
                If TypeOf (Layer) Is FeatureLayer Then
                    'Définir le FeatureLayer de construction
                    featureLayer = Layer
                End If
            End If

            'Vérifier si le FeatureLayer n'est pas spécifié
            If featureLayer Is Nothing Then
                'Définir le Template de création courant
                Dim template = EditingTemplate.Current
                'Vérifier si le Template est spécifié
                If template IsNot Nothing Then
                    'Définir le FeatureLayer de construction
                    featureLayer = template.Layer
                End If
            End If

            'Vérifier si le FeatureLayer est spécifié
            If featureLayer IsNot Nothing Then
                'Définir une nouvelle opération
                op = New EditOperation()
                'Définir le nom de l'opération
                op.Name = "Créer éléments"

                'Traiter toutes les géométries de la liste
                For Each geometrie In modGeometrieTravail.GeometrieTravail
                    'Vérifier si le type de la géométrie correspond à celui du template
                    If geometrie.GeometryType = featureLayer.GetFeatureClass.GetDefinition.GetShapeType Then
                        'Créer l'élément selon le template et la géométrie
                        op.Create(featureLayer, geometrie)
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
                'Vérifier la présence des géométries de travail
                If modGeometrieTravail.GeometrieTravail.Count > 0 And (MapView.Active.GetSelectedLayers.Count = 1 Or EditingTemplate.Current IsNot Nothing) Then
                    'Activer la commande
                    Enabled = True
                End If
            End If
        End If
    End Sub
End Class

