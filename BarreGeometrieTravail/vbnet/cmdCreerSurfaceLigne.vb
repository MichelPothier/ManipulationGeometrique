﻿Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI

'**
'Nom de la composante : cmdCreerSurfaceLigne
'
'''<summary>
'''Commande qui permet de créer une surface dans les géométries de travail en mémoire 
'''à partir des éléments sélectionnés de type ligne de la fenêtre active.
'''</summary>
'''
'''<remarks>
'''Ce traitement est utilisable en mode interactif à l'aide de ses interfaces graphiques et doit être
'''utilisé dans ArcMap (ArcGisESRI).
'''
'''Auteur : Michel Pothier
'''</remarks>
Public Class cmdCreerSurfaceLigne
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
        Dim pMouseCursor As IMouseCursor = Nothing    'Interface qui permet de changer l'image du curseur

        Try
            'Changer le curseur en Sablier pour montrer qu'une tâche est en cours
            pMouseCursor = New MouseCursorClass
            pMouseCursor.SetCursor(2)

            'Appel du module qui effectue le traitement
            modGeometrieTravail.CreerSurfaceLigne()
            Call m_MenuGeometrieTravail.InfoGeometrieTravail()

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        Finally
            pMouseCursor = Nothing
        End Try
    End Sub

    Protected Overrides Sub OnUpdate()
        'Sortir si aucune géométrie de travail
        Enabled = False
        If mpGeometrieTravail Is Nothing And m_MenuGeometrieTravail Is Nothing Then Exit Sub

        'Vérifier si au moins un élément est sélectionné
        If gpMxDoc.FocusMap.SelectionCount > 0 Then
            Enabled = True
        End If
    End Sub
End Class
