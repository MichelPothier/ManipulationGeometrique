Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI

'**
'Nom de la composante : cmdIdentifierSquelette.vb
'
'''<summary>
'''Commande qui permet d'identifier le squelette dans des géométries de travail en mémoire à partir des éléments de type surface sélectionnés de la fenêtre active.
'''</summary>
'''
'''<remarks>
'''Ce traitement est utilisable en mode interactif à l'aide de ses interfaces graphiques et doit être
'''utilisé dans ArcMap (ArcGisESRI).
'''
'''Auteur : Michel Pothier
'''</remarks>
''' 
Public Class cmdIdentifierSquelette
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
        Dim dDistLat As Double = 7.5            'Contient la distance latérale pour éliminer les sommets inutiles.
        Dim dDistMin As Double = 75              'Contient la distance minimum entre 2 sommets.
        Dim dLongMin As Double = 150             'Contient la longueur minimum d'une ligne du squelette non connectée.
        Dim iMethode As Integer = 0             'Contient la méthode pour créer le squelette.
        Dim sParametres As String = Nothing     'Contient les paramètres du traitement.
        Dim sValeur() As String = Nothing       'Contient les valeurs des paramètres du traitement.

        'Changer le curseur en Sablier pour montrer qu'une tâche est en cours
        pMouseCursor = New MouseCursorClass
        pMouseCursor.SetCursor(2)

        Try
            'Demander les paramètres du traitement
            sParametres = InputBox("Entrer la distance latérale, la distance minimale, la longueur minimale et la méthode (0:Delaunay/1:Voronoi)", "Créer le squelette", _
                                   dDistLat.ToString & "," & dDistMin.ToString & "," & dLongMin.ToString & "," & iMethode.ToString)
            'Définir les valeurs
            sValeur = sParametres.Split(CChar(","))

            'Vérifier si les paramètres sont valident
            If sValeur.Length = 4 Then
                'Définir la date de début
                Dim dDateDebut As Date = System.DateTime.Now

                'Définir la distance latérale
                dDistLat = CDbl(sValeur(0))
                'Définir la distance minimale
                dDistMin = CDbl(sValeur(1))
                'Définir la longueur minimale
                dLongMin = CDbl(sValeur(2))
                'Définir la méthode
                iMethode = CInt(sValeur(3))

                'Appel du module qui effectue le traitement
                modGeometrieTravail.CreerSquelettePolygone(dDistLat, dDistMin, dLongMin, iMethode)

                'Afficher et dessiner l'information des géométries de travail
                If m_MenuGeometrieTravail IsNot Nothing Then Call m_MenuGeometrieTravail.InfoGeometrieTravail()

                Debug.Print(System.DateTime.Now.Subtract(dDateDebut).Add(New TimeSpan(5000000)).ToString.Substring(0, 8))
            End If

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