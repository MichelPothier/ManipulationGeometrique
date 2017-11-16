Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Carto

'**
'Nom de la composante : cmdExporter 
'
'''<summary>
'''Commande qui permet d'exporter les géométries de travail en mémoire dans les éléments graphiques du projet.
'''</summary>
'''
'''<remarks>
'''Ce traitement est utilisable en mode interactif à l'aide de ses interfaces graphiques et doit être
'''utilisé dans ArcMap (ArcGisESRI).
'''
'''Auteur : Michel Pothier
'''</remarks>
Public Class cmdExporter
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button
    Dim gpApp As IApplication = Nothing     'Interface ESRI contenant l'application ArcMap
    Dim gpMxDoc As IMxDocument = Nothing    'Interface ESRI contenant un document ArcMap

    Public Sub New()
        Try
            'Vérifier si l'application est définie
            If Not Hook Is Nothing Then
                'Définir l'application
                gpApp = CType(Hook, IApplication)

                'Vérifier si l'application est ArcMap
                If TypeOf Hook Is IMxApplication Then
                    'Rendre active la commande
                    Enabled = True
                    'Définir le document
                    gpMxDoc = CType(gpApp.Document, IMxDocument)

                Else
                    'Rendre désactive la commande
                    Enabled = False
                End If
            End If

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        End Try
    End Sub

    Protected Overrides Sub OnClick()
        'Déclarer les variables de travail
        Dim pMouseCursor As IMouseCursor = Nothing  'Interface qui permet de changer l'image du curseur
        Dim pActiveView As IActiveView = Nothing    'Interface qui permet de rafraichir la sélection de la vue active
        Dim pGraphicsContainerSelect As IGraphicsContainerSelect = Nothing  'Interface contenant les éléments graphiques sélectionnés
        Dim pGraphicsContainer As IGraphicsContainer = Nothing  'Interface contenant les éléments graphiques
        Dim sReponse As MsgBoxResult = Nothing      'Résultat de la réponse demandée

        Try
            'Changer le curseur en Sablier pour montrer qu'une tâche est en cours
            pMouseCursor = New MouseCursorClass
            pMouseCursor.SetCursor(2)

            'Interface pour sélectionner tous les éléments
            pGraphicsContainerSelect = CType(gpMxDoc.FocusMap, IGraphicsContainerSelect)

            'Sélectionner tous les éléments graphiques
            pGraphicsContainerSelect.SelectAllElements()

            'Vérifier la présence d'éléments
            If pGraphicsContainerSelect.ElementSelectionCount > 0 Then
                'Demander si on veut détruire les Bookmarks existants
                sReponse = MsgBox("Désirez-vous vider les éléments graphiques existants de la Map ? ", vbYesNoCancel, _
                "Exporter les géométries de travail en mémoire vers la Map")
                If sReponse = vbCancel Then Exit Sub

                'Vérifier si on doit vider les éléments de la Map
                If sReponse = vbYes Then
                    'Interface pour détruire tous les éléements
                    pGraphicsContainer = CType(gpMxDoc.FocusMap, IGraphicsContainer)
                    'Vider les éléments graphiques de la Map
                    pGraphicsContainer.DeleteAllElements()
                End If
            End If

            'Appel du module qui effectue le traitement
            modGeometrieTravail.Exporter()

            'Initialiser les variables de travail
            pActiveView = CType(gpMxDoc.FocusMap, IActiveView)

            'Rafraîchier la sélection
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, Nothing, Nothing)

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        Finally
            pMouseCursor = Nothing
            pActiveView = Nothing
            pGraphicsContainerSelect = Nothing
            pGraphicsContainer = Nothing
            sReponse = Nothing
        End Try
    End Sub

    Protected Overrides Sub OnUpdate()
        'Sortir si aucune géométrie de travail
        Enabled = False
        If mpGeometrieTravail Is Nothing Then Exit Sub

        'Vérifier si au moins une géométrie est présente
        If mpGeometrieTravail.GeometryCount > 0 Then
            Enabled = True
        End If
    End Sub
End Class
