Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Editor
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Geometry

Public Class tooConstruireSurface
    Inherits ESRI.ArcGIS.Desktop.AddIns.Tool
    'Définir les variables globales de travail
    Private gbOptimiser As Boolean = False                          'Indiquer qu'on ne veut pas optimiser les géométries de travail
    Private gbSimplifier As Boolean = False                         'Indiquer qu'on ne veut pas simplifier les géométries de travail
    Private gpNewPolygonFeedback As INewPolygonFeedback = Nothing   'Interface utilisé pour voir la création d'une géométrie

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

    Protected Overloads Sub OnActivate(ByVal arg As ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs)

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
            If gpNewPolygonFeedback Is Nothing Then
                'Créer un nouveau FeedBack
                gpNewPolygonFeedback = New NewPolygonFeedback
                'Définir la fenêtre graphique
                gpNewPolygonFeedback.Display = pMxApplication.Display
                'Définir le point de départ
                gpNewPolygonFeedback.Start(pPoint)

                'Sinon si le Feedback est créé
            Else
                'Ajouter le point à la construction
                gpNewPolygonFeedback.AddPoint(pPoint)
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
            If gpNewPolygonFeedback IsNot Nothing Then
                'Définir le point correspondant au curseur
                pPoint = m_MxDocument.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y)
                'Définir la référence spatiale
                pPoint.SpatialReference = m_MxDocument.FocusMap.SpatialReference

                'Changer le point selon le snapping
                m_SnapEnvironment.SnapPoint(pPoint)

                'Changer le point de construction selon la position du curseur
                gpNewPolygonFeedback.MoveTo(pPoint)
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
        Dim pGeometry As IPolygon = Nothing 'Interface contenant la nouvelle géométrie à capter

        Try
            'Définir la géométrie contenue dans le Feedback
            pGeometry = gpNewPolygonFeedback.Stop

            'Vérifier si la géométrie du FeedBack est valide
            If pGeometry IsNot Nothing Then
                'Définir la référence spatiale
                pGeometry.SpatialReference = m_MxDocument.FocusMap.SpatialReference
                'Ajouter la géométrie et indiquer si on doit optimiser la géométrie
                AjouterNouvelleSurfaceTravail(pGeometry, gbOptimiser, gbSimplifier)
            End If

            'Désactiver le Feedback
            gpNewPolygonFeedback = Nothing

            'Afficher le contenu de la géométrie de travail
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
