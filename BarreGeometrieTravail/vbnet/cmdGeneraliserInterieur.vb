Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI

'**
'Nom de la composante : cmdGeneraliserInterieur 
'
'''<summary>
'''Commande qui permet de généraliser l'intérieur des éléments de type surface sélectionnés.
'''</summary>
'''
'''<remarks>
''' '''Auteur : Michel Pothier
'''</remarks>
''' 
Public Class cmdGeneraliserInterieur
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
        Dim dDistLat As Double = 1.5            'Contient la distance latérale pour éliminer les sommets inutiles.
        Dim dLargMin As Double = 150             'Contient la largeur minimum de généralisation.
        Dim dLongMin As Double = 150             'Contient la longueur minimum de généralisation.
        Dim dSupMin As Double = 70000             'Contient la superficie minimum de généralisation.
        Dim sParametres As String = Nothing     'Contient les paramètres du traitement.
        Dim sValeur() As String = Nothing       'Contient les valeurs des paramètres du traitement.

        Try
            'Changer le curseur en Sablier pour montrer qu'une tâche est en cours
            pMouseCursor = New MouseCursorClass
            pMouseCursor.SetCursor(2)

            'Définir la date de début
            Dim dDateDebut As Date = System.DateTime.Now

            'Demander les paramètres du traitement
            'sParametres = InputBox("Entrer la distance latérale, la largeur minimale et la longueur minimale", "Généralisation intérieure d'un polygone", _
            '                       dDistLat.ToString & "," & dLargMin.ToString & "," & dLongMin.ToString)
            sParametres = InputBox("Entrer la largeur minimale, la longueur minimale et la superficie minimale", "Généralisation intérieure d'un polygone", _
                                   dLargMin.ToString & "," & dLongMin.ToString & "," & dSupMin.ToString)
            'Définir les valeurs
            sValeur = sParametres.Split(CChar(","))

            'Vérifier si les paramètres sont valident
            If sValeur.Length = 3 Then
                ''Définir la distance latérale
                'dDistLat = CDbl(sValeur(0))
                ''Définir la distance minimale
                'dLargMin = CDbl(sValeur(1))
                ''Définir la longueur minimale
                'dLongMin = CDbl(sValeur(2))
                'Définir la largeur minimale
                dLargMin = CDbl(sValeur(0))
                'Définir la longueur minimale
                dLongMin = CDbl(sValeur(1))
                'Définir la superficie minimale
                dSupMin = CDbl(sValeur(2))

                'Appel du module qui effectue le traitement
                modGeometrieTravail.GeneraliserPolygoneInterieur(dDistLat, dLargMin, dLongMin, dSupMin)
                'modGeometrieTravail.GeneraliserPolygoneInterieur2(dDistLat, dLargMin, dLongMin, dSupMin)

                'Afficher et dessiner l'information des géométries de travail
                Call m_MenuGeometrieTravail.InfoGeometrieTravail()
            End If

            Debug.Print(System.DateTime.Now.Subtract(dDateDebut).Add(New TimeSpan(5000000)).ToString.Substring(0, 8))

        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            'Vider la mémoire
            pMouseCursor = Nothing
        End Try
    End Sub

    Protected Overrides Sub OnUpdate()
        If gpMxDoc.FocusMap.SelectionCount > 0 And m_MenuGeometrieTravail IsNot Nothing Then
            Enabled = True
        Else
            Enabled = False
        End If
    End Sub
End Class
