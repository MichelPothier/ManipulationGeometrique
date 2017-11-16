Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI

'**
'Nom de la composante : cmdActiverMenu 
'
'''<summary>
'''Permet d'activer un menu pour afficher l'information sur les géométries de travail.
'''</summary>
'''
'''<remarks>
''' '''Auteur : Michel Pothier
'''</remarks>
''' 
Public Class cmdActiverMenu
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button
    Private Shared _DockWindow As ESRI.ArcGIS.Framework.IDockableWindow
    Dim gpApp As IApplication = Nothing     'Interface ESRI contenant l'application ArcMap
    Dim gpMxDoc As IMxDocument = Nothing    'Interface ESRI contenant un document ArcMap

    Public Sub New()
        Try
            'Définir les variables de travail
            Dim windowID As UID = New UIDClass
            Dim pGeometryBag As IGeometryBag

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

                    'Créer un nouveau menu
                    windowID.Value = "MPO_BarreGeometrieTravail_dckMenuGeometrieTravail"
                    _DockWindow = My.ArcMap.DockableWindowManager.GetDockableWindow(windowID)

                    'Vérifier la présence du contenant des géométries de travail
                    If mpGeometrieTravail Is Nothing Then
                        'Créer le contenant des géométries de travail vide
                        pGeometryBag = CType(New GeometryBag, IGeometryBag)

                        'Assigner la projection de la fenêtre active
                        pGeometryBag.SpatialReference = gpMxDoc.FocusMap.SpatialReference

                        'Conserver le contenant des géométries de travail
                        mpGeometrieTravail = CType(pGeometryBag, IGeometryCollection)
                    End If

                    'Afficher et dessiner l'information des géométries de travail
                    Call m_MenuGeometrieTravail.InfoGeometrieTravail()

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
            'Sortir si le menu n'est pas créé
            If _DockWindow Is Nothing Then Return

            'Activer ou désactiver le menu
            _DockWindow.Show((Not _DockWindow.IsVisible()))
            Checked = _DockWindow.IsVisible()

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        End Try
    End Sub

    Protected Overrides Sub OnUpdate()
        Try
            Enabled = My.ArcMap.Application IsNot Nothing
        Catch erreur As Exception
            MsgBox(erreur.ToString)
        End Try
    End Sub
End Class


