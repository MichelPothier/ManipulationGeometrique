Imports System.Runtime.InteropServices
Imports System.Drawing
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI

'**
'Nom de la composante : cmdRemplacerGeometrie.vb
'
'''<summary>
'''Commande qui permet de remplacer la géométrie de l'élément sélectionné à partir de la géométrie
'''de l'unité de travail en mémoire.
'''</summary>
'''
'''<remarks>
'''Ce traitement est utilisable en mode interactif à l'aide de ses interfaces graphiques et doit être
'''utilisé dans ArcMap (ArcGisESRI).
'''
'''Auteur : Michel Pothier
'''</remarks>
''' 
Public Class cmdRemplacerGeometrie
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
        Try
            'Remplacer la géométrie de l'élément sélectionné selon la géométrie de l'unité de travail en mémoire
            modUniteTravail.RemplacerGeometrieUniteTravail()

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        End Try
    End Sub

    Protected Overrides Sub OnUpdate()
        Try
            Enabled = m_UniteTravail IsNot Nothing And gpMxDoc.FocusMap.SelectionCount > 0
            If Enabled Then Enabled = m_UniteTravail.EstValide

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        End Try
    End Sub
End Class
