Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Editor

'**
'Nom de la composante : tooConstruireLigne 
'
'''<summary>
'''Tool qui permet de construire des nouvelles lignes dans les géométries de travail.
'''</summary>
'''
'''<remarks>
'''Ce traitement est utilisable en mode interactif à l'aide de ses interfaces graphiques et doit être
'''utilisé dans ArcMap (ArcGisESRI).
'''
'''Auteur : Michel Pothier
'''</remarks>
Public Class tooConstruireLigne
    Inherits ESRI.ArcGIS.Desktop.AddIns.Tool

    'Déclarer les variable de travail
    Private gbOptimiser As Boolean = False                      'Indiquer qu'on ne veut pas optimiser les géométries de travail
    Private gbSimplifier As Boolean = False                     'Indiquer qu'on ne veut pas simplifier les géométries de travail
    Private gpNewlineFeedback As INewLineFeedback = Nothing     'Interface utilisé pour voir la création d'une géométrie

    Public Sub New()
        Try
            'Vérifier si l'application est définie
            If Not Hook Is Nothing Then
                'Définir l'application
                m_Application = CType(Hook, IApplication)

                'Vérifier si l'application est ArcMap
                If TypeOf Hook Is IMxApplication Then
                    'Rendre active la commande
                    Enabled = True
                    'Définir le document
                    m_MxDocument = CType(m_Application.Document, IMxDocument)
                    'Définir l'interface utilisé pour effectuer le snapping
                    m_SnapEnvironment = CType(m_Application.FindExtensionByName("ESRI Object Editor"), ISnapEnvironment)

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
        'Déclarer les variables de travail
        Dim pPoint As IPoint = Nothing                  'Interface contenant le point correspondant au curseur
        Dim pMxApplication As IMxApplication = Nothing  'Interface ESRI contenant le document ArcMap.

        Try
            'Indiquer si on veut optimiser les géométries de travail
            gbOptimiser = arg.Shift

            'Indiquer si on veut simplifier les géométries de travail
            gbSimplifier = arg.Alt

            'Vérifier si on doit créer une nouvelle géométrie de travail
            If arg.Shift = False And arg.Control = False Or mpGeometrieTravail Is Nothing Then
                'Créer une nouvelle géométrie de travail
                mpGeometrieTravail = CType(New GeometryBag, IGeometryCollection)
            End If

            'Interface pour accéder à l'affichage de l'application
            pMxApplication = CType(m_Application, IMxApplication)

            'Définir le point correspondant au curseur
            pPoint = m_MxDocument.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y)
            'Définir la référence spatiale
            pPoint.SpatialReference = m_MxDocument.FocusMap.SpatialReference

            'Changer le point selon le snapping
            m_SnapEnvironment.SnapPoint(pPoint)

            'Vérifier si Le FeedBack n'est pas créé
            If gpNewlineFeedback Is Nothing Then
                'Créer un nouveau FeedBack
                gpNewlineFeedback = New NewLineFeedback
                'Définir la fenêtre graphique
                gpNewlineFeedback.Display = pMxApplication.Display
                'Définir le point de départ
                gpNewlineFeedback.Start(pPoint)

                'Sinon si le Feedback est créé
            Else
                'Ajouter le point à la construction
                gpNewlineFeedback.AddPoint(pPoint)
            End If

        Catch erreur As Exception
            'Envoyer un message d'erreur
            MsgBox(erreur.ToString)
        Finally
            'Vider la mémoire
            pPoint = Nothing
            pMxApplication = Nothing
        End Try
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)
        'Déclarer les variables de travail
        Dim pPoint As IPoint = Nothing  'Interface contenant le point correspondant au curseur

        Try
            'Vérifier si le Feedback est créé
            If gpNewlineFeedback IsNot Nothing Then
                'Définir le point correspondant au curseur
                pPoint = m_MxDocument.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y)
                'Définir la référence spatiale
                pPoint.SpatialReference = m_MxDocument.FocusMap.SpatialReference

                'Changer le point selon le snapping
                m_SnapEnvironment.SnapPoint(pPoint)

                'Changer le point de construction selon la position du curseur
                gpNewlineFeedback.MoveTo(pPoint)
            End If

        Catch erreur As Exception
            'Envoyer un message d'erreur
            MsgBox(erreur.ToString)
        Finally
            'Vider la mémoire
            pPoint = Nothing
        End Try
    End Sub

    Protected Overloads Overrides Sub OnDoubleClick()
        'Déclarer les variables de travail
        Dim pGeometry As IPolyline = Nothing 'Interface contenant la nouvelle géométrie à capter

        Try
            'Définir la géométrie contenue dans le Feedback
            pGeometry = gpNewlineFeedback.Stop

            'Vérifier si la géométrie du FeedBack est valide
            If pGeometry IsNot Nothing Then
                'Définir la référence spatiale
                pGeometry.SpatialReference = m_MxDocument.FocusMap.SpatialReference
                'Ajouter la géométrie et indiquer si on doit optimiser la géométrie
                AjouterNouvelleLigneTravail(pGeometry, gbOptimiser, gbSimplifier)
            End If

            'Désactiver le Feedback
            gpNewlineFeedback = Nothing

            'Afficher et dessiner l'information des géométries de travail
            Call m_MenuGeometrieTravail.InfoGeometrieTravail()

        Catch erreur As Exception
            'Envoyer un message d'erreur
            MsgBox(erreur.ToString)
        Finally
            'Vider la mémoire
            pGeometry = Nothing
        End Try
    End Sub

    Protected Overrides Sub OnUpdate()
        'Vérifier si la géométrie de travail est présente
        If mpGeometrieTravail IsNot Nothing And m_MenuGeometrieTravail IsNot Nothing Then
            Enabled = True
        Else
            Enabled = False
        End If
    End Sub
End Class
