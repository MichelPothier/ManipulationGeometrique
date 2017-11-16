Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI

'**
'Nom de la composante : cmdDifferenceInverser.vb
'
'''<summary>
'''Commande qui permet d'effectuer la différence inverser (NOTAND) entre les géométries de
'''travail en mémoire et les éléments sélectionnés.
'''</summary>
'''
'''<remarks>
'''Ce traitement est utilisable en mode interactif à l'aide de ses interfaces graphiques et doit être
'''utilisé dans ArcMap (ArcGisESRI).
'''
'''Auteur : Michel Pothier
'''</remarks>
''' 
Public Class cmdDifferenceInverser
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
            modGeometrieTravail.DifferenceInverser()
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

        'Vérifier si au moins une géométrie est présente et un élément est sélectionné
        If mpGeometrieTravail.GeometryCount > 0 And gpMxDoc.FocusMap.SelectionCount > 0 Then
            Enabled = True
        End If
    End Sub
End Class
