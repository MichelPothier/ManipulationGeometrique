Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geometry

'**
'Nom de la composante : cmdImporter 
'
'''<summary>
'''Commande qui permet d'importer les géométries de travail contenu dans les éléments graphiques du MapFrame.
'''</summary>
'''
'''<remarks>
'''Ce traitement est utilisable en mode interactif à l'aide de ses interfaces graphiques et doit être
'''utilisé dans ArcMap (ArcGisESRI).
'''
'''Auteur : Michel Pothier
'''</remarks>
Public Class cmdImporter
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
        Dim pGraphicsContainerSelect As IGraphicsContainerSelect = Nothing  'Interface contenant les éléments graphiques
        Dim sReponse As MsgBoxResult = Nothing  'Contient la réponse à la question

        Try
            'Changer le curseur en Sablier pour montrer qu'une tâche est en cours
            pMouseCursor = New MouseCursorClass
            pMouseCursor.SetCursor(2)

            'Interface pour sélectionner tous les éléments
            pGraphicsContainerSelect = CType(gpMxDoc.FocusMap, IGraphicsContainerSelect)

            'Sélectionner tous les éléments
            pGraphicsContainerSelect.SelectAllElements()

            'Vérifier la présence d'éléments
            If pGraphicsContainerSelect.ElementSelectionCount > 0 Then
                'Vérifier la présence de Bookmarks
                If mpGeometrieTravail.GeometryCount > 0 Then
                    'Déclarer les variables de travail

                    'Demander si on veut conserver les Bookmarks en mémoire
                    sReponse = MsgBox("Désirez-vous vider les géométries de travail existants en mémoire ? ", vbYesNoCancel, _
                    "Exporter les éléments graphiques de la Map vers la mémoire")
                    If sReponse = vbCancel Then Exit Sub

                    'Vérifier si on doit vider les Bookmark de la Map
                    If sReponse = vbYes Then
                        'Vider les Bookmarks de la Map
                        mpGeometrieTravail = CType(New GeometryBag, IGeometryCollection)
                    End If
                End If

                'Sinon
            Else
                'Message d'information
                MsgBox("ATTENTION : Aucun élément graphique présent")
            End If

            'Appel du module qui effectue le traitement
            modGeometrieTravail.Importer()

            'Initialiser les couleurs
            'Call modGeometrieTravail.InitSymbole()

            'Afficher l'information du numéro de la géométrie de travail
            Call m_MenuGeometrieTravail.InfoGeometrieTravail()

            'Rafraîchir le menu
            'm_MenuGeometrieTravail.Refresh()

            'Appel du module qui effectue le traitement pour dessiner les géométries de travail
            'modGeometrieTravail.bDessinerGeometrie(CType(mpGeometrieTravail, IGeometry), True)

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        Finally
            pMouseCursor = Nothing
            pGraphicsContainerSelect = Nothing
            sReponse = Nothing
        End Try

    End Sub

    Protected Overrides Sub OnUpdate()
        'Vérifier si la géométrie de travail est présente
        If mpGeometrieTravail IsNot Nothing Then
            Enabled = True
        Else
            Enabled = False
        End If
    End Sub
End Class
