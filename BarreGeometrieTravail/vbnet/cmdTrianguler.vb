Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI

'**
'Nom de la composante : cmdTrianguler.vb
'
'''<summary>
'''Commande qui permet de trianguler les éléments sélectionnés de la fenêtre active et de mettre les lignes de triangulation dans la géométrie de travail en mémoire.
'''</summary>
'''
'''<remarks>
'''Ce traitement est utilisable en mode interactif à l'aide de ses interfaces graphiques et doit être
'''utilisé dans ArcMap (ArcGisESRI).
'''
'''Auteur : Michel Pothier
'''</remarks>
''' 
Public Class cmdTrianguler
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
        Dim dDistMin As Double = 20             'Contient la distance minimum entre 2 sommets.
        Dim dLongMin As Double = 50             'Contient la longueur minimum d'une ligne du squellette non connectée.
        Dim sParametres As String = Nothing     'Contient les paramètres du traitement.
        Dim sValeur() As String = Nothing       'Contient les valeurs des paramètres du traitement.

        Try
            'Changer le curseur en Sablier pour montrer qu'une tâche est en cours
            pMouseCursor = New MouseCursorClass
            pMouseCursor.SetCursor(2)

            'Définir la date de début
            Dim dDateDebut As Date = System.DateTime.Now

            'Demander les paramètres du traitement
            sParametres = InputBox("Entrer la distance latérale et la distance minimale", "Créer les lignes des triangles de Delaunay", _
                                   dDistLat.ToString & "," & dDistMin.ToString)
            'Définir les valeurs
            sValeur = sParametres.Split(CChar(","))

            'Vérifier si les paramètres sont valident
            If sValeur.Length = 2 Then
                'Définir la distance latérale
                dDistLat = CDbl(sValeur(0))
                'Définir la distance minimale
                dDistMin = CDbl(sValeur(1))

                'Appel du module qui effectue le traitement
                modGeometrieTravail.CreerLignesTriangles(dDistLat, dDistMin)

                'Afficher et dessiner l'information des géométries de travail
                Call m_MenuGeometrieTravail.InfoGeometrieTravail()
            End If

            Debug.Print(System.DateTime.Now.Subtract(dDateDebut).Add(New TimeSpan(5000000)).ToString.Substring(0, 8))

        Catch ex As Exception
            Throw
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
