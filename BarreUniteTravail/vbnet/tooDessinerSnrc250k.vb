Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Display
'**
'Nom de la composante : TooDessinerPointSnrc250K.vb
'
'''<summary>
'''Commande qui permet de dessiner dans la fenêtre graphique la géométrie du SNRC 250K en fonction du point donnée.
'''</summary>
'''
'''<remarks>
'''Ce traitement est utilisable en mode interactif à l'aide de ses interfaces graphiques et doit être
'''utilisé dans ArcMap (ArcGisESRI).
'''
'''Auteur : Michel Pothier
'''</remarks>
''' 
Public Class tooDessinerSnrc250k
    Inherits ESRI.ArcGIS.Desktop.AddIns.Tool

    'Déclarer les variable de travail
    Private gpApp As IApplication
    Private gpMxDoc As IMxDocument
    Private mpPoint As IPoint
    Private mpFeedbackEnv As INewEnvelopeFeedback
    Private mbMouseDown As Boolean

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

    Protected Overrides Sub OnMouseDown(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)
        'On ne fait rien si ce n'est pas le button gauche de la souris
        If arg.Button <> Windows.Forms.MouseButtons.Left Then Exit Sub

        Try
            'Écrire une trace de début
            'Call citsMod_FichierTrace.EcrireMessageTrace("Debut")

            'Transformer la position de la souris en postion de la map
            mpPoint = gpMxDoc.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y)

            'Conserver le point de la position actuelle de la souris et indiquer le début du traitement
            mbMouseDown = True

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        Finally
            'Écrire une trace de fin
            'Call citsMod_FichierTrace.EcrireMessageTrace("Fin")
            'Vider la mémoire
        End Try
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)
        Try
            'Écrire une trace de début
            'Call citsMod_FichierTrace.EcrireMessageTrace("Debut")

            'Vérifier si le traitement a été initialisé
            If mpPoint Is Nothing Or mbMouseDown = False Then
                Exit Sub
            End If

            'vérifier si l'enveloppe a été initialisée
            If (mpFeedbackEnv Is Nothing) Then
                'Créer l'enveloppe virtuelle initiale pour définir la zone d'affichage
                mpFeedbackEnv = New NewEnvelopeFeedback
                'Définir la fenêtre d'affichage
                mpFeedbackEnv.Display = gpMxDoc.ActiveView.ScreenDisplay
                'Initialiser le premier point de l'enveloppe virtuelle
                mpFeedbackEnv.Start(mpPoint)
            End If

            'Transformer la position de la souris en postion de la map
            mpPoint = gpMxDoc.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y)

            'Bouger l'enveloppe au point de la position actuelle de la souris
            mpFeedbackEnv.MoveTo(mpPoint)

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        Finally
            'Écrire une trace de fin
            'Call citsMod_FichierTrace.EcrireMessageTrace("Fin")
        End Try
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)
        'Déclarer les variables de travail
        Dim pEnvelope As IEnvelope = Nothing                'Interface contenant l'enveloppe utilisé pour afficher les SNRCs
        Dim pPoint As IPoint = Nothing                      'Interface contenant le point de recherche du SNRC
        Dim oSnrc As clsSNRC = Nothing                      'Objet contenant l'information d'un SNRC
        Dim pPolygon As IPolygon = Nothing                  'Interface ESRI contenant le polygone de l'unité de travail.
        Dim pScreenDisplay As IScreenDisplay = Nothing      'Interface ESRI contenant la fenêtre d'affichage.
        Dim pTrackCancel As ITrackCancel = Nothing          'Interface pour permet l'arrêt de l'affichage
        Dim bArreter As Boolean = False                     'Indique si on veut arrêter

        Try
            'Initialiser les variables de travail
            pScreenDisplay = gpMxDoc.ActiveView.ScreenDisplay

            'Si aucune enveloppe n'est captée, c'est un point qui est capté
            If mpFeedbackEnv Is Nothing Then
                'Sortir si aucun point n'a été défini
                If mpPoint Is Nothing Then
                    Exit Sub
                End If
                'Définir l'enveloppe d'affichage
                pEnvelope = mpPoint.Envelope

                'Si c'est une enveloppe qui est captée
            Else
                'Définir l'enveloppe d'affichage
                pEnvelope = mpFeedbackEnv.Stop

                'Il faut que l'enveloppe soit dans la même projection que le projet.  
                'On peut utiliser la référence spatiale du point de départ.
                pEnvelope.SpatialReference = mpPoint.SpatialReference
            End If
            'Projeter l'enveloppe
            pEnvelope.Project(ReferenceSpatialeGeoNad83)

            'Initialiser l'interface d'annlation du traitement
            pTrackCancel = New CancelTracker
            pTrackCancel.CancelOnKeyPress = True

            'Initialiser le point de départ
            pPoint = pEnvelope.UpperLeft

            'Traiter tous les SNRC pour toutes les lignes
            Do
                'Traiter tous les SNRCs sur une ligne
                Do
                    'Définir le numéro SNRC à partir d'un point et de l'échelle 250K
                    oSnrc = New clsSNRC(pPoint, 250000)

                    'Vérifier si le SNRC est valide
                    If oSnrc.EstValide Then
                        'Créer le polygone de l'unité de travail
                        pPolygon = oSnrc.PolygoneGeo(1)

                        'Transformation du système de coordonnées selon la vue active
                        pPolygon.Project(gpMxDoc.FocusMap.SpatialReference)
                        pPoint.Project(gpMxDoc.FocusMap.SpatialReference)

                        'Afficher la géométrie avec sa symbologie dans la vue active
                        With pScreenDisplay
                            .StartDrawing(pScreenDisplay.hDC, CShort(esriScreenCache.esriNoScreenCache))
                            'Dessiner la surface
                            .SetSymbol(m_SymboleSurface)
                            .DrawPolygon(pPolygon)
                            'Dessiner le point
                            .SetSymbol(m_SymbolePoint)
                            .DrawPoint(pPoint)
                            'Dessiner le texte
                            .SetSymbol(m_SymboleTexte)
                            .DrawText(pPoint, oSnrc.Numero)
                            .FinishDrawing()
                        End With

                        'Transformation du système de coordonnées selon la vue active
                        pPolygon.Project(ReferenceSpatialeGeoNad83)
                        pPoint.Project(ReferenceSpatialeGeoNad83)

                        'Indiquer si on veut arreter
                        bArreter = pPolygon.Envelope.XMax > pEnvelope.XMax
                    Else
                        'Indiquer qu'on veut arreter
                        bArreter = True
                    End If

                    'Traiter le Snrc suivant sur la ligne
                    pPoint.X = pPoint.X + oSnrc.DeltaLongitude(pPoint.Y, 250000)

                    'Sortir si annuler
                    If pTrackCancel.Continue = False Then Exit Do
                Loop Until pPoint.X > pEnvelope.XMax And bArreter

                'Sortir si annuler
                If pTrackCancel.Continue = False Then Exit Do

                'Vérifier si le polygone est invalide
                If pPolygon Is Nothing Then
                    'Indiquer si on veut arreter
                    bArreter = True
                Else
                    'Indiquer si on veut arreter
                    bArreter = pPolygon.Envelope.YMin < pEnvelope.YMin
                End If

                'Réinitialiser le point en X
                pPoint.X = pEnvelope.XMin
                'Traiter une nouvelle ligne
                pPoint.Y = pPoint.Y - oSnrc.DeltaLatitude(pPoint.Y, 250000)
            Loop Until pPoint.Y < pEnvelope.YMin And bArreter

            'Conserver le SNRC dans l'unité de travail en mémoire
            modUniteTravail.m_UniteTravail = oSnrc
            'Mettre à jour la description de l'information du Snrc
            m_MenuUniteTravail.rtxDescription.Text = oSnrc.Information
            m_MenuUniteTravail.cboNumero.Text = oSnrc.Numero


        Catch erreur As Exception
            MsgBox(erreur.ToString)
        Finally
            'Écrire une trace de fin
            'Call citsMod_FichierTrace.EcrireMessageTrace("Fin")
            'Réinitialiser et vider la mémoire
            mbMouseDown = False
            mpFeedbackEnv = Nothing
            mpPoint = Nothing
            pEnvelope = Nothing
            pPoint = Nothing
            oSnrc = Nothing
            pPolygon = Nothing
            pScreenDisplay = Nothing
            pTrackCancel = Nothing
        End Try
    End Sub

    Protected Overrides Sub OnUpdate()
        Try
            Enabled = m_UniteTravail IsNot Nothing
            If Enabled Then Enabled = m_UniteTravail.EstValide

        Catch erreur As Exception
            MsgBox(erreur.ToString)
        End Try
    End Sub
End Class
