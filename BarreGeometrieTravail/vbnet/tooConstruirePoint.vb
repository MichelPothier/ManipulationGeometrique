Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Editor
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.esriSystem

'**
'Nom de la composante : tooConstruirePoint
'
'''<summary>
'''Tool qui permet de construire des nouveaux points dans les géométries de travail.
'''</summary>
'''
'''<remarks>
'''Ce traitement est utilisable en mode interactif à l'aide de ses interfaces graphiques et doit être
'''utilisé dans ArcMap (ArcGisESRI).
'''
'''Auteur : Michel Pothier
'''</remarks>
Public Class tooConstruirePoint
    Inherits ESRI.ArcGIS.Desktop.AddIns.Tool
    'Définir les variables globales de travail
    Private gbOptimiser As Boolean = False                      'Indiquer qu'on ne veut pas optimiser les géométries de travail
    Private gbSimplifier As Boolean = False                     'Indiquer qu'on ne veut pas simplifier les géométries de travail
    Private gpMultipoint As IMultipoint = Nothing               'Interface contenant le nouveau multipoint
    Private gpMovePointFeedback As IMovePointFeedback = Nothing 'Interface utilisé pour voir la création d'une géométrie

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

    Protected Overrides Sub OnActivate()
        'Déclarer les variables de travail
        Dim pPoint As IPoint = Nothing                  'Interface contenant le point correspondant au curseur
        Dim pClone As IClone = Nothing                  'Interface ESRI qui permet de clone une géométrie
        Dim pMxApplication As IMxApplication = Nothing  'Interface ESRI contenant le document ArcMap.
        Dim pSymbol As IMarkerSymbol = Nothing          'Interface ESRI contenant le symbol du Feedback

        Try
            'Interface pour accéder à l'affichage de l'application
            pMxApplication = CType(m_Application, IMxApplication)

            'Définir le point correspondant au curseur
            pPoint = m_MxDocument.ActiveView.Extent.UpperLeft
            'Définir la référence spatiale
            pPoint.SpatialReference = m_MxDocument.FocusMap.SpatialReference

            'Créer un nouveau FeedBack
            gpMovePointFeedback = New MovePointFeedback
            'Définir la fenêtre graphique
            gpMovePointFeedback.Display = pMxApplication.Display

            'Redéfinir la grosseur du symbol selon la tolérance de recherche en pixel
            pSymbol = CType(gpMovePointFeedback.Symbol, IMarkerSymbol)
            pSymbol.Size = m_MxDocument.SearchTolerancePixels

            'Interface pour cloner le point
            pClone = CType(pPoint, IClone)
            'Définir le point de départ
            gpMovePointFeedback.Start(pPoint, CType(pClone.Clone, IPoint))

        Catch erreur As Exception
            'Envoyer un message d'erreur
            MsgBox(erreur.ToString)
        Finally
            'Vider la mémoire
            pPoint = Nothing
            pClone = Nothing
            pSymbol = Nothing
            pMxApplication = Nothing
        End Try
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)
        'Déclarer les variables de travail
        Dim pPoint As IPoint = Nothing                  'Interface contenant le point correspondant au curseur
        Dim pPointColl As IPointCollection = Nothing    'Interface utilisé pour insérer les points dans le multipoint
        Dim pMxApplication As IMxApplication = Nothing  'Interface ESRI contenant le document ArcMap.
        Dim pClone As IClone = Nothing                  'Interface ESRI qui permet de clone une géométrie

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

            'Vérifier si le Multipoint n'est pas créé
            If gpMultipoint Is Nothing Then
                'Créer un nouveau multipoint
                gpMultipoint = CType(New Multipoint, IMultipoint)
                'Définir la référence spatiale
                gpMultipoint.SpatialReference = m_MxDocument.FocusMap.SpatialReference
            End If

            'Interface pour ajouter les points aux multipoint
            pPointColl = CType(gpMultipoint, IPointCollection)

            'Interface pour cloner le point
            pClone = CType(pPoint, IClone)
            'Ajouter le point à la construction
            gpMovePointFeedback.Start(pPoint, CType(pClone.Clone, IPoint))
            'Ajouter le nouveau point
            pPointColl.AddPoint(pPoint)

        Catch erreur As Exception
            'Envoyer un message d'erreur
            MsgBox(erreur.ToString)
        Finally
            'Vider la mémoire
            pPoint = Nothing
            pClone = Nothing
            pPointColl = Nothing
            pMxApplication = Nothing
        End Try
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)
        'Déclarer les variables de travail
        Dim pPoint As IPoint = Nothing  'Interface contenant le point correspondant au curseur

        Try
            'Vérifier si le Feedback est créé
            If gpMovePointFeedback IsNot Nothing Then
                'Définir le point correspondant au curseur
                pPoint = m_MxDocument.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y)
                'Définir la référence spatiale
                pPoint.SpatialReference = m_MxDocument.FocusMap.SpatialReference

                'Changer le point selon le snapping
                m_SnapEnvironment.SnapPoint(pPoint)

                'Changer le point de construction selon la position du curseur
                gpMovePointFeedback.MoveTo(pPoint)
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
        Try
            'Définir la géométrie contenue dans le Feedback
            gpMovePointFeedback.Stop()

            'Vérifier si la géométrie du FeedBack est valide
            If gpMultipoint IsNot Nothing Then
                'Ajouter la géométrie et indiquer si on doit optimiser la géométrie
                AjouterNouveauPointTravail(gpMultipoint, gbOptimiser, gbSimplifier)
            End If

            'Désactiver le multipoint
            gpMultipoint = Nothing

            'Afficher le contenu de la géométrie de travail
            Call m_MenuGeometrieTravail.InfoGeometrieTravail()

        Catch erreur As Exception
            'Envoyer un message d'erreur
            MsgBox(erreur.ToString)
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
