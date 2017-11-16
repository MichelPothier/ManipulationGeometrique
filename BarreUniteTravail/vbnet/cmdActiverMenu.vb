Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Framework
'**
'Nom de la composante : cmdActiverMenu.vb
'
'''<summary>
'''Commande qui permet d'activer le menu pour définir une unité de travail et afficher sa description. 
'''Des paramètres d'affichage sont aussi disponibles.
'''</summary>
'''
'''<remarks>
'''Ce traitement est utilisable en mode interactif à l'aide de ses interfaces graphiques et doit être
'''utilisé dans ArcMap (ArcGisESRI).
'''
'''Auteur : Michel Pothier
'''</remarks>
''' 
Public Class cmdActiverMenu
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button
    Private Shared _DockWindow As ESRI.ArcGIS.Framework.IDockableWindow

    Public Sub New()
        Try
            Dim windowID As UID = New UIDClass
            'Créer un nouveau menu
            windowID.Value = "MPO_BarreUniteTravail_dckMenuUniteTravail"
            _DockWindow = My.ArcMap.DockableWindowManager.GetDockableWindow(windowID)

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        End Try
    End Sub

    Protected Overrides Sub OnClick()
        Try
            If _DockWindow Is Nothing Then
                Return
            End If

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
