Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Editor
Imports ESRI.ArcGIS.EditorExt
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.esriSystem

'**
'Nom de la composante : CancelException.vb
'
'''<summary>
''' Classe contenant une exception permettant d'annulé l'exécution d'un traitement.
'''</summary>
'''
'''<remarks>
''' Auteur : Michel Pothier
''' Date : 24 mars 2016
'''</remarks>
''' 
Public Class CancelException
    Inherits Exception

    Public Sub New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class

'**
'Nom de la composante : modGeometrieTravail.vb 
'
'''<summary>
''' Librairies de routines utilisée pour traiter les géométries de travail.
'''</summary>
'''
'''<remarks>
'''Auteur : Michel Pothier
'''</remarks>
''' 
Module modGeometrieTravail
    'Liste des variables publiques utilisées
    '''<summary> Classe contenant le menu des géométries de travail. </summary>
    Public m_MenuGeometrieTravail As dckMenuGeometrieTravail
    '''<summary> Interface ESRI contenant les géométries de travail</summary>
    Public mpGeometrieTravail As IGeometryCollection
    '''<summary> Interface ESRI contenant le symbol pour la texte.</summary>
    Public mpSymboleTexte As ISymbol
    '''<summary> Interface ESRI contenant le symbol pour les sommets d'une géométrie.</summary>
    Public mpSymboleSommet As ISymbol
    '''<summary> Interface ESRI contenant le symbol pour la géométrie de type point.</summary>
    Public mpSymbolePoint As ISymbol
    '''<summary> Interface ESRI contenant le symbol pour la géométrie de type ligne.</summary>
    Public mpSymboleLigne As ISymbol
    '''<summary> Interface ESRI contenant le symbol pour la géométrie de type surface.</summary>
    Public mpSymboleSurface As ISymbol
    ''' <summary>Interface ESRI contenant l'application ArcMap.</summary>
    Public m_Application As IApplication = Nothing
    ''' <summary>Interface ESRI contenant le document ArcMap.</summary>
    Public m_MxDocument As IMxDocument = Nothing
    ''' <summary>Interface ESRI contenant l'environnement pour faire le snapping.</summary>
    Public m_SnapEnvironment As ISnapEnvironment = Nothing
    ''' <summary>Interface ESRI contenant les paramètres de la topologie courante.</summary>
    Public m_MapTopology As IMapTopology = Nothing
    ''' <summary>Interface ESRI contenant la topologie courante.</summary>
    Public m_TopologyGraph As ITopologyGraph4 = Nothing
    ''' <summary>Interface ESRI utilisé pour visualiser la construction de géométrie.</summary>
    Public mpRubberBand As IRubberBand

    '''<summary>Valeur initiale de la dimension en hauteur du menu.</summary>
    Public m_Height As Integer = 300
    '''<summary>Valeur initiale de la dimension en largeur du menu.</summary>
    Public m_Width As Integer = 300

    '''<summary>
    ''' Initialiser les couleurs par défaut pour le texte qui sera affiché avec les
    ''' géométries déssiné et initialiser les 3 couleurs pour les géométries de type
    ''' point, ligne et surface.
    '''</summary>
    '''
    Public Sub InitSymbole()
        'Déclarer les variables de travail
        Dim pRgbColor As IRgbColor = Nothing                'Interface ESRI contenant la couleur RGB.
        Dim pTextSymbol As ITextSymbol = Nothing            'Interface ESRI contenant un symbole de texte.
        Dim pMarkerSymbol As ISimpleMarkerSymbol = Nothing  'Interface ESRI contenant un symbole de point.
        Dim pLineSymbol As ISimpleLineSymbol = Nothing      'Interface ESRi contenant un symbole de ligne.
        Dim pFillSymbol As ISimpleFillSymbol = Nothing      'Interface ESRI contenant un symbole de surface.

        'Permet d'initialiser la symbologie
        Try
            'Vérifier si le symbole est invalide
            If mpSymboleTexte Is Nothing Then
                'Définir la couleur pour le texte
                pRgbColor = New RgbColor
                pRgbColor.RGB = 0
                'Définir la symbologie pour le texte
                pTextSymbol = New ESRI.ArcGIS.Display.TextSymbol
                pTextSymbol.Color = pRgbColor
                pTextSymbol.Font.Bold = True
                pTextSymbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHACenter
                'Conserver le symbole
                mpSymboleTexte = CType(pTextSymbol, ISymbol)
            End If

            'Vérifier si le symbole est invalide
            If mpSymboleSommet Is Nothing Then
                'Définir la couleur rouge pour le polygon
                pRgbColor = New RgbColor
                pRgbColor.Blue = 255
                'Définir la symbologie pour la limite d'un polygone
                pMarkerSymbol = New SimpleMarkerSymbol
                pMarkerSymbol.Color = pRgbColor
                pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle
                pMarkerSymbol.Size = 3
                'Conserver le symbole
                mpSymboleSommet = CType(pMarkerSymbol, ISymbol)
            End If

            'Vérifier si le symbole est invalide
            If mpSymbolePoint Is Nothing Then
                'Définir la couleur rouge pour le polygon
                pRgbColor = New RgbColor
                pRgbColor.Red = 255
                'Définir la symbologie pour la limite d'un polygone
                pMarkerSymbol = New SimpleMarkerSymbol
                pMarkerSymbol.Color = pRgbColor
                pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSSquare
                pMarkerSymbol.Size = 5
                'Conserver le symbole 
                mpSymbolePoint = CType(pMarkerSymbol, ISymbol)
            End If

            'Vérifier si le symbole est invalide
            If mpSymboleLigne Is Nothing Then
                'Définir la couleur rouge pour le polygon
                pRgbColor = New RgbColor
                pRgbColor.Red = 255
                'Définir la symbologie pour la limite d'un polygone
                pLineSymbol = New SimpleLineSymbol
                pLineSymbol.Color = pRgbColor
                pLineSymbol.Style = esriSimpleLineStyle.esriSLSDot
                pLineSymbol.Width = 3
                'Conserver le symbole en mémoire
                mpSymboleLigne = CType(pLineSymbol, ISymbol)
            End If

            'Vérifier si le symbole est invalide
            If mpSymboleSurface Is Nothing Then
                'Définir la couleur rouge pour le polygon
                pRgbColor = New RgbColor
                pRgbColor.Red = 255
                'Définir la symbologie pour la limite d'un polygone
                pLineSymbol = New SimpleLineSymbol
                pLineSymbol.Color = pRgbColor
                'Définir la symbologie pour l'intérieur d'un polygone
                pFillSymbol = New SimpleFillSymbol
                pFillSymbol.Color = pRgbColor
                pFillSymbol.Outline = pLineSymbol
                pFillSymbol.Style = esriSimpleFillStyle.esriSFSBackwardDiagonal
                'Conserver le symbole 
                mpSymboleSurface = CType(pFillSymbol, ISymbol)
            End If
        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            pRgbColor = Nothing
            pTextSymbol = Nothing
            pMarkerSymbol = Nothing
            pLineSymbol = Nothing
            pFillSymbol = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Fonction qui permet d'afficher l'information à l'écran sur les géométries contenues dans une
    ''' GeometryBag. L'information affichée est le nombre totale de géométries, le nombre de
    ''' points, le nombre de lignes, la longueur totale des lignes, le nombre de surfaces, la superficie
    ''' totale des surfaces.
    '''</summary>
    '''
    '''<returns>     'La fonction va retourner un "String". Si le traitement n'a pas réussi le "String" est vide. </returns>
    ''' 
    Public Function InfoGeometryBag() As String
        'Déclarer les variables de travail
        Dim pGeometry As IGeometry = Nothing    'Interface ESRI contenant une géométrie.
        Dim pPolyline As IPolyline = Nothing    'Interface ESRI contenant une ligne.
        Dim pPolygon As IPolygon2 = Nothing     'Interface ESRI contenant une surface.
        Dim pArea As IArea = Nothing            'Interface ESRI utilisée pour le calcul de surficie.
        Dim pGeomColl As IGeometryCollection = Nothing   'Interface ESRI contenant les composantes d'une géométrie.
        Dim pPointColl As IPointCollection = Nothing     'Interface ESRI pour calculer le nombre de sous-points.

        Dim lNbPoint As Long = 0            'Nombre total de Points dans la GeometryBag.
        Dim lNbMultiPoint As Long = 0       'Nombre total de Multipoints dans la GeometryBag.
        Dim lNbSommetPoint As Long = 0      'Nombre total de Sommets Points dans la GeometryBag.
        Dim lNbPolyligne As Long = 0        'Nombre total de Polylignes dans la GeometryBag.
        Dim lNbLigne As Long = 0            'Nombre total de Ligne dans la GeometryBag.
        Dim lNbSommetLigne As Long = 0      'Nombre total de Sommets Lignes dans la GeometryBag.
        Dim lNbPolygone As Long = 0         'Nombre total de Polygones dans la GeometryBag.
        Dim lNbExtRing As Long = 0          'Nombre total d'anneaux extérieurs dans la GeometryBag.
        Dim lNbIntRing As Long = 0          'Nombre total d'anneaux intérieurs  dans la GeometryBag.
        Dim lNbSommetSurface As Long = 0    'Nombre total de Sommets Surface dans la GeometryBag.
        Dim dLongueurLigne As Double = 0    'Longueur totale des lignes.
        Dim dLongueurSurface As Double = 0  'Longueur totale des surfaces.
        Dim dSuperficie As Double = 0       'Superficie totale des lignes.

        'Initialiser la valeur de retour
        InfoGeometryBag = ""

        Try
            'Traiter toutes les géométries
            For i = 0 To mpGeometrieTravail.GeometryCount - 1
                'Interface pour traiter une géométrie
                pGeometry = mpGeometrieTravail.Geometry(i)

                'Vérifier si la géométrie est un point
                If pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                    'Compter le nombre total de point
                    lNbPoint = lNbPoint + 1

                    'Vérifier si la géométrie est un MultiPoint
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                    'Compter le nombre total de MultiPoint
                    lNbMultiPoint = lNbMultiPoint + 1
                    'Interface pour calculer le nombre de sommets point
                    pPointColl = CType(pGeometry, IPointCollection)
                    'Compter le nombre total de MultiPoint
                    lNbSommetPoint = lNbSommetPoint + pPointColl.PointCount

                    'Vérifier si la géométrie est une ligne
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then
                    'Compter le nombre total de ligne
                    lNbPolyligne = lNbPolyligne + 1
                    'Interface pour calculer le nombre de Ligne
                    pGeomColl = CType(pGeometry, IGeometryCollection)
                    'Compter le nombre total de segments
                    lNbLigne = lNbLigne + pGeomColl.GeometryCount
                    'Interface pour calculer le nombre de sommets point
                    pPointColl = CType(pGeometry, IPointCollection)
                    'Compter le nombre total de sommets
                    lNbSommetLigne = lNbSommetLigne + pPointColl.PointCount
                    'Interface pour calculer la longueur
                    pPolyline = CType(pGeometry, IPolyline)
                    'Calculer la longueur totale
                    dLongueurLigne = dLongueurLigne + pPolyline.Length

                    'Vérifier si la géométrie est une surface
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolygon Then
                    'Compter le nombre total de surface
                    lNbPolygone = lNbPolygone + 1

                    'Interface pour extraire le nombre d'anneaux
                    pGeomColl = CType(pGeometry, IGeometryCollection)

                    'Interface pour extraire les anneaux extérieurs
                    pPolygon = CType(pGeometry, IPolygon2)

                    Try
                        'Calculer le nombre de polygone extérieur
                        lNbExtRing = lNbExtRing + pPolygon.ExteriorRingCount
                        'Calculer le nombre de polygone intérieur
                        lNbIntRing = lNbIntRing + (pGeomColl.GeometryCount - pPolygon.ExteriorRingCount)
                    Catch ex As Exception
                        'Si la géométrie n'Est pas simple, il n'y a que des anneaux extérieurs
                        lNbExtRing = lNbExtRing + pGeomColl.GeometryCount
                    End Try

                    'Interface pour calculer le nombre de sommets point
                    pPointColl = CType(pGeometry, IPointCollection)
                    'Compter le nombre total de sommets
                    lNbSommetSurface = lNbSommetSurface + pPointColl.PointCount
                    'Calculer la longueur des surfaces
                    dLongueurSurface = dLongueurSurface + pPolygon.Length
                    'Interface pour calculer la superficie
                    pArea = CType(pGeometry, IArea)
                    'Calculer la superficie
                    dSuperficie = dSuperficie + pArea.Area
                End If
            Next i

            'Retourner le nombre de géométries présentes
            InfoGeometryBag = "Nombre total de géométries : " & mpGeometrieTravail.GeometryCount & vbCrLf & vbCrLf

            'Retourner l'information plus complète
            If mpGeometrieTravail.GeometryCount > 0 Then
                'Retourner l'information pour les points
                If lNbPoint > 0 Then
                    InfoGeometryBag = InfoGeometryBag & "Nombre de géométries (Point) : " & lNbPoint & vbCrLf & vbCrLf
                End If

                'Retourner l'information pour les multipoints
                If lNbMultiPoint > 0 Then
                    InfoGeometryBag = InfoGeometryBag & "Nombre de géométries (Multipoint) : " & lNbMultiPoint & vbCrLf
                    InfoGeometryBag = InfoGeometryBag & "Nombre de sommets (Point) : " & lNbSommetPoint & vbCrLf & vbCrLf
                End If

                'Retourner l'information pour les lignes
                If lNbPolyligne > 0 Then
                    InfoGeometryBag = InfoGeometryBag & "Nombre de géométries (Polyline) : " & lNbPolyligne & vbCrLf
                    InfoGeometryBag = InfoGeometryBag & "Nombre de lignes (Path) : " & lNbLigne & vbCrLf
                    InfoGeometryBag = InfoGeometryBag & "Nombre de sommets (Point) : " & lNbSommetLigne & vbCrLf
                    InfoGeometryBag = InfoGeometryBag & "Longueur totale : " & dLongueurLigne.ToString("F2") & vbCrLf & vbCrLf
                End If

                'Retourner l'information pour les surfaces
                If lNbPolygone > 0 Then
                    InfoGeometryBag = InfoGeometryBag & "Nombre de géométries (Polygon) : " & lNbPolygone & vbCrLf
                    InfoGeometryBag = InfoGeometryBag & "Nombre d'anneaux (ExteriorRing) : " & lNbExtRing & vbCrLf
                    InfoGeometryBag = InfoGeometryBag & "Nombre d'anneaux (InteriorRing) : " & lNbIntRing & vbCrLf
                    InfoGeometryBag = InfoGeometryBag & "Nombre de sommets (Point) : " & lNbSommetSurface & vbCrLf
                    InfoGeometryBag = InfoGeometryBag & "Longueur totale : " & dLongueurSurface.ToString("F2") & vbCrLf
                    InfoGeometryBag = InfoGeometryBag & "Superficie totale : " & dSuperficie.ToString("F2") & vbCrLf
                End If
            End If

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pGeometry = Nothing
            pPolyline = Nothing
            pPolygon = Nothing
            pArea = Nothing
            pGeomColl = Nothing
            pPointColl = Nothing
        End Try
    End Function

    '''<summary>
    ''' Permet d'afficher la fenêtre graphique selon l'enveloppe des géométries de
    ''' travail plus 10%. Cette fonction est seulement appelé si le checkbox "Zoom 
    ''' selon les géométries de travail" est coché.
    '''</summary>
    '''<param name=" pGeometry ">Interface contenant la géométrie à effectuer un Zoom.</param>
    '''
    Public Sub ZoomGeometrieTravail(ByVal pGeometry As IGeometry)
        'Déclarer les variables de travail
        Dim pEnvelope As IEnvelope = Nothing        'Interface ESRI contenant l'enveloppe de la géométrie de travail.

        Try
            'Vérifier si le polygone est vide
            If pGeometry.IsEmpty Then
                Exit Sub
            End If

            'Vérifier si la géométrie est un point
            If pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                'Définir la nouvelle fenêtre de travail
                pEnvelope = m_MxDocument.ActiveView.Extent

                'Recentrer l'exveloppe selon le point
                pEnvelope.CenterAt(CType(pGeometry, IPoint))

                'Si la géométrie n'est pas un point
            Else
                'Définir l'enveloppe de l'élément en erreur qui n'est pas un point
                pEnvelope = pGeometry.Envelope

                'Agrandir l'enveloppe de 10% de l'élément en erreur
                pEnvelope.Expand(pEnvelope.Width / 10, pEnvelope.Height / 10, False)
            End If

            'Définir la nouvelle fenêtre de travail
            m_MxDocument.ActiveView.Extent = pEnvelope

            'Rafraîchier l'affichage
            m_MxDocument.ActiveView.Refresh()

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            pEnvelope = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Fonction qui permet de dessiner dans la vue active les géométries Point, MultiPoint, 
    ''' Polyline et/ou Polygon. Ces géométries peuvent être contenu dans un GeometryBag. 
    ''' Un point est représenté par un carré, une ligne est représentée par une ligne pleine 
    ''' et la surface est représentée par une ligne pleine pour la limite et des lignes à
    ''' 45 dégrés pour l'intérieur. On peut afficher le numéro de la géométrie pour un GeometryBag.
    '''</summary>
    '''
    '''<param name="pGeometry"> Interface ESRI contenant la géométrie èa dessiner.</param>
    '''<param name="bRafraichir"> Indique si on doit rafraîchir la vue active.</param>
    '''<param name="bGeometrie"> Indique si on doit dessiner la géométrie.</param>
    '''<param name="bSommet"> Indique si on doit dessiner les sommets de la géométrie.</param>
    '''<param name="bNumero"> Indique si on doit dessiner les numéros de la géométrie.</param>
    '''<param name="pTrackCancel"> Permet d'annuler le traitement avec la touche ESC du clavier.</param>
    ''' 
    '''<return>Un booleen est retourner pour indiquer la fonction s'est bien exécutée.</return>
    ''' 
    Public Function bDessinerGeometrie(ByVal pGeometry As IGeometry,
                                       Optional ByVal bRafraichir As Boolean = False, _
                                       Optional ByVal bGeometrie As Boolean = False,
                                       Optional ByVal bSommet As Boolean = False,
                                       Optional ByVal bNumero As Boolean = False,
                                       Optional ByRef pTrackCancel As ITrackCancel = Nothing) As Boolean
        'Déclarer les variables de travail
        Dim pScreenDisplay As IScreenDisplay = Nothing  'Interface ESRI contenant le document de ArcMap.
        Dim pGeomColl As IGeometryCollection = Nothing  'Interface ESRI contenant la fenêtre d'affichage.
        Dim pPath As IPolyline = Nothing                'Interface ESRI contenant la polyline du Path.
        Dim pGeomTexte As IGeometry = Nothing           'Interface ESRI pour la position du texte d'une surface.

        Try
            'Vérifier si la géométrie est absente
            If pGeometry Is Nothing Then Exit Function

            'Vérifier si la géométrie est vide
            If pGeometry.IsEmpty Then Exit Function

            'Initialiser les variables de travail
            pScreenDisplay = m_MxDocument.ActiveView.ScreenDisplay

            'Vérifier si on doit rafraichir l'écran
            If bRafraichir Then
                'Rafraîchier l'affichage
                m_MxDocument.ActiveView.Refresh()
                System.Windows.Forms.Application.DoEvents()
            End If

            'Vérifier si la géométrie est un GeometryBag
            If pGeometry.GeometryType = esriGeometryType.esriGeometryBag Then
                'Interface pour traiter toutes les géométries présentes dans le GeometryBag
                pGeomColl = CType(pGeometry, IGeometryCollection)

                'Dessiner toutes les géométrie présentes dans une collection de géométrie
                For i = 0 To pGeomColl.GeometryCount - 1
                    'Vérifier s'il faut dessiner la géométrie
                    If bGeometrie Then
                        'Dessiner la géométrie contenu dans un GeometrieBag
                        Call bDessinerGeometrie(pGeomColl.Geometry(i), False, bGeometrie, False, False, pTrackCancel)
                    End If

                    'Vérifier s'il faut dessiner les sommets de la géométrie
                    If bSommet Then
                        'Dessiner les sommets de la géométrie contenu dans un GeometrieBag
                        Call bDessinerSommet(pGeomColl.Geometry(i), False, False, pTrackCancel)
                    End If

                    'Vérifier si on veut afficher le texte du numéro de géométrie
                    If bNumero Then
                        'Trouver le centre de la géométrie
                        pGeomTexte = CentreGeometrie(pGeomColl.Geometry(i))

                        'Afficher le texte avec sa symbologie dans la vue active
                        With pScreenDisplay
                            .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                            .SetSymbol(mpSymboleTexte)
                            .DrawText(pGeomTexte, CStr(i + 1))
                            .FinishDrawing()
                        End With
                    End If
                Next i

                'Vérifier si la géométrie est un point
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                'Transformation du système de coordonnées selon la vue active
                pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

                'Vérifier s'il faut dessiner la géométrie
                If bGeometrie Then
                    'Afficher la géométrie avec sa symbologie dans la vue active
                    With pScreenDisplay
                        .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                        .SetSymbol(mpSymbolePoint)
                        .DrawPoint(pGeometry)
                        .FinishDrawing()
                    End With
                End If

                'Vérifier si la géométrie est un multi-point
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                'Transformation du système de coordonnées selon la vue active
                pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

                'Vérifier s'il faut dessiner la géométrie
                If bGeometrie Then
                    'Afficher la géométrie avec sa symbologie dans la vue active
                    With pScreenDisplay
                        .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                        .SetSymbol(mpSymbolePoint)
                        .DrawMultipoint(pGeometry)
                        .FinishDrawing()
                    End With
                End If

                'Vérifier s'il faut dessiner les sommets de la géométrie
                If bSommet Then
                    'Dessiner les sommets de la géométrie contenu dans un GeometrieBag
                    Call bDessinerSommet(pGeometry, False, False, pTrackCancel)
                End If

                'Vérifier si on veut afficher le texte du numéro de géométrie
                If bNumero Then
                    'Interface pour traiter toutes les géométries présentes dans le GeometryBag
                    pGeomColl = CType(pGeometry, IGeometryCollection)

                    'Dessiner toutes les géométrie présentes dans une collection de géométrie
                    For i = 0 To pGeomColl.GeometryCount - 1
                        'Vérifier si la géométrie n'est pas vide
                        If Not pGeomColl.Geometry(i).IsEmpty Then
                            'Trouver le centre de la géométrie
                            pGeomTexte = CentreGeometrie(pGeomColl.Geometry(i))

                            'Afficher le texte avec sa symbologie dans la vue active
                            With pScreenDisplay
                                .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                                .SetSymbol(mpSymboleTexte)
                                .DrawText(pGeomTexte, CStr(i + 1))
                                .FinishDrawing()
                            End With
                        End If
                    Next
                End If

                'Vérifier si la géométrie est une ligne
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then
                'Transformation du système de coordonnées selon la vue active
                pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

                'Vérifier s'il faut dessiner la géométrie
                If bGeometrie Then
                    'Afficher la géométrie avec sa symbologie dans la vue active
                    With pScreenDisplay
                        .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                        .SetSymbol(mpSymboleLigne)
                        .DrawPolyline(pGeometry)
                        .FinishDrawing()
                    End With
                End If

                'Vérifier s'il faut dessiner les sommets de la géométrie
                If bSommet Then
                    'Dessiner les sommets de la géométrie contenu dans un GeometrieBag
                    Call bDessinerSommet(pGeometry, False, False, pTrackCancel)
                End If

                'Vérifier si on veut afficher le texte du numéro de géométrie
                If bNumero Then
                    'Interface pour traiter toutes les géométries présentes dans le GeometryBag
                    pGeomColl = CType(pGeometry, IGeometryCollection)

                    'Dessiner toutes les géométrie présentes dans une collection de géométrie
                    For i = 0 To pGeomColl.GeometryCount - 1
                        'Vérifier si la géométrie n'est pas vide
                        If Not pGeomColl.Geometry(i).IsEmpty Then
                            'Trouver le centre de la géométrie
                            pGeomTexte = CentreGeometrie(pGeomColl.Geometry(i))

                            'Afficher le texte avec sa symbologie dans la vue active
                            With pScreenDisplay
                                .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                                .SetSymbol(mpSymboleTexte)
                                .DrawText(pGeomTexte, CStr(i + 1))
                                .FinishDrawing()
                            End With
                        End If
                    Next
                End If

                'Vérifier si la géométrie est une surface
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolygon _
            Or pGeometry.GeometryType = esriGeometryType.esriGeometryEnvelope Then
                'Transformation du système de coordonnées selon la vue active
                pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

                'Vérifier s'il faut dessiner la géométrie
                If bGeometrie Then
                    'Afficher la géométrie avec sa symbologie dans la vue active
                    With pScreenDisplay
                        .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                        .SetSymbol(mpSymboleSurface)
                        .DrawPolygon(pGeometry)
                        .FinishDrawing()
                    End With
                End If

                'Vérifier s'il faut dessiner les sommets de la géométrie
                If bSommet Then
                    'Dessiner les sommets de la géométrie contenu dans un GeometrieBag
                    Call bDessinerSommet(pGeometry, False, False, pTrackCancel)
                End If

                'Vérifier si on veut afficher le texte du numéro de géométrie
                If bNumero Then
                    'Interface pour traiter toutes les géométries présentes dans le GeometryBag
                    pGeomColl = CType(pGeometry, IGeometryCollection)

                    'Dessiner toutes les géométrie présentes dans une collection de géométrie
                    For i = 0 To pGeomColl.GeometryCount - 1
                        'Vérifier si la géométrie n'est pas vide
                        If Not pGeomColl.Geometry(i).IsEmpty Then
                            'Trouver le centre de la géométrie
                            pGeomTexte = CentreGeometrie(pGeomColl.Geometry(i))

                            'Afficher le texte avec sa symbologie dans la vue active
                            With pScreenDisplay
                                .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                                .SetSymbol(mpSymboleTexte)
                                .DrawText(pGeomTexte, CStr(i + 1))
                                .FinishDrawing()
                            End With
                        End If
                    Next
                End If

                'Vérifier si la géométrie est un Path ou un Ring
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPath Or pGeometry.GeometryType = esriGeometryType.esriGeometryRing Then
                'Transformer le path en polyline
                pPath = PathToPolyline(CType(pGeometry, IPath))

                'Vérifier s'il faut dessiner la géométrie
                If bGeometrie Then
                    'Afficher la géométrie avec sa symbologie dans la vue active
                    With pScreenDisplay
                        .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                        .SetSymbol(mpSymboleLigne)
                        .DrawPolyline(pPath)
                        .FinishDrawing()
                    End With
                End If

                'Vérifier s'il faut dessiner les sommets de la géométrie
                If bSommet Then
                    'Dessiner les sommets de la géométrie et les numéros au besoin
                    Call bDessinerSommet(pPath, False, bNumero, pTrackCancel)
                End If
            End If

            'Retourner le résultat
            bDessinerGeometrie = True

            'Vérifier si un Cancel a été effectué
            If pTrackCancel.Continue = False Then Throw New CancelException("Traitement annulé !")

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            pScreenDisplay = Nothing
            pGeomColl = Nothing
            pPath = Nothing
            pGeomTexte = Nothing
        End Try
    End Function

    '''<summary>
    ''' Routine qui permet de créer un Polyline à partir d'un Path ou un Ring.
    ''' 
    '''<param name="pPath"> Interface contenant un Path.</param>
    '''
    '''<return>Le Polyline contenant le Path spécifié.</return>
    ''' 
    '''</summary>
    '''
    Private Function PathToPolyline(ByVal pPath As IPath) As IPolyline
        'Déclarer les variables de travail
        Dim pPointColl As IPointCollection = Nothing      'Interface pour ajouter les Path.
        Dim pTopoOp As ITopologicalOperator2 = Nothing      'Interface utilisée pour simplifier la géométrie.
        Dim pClone As IClone = Nothing                      'Interface pour cloner la géométrie.

        'Définir la valeur par défaut
        PathToPolyline = New Polyline
        'Définir la référence spatial
        PathToPolyline.SpatialReference = pPath.SpatialReference

        Try
            'Interface pour ajouter le Path dans la Polyline
            pPointColl = CType(PathToPolyline, IPointCollection)

            'Interface pour cloner la géométrie
            pClone = CType(pPath, IClone)

            'Ajouter le Path
            pPointColl.AddPointCollection(CType(pClone.Clone, IPointCollection))

            'Interface pour simplifier
            pTopoOp = CType(PathToPolyline, ITopologicalOperator2)
            pTopoOp.IsKnownSimple_2 = False
            pTopoOp.Simplify()

        Catch ex As Exception
            'Retourner l'erreur
            Throw ex
        Finally
            'Vider la mémoire
            pPointColl = Nothing
            pTopoOp = Nothing
            pClone = Nothing
        End Try
    End Function

    '''<summary>
    ''' Routine qui permet de retourner le centre d'une géométrie.
    ''' 
    '''<param name="pTrackCancel"> Permet d'annuler la sélection avec la touche ESC du clavier.</param>
    '''<param name="bEnleverSelection"> Indique si on doit enlever les éléments de la sélection, sinon ils sont conservés. Défaut=True.</param>
    '''
    '''<return>Les géométries des éléments qui respectent ou non la comparaison avec ses éléments en relation.</return>
    ''' 
    '''</summary>
    '''
    Private Function CentreGeometrie(ByRef pGeometry As IGeometry) As IPoint
        'Déclarer les variables de travail
        Dim pArea As IArea = Nothing                        'Interface pour extraire un point à l'intérieur du polygon.
        Dim pPolyline As IPolyline = Nothing                'Interface pour extraire un point sur la ligne.
        Dim pPath As IPath = Nothing                        'Interface pour extraire un point sur le Path.
        Dim pPointColl As IPointCollection = Nothing        'Interface pour extraire un point sur le multipoint.
        Dim pPoint As IPoint = New Point                    'Interface contenant le point du centre de la ligne.
        Dim pClone As IClone = Nothing                      'Interface pour cloner une géométrie.

        'Définir la géométrie par défaut
        CentreGeometrie = Nothing

        Try
            'Vérifier si la géométrie est vide
            If pGeometry.IsEmpty Then
                'Interface pour extraire le centre de la classe de sélection
                pArea = CType(m_MxDocument.ActiveView.Extent, IArea)
                'Extraire le centre de la classe de sélection
                pPoint = pArea.LabelPoint

            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolygon Then
                'Cloner la géométrie
                pClone = CType(pGeometry, IClone)
                'Interface pour extraire un point à l'intérieur du polygon
                pArea = CType(pClone.Clone, IArea)
                'Extraire le centroide intérieur
                pPoint = pArea.LabelPoint

                'si la géométrie est un Polyline
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then
                'Interface pour extraire un point sur la ligne
                pPolyline = CType(pGeometry, IPolyline)
                'Extraire le point au centre de la ligne
                pPolyline.QueryPoint(esriSegmentExtension.esriNoExtension, pPolyline.Length / 2, False, pPoint)

                'si la géométrie est un Polyline
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPath Or pGeometry.GeometryType = esriGeometryType.esriGeometryRing Then
                'Interface pour extraire un point sur la ligne
                pPath = CType(pGeometry, IPath)
                'Extraire le point au centre de la ligne
                pPath.QueryPoint(esriSegmentExtension.esriNoExtension, pPath.Length / 2, False, pPoint)

                'si la géométrie est un Multipoint
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                'Interface pour extraire un point sur le multipoint
                pPointColl = CType(pGeometry, IPointCollection)
                'Extraire le premier point sur le multipoint
                pClone = CType(pPointColl.Point(0), IClone)
                'Cloner le premier point sur le multipoint
                pPoint = CType(pClone.Clone, IPoint)

                'si la géométrie est un Point
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                'Extraire le premier point sur le multipoint
                pClone = CType(pGeometry, IClone)
                'Cloner le premier point sur le multipoint
                pPoint = CType(pClone.Clone, IPoint)
            End If

            'Définir le centre de la géométrie
            CentreGeometrie = pPoint

        Catch ex As Exception
            'Retourner l'erreur
            Throw ex
        Finally
            'Vider la mémoire
            pArea = Nothing
            pPolyline = Nothing
            pPath = Nothing
            pPointColl = Nothing
            pPoint = Nothing
            pClone = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de dessiner dans la vue active les sommets des géométries Point, MultiPoint, Polyline et/ou Polygon.
    ''' Ces géométries peuvent être contenu dans un GeometryBag. Les sommets sont représentés par un cercle.
    '''</summary>
    '''
    '''<param name="pGeometry"> Interface ESRI contenant la géométrie utilisée pour dessiner les sommets.</param>
    '''<param name="bRafraichir"> Indique si on doit rafraîchir la vue active.</param>
    '''<param name="bNumero"> Indique si on doit dessiner le numéro du sommet.</param>
    '''<param name="pTrackCancel"> Permet d'annuler le traitement avec la touche ESC du clavier.</param>
    ''' 
    '''<return>Un booleen est retourner pour indiquer la fonction s'est bien exécutée.</return>
    ''' 
    Public Function bDessinerSommet(ByVal pGeometry As IGeometry, _
                                    Optional ByVal bRafraichir As Boolean = False, _
                                    Optional ByVal bNumero As Boolean = False, _
                                    Optional ByRef pTrackCancel As ITrackCancel = Nothing) As Boolean
        'Déclarer les variables de travail
        Dim pScreenDisplay As IScreenDisplay = Nothing  'Interface ESRI contenant le document de ArcMap.
        Dim pGeomColl As IGeometryCollection = Nothing  'Interface ESRI contenant la fenêtre d'affichage.
        Dim pMultiPoint As IMultipoint = Nothing        'Interface contenant les sommets de la géométrie
        Dim pPointColl As IPointCollection = Nothing    'Interface utilisée pour transformer la géométrie en multipoint

        Try
            'Vérifier si la géométrie est absente
            If pGeometry Is Nothing Then Exit Function

            'Vérifier si la géométrie est vide
            If pGeometry.IsEmpty Then Exit Function

            'Initialiser les variables de travail
            pScreenDisplay = m_MxDocument.ActiveView.ScreenDisplay

            'Vérifier si on doit rafraichir l'écran
            If bRafraichir Then
                'Rafraîchier l'affichage
                m_MxDocument.ActiveView.Refresh()
                System.Windows.Forms.Application.DoEvents()
            End If

            'Transformation du système de coordonnées selon la vue active
            pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

            'Vérifier si la géométrie est un GeometryBag
            If pGeometry.GeometryType = esriGeometryType.esriGeometryBag Then
                'Interface pour traiter toutes les géométries présentes dans le GeometryBag
                pGeomColl = CType(pGeometry, IGeometryCollection)

                'Dessiner toutes les géométrie présentes dans une collection de géométrie
                For i = 0 To pGeomColl.GeometryCount - 1
                    'Dessiner les sommets de la géométrie contenu dans un GeometrieBag
                    Call bDessinerSommet(pGeomColl.Geometry(i), False, False, pTrackCancel)
                Next i

                'Vérifier si la géométrie est un point
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                'Afficher la géométrie avec sa symbologie dans la vue active
                With pScreenDisplay
                    .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                    .SetSymbol(mpSymboleSommet)
                    .DrawPoint(pGeometry)
                    .FinishDrawing()
                End With

                'Vérifier si la géométrie est un multi-point
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                'Afficher la géométrie avec sa symbologie dans la vue active
                With pScreenDisplay
                    .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                    .SetSymbol(mpSymboleSommet)
                    .DrawMultipoint(pGeometry)
                    .FinishDrawing()
                End With

                'Vérifier si la géométrie est une ligne ou une surface
            Else
                'Créer un nouveau multipoint vide
                pMultiPoint = CType(New Multipoint, IMultipoint)
                pMultiPoint.SpatialReference = pGeometry.SpatialReference

                'Transformer la géométrie en multipoint
                pPointColl = CType(pMultiPoint, IPointCollection)
                pPointColl.AddPointCollection(CType(pGeometry, IPointCollection))

                'Afficher les sommets de la géométrie avec la symbologie des sommets dans la vue active
                With pScreenDisplay
                    .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                    .SetSymbol(mpSymboleSommet)
                    .DrawMultipoint(pMultiPoint)
                    .FinishDrawing()
                End With

                'Vérifier si on veut afficher le texte du numéro de géométrie
                If bNumero Then
                    'Interface pour extraire les sommets de la géométrie
                    pPointColl = CType(pGeometry, IPointCollection)

                    'Traiter tous les sommets
                    For i = 0 To pPointColl.PointCount - 1
                        'Afficher le texte avec sa symbologie dans la vue active
                        With pScreenDisplay
                            .StartDrawing(pScreenDisplay.hDC, CType(ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache, Short))
                            .SetSymbol(mpSymboleTexte)
                            .DrawText(pPointColl.Point(i), CStr(i + 1))
                            .FinishDrawing()
                        End With
                    Next
                End If
            End If

            'Retourner le résultat
            bDessinerSommet = True

            'Vérifier si un Cancel a été effectué
            If pTrackCancel.Continue = False Then Throw New CancelException("Traitement annulé !")

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            pScreenDisplay = Nothing
            pGeomColl = Nothing
            pMultiPoint = Nothing
            pPointColl = Nothing
        End Try
    End Function

    '''<summary>
    ''' Permet d'effectuer la sélection des élément de la map active selon les géométries de travail.
    '''</summary>
    '''
    Public Sub SelectionnerElement()
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing

        Try
            'Définir la map active
            pMap = m_MxDocument.ActiveView.FocusMap

            'Sélectionner les éléments selon les géométrie de travail
            pMap.SelectByShape(CType(mpGeometrieTravail, IGeometry), Nothing, False)

            'Affichage de la sélection
            m_MxDocument.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, Nothing, Nothing)

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            pMap = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Permet de dessiner dans la vue active les géométries de travail à partir des lignes 
    ''' sélectionnées.
    '''</summary>
    '''
    Public Sub CreerSurfaceLigne()
        Try
            'Créer un nouveau GeometryBag vide
            mpGeometrieTravail = CType(New GeometryBag, IGeometryCollection)

            'Ajouter la surface créée dans les géométries de travail
            mpGeometrieTravail.AddGeometry(pCreerSurfaceSelonLigneSelection())

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        End Try
    End Sub

    '''<summary>
    ''' Fonction qui permet de créer une surface à partir d'une série de lignes sélectionnées.
    '''</summary>
    '''
    '''<returns>     'La fonction va retourner un "IPolygon". Si le traitement n'a pas réussi le "IPolygon" est à "Nothing". </returns>
    ''' 
    Public Function pCreerSurfaceSelonLigneSelection() As IPolygon
        'Déclarer les variables de travail
        Dim pEnumFeature As IEnumFeature = Nothing            'Interface ESRI pour traiter les éléments sélectionnés.
        Dim pFeature As IFeature = Nothing                    'Interface ESRI contenant un élément sélectionné.
        Dim pRing As IRing = Nothing                          'Interface ESRI contenant une composante de surface.
        Dim pRingPointColl As IPointCollection = Nothing      'Interface ESRI contenant la liste des points d'un Ring.
        Dim pTempPolyline As ITopologicalOperator2 = Nothing  'Interface ESRI pour fusionner les lignes de la surface.
        Dim pTopoOp As ITopologicalOperator2 = Nothing        'Interface ESRI pour rendre la nouvelle surface valide.
        Dim pGeomCollLigne As IGeometryCollection = Nothing   'Interface ESRI contenant les composantes d'une ligne.
        Dim pGeomCollSurface As IGeometryCollection = Nothing 'Interface ESRI contenant les composantes d'une surface.

        'Initialiser la valeur de retour
        pCreerSurfaceSelonLigneSelection = Nothing

        Try
            'Définir une ligne de travail
            pTempPolyline = CType(New Polyline, ITopologicalOperator2)

            'Initialiser la recher des éléments sélectionnés
            pEnumFeature = CType(m_MxDocument.FocusMap.FeatureSelection, IEnumFeature)
            pEnumFeature.Reset()

            'Trouver le premier élément de la sélection
            pFeature = pEnumFeature.Next

            'Union de toutes les lignes sélectionnées
            Do While Not pFeature Is Nothing
                'Vérifier la géométrie de l'élément est une ligne
                If pFeature.Shape.GeometryType = esriGeometryType.esriGeometryPolyline Then
                    'Ajouter la ligne trouvée dans la géométrie de travail
                    pTempPolyline = CType(pTempPolyline.Union(pFeature.ShapeCopy), ITopologicalOperator2)
                End If
                'Trouver le prochain élément de la sélection
                pFeature = pEnumFeature.Next
            Loop

            'Interface pour traiter chaque composante d'une ligne
            pGeomCollLigne = CType(pTempPolyline, IGeometryCollection)

            'Interface pour ajouter des Ring à une surface
            pGeomCollSurface = CType(New Polygon, IGeometryCollection)

            'Interface pour rendre la surface valide
            pTopoOp = CType(pGeomCollSurface, ITopologicalOperator2)

            'Traiter toutes les composantes de lignes
            For i = 0 To pGeomCollLigne.GeometryCount - 1
                'Définir la surface résultante
                pRingPointColl = New Ring

                'Créer une surface à partir de la ligne de travail
                pRingPointColl.AddPointCollection(CType(pGeomCollLigne.Geometry(CInt(i)), IPointCollection))

                'Définir un Ring de la surface
                pRing = CType(pRingPointColl, IRing)

                'Fermer la surface
                pRing.Close()

                'Ajouter un Ring à la surface
                pGeomCollSurface.AddGeometry(pRing)

                'Rendre la surface valide
                pTopoOp.Simplify()
            Next i

            'Retourner la surface valide
            pCreerSurfaceSelonLigneSelection = CType(pTopoOp, IPolygon)

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            pEnumFeature = Nothing
            pFeature = Nothing
            pRing = Nothing
            pRingPointColl = Nothing
            pTempPolyline = Nothing
            pTopoOp = Nothing
            pGeomCollLigne = Nothing
            pGeomCollSurface = Nothing
        End Try
    End Function

    '''<summary>
    ''' Permet d'identifier les géométries de travail en mémoire à partir de la sélection des
    ''' éléments de la vue active.
    '''</summary>
    '''
    Public Sub IdentifierGeometrie()
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                  'Interface ESRI contenant la Map active.
        Dim pEnumFeature As IEnumFeature = Nothing  'Interface ESRI utilisé pour extraire les éléments de la sélection.
        Dim pFeature As IFeature = Nothing          'Interface ESRI contenant un élément en sélection.
        Dim pGeometryBag As IGeometryBag = Nothing  'Interface ESRI contenant des géométries en mémoire.

        Try
            'Définir le document de ArcMap
            m_MxDocument = m_MxDocument

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vider la mémoire
            mpGeometrieTravail = Nothing
            'Récupérer la mémoire
            GC.Collect()

            'Créer un nouveau GeometryBag vide
            mpGeometrieTravail = CType(New GeometryBag, IGeometryCollection)

            'Interface pour définir la référence spatiale
            pGeometryBag = CType(mpGeometrieTravail, IGeometryBag)

            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference

            'Interface pour extraire le premier élément de la sélection
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Extraire le premier élément de la sélection
            pFeature = pEnumFeature.Next

            'Traitre tous les éléments de la sélection
            Do Until pFeature Is Nothing
                'Vérifier si la géométrie n'est pas nulle
                If pFeature.Shape IsNot Nothing Then
                    'Remplir le GeometryBag de travail
                    mpGeometrieTravail.AddGeometry(pFeature.ShapeCopy)
                End If

                'Extraire le prochain élément de la sélection
                pFeature = pEnumFeature.Next
            Loop

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryBag = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Permet d'identifier les limites des géométries de travail en mémoire à partir de la sélection des éléments de la vue active.
    '''</summary>
    '''
    Public Sub IdentifierLimite()
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                      'Interface ESRI contenant la Map active.
        Dim pEnumFeature As IEnumFeature = Nothing      'Interface ESRI utilisé pour extraire les éléments de la sélection.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant un élément en sélection.
        Dim pGeometryBag As IGeometryBag = Nothing      'Interface ESRI contenant des géométries en mémoire.
        Dim pTopoOp As ITopologicalOperator = Nothing   'Interface ESRI qui permet d'extraire les limites des géométries

        Try
            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vider la mémoire
            mpGeometrieTravail = Nothing
            'Récupérer la mémoire
            GC.Collect()

            'Créer un nouveau GeometryBag vide
            mpGeometrieTravail = CType(New GeometryBag, IGeometryCollection)

            'Interface pour définir la référence spatiale
            pGeometryBag = CType(mpGeometrieTravail, IGeometryBag)

            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference

            'Interface pour extraire le premier élément de la sélection
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Extraire le premier élément de la sélection
            pFeature = pEnumFeature.Next

            'Traitre tous les éléments de la sélection
            Do Until pFeature Is Nothing
                'Vérifier si la géométrie n'est pas nulle
                If pFeature.Shape IsNot Nothing Then
                    'Vérifier si la géométrie est de type ligne ou surface
                    If pFeature.Shape.Dimension > esriGeometryDimension.esriGeometry0Dimension Then
                        'Interface pour extraire les limites
                        pTopoOp = CType(pFeature.Shape, ITopologicalOperator)

                        'Remplir le GeometryBag de travail
                        mpGeometrieTravail.AddGeometry(pTopoOp.Boundary)
                    End If
                End If

                'Extraire le prochain élément de la sélection
                pFeature = pEnumFeature.Next
            Loop

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryBag = Nothing
            pTopoOp = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Permet d'identifier les sommets des géométries de travail en mémoire à partir de la sélection des éléments de la vue active.
    '''</summary>
    '''
    Public Sub IdentifierSommet()
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                      'Interface ESRI contenant la Map active.
        Dim pEnumFeature As IEnumFeature = Nothing      'Interface ESRI utilisé pour extraire les éléments de la sélection.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant un élément en sélection.
        Dim pGeometryBag As IGeometryBag = Nothing      'Interface ESRI contenant des géométries en mémoire.
        Dim pMultipoint As IMultipoint = Nothing        'Interface contenant les sommets.
        Dim pPointColl As IPointCollection = Nothing    'Interface pour ajouter des sommets.

        Try
            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vider la mémoire
            mpGeometrieTravail = Nothing
            'Récupérer la mémoire
            GC.Collect()

            'Créer un nouveau GeometryBag vide
            pGeometryBag = New GeometryBag
            pGeometryBag.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les géométries
            mpGeometrieTravail = CType(pGeometryBag, IGeometryCollection)

            'Interface pour extraire le premier élément de la sélection
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Extraire le premier élément de la sélection
            pFeature = pEnumFeature.Next

            'Traitre tous les éléments de la sélection
            Do Until pFeature Is Nothing
                'Vérifier si la géométrie n'est pas nulle
                If pFeature.Shape IsNot Nothing Then
                    'Vérifier si la géométrie est de type point
                    If pFeature.Shape.GeometryType = esriGeometryType.esriGeometryPoint Then
                        'Créer un nouveau GeometryBag vide
                        pMultipoint = New Multipoint
                        pMultipoint.SpatialReference = pMap.SpatialReference

                        'Interface pour ajouter les sommets
                        pPointColl = CType(pMultipoint, IPointCollection)

                        'Ajouter le sommet
                        pPointColl.AddPoint(CType(pFeature.ShapeCopy, IPoint))

                        'Remplir le GeometryBag de travail
                        mpGeometrieTravail.AddGeometry(pMultipoint)

                        'Si la géométrie est de type multipoint
                    ElseIf pFeature.Shape.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                        'Remplir le GeometryBag de travail
                        mpGeometrieTravail.AddGeometry(pFeature.ShapeCopy)

                        'Si la géométrie est de type ligne ou surface
                    Else
                        'Créer un nouveau GeometryBag vide
                        pMultipoint = New Multipoint
                        pMultipoint.SpatialReference = pMap.SpatialReference

                        'Interface pour ajouter les sommets
                        pPointColl = CType(pMultipoint, IPointCollection)

                        'Ajouter les sommets
                        pPointColl.AddPointCollection(CType(pFeature.ShapeCopy, IPointCollection))

                        'Remplir le GeometryBag de travail
                        mpGeometrieTravail.AddGeometry(pMultipoint)
                    End If
                End If

                'Extraire le prochain élément de la sélection
                pFeature = pEnumFeature.Next
            Loop

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryBag = Nothing
            pMultipoint = Nothing
            pPointColl = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet d'identifier la géométrie des Edges sélectionnés et de les mettre dans la géométrie de travail en mémoire. 
    '''</summary>
    '''
    Public Sub IdentifierEdge()
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                          'Interface ESRI contenant la Map active.
        Dim pEnumFeature As IEnumFeature = Nothing          'Interface ESRI utilisé pour extraire les éléments de la sélection.
        Dim pEdge As IPolyline = Nothing                    'Interface contenant le Edge traité
        Dim pEnumTopoEdge As IEnumTopologyEdge = Nothing    'Interface pour extraire les Edges sélectionnés
        Dim pTopoEdge As ITopologyEdge = Nothing            'Interface contenant un Edge sélectionné
        Dim pEnumTopoParent As IEnumTopologyParent = Nothing 'Interface pour extraire les éléments des Edges sélectionnés
        Dim pTopologyParent As esriTopologyParent = Nothing 'Interface contenant les parents des edges
        Dim pGeometryBag As IGeometryBag = Nothing          'Interface ESRI contenant des géométries en mémoire.
        Dim pGeometry As IGeometry = Nothing                'Interface ESRI contenant une géométrie de travail en mémoire.
        Dim pTopoOp As ITopologicalOperator2 = Nothing      'Interface ESRI qui permet d'extraire l'intersection des géométries

        Try
            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vider la mémoire
            mpGeometrieTravail = Nothing
            'Récupérer la mémoire
            GC.Collect()

            'Créer un nouveau GeometryBag vide
            mpGeometrieTravail = CType(New GeometryBag, IGeometryCollection)

            'Interface pour définir la référence spatiale
            pGeometryBag = CType(mpGeometrieTravail, IGeometryBag)

            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference

            'Interface pour extraire les Edges sélectionnés
            pEnumTopoEdge = m_TopologyGraph.EdgeSelection

            'Traiter tous les Edges sélectionnés
            For i = 1 To pEnumTopoEdge.Count
                'Extraire un Edge sélectionné
                pTopoEdge = pEnumTopoEdge.Next

                'Définir le Edge traité
                pEdge = CType(pTopoEdge.Geometry, IPolyline)

                'Interface pour extraire les éléments parent du Edge sélectionné
                pEnumTopoParent = pTopoEdge.Parents

                'Traiter tous les parents du Edge sélectionné
                If pEnumTopoParent.Count > 0 Then
                    'Interface pour extraire la classe et le OID d'un élément parent
                    pTopologyParent = pEnumTopoParent.Next()
                    'Interface pour trouver l'intersection
                    pTopoOp = CType(m_TopologyGraph.GetParentGeometry(pTopologyParent.m_pFC, pTopologyParent.m_FID), ITopologicalOperator2)
                    'Extraire l'intersection entre la géométrie de l'élément et du Edge afin de conserver le Z et le M au besoin
                    pGeometry = pTopoOp.Intersect(pEdge, esriGeometryDimension.esriGeometry1Dimension)
                    'Remplir le GeometryBag de travail
                    mpGeometrieTravail.AddGeometry(pGeometry)
                End If
            Next

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pEdge = Nothing
            pEnumTopoEdge = Nothing
            pTopoEdge = Nothing
            pEnumTopoParent = Nothing
            pTopologyParent = Nothing
            pGeometryBag = Nothing
            pGeometry = Nothing
            pTopoOp = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Permet de modifier le début des sommets des polygones en mémoire à partir de la sélection des éléments de la vue active.
    '''</summary>
    '''
    Public Sub ModifierDebutSommet()
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                  'Interface ESRI contenant la Map active.
        Dim pEnumFeature As IEnumFeature = Nothing  'Interface ESRI utilisé pour extraire les éléments de la sélection.
        Dim pFeature As IFeature = Nothing          'Interface ESRI contenant un élément en sélection.
        Dim pGeometryBag As IGeometryBag = Nothing  'Interface ESRI contenant des géométries en mémoire.
        Dim pGeometry As IGeometry = Nothing        'Interface contenant une géométrie.

        Try
            'Définir le document de ArcMap
            m_MxDocument = m_MxDocument

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vider la mémoire
            mpGeometrieTravail = Nothing
            'Récupérer la mémoire
            GC.Collect()

            'Créer un nouveau GeometryBag vide
            pGeometryBag = New GeometryBag

            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference

            'Interface pour ajouter les géométrie du diagramme de Voronoi
            mpGeometrieTravail = CType(pGeometryBag, IGeometryCollection)

            'Interface pour extraire le premier élément de la sélection
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Extraire le premier élément de la sélection
            pFeature = pEnumFeature.Next

            'Traitre tous les éléments de la sélection
            Do Until pFeature Is Nothing
                'Vérifier si la géométrie n'est pas nulle
                If pFeature.Shape IsNot Nothing Then
                    'Interface pour projeter la géométrie
                    pGeometry = pFeature.ShapeCopy
                    'Projeter la géométrie
                    pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

                    'Vérifier si la géométrie est une surface
                    If pGeometry.GeometryType = esriGeometryType.esriGeometryPolygon Then
                        Dim pPolygon As IPolygon
                        pPolygon = modGeometrieTravail.ModifierDebutSommet(CType(pGeometry, IPolygon))
                        'Ajouter la nouvelle géométrie
                        mpGeometrieTravail.AddGeometry(pPolygon)
                    End If
                End If

                'Extraire le prochain élément de la sélection
                pFeature = pEnumFeature.Next
            Loop

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryBag = Nothing
            pGeometry = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Permet de retourner un nouveau polygone dont le début de sommet de chaque anneau a été modifié en fonction de la position minimum maximum en X ou Y.
    '''</summary>
    '''
    '''<param name="pPolygon"> Polygone originale à traiter </param>
    '''
    '''<returns>Ipolygone contenant le nouveau polygone dont le début de sommets de ses composantes a été modifié.</returns>
    '''
    Public Function ModifierDebutSommet(ByVal pPolygon As IPolygon) As IPolygon
        'Déclarer les variables de travail
        Dim pPolygonNew As IPolygon = Nothing               'Interface contenant le nouveau polygone.
        Dim pGeomColl As IGeometryCollection = Nothing      'Interface pour extraire les anneaux du polygone.
        Dim pGeomCollNew As IGeometryCollection = Nothing   'Interface pour ajouter les anneaux du polygone.
        Dim pPointColl As IPointCollection = Nothing        'Interface pour ajouter les sommets de l'anneau du polygone.
        Dim pPointCollNew As IPointCollection = Nothing     'Interface pour ajouter les sommets du nouvel anneau du polygone.
        Dim pRing As IRing = Nothing                        'Interface contenant un anneau du polygone.
        Dim pRingNew As IRing = Nothing                     'Interface contenant un nouvel anneau du polygone.
        Dim iNoDeb As Integer = 0      'Contient la position du point minimum.

        'Définir la valeur par défaut
        ModifierDebutSommet = pPolygon

        Try
            'Vérifier si la géométrie est valide
            If pPolygon IsNot Nothing Then
                'Créer un nouveau polygone vide
                pPolygonNew = New Polygon
                pPolygonNew.SpatialReference = pPolygon.SpatialReference
                'Interface pour ajouter les anneaux du polygone
                pGeomCollNew = CType(pPolygonNew, IGeometryCollection)

                'Interface pour extraire les anneaux du polygone
                pGeomColl = CType(pPolygon, IGeometryCollection)

                'Traiter tous les anneaux du  polygone
                For i = 0 To pGeomColl.GeometryCount - 1
                    'Définir l'anneau à traiter
                    pRing = CType(pGeomColl.Geometry(i), IRing)
                    'Interface pour extraire les sommets de l'anneau
                    pPointColl = CType(pRing, IPointCollection)

                    'Définir la position minimum défaut
                    iNoDeb = 0

                    'Vérifier si le nouveau début est basé sur le minimum en X
                    If pRing.Envelope.Width > pRing.Envelope.Height Then
                        'Traiter tous les sommets de l'anneau
                        For j = 1 To pPointColl.PointCount - 1
                            'Vérifier si le sommet X traiter est inférieur au dernier trouvé
                            If pPointColl.Point(j).X < pPointColl.Point(iNoDeb).X Then
                                'Conserver le numéro du sommet minimum
                                iNoDeb = j
                            End If
                        Next

                        'Si le nouveau début est basé sur le minimum en Y
                    Else
                        'Traiter tous les sommets de l'anneau
                        For j = 1 To pPointColl.PointCount - 1
                            'Vérifier si le sommet Y traiter est inférieur au dernier trouvé
                            If pPointColl.Point(j).Y < pPointColl.Point(iNoDeb).Y Then
                                'Conserver le numéro du sommet minimum
                                iNoDeb = j
                            End If
                        Next
                    End If

                    'Vérifier si le numéro de début ne change pas
                    If iNoDeb = 0 Then
                        'Ajouter l'anneau originale
                        pGeomCollNew.AddGeometry(pRing)

                        'Si le numéro de début change
                    Else
                        'Créer un nouvel anneau vide
                        pRingNew = New Ring
                        pRingNew.SpatialReference = pRing.SpatialReference
                        'Interface pour extraire les sommets de l'anneau
                        pPointCollNew = CType(pRingNew, IPointCollection)
                        'Traiter tous les points de fin
                        For j = iNoDeb To pPointColl.PointCount - 1
                            'Ajouter le point
                            pPointCollNew.AddPoint(pPointColl.Point(j))
                        Next
                        'Traiter tous les points de début
                        For j = 1 To iNoDeb - 1
                            'Ajouter le point
                            pPointCollNew.AddPoint(pPointColl.Point(j))
                        Next

                        'Fermer l'anneau
                        pRingNew.Close()

                        'Ajouter le nouvel anneau
                        pGeomCollNew.AddGeometry(pRingNew)
                    End If
                Next

                'Retourner le nouveau polygone
                ModifierDebutSommet = pPolygonNew
            End If

        Catch ex As Exception
            Throw
        Finally
            'Vider la mémoire
            pPolygonNew = Nothing
            pGeomColl = Nothing
            pGeomCollNew = Nothing
            pPointColl = Nothing
            pPointCollNew = Nothing
            pRing = Nothing
            pRingNew = Nothing
        End Try
    End Function

    '''<summary>
    ''' Permet de créer les lignes des triangles de Delaunay des éléments sélectionnés de la vue active et les mettre en mémoire dans la géométrie de travail.
    '''</summary>
    '''
    '''<param name="dDistLat"> Distance latérale utilisée pour éliminer des sommets en trop.</param>
    '''<param name="dDistMin"> Distance minimum utilisée pour ajouter des sommets.</param>
    ''' 
    Public Sub CreerLignesTriangles(ByVal dDistLat As Double, dDistMin As Double)
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                      'Interface ESRI contenant la Map active.
        Dim pEnumFeature As IEnumFeature = Nothing      'Interface ESRI utilisé pour extraire les éléments de la sélection.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant un élément en sélection.
        Dim pGeometryBag As IGeometryBag = Nothing      'Interface ESRI contenant des géométries en mémoire.
        Dim pGeometry As IGeometry = Nothing            'Interface contenant une géométrie.
        Dim pGeomColl As IGeometryCollection = Nothing  'Interface pour ajouter des géométries.
        Dim pTriangles As IPolyline = Nothing
        Dim pPolyline As IPolyline = Nothing
        Dim pPolylineExt As IPolyline = Nothing
        Dim pPolygon As IPolygon = Nothing
        Dim pPolygonExt As IPolygon = Nothing
        Dim pLignesTriangles As IPolyline = Nothing     'Interface contenant les lignes des triangles.
        Dim pLignesInt As IPolyline = Nothing           'Interface contenant les lignes des triangles intérieures.
        Dim pLignesExt As IPolyline = Nothing           'Interface contenant les lignes des triangles intérieures.
        Dim pTopoOp As ITopologicalOperator2 = Nothing

        Try
            'Définir le document de ArcMap
            m_MxDocument = m_MxDocument

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vider la mémoire
            mpGeometrieTravail = Nothing
            'Récupérer la mémoire
            GC.Collect()

            'Créer un nouveau GeometryBag vide
            pGeometryBag = New GeometryBag

            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference

            'Interface pour ajouter les géométrie du diagramme de Voronoi
            mpGeometrieTravail = CType(pGeometryBag, IGeometryCollection)

            'Interface pour extraire le premier élément de la sélection
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Extraire le premier élément de la sélection
            pFeature = pEnumFeature.Next

            'Traitre tous les éléments de la sélection
            Do Until pFeature Is Nothing
                'Vérifier si la géométrie n'est pas nulle
                If pFeature.Shape IsNot Nothing Then
                    'Interface pour projeter la géométrie
                    pGeometry = pFeature.ShapeCopy
                    'Projeter la géométrie
                    pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

                    'Si la géométrie est une surface
                    If pGeometry.GeometryType = esriGeometryType.esriGeometryPolygon Then
                        'Définir le polygone
                        pPolygon = CType(pGeometry, IPolygon)

                        'Enlever les sommets en trop afin d'enlever les extrémités en ligne droite
                        'pPolygon.Generalize(dDistLat)
                        'Densifier les sommets
                        pPolygon.Densify(dDistMin, 0)

                        'Définir le polygone extérieur
                        pPolygonExt = TriangulationDelaunay.CreerPolygoneExterieur(pPolygon, dDistMin)
                        'Ajouter le polygone extérieure
                        mpGeometrieTravail.AddGeometry(pPolygonExt)

                        'Créer les lignes intérieures et extérieures des triangles
                        pTriangles = TriangulationDelaunay.CreerPolyligneTrianglesDelaunay(pPolygonExt, pLignesTriangles)

                        'Interface pour couper les lignes
                        pTopoOp = CType(pLignesTriangles, ITopologicalOperator2)

                        'Créer les lignes intérieures
                        pLignesInt = CType(pTopoOp.Intersect(pPolygon, esriGeometryDimension.esriGeometry1Dimension), IPolyline)
                        'Ajouter les lignes des triangles intérieures
                        mpGeometrieTravail.AddGeometry(pLignesInt)

                        'Créer les lignes extérieures
                        pLignesExt = CType(pTopoOp.Intersect(pPolygonExt, esriGeometryDimension.esriGeometry1Dimension), IPolyline)
                        'Ajouter les lignes des triangles extérieures
                        mpGeometrieTravail.AddGeometry(pLignesExt)

                        'Ajouter les lignes des triangles extérieures
                        mpGeometrieTravail.AddGeometry(pTriangles)

                        'Si la géométrie est une ligne
                    ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then
                        'Définir la polyligne
                        pPolyline = CType(pGeometry, IPolyline)

                        'Enlever les sommets en trop afin d'enlever les extrémités en ligne droite
                        'pPolyline.Generalize(dDistLat)
                        'Densifier les sommets
                        pPolyline.Densify(dDistMin, 0)

                        'Définir le polygone extérieur
                        pPolygonExt = TriangulationDelaunay.CreerPolygoneExterieur(pPolyline, dDistMin)
                        'Ajouter le polygone extérieure
                        mpGeometrieTravail.AddGeometry(pPolygonExt)

                        'Interface pour extraire la limite du polygone extérieur
                        pTopoOp = CType(pPolygonExt, ITopologicalOperator2)
                        'Extraire la limite du polygone extérieur
                        pPolylineExt = CType(pTopoOp.Boundary, IPolyline)
                        'Interface pour ajouter des géométries
                        pGeomColl = CType(pPolylineExt, IGeometryCollection)
                        'Ajouter des géométries
                        pGeomColl.AddGeometryCollection(CType(pPolyline, IGeometryCollection))

                        'Ajouter la polyligne extérieure
                        mpGeometrieTravail.AddGeometry(pPolylineExt)

                        'Ajouter les lignes des triangles
                        'mpGeometrieTravail.AddGeometry(TriangulationDelaunay.CreerPolyligneTrianglesDelaunay(pPolylineExt))

                        'Créer les lignes intérieures et extérieures des triangles
                        pTriangles = TriangulationDelaunay.CreerPolyligneTrianglesDelaunay(pPolylineExt, pLignesTriangles)

                        'Extraire la limite du polygone extérieur
                        pLignesTriangles = CType(pTopoOp.Intersect(pLignesTriangles, esriGeometryDimension.esriGeometry1Dimension), IPolyline)

                        'Ajouter les lignes des triangles extérieures
                        mpGeometrieTravail.AddGeometry(pLignesTriangles)

                        'Ajouter les lignes des triangles extérieures
                        mpGeometrieTravail.AddGeometry(pTriangles)
                    End If
                End If

                'Extraire le prochain élément de la sélection
                pFeature = pEnumFeature.Next
            Loop

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryBag = Nothing
            pGeometry = Nothing
            pLignesInt = Nothing
            pLignesExt = Nothing
            pTopoOp = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Permet de créer les squelettes des éléments sélectionnés de la vue active et les mettre en mémoire dans la géométrie de travail.
    '''</summary>
    '''
    '''<param name="dDistLat"> Distance latérale utilisée pour éliminer des sommets en trop.</param>
    '''<param name="dDistMin"> Distance minimum utilisée pour ajouter des sommets.</param>
    '''<param name="dLongMin"> Longueur minimale utilisée pour éliminer des lignes du squelette trop petites.</param>
    ''' 
    Public Sub CreerSquelettePolygone(ByVal dDistLat As Double, dDistMin As Double, ByVal dLongMin As Double, Optional ByVal iMethode As Integer = 0)
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                      'Interface ESRI contenant la Map active.
        Dim pEnumFeature As IEnumFeature = Nothing      'Interface ESRI utilisé pour extraire les éléments de la sélection.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant un élément en sélection.
        Dim pGeometryBag As IGeometryBag = Nothing      'Interface ESRI contenant les géométries à traiter.
        Dim pGeometryBagRel As IGeometryBag = Nothing   'Interface ESRI contenant les géométries en relation.
        Dim pGeometry As IGeometry = Nothing            'Interface contenant une géométrie.
        Dim pPolygon As IPolygon4 = Nothing              'Interface contenant le super polygone à traiter.
        Dim pSquelette As IPolyline = Nothing           'Interface contenant le squelette du polygone.
        Dim pBagDroites As IGeometryBag = Nothing       'Interface contenant les droites de Delaunay du polygone.
        Dim pGeomColl As IGeometryCollection = Nothing  'Interface pour ajouter les géométries à traiter.
        Dim pGeomCollRel As IGeometryCollection = Nothing 'Interface pour ajouter les géométries en relation.
        Dim pTopoOp As ITopologicalOperator2 = Nothing  'Interface pour fusionner les polygones à traiter.
        Dim pSpatialRefRes As ISpatialReferenceResolution = Nothing 'Interface qui permet d'initialiser la résolution XY.
        Dim pSpatialRefTol As ISpatialReferenceTolerance = Nothing  'Interface qui permet d'initialiser la tolérance XY.
        Dim pPointsConnexion As IMultipoint = Nothing   'Interface contenant les points de connexion.

        Try
            'Définir le document de ArcMap
            m_MxDocument = m_MxDocument

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vider la mémoire
            mpGeometrieTravail = Nothing
            'Récupérer la mémoire
            GC.Collect()

            'Initialiser la résolution
            pSpatialRefRes = CType(pMap.SpatialReference, ISpatialReferenceResolution)
            pSpatialRefRes.SetDefaultXYResolution()
            'Interface pour définir la tolérance XY
            pSpatialRefTol = CType(pMap.SpatialReference, ISpatialReferenceTolerance)
            pSpatialRefTol.XYTolerance = pSpatialRefRes.XYResolution(True) * 2
            pSpatialRefTol.XYTolerance = 0.001

            'Créer un nouveau GeometryBag vide pour les polygones à traiter
            pGeometryBag = New GeometryBag
            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les polygones à traiter
            pGeomColl = CType(pGeometryBag, IGeometryCollection)

            'Créer un nouveau GeometryBag vide pour les géométries en relation
            pGeometryBagRel = New GeometryBag
            'Définir la référence spatiale
            pGeometryBagRel.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les  géométries en relation
            pGeomCollRel = CType(pGeometryBagRel, IGeometryCollection)

            'Interface pour extraire le premier élément de la sélection
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Extraire le premier élément de la sélection
            pFeature = pEnumFeature.Next

            'Traitre tous les éléments de la sélection
            Do Until pFeature Is Nothing
                'Vérifier si la géométrie n'est pas nulle
                If pFeature.Shape IsNot Nothing Then
                    'Interface pour projeter la géométrie
                    pGeometry = pFeature.Shape

                    'Projeter la géométrie
                    pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

                    'Vérifier si la géométrie est une surface
                    If pFeature.Shape.GeometryType = esriGeometryType.esriGeometryPolygon Then
                        'Ajouter le polygone à traiter
                        pGeomColl.AddGeometry(pGeometry)

                        'Si la géométrie n'est pas une surface
                    Else
                        'Ajouter la géométrie en relation
                        pGeomCollRel.AddGeometry(pGeometry)
                    End If
                End If

                'Extraire le prochain élément de la sélection
                pFeature = pEnumFeature.Next
            Loop

            'Créer un nouveau polygone vide
            pPolygon = New Polygon
            'Définir la référence spatiale
            pPolygon.SpatialReference = pMap.SpatialReference

            'Simplifier le SuperPolygon
            pTopoOp = CType(pPolygon, ITopologicalOperator2)
            pTopoOp.ConstructUnion(CType(pGeomColl, IEnumGeometry))

            'Créer un nouveau GeometryBag vide
            pGeometryBag = New GeometryBag
            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les géométrie du diagramme de Voronoi
            mpGeometrieTravail = CType(pGeometryBag, IGeometryCollection)

            'Vérifier si le polygone n'est pas vide
            If Not pPolygon.IsEmpty Then
                'Vérifier si la méthode est Delaunay
                If iMethode = 0 Then
                    'Extraire les points d'intersection
                    pPointsConnexion = clsGeneraliserGeometrie.ExtrairePointsIntersection(pPolygon, pGeometryBagRel)

                    'Créer le squelette du polygone
                    TriangulationDelaunay.CreerSquelettePolygoneDelaunay(pPolygon, pPointsConnexion, dDistLat, dDistMin, dLongMin, pSquelette, pBagDroites)

                    'Ajouter les lignes du squelette dans le Bag selon la méthode de Delaunay
                    mpGeometrieTravail.AddGeometry(pSquelette)

                    'Ajouter le polygone de généralisation
                    'mpGeometrieTravail.AddGeometry(pPointsConnexion)

                    'Ajouter les droites de Delaunay
                    'mpGeometrieTravail.AddGeometry(pBagDroites)

                    'Si la méthode est Voronoy
                Else
                    'Extraire les points d'intersection
                    pPointsConnexion = clsGeneraliserGeometrie.ExtrairePointsIntersection(pPolygon, pGeometryBagRel)

                    'Définir le squelette
                    pSquelette = DiagrammeVoronoi.CreerSquelettePolygoneVoronoi(pPolygon, pPointsConnexion, dDistLat, dDistMin, dLongMin)

                    'Ajouter les lignes du squelette dans le Bag selon la méthode de Voronoi
                    mpGeometrieTravail.AddGeometry(pSquelette)

                    'Ajouter le polygone de généralisation
                    'mpGeometrieTravail.AddGeometry(pPointsConnexion)
                End If
            End If

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryBag = Nothing
            pGeometry = Nothing
            pSquelette = Nothing
            pBagDroites = Nothing
            pPointsConnexion = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Permet de créer la généralisation intérieure de polygone pour les éléments sélectionnés de la vue active et les mettre en mémoire dans la géométrie de travail.
    '''</summary>
    '''
    '''<param name="dDistLat"> Distance latérale utilisée pour éliminer des sommets en trop.</param>
    '''<param name="dLargMin"> Largeur minimale de généralisation.</param>
    '''<param name="dLongMin"> Longueur minimale de généralisation.</param>
    '''<param name="dSupMin"> Contient la superficie de généralisation minimum.</param>
    ''' 
    Public Sub GeneraliserPolygoneInterieur(ByVal dDistLat As Double, dLargMin As Double, ByVal dLongMin As Double, ByVal dSupMin As Double)
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                      'Interface ESRI contenant la Map active.
        Dim pEnumFeature As IEnumFeature = Nothing      'Interface ESRI utilisé pour extraire les éléments de la sélection.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant un élément en sélection.
        Dim pGeometryBag As IGeometryBag = Nothing      'Interface ESRI contenant les géométries à traiter.
        Dim pGeometryBagRel As IGeometryBag = Nothing   'Interface ESRI contenant les géométries en relation.
        Dim pGeometry As IGeometry = Nothing            'Interface contenant une géométrie.
        Dim pPolygon As IPolygon4 = Nothing             'Interface contenant le super polygone à traiter.
        Dim pPolygonGen As IPolygon = Nothing           'Interface contenant le polygone de généralisation.
        Dim pPolylineErr As IPolyline = Nothing         'Interface contenant la polyligne d'erreur de généralisation.
        Dim pSquelette As IPolyline = Nothing           'Interface contenant le squelette du polygone.
        Dim pBagDroites As IGeometryBag = Nothing       'Interface contenant les droites de Delaunay.
        Dim pGeomColl As IGeometryCollection = Nothing  'Interface pour ajouter les géométries à traiter.
        Dim pGeomCollRel As IGeometryCollection = Nothing   'Interface pour ajouter les géométries en relation.
        Dim pTopoOp As ITopologicalOperator2 = Nothing      'Interface pour fusionner les polygones à traiter.
        Dim pSpatialRefRes As ISpatialReferenceResolution = Nothing 'Interface qui permet d'initialiser la résolution XY.
        Dim pSpatialRefTol As ISpatialReferenceTolerance = Nothing  'Interface qui permet d'initialiser la tolérance XY.
        Dim pPointsConnexion As IMultipoint = Nothing       'Interface contenant les points de connexion.

        Try
            'Définir le document de ArcMap
            m_MxDocument = m_MxDocument

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vider la mémoire
            mpGeometrieTravail = Nothing
            'Récupérer la mémoire
            GC.Collect()

            'Initialiser la résolution
            pSpatialRefRes = CType(pMap.SpatialReference, ISpatialReferenceResolution)
            pSpatialRefRes.SetDefaultXYResolution()
            'Interface pour définir la tolérance XY
            pSpatialRefTol = CType(pMap.SpatialReference, ISpatialReferenceTolerance)
            pSpatialRefTol.XYTolerance = pSpatialRefRes.XYResolution(True) * 2
            pSpatialRefTol.XYTolerance = 0.001

            'Créer un nouveau GeometryBag vide pour les polygones à traiter
            pGeometryBag = New GeometryBag
            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les polygones à traiter
            pGeomColl = CType(pGeometryBag, IGeometryCollection)

            'Créer un nouveau GeometryBag vide pour les géométries en relation
            pGeometryBagRel = New GeometryBag
            'Définir la référence spatiale
            pGeometryBagRel.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les  géométries en relation
            pGeomCollRel = CType(pGeometryBagRel, IGeometryCollection)

            'Interface pour extraire le premier élément de la sélection
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Extraire le premier élément de la sélection
            pFeature = pEnumFeature.Next

            'Traitre tous les éléments de la sélection
            Do Until pFeature Is Nothing
                'Interface pour projeter la géométrie
                pGeometry = pFeature.Shape

                'Projeter la géométrie
                pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

                'Vérifier si la géométrie est une surface
                If pFeature.Shape.GeometryType = esriGeometryType.esriGeometryPolygon Then
                    'Ajouter le polygone à traiter
                    pGeomColl.AddGeometry(pGeometry)
                    'Ajouter la géométrie en relation
                    pTopoOp = CType(pGeometry, ITopologicalOperator2)
                    pGeomCollRel.AddGeometry(pTopoOp.Boundary)

                    'Si la géométrie n'est pas une surface
                Else
                    'Ajouter la géométrie en relation
                    pGeomCollRel.AddGeometry(pGeometry)
                End If

                'Extraire le prochain élément de la sélection
                pFeature = pEnumFeature.Next
            Loop

            'Créer un nouveau polygone vide
            pPolygon = New Polygon
            'Définir la référence spatiale
            pPolygon.SpatialReference = pMap.SpatialReference

            'Simplifier le SuperPolygon
            pTopoOp = CType(pPolygon, ITopologicalOperator2)
            pTopoOp.ConstructUnion(CType(pGeomColl, IEnumGeometry))

            'Créer un nouveau GeometryBag vide
            pGeometryBag = New GeometryBag
            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les géométrie du diagramme de Voronoi
            mpGeometrieTravail = CType(pGeometryBag, IGeometryCollection)

            'Vérifier si le polygone n'est pas vide
            If Not pPolygon.IsEmpty Then
                'Extraire les points d'intersection
                pPointsConnexion = clsGeneraliserGeometrie.ExtrairePointsIntersection(pPolygon, pGeometryBagRel)

                'Généraliser l'intérieur du polygone
                Call clsGeneraliserGeometrie.GeneraliserInterieurPolygone(pPolygon, pPointsConnexion, dDistLat, dLargMin, dLongMin, dSupMin, pPolygonGen, pPolylineErr, pSquelette, pBagDroites)

                'Ajouter le polygone de généralisation
                mpGeometrieTravail.AddGeometry(pPolygonGen)

                'Ajouter la polyligne d'erreur de généralisation intérieures
                mpGeometrieTravail.AddGeometry(pPolylineErr)

                'Ajouter le squelette du polygone
                mpGeometrieTravail.AddGeometry(pSquelette)

                'Ajouter le polygone de généralisation
                mpGeometrieTravail.AddGeometry(pPointsConnexion)

                'Ajouter les lignes de Delaunay
                mpGeometrieTravail.AddGeometry(pBagDroites)

                'mpGeometrieTravail = CType(pBagDroites, IGeometryCollection)
            End If

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryBag = Nothing
            pGeometryBagRel = Nothing
            pGeometry = Nothing
            pPolygon = Nothing
            pPolygonGen = Nothing
            pPolylineErr = Nothing
            pGeomColl = Nothing
            pGeomCollRel = Nothing
            pTopoOp = Nothing
            pSpatialRefRes = Nothing
            pSpatialRefTol = Nothing
            pPointsConnexion = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Permet de créer la généralisation intérieure de polygone pour les éléments sélectionnés de la vue active et les mettre en mémoire dans la géométrie de travail.
    '''</summary>
    '''
    '''<param name="dDistLat"> Distance latérale utilisée pour éliminer des sommets en trop.</param>
    '''<param name="dLargMin"> Largeur minimale de généralisation.</param>
    '''<param name="dLongMin"> Longueur minimale de généralisation.</param>
    '''<param name="dSupMin"> Contient la superficie de généralisation minimum.</param>
    ''' 
    Public Sub GeneraliserPolygoneInterieur2(ByVal dDistLat As Double, dLargMin As Double, ByVal dLongMin As Double, ByVal dSupMin As Double)
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                      'Interface ESRI contenant la Map active.
        Dim pEnumFeature As IEnumFeature = Nothing      'Interface ESRI utilisé pour extraire les éléments de la sélection.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant un élément en sélection.
        Dim pGeometryBag As IGeometryBag = Nothing      'Interface ESRI contenant les géométries à traiter.
        Dim pGeometry As IGeometry = Nothing            'Interface contenant une géométrie.
        Dim pPolygon As IPolygon4 = Nothing             'Interface contenant le super polygone à traiter.
        Dim pPolygonGen As IPolygon = Nothing           'Interface contenant le polygone de généralisation.
        Dim pPolylineErr As IPolyline = Nothing         'Interface contenant la polyligne d'erreur de généralisation.
        Dim pSquelette As IPolyline = Nothing           'Interface contenant le squelette du polygone.
        Dim pBagDroites As IGeometryBag = Nothing       'Interface contenant les droites de Delaunay.
        Dim pGeomColl As IGeometryCollection = Nothing  'Interface pour ajouter les géométries à traiter.
        Dim pGeomCollRel As IGeometryCollection = Nothing   'Interface pour ajouter les géométries en relation.
        Dim pTopoOp As ITopologicalOperator2 = Nothing      'Interface pour fusionner les polygones à traiter.
        Dim pSpatialRefRes As ISpatialReferenceResolution = Nothing 'Interface qui permet d'initialiser la résolution XY.
        Dim pSpatialRefTol As ISpatialReferenceTolerance = Nothing  'Interface qui permet d'initialiser la tolérance XY.
        Dim pPointsConnexion As IMultipoint = Nothing       'Interface contenant les points de connexion.
        Dim pTopologyGraph As ITopologyGraph = Nothing      'Interface contenant la topologie.
        Dim pFeatureLayersColl As Collection = Nothing      'Objet contenant la collection des FeatureLayers utilisés dans la Topologie.
        Dim pGererMapLayer As clsGererMapLayer = Nothing    'Objet utiliser pour extraire la collection des FeatureLayers visibles.

        Try
            'Définir le document de ArcMap
            m_MxDocument = m_MxDocument

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vider la mémoire
            mpGeometrieTravail = Nothing
            'Récupérer la mémoire
            GC.Collect()

            'Initialiser la résolution
            pSpatialRefRes = CType(pMap.SpatialReference, ISpatialReferenceResolution)
            pSpatialRefRes.SetDefaultXYResolution()
            'Interface pour définir la tolérance XY
            pSpatialRefTol = CType(pMap.SpatialReference, ISpatialReferenceTolerance)
            pSpatialRefTol.XYTolerance = pSpatialRefRes.XYResolution(True) * 2
            pSpatialRefTol.XYTolerance = 0.001

            'Créer un nouveau GeometryBag vide pour les polygones à traiter
            pGeometryBag = New GeometryBag
            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les polygones à traiter
            pGeomColl = CType(pGeometryBag, IGeometryCollection)
            'Interface pour ajouter les géométries dans le Bag
            mpGeometrieTravail = CType(pGeometryBag, IGeometryCollection)

            'Objet utiliser pour extraire la collection des FeatureLayers
            pGererMapLayer = New clsGererMapLayer(m_MxDocument.FocusMap)

            'Définir la collection des FeatureLayers utilisés dans la topologie
            pFeatureLayersColl = pGererMapLayer.DefinirCollectionFeatureLayer(False)

            'Création de la topologie
            Debug.Print("Créer la topologie : Précision=" & pSpatialRefTol.XYTolerance.ToString & " ...")
            pTopologyGraph = CreerTopologyGraph(m_MxDocument.ActiveView.Extent, pFeatureLayersColl, pSpatialRefTol.XYTolerance)

            'Interface pour extraire le premier élément de la sélection
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Extraire le premier élément de la sélection
            pFeature = pEnumFeature.Next

            'Traitre tous les éléments de la sélection
            Do Until pFeature Is Nothing
                'Vérifier si la géométrie est une surface
                If pFeature.Shape.GeometryType = esriGeometryType.esriGeometryPolygon Then
                    'Interface pour projeter la géométrie
                    pPolygon = CType(pFeature.Shape, IPolygon4)

                    'Projeter la géométrie
                    pPolygon.Project(m_MxDocument.FocusMap.SpatialReference)

                    'Extraire les points d'intersection
                    pPointsConnexion = clsGeneraliserGeometrie.ExtrairePointsIntersection(pFeature, pTopologyGraph)

                    'Généraliser l'intérieur du polygone
                    Call clsGeneraliserGeometrie.GeneraliserInterieurPolygone(pPolygon, pPointsConnexion, dDistLat, dLargMin, dLongMin, dSupMin,
                                                                              pPolygonGen, pPolylineErr, pSquelette, pBagDroites)

                    'Ajouter le polygone de généralisation
                    mpGeometrieTravail.AddGeometry(pPolygonGen)

                    'Ajouter la polyligne d'erreur de généralisation intérieures
                    mpGeometrieTravail.AddGeometry(pPolylineErr)

                    'Ajouter le squelette du polygone
                    mpGeometrieTravail.AddGeometry(pSquelette)

                    'Ajouter le polygone de généralisation
                    'mpGeometrieTravail.AddGeometry(pPointsConnexion)

                    'Ajouter les lignes de Delaunay
                    'mpGeometrieTravail.AddGeometry(pBagDroites)
                End If

                'Extraire le prochain élément de la sélection
                pFeature = pEnumFeature.Next
            Loop

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryBag = Nothing
            pGeometry = Nothing
            pPolygon = Nothing
            pPolygonGen = Nothing
            pPolylineErr = Nothing
            pGeomColl = Nothing
            pGeomCollRel = Nothing
            pTopoOp = Nothing
            pSpatialRefRes = Nothing
            pSpatialRefTol = Nothing
            pPointsConnexion = Nothing
            pTopologyGraph = Nothing
            pFeatureLayersColl = Nothing
            pGererMapLayer = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Permet de créer la généralisation extérieure de polygone pour les éléments sélectionnés de la vue active et les mettre en mémoire dans la géométrie de travail.
    '''</summary>
    '''
    '''<param name="dDistLat"> Distance latérale utilisée pour éliminer des sommets en trop.</param>
    '''<param name="dLargMin"> Largeur minimale de généralisation.</param>
    '''<param name="dLongMin"> Longueur minimale de généralisation.</param>
    '''<param name="dSupMin"> Contient la superficie de généralisation minimum.</param>
    ''' 
    Public Sub GeneraliserPolygoneExterieur(ByVal dDistLat As Double, dLargMin As Double, ByVal dLongMin As Double, ByVal dSupMin As Double)
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                      'Interface ESRI contenant la Map active.
        Dim pEnumFeature As IEnumFeature = Nothing      'Interface ESRI utilisé pour extraire les éléments de la sélection.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant un élément en sélection.
        Dim pGeometryBag As IGeometryBag = Nothing      'Interface ESRI contenant les géométries à traiter.
        Dim pGeometryBagRel As IGeometryBag = Nothing   'Interface ESRI contenant les géométries en relation.
        Dim pGeometry As IGeometry = Nothing            'Interface contenant une géométrie.
        Dim pPolygon As IPolygon = Nothing              'Interface contenant le super polygone à traiter.
        Dim pPolygonGen As IPolygon = Nothing           'Interface contenant le polygone de généralisation.
        Dim pPolylineErr As IPolyline = Nothing         'Interface contenant les lignes de généralisation.
        Dim pSquelette As IPolyline = Nothing           'Interface contenant le squelette du polygone.
        Dim pBagDroites As IGeometryBag = Nothing       'Interface contenant les droites de Delaunay.
        Dim pGeomColl As IGeometryCollection = Nothing  'Interface pour ajouter les géométries à traiter.
        Dim pGeomCollRel As IGeometryCollection = Nothing 'Interface pour ajouter les géométries en relation.
        Dim pTopoOp As ITopologicalOperator2 = Nothing  'Interface pour fusionner les polygones à traiter.
        Dim pSpatialRefRes As ISpatialReferenceResolution = Nothing 'Interface qui permet d'initialiser la résolution XY.
        Dim pSpatialRefTol As ISpatialReferenceTolerance = Nothing  'Interface qui permet d'initialiser la tolérance XY.
        Dim pPointsConnexion As IMultipoint = Nothing   'Interface contenant les points de connexion.

        Try
            'Définir le document de ArcMap
            m_MxDocument = m_MxDocument

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vider la mémoire
            mpGeometrieTravail = Nothing
            'Récupérer la mémoire
            GC.Collect()

            'Initialiser la résolution
            pSpatialRefRes = CType(pMap.SpatialReference, ISpatialReferenceResolution)
            pSpatialRefRes.SetDefaultXYResolution()
            'Interface pour définir la tolérance XY
            pSpatialRefTol = CType(pMap.SpatialReference, ISpatialReferenceTolerance)
            pSpatialRefTol.XYTolerance = pSpatialRefRes.XYResolution(True) * 2
            pSpatialRefTol.XYTolerance = 0.001

            'Créer un nouveau GeometryBag vide pour les polygones à traiter
            pGeometryBag = New GeometryBag
            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les polygones à traiter
            pGeomColl = CType(pGeometryBag, IGeometryCollection)

            'Créer un nouveau GeometryBag vide pour les géométries en relation
            pGeometryBagRel = New GeometryBag
            'Définir la référence spatiale
            pGeometryBagRel.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les  géométries en relation
            pGeomCollRel = CType(pGeometryBagRel, IGeometryCollection)

            'Interface pour extraire le premier élément de la sélection
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Extraire le premier élément de la sélection
            pFeature = pEnumFeature.Next

            'Traitre tous les éléments de la sélection
            Do Until pFeature Is Nothing
                'Interface pour projeter la géométrie
                pGeometry = pFeature.Shape

                'Projeter la géométrie
                pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

                'Vérifier si la géométrie est une surface
                If pFeature.Shape.GeometryType = esriGeometryType.esriGeometryPolygon Then
                    'Ajouter le polygone à traiter
                    pGeomColl.AddGeometry(pGeometry)

                    'Si la géométrie n'est pas une surface
                Else
                    'Ajouter la géométrie en relation
                    pGeomCollRel.AddGeometry(pGeometry)
                End If

                'Extraire le prochain élément de la sélection
                pFeature = pEnumFeature.Next
            Loop

            'Créer un nouveau polygone vide
            pPolygon = New Polygon
            'Définir la référence spatiale
            pPolygon.SpatialReference = pMap.SpatialReference

            'Simplifier le SuperPolygon
            pTopoOp = CType(pPolygon, ITopologicalOperator2)
            pTopoOp.ConstructUnion(CType(pGeomColl, IEnumGeometry))

            'Créer un nouveau GeometryBag vide
            pGeometryBag = New GeometryBag
            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les géométrie du diagramme de Voronoi
            mpGeometrieTravail = CType(pGeometryBag, IGeometryCollection)

            'Vérifier si le polygone n'est pas vide
            If Not pPolygon.IsEmpty Then
                'Extraire les points d'intersection
                pPointsConnexion = clsGeneraliserGeometrie.ExtrairePointsIntersection(pPolygon, pGeometryBagRel)

                'Généraliser l'extérieur du polygone
                Call clsGeneraliserGeometrie.GeneraliserExterieurPolygone(pPolygon, pPointsConnexion, dDistLat, dLargMin, dLongMin, dSupMin,
                                                                          pPolygonGen, pPolylineErr, pSquelette, pBagDroites)

                'mpGeometrieTravail = CType(pBagDroites, IGeometryCollection)

                'Ajouter le polygone de généralisation
                mpGeometrieTravail.AddGeometry(pPolygonGen)

                'Ajouter la polyligne d'erreur de généralisation intérieures
                mpGeometrieTravail.AddGeometry(pPolylineErr)

                'Ajouter le squelette du polygone
                mpGeometrieTravail.AddGeometry(pSquelette)

                'Ajouter le polygone de généralisation
                mpGeometrieTravail.AddGeometry(pPointsConnexion)

                ''-----------------------------------------
                ''Enlever les droites dont les 2 extrémités ne touchent pas à la ligne à traiter
                ''du Bag des droites de la ligne-côté droit
                ''-----------------------------------------
                'Dim pDroite As IPolyline = Nothing
                'pTopoOp = CType(pPolygon, ITopologicalOperator2)
                ''Interface pour vérifier la connexion avec la ligne à traiter
                'Dim pRelOp As IRelationalOperator = CType(pTopoOp.Boundary, IRelationalOperator)
                ''Interface pour extraire les lignes du Bag
                'Dim pGeomCollD As IGeometryCollection = CType(pBagDroites, IGeometryCollection)
                ''Traiter toutes les droites du Bag
                'For j = pGeomCollD.GeometryCount - 1 To 0 Step -1
                '    'Définir une droite du Bag des droites
                '    pDroite = CType(pGeomCollD.Geometry(j), IPolyline)
                '    'Vérifier si une ou les 2 extrémités de la droite est disjoint de la ligne à traiter
                '    If pRelOp.Disjoint(pDroite.FromPoint) Or pRelOp.Disjoint(pDroite.ToPoint) Then
                '        'Enlever la droite du Bag
                '        pGeomCollD.RemoveGeometries(j, 1)
                '    End If
                'Next

                'Ajouter les lignes de Delaunay
                mpGeometrieTravail.AddGeometry(pBagDroites)
            End If

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryBag = Nothing
            pGeometry = Nothing
            pPointsConnexion = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Permet de créer la généralisation de ligne fractionnée pour les éléments sélectionnés de la vue active et les mettre en mémoire dans la géométrie de travail.
    '''</summary>
    '''
    '''<param name="dDistLat"> Distance latérale utilisée pour éliminer des sommets en trop.</param>
    '''<param name="dLargGenMin"> Largeur minimale de généralisation.</param>
    '''<param name="dLongGenMin"> Longueur minimale de généralisation.</param>
    '''<param name="dLongMin"> Longueur minimale d'une ligne.</param>
    ''' 
    Public Sub GeneraliserLigneFractionnee(ByVal dDistLat As Double, dLargGenMin As Double, ByVal dLongGenMin As Double, ByVal dLongMin As Double)
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                      'Interface ESRI contenant la Map active.
        Dim pEnumFeature As IEnumFeature = Nothing      'Interface ESRI utilisé pour extraire les éléments de la sélection.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant un élément en sélection.
        Dim pGeometryBag As IGeometryBag = Nothing      'Interface ESRI contenant les géométries à traiter.
        Dim pGeometry As IGeometry = Nothing            'Interface contenant une géométrie.
        Dim pPolyline As IPolyline = Nothing            'Interface contenant la super polyligne à traiter.
        Dim pPolylineGen As IPolyline = Nothing         'Interface contenant les lignes de généralisation.
        Dim pPolylineErr As IPolyline = Nothing         'Interface contenant les lignes de généralisation en erreur.
        Dim pSquelette As IPolyline = Nothing           'Interface contenant le squelette de la polyligne.
        Dim pSqueletteEnv As IPolyline = Nothing        'Interface contenant le squelette de la polyligne avec son enveloppe.
        Dim pBagDroites As IGeometryBag = Nothing       'Interface contenant les droites de Delaunay.
        Dim pBagDroitesEnv As IGeometryBag = Nothing    'Interface contenant les droites de Delaunay avec son enveloppe.
        Dim pGeomColl As IGeometryCollection = Nothing  'Interface pour ajouter les géométries à traiter.
        Dim pTopoOp As ITopologicalOperator2 = Nothing  'Interface pour fusionner les polygones à traiter.
        Dim pSpatialRefRes As ISpatialReferenceResolution = Nothing 'Interface qui permet d'initialiser la résolution XY.
        Dim pSpatialRefTol As ISpatialReferenceTolerance = Nothing  'Interface qui permet d'initialiser la tolérance XY.
        Dim pPointsConnexion As IMultipoint = Nothing   'Interface contenant les points de connexion.

        Try
            'Définir le document de ArcMap
            m_MxDocument = m_MxDocument

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vider la mémoire
            mpGeometrieTravail = Nothing
            'Récupérer la mémoire
            GC.Collect()

            'Initialiser la résolution
            pSpatialRefRes = CType(pMap.SpatialReference, ISpatialReferenceResolution)
            pSpatialRefRes.SetDefaultXYResolution()
            'Interface pour définir la tolérance XY
            pSpatialRefTol = CType(pMap.SpatialReference, ISpatialReferenceTolerance)
            pSpatialRefTol.XYTolerance = pSpatialRefRes.XYResolution(True) * 2
            pSpatialRefTol.XYTolerance = 0.001

            'Créer un nouveau GeometryBag vide pour les polygones à traiter
            pGeometryBag = New GeometryBag
            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les polygones à traiter
            pGeomColl = CType(pGeometryBag, IGeometryCollection)

            'Interface pour extraire le premier élément de la sélection
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Extraire le premier élément de la sélection
            pFeature = pEnumFeature.Next

            'Traitre tous les éléments de la sélection
            Do Until pFeature Is Nothing
                'Interface pour projeter la géométrie
                pGeometry = pFeature.Shape

                'Projeter la géométrie
                pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

                'Vérifier si la géométrie est une ligne
                If pFeature.Shape.GeometryType = esriGeometryType.esriGeometryPolyline Then
                    'Ajouter le polygone à traiter
                    pGeomColl.AddGeometry(pGeometry)
                End If

                'Extraire le prochain élément de la sélection
                pFeature = pEnumFeature.Next
            Loop

            'Créer une nouvelle polyligne vide
            pPolyline = New Polyline
            'Définir la référence spatiale
            pPolyline.SpatialReference = pMap.SpatialReference

            'Simplifier la SuperPolyligne
            pTopoOp = CType(pPolyline, ITopologicalOperator2)
            pTopoOp.ConstructUnion(CType(pGeomColl, IEnumGeometry))

            'Créer un nouveau GeometryBag vide
            pGeometryBag = New GeometryBag
            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les géométrie du diagramme de Voronoi
            mpGeometrieTravail = CType(pGeometryBag, IGeometryCollection)

            'Vérifier si le polyligne n'est pas vide
            If Not pPolyline.IsEmpty Then
                'Extraire les points d'intersection
                pPointsConnexion = CType(pTopoOp.Boundary, IMultipoint)

                'Filtre latérale
                'dDistLat = 7.5
                'pPolyline.Generalize(dDistLat)

                'Fractionner une polyligne
                Dim pLigneFractionnee As IPolyline = FractionnerPolyligne(pPolyline)

                ''Ajouter la polyligne de généralisation en erreur
                mpGeometrieTravail.AddGeometry(pLigneFractionnee)

                'Généraliser la polyligne
                Call clsGeneraliserGeometrie.GeneraliserLigne(pLigneFractionnee, pPointsConnexion, dDistLat, dLargGenMin, dLongGenMin, dLongMin,
                                                              pPolylineGen, pPolylineErr, pSquelette, pSqueletteEnv, pBagDroites, pBagDroitesEnv)

                'Ajouter la polyligne de généralisation en erreur
                mpGeometrieTravail.AddGeometry(pPolylineGen)

                'Ajouter la polyligne généralisé
                mpGeometrieTravail.AddGeometry(pPolylineErr)

                ''Ajouter le squelette de la polyligne
                'mpGeometrieTravail.AddGeometry(pSquelette)

                ''Ajouter le squelette de la polyligne avec son enveloppe
                'mpGeometrieTravail.AddGeometry(pSqueletteEnv)

                'Ajouter le Bag des droites de la polyligne
                mpGeometrieTravail.AddGeometry(pBagDroites)

                'Ajouter le Bag des droites de la polyligne avec son enveloppe
                'mpGeometrieTravail.AddGeometry(pBagDroitesEnv)

                'Ajouter les points de connexion de la polyligne
                'mpGeometrieTravail.AddGeometry(pPointsConnexion)

                'mpGeometrieTravail = CType(pBagDroites, IGeometryCollection)
            End If

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryBag = Nothing
            pGeometry = Nothing
            pPolyline = Nothing
            pPolylineGen = Nothing
            pPolylineErr = Nothing
            pSquelette = Nothing
            pSqueletteEnv = Nothing
            pBagDroites = Nothing
            pBagDroitesEnv = Nothing
            pGeomColl = Nothing
            pTopoOp = Nothing
            pSpatialRefRes = Nothing
            pSpatialRefTol = Nothing
            pPointsConnexion = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Fonction qui permet de fractionner une polyligne selon les angles aigus et obtus.
    ''' La polyligne résultante contiendra des lignes (Path) de sens inversé entre les changement d'angle aigu et obtu des droites consécutives.
    '''</summary>
    '''
    '''<param name="pPolyline"> Interface contenant la polyligne à fractionner.</param>
    ''' 
    ''' <returns>IPolyline contenant la polyligne fractionnée.</returns>
    ''' 
    Public Function FractionnerPolyligne(ByVal pPolyline As IPolyline) As IPolyline
        'Déclarer les variables de travail
        Dim pLigneFractionnee As IPolyline = Nothing        'Interface contenant la ligne fractionnée.
        Dim pPath As IPath = Nothing                        'Interface contenant une partie de la ligne fractionnée.
        Dim pGeomColl As IGeometryCollection = Nothing      'Interface pour extraire les lignes.
        Dim pGeomCollAdd As IGeometryCollection = Nothing   'Interface pour ajouter les lignes.
        Dim pSegColl As ISegmentCollection = Nothing        'Interface pour extraire les segments.
        Dim pSegCollAdd As ISegmentCollection = Nothing     'Interface pour ajouter les segments.
        Dim pSegment As ISegment = Nothing                  'Interface contenant un segment.
        Dim pDemiSegment As ISegment = Nothing              'Interface contenant un demi segment.
        Dim pPoint As Point = Nothing                       'Interface contenant un point.
        Dim dAngleDepart As Double = -1                     'Contient l'angle de départ.
        Dim dAngle As Double = -1                           'Contient l'angle de la droite traitée.
        Dim dDiffDepart As Double = -1                      'Contient la différence d'angle entre deux droites consécutives de départ.
        Dim dDiff As Double = -1                            'Contient la différence d'angle entre deux droites consécutives.

        'Par defaut, la ligne fractionnée est la même que la ligne à traiter
        FractionnerPolyligne = pPolyline

        Try
            'Sortir si la polyligne est vide
            If pPolyline.IsEmpty Then Exit Function

            'Créer une ligne fractionnée vide
            pLigneFractionnee = New Polyline
            pLigneFractionnee.SpatialReference = pPolyline.SpatialReference
            'Interface pour ajouter une ligne
            pGeomCollAdd = CType(pLigneFractionnee, IGeometryCollection)

            'Interface pour extraire les composantes
            pGeomColl = CType(pPolyline, IGeometryCollection)
            'Traiter toutes les composantes
            For i = 0 To pGeomColl.GeometryCount - 1
                'Initialiser les angles
                dAngleDepart = -1
                dAngle = -1

                'Interface pour extraire les segments
                pSegColl = CType(pGeomColl.Geometry(i), ISegmentCollection)
                'Traiter tous les segments de la composante
                For j = 1 To pSegColl.SegmentCount - 1
                    'Définir le segment à traiter
                    pSegment = pSegColl.Segment(j)

                    'Vérifier si l'angle de départ n'est pas initialisée
                    If dAngleDepart = -1 Then
                        'Initialiser l'angle de départ
                        dAngleDepart = clsGeneraliserGeometrie.Angle(pSegColl.Segment(j - 1).FromPoint, pSegColl.Segment(j - 1).ToPoint)
                        'Initialiser l'angle selon l'angle de départ
                        dAngle = clsGeneraliserGeometrie.Angle(pSegColl.Segment(j).FromPoint, pSegColl.Segment(j).ToPoint)

                        'Définir la différence d'angle
                        dDiff = dAngleDepart - dAngle
                        'Définir l'angle entre les deux droites consécutives
                        If dDiff > -180 And dDiff < 180 Then
                            dDiff = 180 - dDiff
                        ElseIf dDiff < -180 Then
                            dDiff = Math.Abs(180 + dDiff)
                        ElseIf dDiff > 180 Then
                            dDiff = 540 - dDiff
                        End If
                        'Debug.Print(dAngleDepart.ToString + "-" + dAngle.ToString + "=" + dDiff.ToString + ">" + dDiff.ToString)

                        'Créer une partie de ligne fractionnée vide
                        pPath = New Path
                        pPath.SpatialReference = pPolyline.SpatialReference

                        'Interface pour ajouter un segment
                        pSegCollAdd = CType(pPath, ISegmentCollection)
                        'Ajouter un segment
                        pSegCollAdd.AddSegment(pSegColl.Segment(j - 1))

                        'Si l'angle de départ est initialisée
                    Else
                        'Initialiser l'angle selon l'angle de départ
                        dAngle = clsGeneraliserGeometrie.Angle(pSegColl.Segment(j).FromPoint, pSegColl.Segment(j).ToPoint)

                        'Définir la différence d'angle
                        dDiff = dAngleDepart - dAngle
                        'Définir l'angle entre les deux droites consécutives
                        If dDiff > -180 And dDiff < 180 Then
                            dDiff = 180 - dDiff
                        ElseIf dDiff < -180 Then
                            dDiff = Math.Abs(180 + dDiff)
                        ElseIf dDiff > 180 Then
                            dDiff = 540 - dDiff
                        End If
                        'Debug.Print(dAngleDepart.ToString + "-" + dAngle.ToString + "=" + dDiff.ToString + ">" + dDiff.ToString)

                        'Vérifier si l'état (aigu ou obtu) de la différence d'angle est différent
                        If (dDiff < 180 And dDiffDepart > 180) Or (dDiff > 180 And dDiffDepart < 180) Then
                            'Définir le demi segment vide
                            pDemiSegment = New Line
                            pDemiSegment.SpatialReference = pPolyline.SpatialReference
                            'Définir le point vide
                            pPoint = New Point
                            pPoint.SpatialReference = pPolyline.SpatialReference
                            'Calculer la position du centre du segment
                            pPoint.X = (pSegColl.Segment(j - 1).FromPoint.X + pSegColl.Segment(j - 1).ToPoint.X) / 2
                            pPoint.Y = (pSegColl.Segment(j - 1).FromPoint.Y + pSegColl.Segment(j - 1).ToPoint.Y) / 2
                            'Définir le premier point
                            pDemiSegment.FromPoint = pSegColl.Segment(j - 1).FromPoint
                            'Définir le deuxième point
                            pDemiSegment.ToPoint = pPoint
                            'Ajouter un demi segment
                            pSegCollAdd.AddSegment(pDemiSegment)
                            'Changer l'orientation si l'angle est obtu
                            If dDiffDepart > 180 Then pPath.ReverseOrientation()
                            'Ajouter une ligne fractionnée
                            pGeomCollAdd.AddGeometry(pPath)

                            'Créer une partie de ligne fractionnée vide
                            pPath = New Path
                            pPath.SpatialReference = pPolyline.SpatialReference
                            'Définir le demi segment vide
                            pDemiSegment = New Line
                            pDemiSegment.SpatialReference = pPolyline.SpatialReference
                            'Définir le point vide
                            pPoint = New Point
                            pPoint.SpatialReference = pPolyline.SpatialReference
                            'Calculer la position du centre du segment
                            pPoint.X = (pSegColl.Segment(j - 1).FromPoint.X + pSegColl.Segment(j - 1).ToPoint.X) / 2
                            pPoint.Y = (pSegColl.Segment(j - 1).FromPoint.Y + pSegColl.Segment(j - 1).ToPoint.Y) / 2
                            'Interface pour ajouter un segment
                            pSegCollAdd = CType(pPath, ISegmentCollection)
                            'Définir le premier point
                            pDemiSegment.FromPoint = pPoint
                            'Définir le premier point
                            pDemiSegment.ToPoint = pSegColl.Segment(j - 1).ToPoint
                            'Ajouter un demi segment
                            pSegCollAdd.AddSegment(pDemiSegment)

                            'Si l'état (aigu ou obtu) de la différence d'angle est le même
                        Else
                            'Ajouter un segment
                            pSegCollAdd.AddSegment(pSegColl.Segment(j - 1))
                        End If
                    End If

                    'Initialiser l'angle de départ
                    dAngleDepart = dAngle
                    'Initialiser l'angle de différence de départ
                    dDiffDepart = dDiff
                Next
            Next

            'Ajouter un segment
            pSegCollAdd.AddSegment(pSegment)
            'Changer l'orientation si l'angle est obtu
            If dDiffDepart > 180 Then pPath.ReverseOrientation()
            'Ajouter une ligne fractionnée
            pGeomCollAdd.AddGeometry(pPath)

            'Retourner la polyligne fractionnée
            FractionnerPolyligne = pLigneFractionnee

        Catch ex As Exception
            Throw
        Finally
            'Vider la mémoire
            pLigneFractionnee = Nothing
            pPath = Nothing
            pGeomColl = Nothing
            pGeomCollAdd = Nothing
            pSegColl = Nothing
            pSegCollAdd = Nothing
        End Try
    End Function

    '''<summary>
    ''' Permet de créer la généralisation de ligne pour les éléments sélectionnés de la vue active et les mettre en mémoire dans la géométrie de travail.
    '''</summary>
    '''
    '''<param name="dDistLat"> Distance latérale utilisée pour éliminer des sommets en trop.</param>
    '''<param name="dLargGenMin"> Largeur minimale de généralisation.</param>
    '''<param name="dLongGenMin"> Longueur minimale de généralisation.</param>
    '''<param name="dLongMin"> Longueur minimale d'une ligne.</param>
    '''<param name="iMethode"> Méthode de généralisation (0Droite/1:Gauche).</param>
    ''' 
    Public Sub GeneraliserLigne(ByVal dDistLat As Double, dLargGenMin As Double, ByVal dLongGenMin As Double, ByVal dLongMin As Double, ByVal iMethode As Integer)
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                      'Interface ESRI contenant la Map active.
        Dim pEnumFeature As IEnumFeature = Nothing      'Interface ESRI utilisé pour extraire les éléments de la sélection.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant un élément en sélection.
        Dim pGeometryBag As IGeometryBag = Nothing      'Interface ESRI contenant les géométries à traiter.
        Dim pGeometry As IGeometry = Nothing            'Interface contenant une géométrie.
        Dim pPolyline As IPolyline = Nothing            'Interface contenant la super polyligne à traiter.
        Dim pPolylineGen As IPolyline = Nothing         'Interface contenant les lignes de généralisation.
        Dim pPolylineErr As IPolyline = Nothing         'Interface contenant les lignes de généralisation en erreur.
        Dim pSquelette As IPolyline = Nothing           'Interface contenant le squelette de la polyligne.
        Dim pSqueletteEnv As IPolyline = Nothing        'Interface contenant le squelette de la polyligne avec son enveloppe.
        Dim pBagDroites As IGeometryBag = Nothing       'Interface contenant les droites de Delaunay.
        Dim pBagDroitesEnv As IGeometryBag = Nothing    'Interface contenant les droites de Delaunay avec son enveloppe.
        Dim pGeomColl As IGeometryCollection = Nothing  'Interface pour ajouter les géométries à traiter.
        Dim pTopoOp As ITopologicalOperator2 = Nothing  'Interface pour fusionner les polygones à traiter.
        Dim pSpatialRefRes As ISpatialReferenceResolution = Nothing 'Interface qui permet d'initialiser la résolution XY.
        Dim pSpatialRefTol As ISpatialReferenceTolerance = Nothing  'Interface qui permet d'initialiser la tolérance XY.
        Dim pPointsConnexion As IMultipoint = Nothing   'Interface contenant les points de connexion.

        Try
            'Définir le document de ArcMap
            m_MxDocument = m_MxDocument

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vider la mémoire
            mpGeometrieTravail = Nothing
            'Récupérer la mémoire
            GC.Collect()

            'Initialiser la résolution
            pSpatialRefRes = CType(pMap.SpatialReference, ISpatialReferenceResolution)
            pSpatialRefRes.SetDefaultXYResolution()
            'Interface pour définir la tolérance XY
            pSpatialRefTol = CType(pMap.SpatialReference, ISpatialReferenceTolerance)
            pSpatialRefTol.XYTolerance = pSpatialRefRes.XYResolution(True) * 2
            pSpatialRefTol.XYTolerance = 0.001

            'Créer un nouveau GeometryBag vide pour les polygones à traiter
            pGeometryBag = New GeometryBag
            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les polygones à traiter
            pGeomColl = CType(pGeometryBag, IGeometryCollection)

            'Interface pour extraire le premier élément de la sélection
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Extraire le premier élément de la sélection
            pFeature = pEnumFeature.Next

            'Traitre tous les éléments de la sélection
            Do Until pFeature Is Nothing
                'Interface pour projeter la géométrie
                pGeometry = pFeature.Shape

                'Projeter la géométrie
                pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

                'Vérifier si la géométrie est une ligne
                If pFeature.Shape.GeometryType = esriGeometryType.esriGeometryPolyline Then
                    'Ajouter le polygone à traiter
                    pGeomColl.AddGeometry(pGeometry)
                End If

                'Extraire le prochain élément de la sélection
                pFeature = pEnumFeature.Next
            Loop

            'Créer une nouvelle polyligne vide
            pPolyline = New Polyline
            'Définir la référence spatiale
            pPolyline.SpatialReference = pMap.SpatialReference

            'Simplifier la SuperPolyligne
            pTopoOp = CType(pPolyline, ITopologicalOperator2)
            pTopoOp.ConstructUnion(CType(pGeomColl, IEnumGeometry))

            'Créer un nouveau GeometryBag vide
            pGeometryBag = New GeometryBag
            'Définir la référence spatiale
            pGeometryBag.SpatialReference = pMap.SpatialReference
            'Interface pour ajouter les géométrie du diagramme de Voronoi
            mpGeometrieTravail = CType(pGeometryBag, IGeometryCollection)

            'Vérifier si le polyligne n'est pas vide
            If Not pPolyline.IsEmpty Then
                'Vérifier si la méthode est 1:Gauche ou 3:Gauche-Droite
                If iMethode = 1 Or iMethode = 3 Then
                    'Renverser l'odre des sommets
                    pPolyline.ReverseOrientation()
                End If

                'Extraire les points d'intersection
                pPointsConnexion = CType(pTopoOp.Boundary, IMultipoint)

                'Filtrer la géométrie
                pPolyline.Generalize(dDistLat)
                'Généraliser la polyligne
                Call clsGeneraliserGeometrie.GeneraliserPolyligne(pPolyline, pPointsConnexion, dDistLat, dLargGenMin, dLongGenMin, dLongMin,
                                                                  pPolylineGen, pPolylineErr, pSquelette, pSqueletteEnv, pBagDroites, pBagDroitesEnv)

                'Vérifier si la méthode est 2:Droite/Gauche ou 3:Gauche-Droite
                If iMethode = 2 Or iMethode = 3 Then
                    'Redéfinir la polyligne
                    pPolyline = pPolylineGen
                    'Interface pour ajouter des géométries
                    pGeomColl = CType(pPolylineErr, IGeometryCollection)
                    'Renverser l'odre des sommets
                    pPolyline.ReverseOrientation()
                    'Filtre la géométrie
                    pPolyline.Generalize(dDistLat)
                    'Généraliser la polyligne
                    Call clsGeneraliserGeometrie.GeneraliserPolyligne(pPolyline, pPointsConnexion, dDistLat, dLargGenMin, dLongGenMin, dLongMin,
                                                                      pPolylineGen, pPolylineErr, pSquelette, pSqueletteEnv, pBagDroites, pBagDroitesEnv)
                    'Ajouter les lignes en erreur
                    pGeomColl.AddGeometryCollection(CType(pPolylineErr, IGeometryCollection))
                    'Redéfinir les géométries en erreur
                    pPolylineErr = CType(pGeomColl, IPolyline)
                End If

                'Ajouter la polyligne de généralisation en erreur
                mpGeometrieTravail.AddGeometry(pPolylineGen)

                'Ajouter la polyligne généralisé
                mpGeometrieTravail.AddGeometry(pPolylineErr)

                'Vérifier si la méthode est 2:Droite/Gauche ou 3:Gauche-Droite
                If iMethode = 0 Or iMethode = 1 Then
                    'Ajouter le squelette de la polyligne
                    mpGeometrieTravail.AddGeometry(pSquelette)

                    'Ajouter le squelette de la polyligne avec son enveloppe
                    mpGeometrieTravail.AddGeometry(pSqueletteEnv)

                    'Ajouter le Bag des droites de la polyligne
                    mpGeometrieTravail.AddGeometry(pBagDroites)

                    'Ajouter le Bag des droites de la polyligne avec son enveloppe
                    mpGeometrieTravail.AddGeometry(pBagDroitesEnv)

                    'Ajouter les points de connexion de la polyligne
                    mpGeometrieTravail.AddGeometry(pPointsConnexion)
                End If

                'mpGeometrieTravail = CType(pBagDroites, IGeometryCollection)
            End If

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryBag = Nothing
            pGeometry = Nothing
            pPolyline = Nothing
            pPolylineGen = Nothing
            pPolylineErr = Nothing
            pSquelette = Nothing
            pSqueletteEnv = Nothing
            pBagDroites = Nothing
            pBagDroitesEnv = Nothing
            pGeomColl = Nothing
            pTopoOp = Nothing
            pSpatialRefRes = Nothing
            pSpatialRefTol = Nothing
            pPointsConnexion = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Fonction qui permet de créer une géométrie point et l'ajouter aux géométries de travail déja
    ''' présentes. On peut détruire les géométries présentes, créer une nouvelle géométrie et ajouter la
    ''' géométrie à celles déja existantes. 
    '''</summary>
    '''
    '''<param name=" Shift "> 3 possibilité: 0:Nouvelle géométrie, 1 ou 2 :Ajouter aux existantes </param>
    '''
    '''<returns> 'La fonction va retourner un "Boolean". Si le traitement n'a pas réussi le "Boolean" est à "False".</returns>
    ''' 
    Public Function bAjouterNouveauPointTravail(ByVal Shift As Long) As Boolean
        'Déclarer les variables de travail
        Dim pPoint As IPoint = Nothing                  'Interface ESRI contenant un point.
        Dim pMxApplication As IMxApplication = Nothing  'Interface ESRI contenant le document ArcMap.
        Dim pSnapEnv As ISnapEnvironment = Nothing      'Interface ESRI contenant le Snapping.

        Try
            'Interface pour accéder à l'affichage de l'application
            pMxApplication = CType(m_Application, IMxApplication)

            'Interface contenant le Snapping
            pSnapEnv = CType(m_Application.FindExtensionByName("ESRI Object Editor"), ISnapEnvironment)

            'Vérifier si les géométries de travail sont initialisées
            If mpGeometrieTravail Is Nothing Then
                'Initialiser les géométries de travail
                mpGeometrieTravail = CType(New GeometryBag, IGeometryCollection)
            End If

            'Vérifier si on doit créer une nouvelle géométrie de travail
            If Shift = 0 Then
                'Créer une nouvelle géométrie de travail
                mpGeometrieTravail = CType(New GeometryBag, IGeometryCollection)
            End If

            'Définir le nouveau point
            pPoint = CType(mpRubberBand.TrackNew(pMxApplication.Display, Nothing), IPoint)

            'Changer le point selon le snapping
            pSnapEnv.SnapPoint(pPoint)

            'Vérifier si le point est présent
            If Not pPoint Is Nothing Then
                'Ajouter le nouveau point aux géométries de travail
                mpGeometrieTravail.AddGeometry(pPoint)
            End If

            'Retourner le résultat
            bAjouterNouveauPointTravail = True

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pPoint = Nothing
            pMxApplication = Nothing
            pSnapEnv = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de créer une géométrie ligne et l'ajouter aux géométries de travail déja
    ''' présentes. On peut détruire les géométries présentes, créer une nouvelle géométrie et ajouter la
    ''' géométrie à celles déja existantes. 
    '''</summary>
    '''
    '''<param name=" Shift "> 3 possibilité: 0:Nouvelle géométrie, 1:Complexe (shift), 2:Multiple (ctrl) </param>
    '''
    '''<returns> 'La fonction va retourner un "Boolean". Si le traitement n'a pas réussi le "Boolean" est à "False".</returns>
    ''' 
    Public Function bAjouterNouvelleLigneTravail(ByVal Shift As Long) As Boolean
        'Déclarer les variables de travail
        Dim pPolyline As IPolyline = Nothing              'Interface ESRI contenant une ligne.
        Dim pGeometry As IGeometry = Nothing              'Interface ESRI contenant une géométrie.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI contenant des nouvelles composantes.
        Dim pNewGeomColl As IGeometryCollection = Nothing 'Interface ESRI contenant des nouvelles composantes.
        Dim pNewGeomBag As IGeometryCollection = Nothing  'Interface ESRI contenant des géométries de types différents.
        Dim pTopoOp As ITopologicalOperator2 = Nothing    'Interface ESRI pour traiter la topologie des géométries.
        Dim pMxApplication As IMxApplication = Nothing    'Interface ESRI contenant le document ArcMap.

        Try
            'Interface pour accéder à l'affichage de l'application
            pMxApplication = CType(m_Application, IMxApplication)

            'Vérifier si les géométries de travail sont initialisées
            If mpGeometrieTravail Is Nothing Then
                'Initialiser les géométries de travail
                mpGeometrieTravail = CType(New GeometryBag, IGeometryCollection)
            End If

            'Vérifier si on doit créer une nouvelle géométrie de travail
            If Shift = 0 Then
                'Créer une nouvelle géométrie de travail
                mpGeometrieTravail = CType(New GeometryBag, IGeometryCollection)
            End If

            'Définir la nouvelle ligne
            pPolyline = CType(mpRubberBand.TrackNew(pMxApplication.Display, Nothing), IPolyline)

            'Vérifier si la ligne est valide
            If pPolyline IsNot Nothing Then
                'Interface pour simplifier la géométrie
                pTopoOp = CType(pPolyline, ITopologicalOperator2)
                pTopoOp.IsKnownSimple_2 = False
                pTopoOp.Simplify()

                'Ajouter la nouvelle ligne aux géométries de travail
                mpGeometrieTravail.AddGeometry(pPolyline)

                'Créer un nouveau GeometryBag de travail
                pNewGeomBag = CType(New GeometryBag, IGeometryCollection)

                'Créer une nouvelle géométrie de type ligne
                pNewGeomColl = CType(New Polyline, IGeometryCollection)

                'Traiter chaque géométrie des géométries de travail
                For i = 0 To mpGeometrieTravail.GeometryCount - 1
                    'Extraire une géométrie de travail
                    pGeometry = mpGeometrieTravail.Geometry(i)

                    'Vérifier si la géométrie est un ring à ajouter
                    If pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then
                        'Vérifier si on ajoute un path
                        If Shift = 1 Then
                            'Interface pour extraire tous les path de la ligne
                            pGeomColl = CType(pGeometry, IGeometryCollection)

                            'Traiter chaque Ring du polygone
                            For j = 0 To pGeomColl.GeometryCount - 1
                                'Ajouter tous les Rings dans un nouveau polygone
                                pNewGeomColl.AddGeometry(pGeomColl.Geometry(j))

                                'Valider la nouvelle surface
                                pTopoOp = CType(pNewGeomColl, ITopologicalOperator2)
                                pTopoOp.Simplify()
                            Next j

                            'Vérifier si on ajoute une ligne
                        Else
                            pNewGeomBag.AddGeometry(pGeometry)
                        End If

                        'Ajouter un point, une ligne ou une surface dans les géométries de travail
                    Else
                        pNewGeomBag.AddGeometry(pGeometry)
                    End If
                Next i

                If Shift = 1 Then
                    'Ajouter la nouvelle surface
                    pNewGeomBag.AddGeometry(CType(pNewGeomColl, IGeometry))
                End If

                'Ajouter la nouvelle surface aux géométries de travail
                mpGeometrieTravail = pNewGeomBag
            End If

            'Retourner le résultat
            bAjouterNouvelleLigneTravail = True

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pPolyline = Nothing
            pTopoOp = Nothing
            pGeometry = Nothing
            pGeomColl = Nothing
            pNewGeomColl = Nothing
            pNewGeomBag = Nothing
            pTopoOp = Nothing
            pMxApplication = Nothing
        End Try
    End Function

    '''<summary>
    ''' Routine qui permet d'ajouter un nouveau Multipoint aux géométries de travail existantes ou non.
    ''' 
    ''' Si le paramètre Optimiser est Vrai, toutes les géométries de type Multipoint contenue dans les géométries de travail 
    ''' seront fusionnée en une seul Multipoint.
    '''</summary>
    '''
    '''<param name=" pMultipoint "> Interface ESRI contenant le nouveau Multipoint à traiter.</param>
    '''<param name=" bOptimiser "> Indiquer si on doit optimiser toutes les géométries de travail de type point. </param>
    '''<param name=" bSimplifier "> Indiquer si on doit simplifier toutes les géométries de travail de type ligne. </param>
    '''
    Public Sub AjouterNouveauPointTravail(ByVal pMultipoint As IMultipoint, ByVal bOptimiser As Boolean, ByVal bSimplifier As Boolean)
        'Déclarer les variables de travail
        Dim pNewMultipoint As IMultipoint = Nothing         'Interface ESRI contenant une point multiple.
        Dim pGeometry As IGeometry = Nothing                'Interface ESRI contenant une géométrie.
        Dim pGeomColl As IGeometryCollection = Nothing      'Interface ESRI contenant des nouvelles composantes.
        Dim pNewGeomColl As IGeometryCollection = Nothing   'Interface ESRI contenant des nouvelles composantes.
        Dim pNewGeomBag As IGeometryCollection = Nothing    'Interface ESRI contenant des géométries de types différents.
        Dim pTopoOp As ITopologicalOperator2 = Nothing      'Interface ESRI pour traiter la topologie des géométries.

        Try
            'Vérifier si le multipoint est valide
            If pMultipoint IsNot Nothing Then
                'Vérifier si on doit simplifier
                If bSimplifier Then
                    'Interface pour simplifier la géométrie
                    pTopoOp = CType(pMultipoint, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()
                End If

                'Si on doit optimiser
                If bOptimiser Then
                    'Ajouter le nouveau multipoint aux géométries de travail
                    mpGeometrieTravail.AddGeometry(pMultipoint)

                    'Créer un nouveau GeometryBag de travail
                    pNewGeomBag = CType(New GeometryBag, IGeometryCollection)

                    'Créer une nouvelle géométrie de type multipoint
                    pNewMultipoint = CType(New Multipoint, IMultipoint)

                    'Interface pour ajouter des nouvelles composantes
                    pNewGeomColl = CType(pNewMultipoint, IGeometryCollection)

                    'Traiter chaque géométrie des géométries de travail
                    For i = 0 To mpGeometrieTravail.GeometryCount - 1
                        'Extraire une géométrie de travail
                        pGeometry = mpGeometrieTravail.Geometry(i)

                        'Vérifier si la géométrie est un multipoint
                        If pGeometry.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                            'Interface pour extraire tous les path de la ligne
                            pGeomColl = CType(pGeometry, IGeometryCollection)

                            'Traiter chaque Point du multipoint
                            For j = 0 To pGeomColl.GeometryCount - 1
                                'Ajouter tous les points dans le multipoint
                                pNewGeomColl.AddGeometry(pGeomColl.Geometry(j))
                            Next j

                            'Vérifier si on doit simplifier
                            If bSimplifier Then
                                'Valider le nouveau Multipoint
                                pTopoOp = CType(pNewGeomColl, ITopologicalOperator2)
                                pTopoOp.IsKnownSimple_2 = False
                                pTopoOp.Simplify()
                            End If

                            'Vérifier si la géométrie est un point
                        ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                            'Ajouter tous les points dans le multipoint
                            pNewGeomColl.AddGeometry(pGeometry)

                            'Ajouter une ligne ou une surface dans les géométries de travail
                        Else
                            pNewGeomBag.AddGeometry(pGeometry)
                        End If
                    Next i

                    'Ajouter le nouveau Multipoint
                    pNewGeomBag.AddGeometry(CType(pNewGeomColl, IGeometry))

                    'Redéfinir les géométries de travail
                    mpGeometrieTravail = pNewGeomBag

                    'Si on optimise pas
                Else
                    'Interface pour extraire chaque pooint
                    pGeomColl = CType(pMultipoint, IGeometryCollection)

                    'Traiter tous les points du Multipoint
                    For i = 0 To pGeomColl.GeometryCount - 1
                        'Ajouter le point
                        mpGeometrieTravail.AddGeometry(pGeomColl.Geometry(i))
                    Next
                End If
            End If

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pNewMultipoint = Nothing
            pTopoOp = Nothing
            pGeometry = Nothing
            pGeomColl = Nothing
            pNewGeomColl = Nothing
            pNewGeomBag = Nothing
            pTopoOp = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet d'ajouter une nouvelle ligne aux géométries de travail existantes ou non.
    ''' 
    ''' Si le paramètre Optimiser est Vrai, toutes les géométries de type ligne contenue dans les géométries de travail 
    ''' seront fusionnée en une seule.
    '''</summary>
    '''
    '''<param name=" pPolyline "> Interface ESRI contenant la nouvelle ligne à traiter.</param>
    '''<param name=" bOptimiser "> Indiquer si on doit optimiser toutes les géométries de travail de type ligne. </param>
    '''<param name=" bSimplifier "> Indiquer si on doit simplifier toutes les géométries de travail de type ligne. </param>
    ''' 
    Public Sub AjouterNouvelleLigneTravail(ByVal pPolyline As IPolyline, ByVal bOptimiser As Boolean, ByVal bSimplifier As Boolean)
        'Déclarer les variables de travail
        Dim pNewPolyline As IPolyline = Nothing             'Interface ESRI contenant une ligne.
        Dim pGeometry As IGeometry = Nothing                'Interface ESRI contenant une géométrie.
        Dim pGeomColl As IGeometryCollection = Nothing      'Interface ESRI contenant des nouvelles composantes.
        Dim pNewGeomColl As IGeometryCollection = Nothing   'Interface ESRI contenant des nouvelles composantes.
        Dim pNewGeomBag As IGeometryCollection = Nothing    'Interface ESRI contenant des géométries de types différents.
        Dim pTopoOp As ITopologicalOperator2 = Nothing      'Interface ESRI pour traiter la topologie des géométries.

        Try
            'Vérifier si la ligne est présente
            If pPolyline IsNot Nothing Then
                'Vérifier si on doit simplifier
                If bSimplifier Then
                    'Interface pour simplifier la géométrie
                    pTopoOp = CType(pPolyline, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()
                End If

                'Ajouter la nouvelle ligne aux géométries de travail
                mpGeometrieTravail.AddGeometry(pPolyline)

                'Si on doit optimiser
                If bOptimiser Then
                    'Créer un nouveau GeometryBag de travail
                    pNewGeomBag = CType(New GeometryBag, IGeometryCollection)

                    'Créer une nouvelle géométrie de type ligne
                    pNewPolyline = CType(New Polyline, IPolyline)

                    'Interface pour ajouter des nouvelles composantes
                    pNewGeomColl = CType(pNewPolyline, IGeometryCollection)

                    'Traiter chaque géométrie des géométries de travail
                    For i = 0 To mpGeometrieTravail.GeometryCount - 1
                        'Extraire une géométrie de travail
                        pGeometry = mpGeometrieTravail.Geometry(i)

                        'Vérifier si la géométrie est une ligne
                        If pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then
                            'Interface pour extraire tous les path de la ligne
                            pGeomColl = CType(pGeometry, IGeometryCollection)

                            'Traiter chaque Path de la ligne
                            For j = 0 To pGeomColl.GeometryCount - 1
                                'Ajouter tous les Path dans la nouvelle ligne
                                pNewGeomColl.AddGeometry(pGeomColl.Geometry(j))
                            Next j

                            'Vérifier si on doit simplifier
                            If bSimplifier Then
                                'Valider la nouvelle ligne
                                pTopoOp = CType(pNewGeomColl, ITopologicalOperator2)
                                pTopoOp.IsKnownSimple_2 = False
                                pTopoOp.Simplify()
                            End If

                            'Ajouter un point, un multipoint ou une surface dans les géométries de travail
                        Else
                            pNewGeomBag.AddGeometry(pGeometry)
                        End If
                    Next i

                    'Ajouter la nouvelle ligne
                    pNewGeomBag.AddGeometry(CType(pNewGeomColl, IGeometry))

                    'Redéfinir les géométries de travail
                    mpGeometrieTravail = pNewGeomBag
                End If
            End If

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pPolyline = Nothing
            pTopoOp = Nothing
            pGeometry = Nothing
            pGeomColl = Nothing
            pNewGeomColl = Nothing
            pNewGeomBag = Nothing
            pTopoOp = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet d'ajouter une nouvelle surface aux géométries de travail existantes ou non.
    ''' 
    ''' Si le paramètre Optimiser est Vrai, toutes les géométries de type surface contenue dans les géométries de travail 
    ''' seront fusionnée en une seule ce qui permettra de créer des trous dans les surfaces lorsque nécessaire.
    '''</summary>
    '''
    '''<param name=" pPolygon "> Interface ESRI contenant la nouvelle surface à traiter.</param>
    '''<param name=" bOptimiser "> Indiquer si on doit optimiser toutes les géométries de travail de type surface. </param>
    '''<param name=" bSimplifier "> Indiquer si on doit simplifier toutes les géométries de travail de type ligne. </param>
    ''' 
    Public Sub AjouterNouvelleSurfaceTravail(ByVal pPolygon As IPolygon, ByVal bOptimiser As Boolean, ByVal bSimplifier As Boolean)
        'Déclarer les variables de travail
        Dim pNewPolygon As IPolygon = Nothing             'Interface ESRI contenant une nouvelle surface.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI contenant les composantes d'une géométrie.
        Dim pGeometry As IGeometry = Nothing              'Interface ESRI contenant une géométrie.
        Dim pNewGeomColl As IGeometryCollection = Nothing 'Interface ESRI contenant des nouvelles composantes.
        Dim pNewGeomBag As IGeometryCollection = Nothing  'Interface ESRI contenant des géométries de types différents.
        Dim pTopoOp As ITopologicalOperator2 = Nothing    'Interface ESRI pour traiter la topologie des géométries.

        Try
            'Vérifier si la surface est valide
            If pPolygon IsNot Nothing Then
                'Vérifier si on doit simplifier
                If bSimplifier Then
                    'Valider la topologie du polygone
                    pTopoOp = CType(pPolygon, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()
                End If

                'Ajouter le nouveau polygon dans les géométries de travail
                mpGeometrieTravail.AddGeometry(pPolygon)

                'Si on doit optimiser
                If bOptimiser Then
                    'Créer un nouveau GeometryBag de travail
                    pNewGeomBag = CType(New GeometryBag, IGeometryCollection)

                    'Créer une nouvelle surface
                    pNewPolygon = CType(New Polygon, IPolygon)

                    'Interface pour ajouter des nouvelles composantes
                    pNewGeomColl = CType(pNewPolygon, IGeometryCollection)

                    'Parcourir chaque géométrie des géométries de travail
                    For i = 0 To mpGeometrieTravail.GeometryCount - 1
                        'Extraire une géométrie de travail
                        pGeometry = mpGeometrieTravail.Geometry(i)

                        'Vérifier si la géométrie est un ring à ajouter
                        If pGeometry.GeometryType = esriGeometryType.esriGeometryPolygon Then
                            'Interface pour extraire tous les rings du polygone
                            pGeomColl = CType(pGeometry, IGeometryCollection)

                            'Traiter chaque Ring du polygone
                            For j = 0 To pGeomColl.GeometryCount - 1
                                'Ajouter tous les Rings dans un nouveau polygone
                                pNewGeomColl.AddGeometry(pGeomColl.Geometry(j))
                            Next j

                            'Vérifier si on doit simplifier
                            If bSimplifier Then
                                'Valider la nouvelle surface
                                pTopoOp = CType(pNewGeomColl, ITopologicalOperator2)
                                pTopoOp.IsKnownSimple_2 = False
                                pTopoOp.Simplify()
                            End If

                            'Ajouter un point, un multipoint ou une ligne dans les géométries de travail
                        Else
                            pNewGeomBag.AddGeometry(pGeometry)
                        End If
                    Next i

                    'Ajouter la nouvelle surface
                    pNewGeomBag.AddGeometry(CType(pNewGeomColl, IGeometry))

                    'Redéfinir les géométries de travail
                    mpGeometrieTravail = pNewGeomBag
                End If
            End If

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pNewPolygon = Nothing
            pGeometry = Nothing
            pGeomColl = Nothing
            pNewGeomColl = Nothing
            pNewGeomBag = Nothing
            pTopoOp = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Fonction qui permet de demander la valeur du buffer et de remplacer les géométries de travail par
    ''' le buffer de ces derniers.
    '''</summary>
    '''
    '''<param name=" pGeometryBag "> Interface ESRI contenant les géométrie de travail</param>
    '''<param name=" dTolerance "> Tolérance utilisée pour créer le tampon sur les géométries de trevail.</param>
    '''
    '''<returns> 'La fonction va retourner un "IGeometryBag". Si le traitement n'a pas réussi le "IGeometryBag" est à "Nothing".</returns>
    ''' 
    Public Function BufferGeometrie(ByVal pGeometryBag As IGeometryBag, ByVal dTolerance As Double) As IGeometryBag
        'Déclarer les variables de travail
        Dim pGeomColl As IGeometryCollection = Nothing
        Dim pNewGeomColl As IGeometryCollection = Nothing
        Dim pTmpGeomColl As IGeometryCollection = Nothing
        Dim pGeometry As IGeometry = Nothing
        Dim pNewGeometry As IGeometry = Nothing
        Dim pTopoOp As ITopologicalOperator = Nothing

        'Définir la valeur de retour par défaut
        BufferGeometrie = Nothing

        Try
            'Créer le niuveau GeometryBag de buffer
            pNewGeomColl = CType(New GeometryBag, IGeometryCollection)

            'Interface pour accéder cahque élément du GeometryBag
            pGeomColl = CType(pGeometryBag, IGeometryCollection)

            'Traiter chaque élément du GeometryBag
            For i = 0 To pGeomColl.GeometryCount - 1
                'Interface pour accéder à une géométrie du GeometryBag
                pGeometry = pGeomColl.Geometry(i)

                'Vérifier si l'élément est un GeometryBag
                If pGeometry.GeometryType = esriGeometryType.esriGeometryBag Then
                    'Créer une zone tampon pour chaque élément du GeometryBag
                    pTmpGeomColl = CType(BufferGeometrie(CType(pGeometry, IGeometryBag), dTolerance), IGeometryCollection)

                    'Remettre les éléments du GeometryBag de temporaire dans le nouveau
                    For j = 0 To pTmpGeomColl.GeometryCount - 1
                        'Ajouter le polygone dans le GeometryBag
                        pNewGeomColl.AddGeometry(pTmpGeomColl.Geometry(j))
                    Next j

                    'Si l'élément n'est pas un GeometryBag
                Else
                    'Interface pour créer le buffer
                    pTopoOp = CType(pGeometry, ITopologicalOperator)

                    'Créer une zone tampon à partir des éléments de travail
                    pNewGeometry = pTopoOp.Buffer(dTolerance)

                    'Ajouter le polygone dans le GeometryBag
                    pNewGeomColl.AddGeometry(pNewGeometry)
                End If
            Next i

            'Retourner le résultat
            BufferGeometrie = CType(pNewGeomColl, IGeometryBag)

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pGeomColl = Nothing
            pNewGeomColl = Nothing
            pTmpGeomColl = Nothing
            pGeometry = Nothing
            pNewGeometry = Nothing
            pTopoOp = Nothing
        End Try
    End Function

    '''<summary>
    ''' Routine qui permet de simplifier toutes les géométries de travail en mémoire.
    '''</summary>
    ''' 
    Public Sub SimplifierGeometrie()
        'Déclarer les variables de travail
        Dim pGeometry As IGeometry = Nothing            'Interface ESRI contenant la géométrie de travail.
        Dim pTopoOp As ITopologicalOperator2 = Nothing  'Interface ESRI pour effectuer le traitement des géométries.

        Try
            'Traiter tous les éléments en mémoire
            For i = 0 To mpGeometrieTravail.GeometryCount - 1
                'Définir la géométrie à traiter
                pGeometry = mpGeometrieTravail.Geometry(i)

                'Vérifier si la géométrie est une surface
                If pGeometry.GeometryType = esriGeometryType.esriGeometryPolygon _
                Or pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline _
                Or pGeometry.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                    'Simplifier la géométrie afin de la rendre valide
                    pTopoOp = CType(pGeometry, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()
                End If
            Next i

        Catch erreur As Exception
            'Retourner l'erreur
            Throw erreur
        Finally
            'Vider la mémoire
            pGeometry = Nothing
            pTopoOp = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet d'effectuer le traitement d'union entre les géométries de travail en mémoire.
    '''</summary>
    ''' 
    Public Sub UnionGeometrie()
        'Déclarer les variables de travail
        Dim pGeometry As IGeometry = Nothing                    'Interface ESRI contenant une géométrie de travail.
        Dim pPointColl As IPointCollection = Nothing            'Interface ESRI utilisé pour ajouter des points dans un multipoint.
        Dim pPolygonColl As IGeometryCollection = Nothing       'Interface ESRI contenant toutes les surfaces.
        Dim pPolylineColl As IGeometryCollection = Nothing      'Interface ESRI contenant toutes les lignes.
        Dim pMultipointColl As IGeometryCollection = Nothing    'Interface ESRI contenant tous les points.
        Dim pPolygon As IPolygon = Nothing              'Interface ESRI contenant l'union des nouvelles surfaces.
        Dim pPolyline As IPolyline = Nothing            'Interface ESRI contenant l'union des nouvelles lignes.
        Dim pMultipoint As IMultipoint = Nothing        'Interface ESRI contenant l'union des nouveaux points.
        Dim pTopoOp As ITopologicalOperator2 = Nothing  'Interface ESRI pour effectuer le traitement des géométries.

        Try
            'Créer un Bag pour toutes les surfaces
            pPolygonColl = CType(New GeometryBag, IGeometryCollection)
            'Créer un Bag pour toutes les lignes
            pPolylineColl = CType(New GeometryBag, IGeometryCollection)
            'Créer un Bag pour tous les points
            pMultipointColl = CType(New GeometryBag, IGeometryCollection)

            'Traiter tous les éléments en mémoire
            For i = 0 To mpGeometrieTravail.GeometryCount - 1
                'Définir la géométrie à traiter
                pGeometry = mpGeometrieTravail.Geometry(i)

                'Vérifier si la géométrie est une surface
                If pGeometry.GeometryType = esriGeometryType.esriGeometryPolygon Then
                    'Ajouter la surface dans le Bag
                    pPolygonColl.AddGeometry(pGeometry)

                    'Vérifier si la géométrie est une ligne
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then
                    'Ajouter la ligne dans le Bag
                    pPolylineColl.AddGeometry(pGeometry)

                    'Vérifier si la géométrie est un multipoint
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                    'Ajouter le multipoint dans le Bag
                    pMultipointColl.AddGeometry(pGeometry)

                    'Vérifier si la géométrie est un point
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                    'Créer un nouveau Multipoint
                    pMultipoint = New Multipoint
                    'Définir la référence spatiale
                    pMultipoint.SpatialReference = pGeometry.SpatialReference
                    'Interface pour ajouter le Point
                    pPointColl = CType(pMultipoint, IPointCollection)
                    'Ajouter le Point
                    pPointColl.AddPoint(CType(pGeometry, IPoint))
                    'Ajouter le Point dans le Bag
                    pMultipointColl.AddGeometry(pMultipoint)
                End If
            Next i

            'Redéfinir les nouvelles géométries de travail
            pGeometry = CType(mpGeometrieTravail, IGeometry)

            'Vider les géométries de travail
            pGeometry.SetEmpty()

            'Vérifier si des surfaces sont présents
            If pPolygonColl.GeometryCount > 0 Then
                'Créer un nouveau polygon vide
                pPolygon = New Polygon
                'Définir la référence spatiale
                pPolygon.SpatialReference = pPolygonColl.Geometry(0).SpatialReference
                pPolygon.SnapToSpatialReference()
                'Interface pour construire les unions
                pTopoOp = CType(pPolygon, ITopologicalOperator2)
                'Construire l'union
                pTopoOp.ConstructUnion(CType(pPolygonColl, IEnumGeometry))
                'Ajouter l'union dans les géométries de travail
                mpGeometrieTravail.AddGeometry(CType(pTopoOp, IGeometry))
            End If

            'Vérifier si des lignes sont présents
            If pPolylineColl.GeometryCount > 0 Then
                'Créer une nouvelle polyline vide
                pPolyline = New Polyline
                'Définir la référence spatiale
                pPolyline.SpatialReference = pPolylineColl.Geometry(0).SpatialReference
                pPolyline.SnapToSpatialReference()
                'Interface pour construire les unions
                pTopoOp = CType(pPolyline, ITopologicalOperator2)
                'Construire l'union
                pTopoOp.ConstructUnion(CType(pPolylineColl, IEnumGeometry))
                'Ajouter l'union dans les géométries de travail
                mpGeometrieTravail.AddGeometry(CType(pTopoOp, IGeometry))
            End If

            'Vérifier si des Points sont présents
            If pMultipointColl.GeometryCount > 0 Then
                'Créer un nouveau Multipoint vide
                pMultipoint = New Multipoint
                'Définir la référence spatiale
                pMultipoint.SpatialReference = pMultipointColl.Geometry(0).SpatialReference
                pMultipoint.SnapToSpatialReference()
                'Interface pour construire les unions
                pTopoOp = CType(pMultipoint, ITopologicalOperator2)
                'Construire l'union
                pTopoOp.ConstructUnion(CType(pMultipointColl, IEnumGeometry))
                'Ajouter l'union dans les géométries de travail
                mpGeometrieTravail.AddGeometry(CType(pTopoOp, IGeometry))
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pGeometry = Nothing
            pPointColl = Nothing
            pPolygonColl = Nothing
            pPolylineColl = Nothing
            pMultipointColl = Nothing
            pPolygon = Nothing
            pPolyline = Nothing
            pMultipoint = Nothing
            pTopoOp = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet d'effectuer la différence multiple sur les géométries de travail selon les
    ''' éléments sélectionnés
    '''</summary>
    '''
    Public Sub DifferenceMultiple()
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                        'Interface ESRI contenant la fenêtre de données active.
        Dim pEnumFeature As IEnumFeature = Nothing        'Interface ESRI pour traiter tous les éléments sélectionnés.
        Dim pFeature As IFeature = Nothing                'Interface ESRI contenant un élément.
        Dim pGeometry As IGeometry = Nothing             'Interface ESRI utilisé pour assigner la référence spatiale.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisé pour ajouter des géométries.

        Try
            'Définir un nouveau GeometryBag vide
            pGeometry = CType(New GeometryBag, IGeometry)

            'Interface pour ajouter des géométries
            pGeomColl = CType(pGeometry, IGeometryCollection)

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Définir la référence spatiale
            pGeometry.SpatialReference = pMap.SpatialReference

            'MInterface pour extraire les éléments sélectionnés
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Trouver le premier élément
            pFeature = pEnumFeature.Next

            'Trouver les autres éléments
            Do Until pFeature Is Nothing
                'Copie de la géométrie originale
                pGeomColl.AddGeometry(pFeature.ShapeCopy)

                'Trouver le prochain élément
                pFeature = pEnumFeature.Next
            Loop

            'Trouver l'intersection entre les éléments sélectionnés et les géométries de travail
            mpGeometrieTravail = CType(fpDifferenceMultiDimension(pGeometry, CType(mpGeometrieTravail, IGeometry)), IGeometryCollection)

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometry = Nothing
            pGeomColl = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Fonction qui permet de retourner les différence points, lignes et surfaces entre deux géométries.
    ''' La géométrie résultante est toujours de même type que la géométrie A, elle est toujours présente
    ''' mais il se peut qu'elle soit vide. 
    '''</summary>
    '''
    '''<param name=" pGeometryA "> Interface ESRI de la Géométrie de base.</param>
    '''<param name=" pGeometryB "> Interface ESRI de la géométrie à comparer.</param>
    '''
    '''<returns> La fonction va retourner un "IGeometry". Si le traitement n'a pas réussi le "IGeometry" est "Nothing"</returns>
    '''     
    Public Function fpDifferenceMultiDimension(ByVal pGeometryA As IGeometry, ByVal pGeometryB As IGeometry) As IGeometry
        'Définir la valeur de retour par défaut
        fpDifferenceMultiDimension = Nothing

        'Vérifier si une des géométries est absente
        If pGeometryA Is Nothing Or pGeometryB Is Nothing Then Exit Function

        'Déclarer les variables de travail
        Dim pGeomDiff As IGeometry = Nothing 'Interface ESRI contenant le résultat des différences.

        Try

            'Vérifier la Géométry A est un Point
            If pGeometryA.GeometryType = esriGeometryType.esriGeometryPoint Then
                'Retrourner la différence entre un point et une géométrie
                pGeomDiff = CType(fpDifferencePoint(CType(pGeometryA, IPoint), pGeometryB), IGeometry)

                'Vérifier la Géométrie A est un Multipoint
            ElseIf pGeometryA.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                'Retrourner la différence entre un multipoint et une géométrie
                pGeomDiff = CType(fpDifferenceMultiPoint(CType(pGeometryA, IMultipoint), pGeometryB), IGeometry)

                'Vérifier la Géométrie A est une Polyline
            ElseIf pGeometryA.GeometryType = esriGeometryType.esriGeometryPolyline Then
                'Retrourner la différence entre une polyline et une géométrie
                pGeomDiff = CType(fpDifferencePolyline(CType(pGeometryA, IPolyline), pGeometryB), IGeometry)

                'Vérifier la Géométrie A est un Polygon
            ElseIf pGeometryA.GeometryType = esriGeometryType.esriGeometryPolygon Then
                'Retrourner la différence entre un Polygon et une géométrie
                pGeomDiff = CType(fpDifferencePolygon(CType(pGeometryA, IPolygon), pGeometryB), IGeometry)

                'Vérifier la Géométrie A est un Bag
            ElseIf pGeometryA.GeometryType = esriGeometryType.esriGeometryBag Then
                'Retrourner la différence entre un Bag et une géométrie
                pGeomDiff = CType(fpDifferenceBag(CType(pGeometryA, IGeometryBag), pGeometryB), IGeometry)
            End If

            'Retour du résultat
            fpDifferenceMultiDimension = CType(pGeomDiff, IGeometry)

        Catch erreur As Exception
            Throw erreur
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner les différence une géométrie point et une autre géométrie.
    ''' La géométrie résultante est toujours de type point, elle est toujours présente mais il se peut qu'elle soit vide. 
    '''</summary>
    '''
    '''<param name=" pPoint "> Interface ESRI du point de base. </param>
    '''<param name=" pGeometry "> Interface ESRI de la géométrie à comparer. </param>
    '''
    '''<returns> La fonction va retourner un "IPoint". Si le traitement n'a pas réussi le "IPoint" est "Nothing".</returns>
    ''' 
    Public Function fpDifferencePoint(ByVal pPoint As IPoint, ByVal pGeometry As IGeometry) As IPoint
        'Définir la valeur de retour par défaut
        fpDifferencePoint = Nothing

        'Vérifier si une des géométries est absente
        If pPoint Is Nothing Or pGeometry Is Nothing Then Exit Function

        'Déclarer les variables de travail
        Dim pClone As IClone = Nothing                'Interface ESRI utilisée pour cloner une géométrie.
        Dim pGeomDiff As IGeometry = Nothing         'Interface ESRI contenant les géométries de différences.
        Dim pRelOp As IRelationalOperator = Nothing   'Interface ESRI pour vérifier la relation entre 2 géométries.

        Try
            'Arrondir les coordonnées selon la référence spatiale
            pPoint.SnapToSpatialReference()
            pGeometry.SnapToSpatialReference()

            'Interface pour cloner une géométrie
            pClone = CType(pPoint, IClone)

            'Cloner le point
            pGeomDiff = CType(pClone.Clone, IGeometry)

            'Vérifier si le point est vide
            If Not (pPoint.IsEmpty Or pGeometry.IsEmpty) Then
                'Interface pour vérifier si le point intersecte la géométrie
                pRelOp = CType(pGeometry, IRelationalOperator)

                'Vérifier si le point n'intersecte pas la géométrie
                If Not pRelOp.Disjoint(pPoint) Then
                    'Vider la géométrie
                    pGeomDiff.SetEmpty()
                End If
            End If

            'Retour du résultat
            fpDifferencePoint = CType(pGeomDiff, IPoint)

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pClone = Nothing
            pGeomDiff = Nothing
            pRelOp = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner les différence une géométrie Multipoint et une autre géométrie.
    ''' La géométrie résultante est toujours de type Multipoint, elle est toujours présente mais il se
    ''' peut qu'elle soit vide.
    '''</summary>
    '''
    '''<param name=" pMultiPoint "> Interface ESRI du Multipoint de base. </param>
    '''<param name=" pGeometry "> Interface ESRI de la géométrie à comparer. </param>
    '''
    '''<returns> La fonction va retourner un "IMultiPoint". Si le traitement n'a pas réussi le "IMultiPoint" est "Nothing".</returns>
    ''' 
    Public Function fpDifferenceMultiPoint(ByVal pMultiPoint As IMultipoint, ByVal pGeometry As IGeometry) As IMultipoint
        'Définir la valeur de retour par défaut
        fpDifferenceMultiPoint = Nothing

        'Vérifier si une des géométries est absente
        If pMultiPoint Is Nothing Or pGeometry Is Nothing Then Exit Function

        'Déclarer les variables de travail
        Dim pClone As IClone = Nothing                  'Interface ESRI utilisée pour cloner une géométrie.
        Dim pGeomDiff As IGeometry = Nothing            'Interface ESRI contenant les géométries de différences.
        Dim pGeomColl As IGeometryCollection = Nothing  'Interface ESRI utilisée pour les composantes du Bag.
        Dim pDiffColl As IGeometryCollection = Nothing  'Interface ESRI utilisée pour les composantes des différences.
        Dim pRelOp As IRelationalOperator = Nothing     'Interface ESRI pour vérifier la relation entre 2 géométries.
        Dim pTopoOp As ITopologicalOperator2 = Nothing  'Interface ESRI pour retourner les géométries de différences.

        Try
            'Interface pour cloner une géométrie
            pClone = CType(pMultiPoint, IClone)

            'Cloner le multipoint
            pGeomDiff = CType(pClone.Clone, IGeometry)

            'Vérifier si le multipoint est vide
            If Not (pMultiPoint.IsEmpty Or pGeometry.IsEmpty) Then
                'Interface pour vérifier si le multipoint intersecte la géométrie
                pRelOp = CType(pGeometry, IRelationalOperator)

                'Vérifier si le multipoint n'intersecte pas la géométrie
                If Not pRelOp.Disjoint(pMultiPoint) Then
                    'Vider la géométrie de différence
                    pGeomDiff.SetEmpty()

                    'Interface pour ajouter les géométries de différence
                    pDiffColl = CType(pGeomDiff, IGeometryCollection)

                    'Interface pour traiter toutes les géométries du Multipoint
                    pGeomColl = CType(pMultiPoint, IGeometryCollection)

                    'Traiter toutes les géométries du MultiPoint
                    For i = 0 To pGeomColl.GeometryCount - 1
                        'Retourner la différence entre une composante et la géométrie
                        pDiffColl.AddGeometry(fpDifferencePoint(CType(pGeomColl.Geometry(i), IPoint), pGeometry))
                    Next i

                    'Interface pour simplifier la géométrie
                    pTopoOp = CType(pDiffColl, ITopologicalOperator2)

                    'Simplifier le multipoint
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()
                End If
            End If

            'Retour du résultat
            fpDifferenceMultiPoint = CType(pGeomDiff, IMultipoint)

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pClone = Nothing
            pGeomDiff = Nothing
            pGeomColl = Nothing
            pDiffColl = Nothing
            pRelOp = Nothing
            pTopoOp = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner les différence une géométrie Polyline et une autre géométrie.
    ''' La géométrie résultante est toujours de type Polyline, elle est toujours présente mais il se peut
    ''' qu'elle soit vide. 
    '''</summary>
    '''
    '''<param name=" pPolyline "> Interface ESRI du Polyline de base. </param>
    '''<param name=" pGeometry "> Interface ESRI de la géométrie à comparer. </param>
    '''
    '''<returns> La fonction va retourner un "Polyline". Si le traitement n'a pas réussi le "Polyline" est "Nothing".</returns>
    ''' 
    Public Function fpDifferencePolyline(ByVal pPolyline As IPolyline, ByVal pGeometry As IGeometry) As IPolyline
        'Définir la valeur de retour par défaut
        fpDifferencePolyline = Nothing

        'Vérifier si une des géométries est absente
        If pPolyline Is Nothing Or pGeometry Is Nothing Then Exit Function

        'Déclarer les variables de travail
        Dim pClone As IClone = Nothing                    'Interface ESRI utilisée pour cloner une géométrie.
        Dim pGeomDiff As IGeometry = Nothing             'Interface ESRI contenant les géométries de différences.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes du Bag.
        Dim pDiffColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes des différences.
        Dim pLimite As IGeometry = Nothing               'Interface ESRI contenant les limites de la géométrie.
        Dim pRelOp As IRelationalOperator = Nothing       'Interface ESRI pour vérifier la relation entre 2 géométries.
        Dim pTopoOp As ITopologicalOperator2 = Nothing    'Interface ESRI pour retourner les géométries de différences.

        Try
            'Arrondir les coordonnées selon la référence spatiale
            pPolyline.SnapToSpatialReference()
            pGeometry.SnapToSpatialReference()

            'Interface pour cloner une géométrie
            pClone = CType(pPolyline, IClone)

            'Cloner le point
            pGeomDiff = CType(pClone.Clone, IGeometry)

            'Vérifier si la polyline est vide
            If Not (pPolyline.IsEmpty Or pGeometry.IsEmpty) Then
                'Interface pour vérifier si le multipoint intersecte la géométrie
                pRelOp = CType(pGeometry, IRelationalOperator)

                'Vérifier si la Polyline n'intersecte pas la géométrie
                If Not pRelOp.Disjoint(pPolyline) Then
                    'Vérifier si la géométrie est une ligne
                    If pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then
                        'Simplifier la géométrie
                        pTopoOp = CType(pGeometry, ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()

                        'Simplifier la polyline
                        pTopoOp = CType(pGeomDiff, ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()

                        'Extraire la géométrie commune à la ligne
                        pGeomDiff = CType(pTopoOp.Difference(pGeometry), IGeometry)

                        'Vérifier si la géométrie est une surface
                    ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolygon Then
                        'Simplifier la géométrie
                        pTopoOp = CType(pGeometry, ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()

                        'Extraire les limites de la surface
                        pLimite = CType(pTopoOp.Boundary, IGeometry)

                        'Simplifier la polyline
                        pTopoOp = CType(pGeomDiff, ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()

                        'Extraire la géométrie commune à la ligne
                        pGeomDiff = CType(pTopoOp.Difference(pGeometry), IGeometry)

                        'Interface pour extraire la différence
                        pTopoOp = CType(pGeomDiff, ITopologicalOperator2)

                        'Extraire la géométrie commune à la limite de la surface
                        pGeomDiff = CType(pTopoOp.Difference(pLimite), IGeometry)

                        'Vérifier si la géométrie est un bag
                    ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryBag Then
                        'Interface pour traiter toutes les composantes du Bag
                        pGeomColl = CType(pGeometry, IGeometryCollection)

                        'Traiter toutes les géométries du Bag
                        For i = 0 To pGeomColl.GeometryCount - 1
                            'Retourner la différence entre la polyline et une composante du Bag
                            pGeomDiff = CType(fpDifferencePolyline(CType(pGeomDiff, IPolyline), CType(pGeomColl.Geometry(i), IGeometry)), IGeometry)
                        Next i
                    End If
                End If
            End If

            'Retour du résultat
            fpDifferencePolyline = CType(pGeomDiff, IPolyline)

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pClone = Nothing
            pGeomDiff = Nothing
            pGeomColl = Nothing
            pDiffColl = Nothing
            pLimite = Nothing
            pRelOp = Nothing
            pTopoOp = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner les différence une géométrie Polygon et une autre géométrie.
    ''' La géométrie résultante est toujours de type Polygon, elle est toujours présente mais il se peut qu'elle soit vide.
    '''</summary>
    '''
    '''<param name=" pPolygon "> Interface ESRI du Polygon de base. </param>
    '''<param name=" pGeometry "> Interface ESRI de la géométrie à comparer. </param>
    '''
    '''<returns> La fonction va retourner un "Polygon". Si le traitement n'a pas réussi le "Polygon" est "Nothing".</returns>
    ''' 
    Public Function fpDifferencePolygon(ByVal pPolygon As IPolygon, ByVal pGeometry As IGeometry) As IPolygon
        'Définir la valeur de retour par défaut
        fpDifferencePolygon = Nothing

        'Vérifier si une des géométries est absente
        If pPolygon Is Nothing Or pGeometry Is Nothing Then Exit Function

        'Déclarer les variables de travail
        Dim pClone As IClone = Nothing                    'Interface ESRI utilisée pour cloner une géométrie.
        Dim pGeomDiff As IGeometry = Nothing             'Interface ESRI contenant les géométries de différences.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes du Bag.
        Dim pRelOp As IRelationalOperator = Nothing       'Interface ESRI pour vérifier la relation entre 2 géométries.
        Dim pTopoOp As ITopologicalOperator2 = Nothing    'Interface ESRI pour retourner les géométries de différences.

        Try
            'Arrondir les coordonnées selon la référence spatiale
            pPolygon.SnapToSpatialReference()
            pGeometry.SnapToSpatialReference()

            'Interface pour cloner une géométrie
            pClone = CType(pPolygon, IClone)

            'Cloner le point
            pGeomDiff = CType(pClone.Clone, IGeometry)

            'Vérifier si le Polygon est vide
            If Not (pPolygon.IsEmpty Or pGeometry.IsEmpty) Then
                'Interface pour vérifier si le multipoint intersecte la géométrie
                pRelOp = CType(pGeometry, IRelationalOperator)

                'Vérifier si la Polygon n'intersecte pas la géométrie
                '        If Not pRelOp.Disjoint(pPolygon) Then
                'Vérifier si la géométrie est une surface
                If pGeometry.GeometryType = esriGeometryType.esriGeometryPolygon Then
                    'Simplifier la géométrie
                    pTopoOp = CType(pGeometry, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Simplifier le polygon
                    pTopoOp = CType(pGeomDiff, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Extraire la géométrie commune à la surface
                    pGeomDiff = CType(pTopoOp.Difference(pGeometry), IGeometry)

                    'Vérifier si la géométrie est un bag
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryBag Then
                    'Interface pour traiter toutes les composantes du Bag
                    pGeomColl = CType(pGeometry, IGeometryCollection)

                    'Traiter toutes les géométries du Bag
                    For i = 0 To pGeomColl.GeometryCount - 1
                        'Retourner la différence entre la polyline et une composante du Bag
                        pGeomDiff = CType(fpDifferencePolygon(CType(pGeomDiff, IPolygon), CType(pGeomColl.Geometry(i), IGeometry)), IGeometry)
                    Next i
                End If
            End If

            'Retour du résultat
            fpDifferencePolygon = CType(pGeomDiff, IPolygon)

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pClone = Nothing
            pGeomDiff = Nothing
            pGeomColl = Nothing
            pRelOp = Nothing
            pTopoOp = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner les différence une géométrie Bag et une autre géométrie.
    ''' La géométrie résultante est toujours de type GeometryBag, elle est toujours présente mais il se peut qu'elle soit vide. 
    '''</summary>
    '''
    '''<param name=" pGeometryBag "> Interface ESRI du GeometryBag de base. </param>
    '''<param name=" pGeometry "> Interface ESRI de la géométrie à comparer. </param>
    '''
    '''<returns> La fonction va retourner un "IGeometryBag". Si le traitement n'a pas réussi le "IGeometryBag" est "Nothing".</returns>
    ''' 
    Public Function fpDifferenceBag(ByVal pGeometryBag As IGeometryBag, ByVal pGeometry As IGeometry) As IGeometryBag
        'Définir la valeur de retour par défaut
        fpDifferenceBag = Nothing

        'Vérifier si une des géométries est absente
        If pGeometryBag Is Nothing Or pGeometry Is Nothing Then Exit Function

        'Déclarer les variables de travail
        Dim pClone As IClone = Nothing                    'Interface ESRI utilisée pour cloner une géométrie.
        Dim pGeomDiff As IGeometry = Nothing             'Interface ESRI contenant les géométries de différences.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes du Bag.
        Dim pDiffColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes des différences.
        Dim pDifference As IGeometry = Nothing           'Interface ESRI contenant une géométrie de travail.

        Try
            'Interface pour cloner une géométrie
            pClone = CType(pGeometryBag, IClone)

            'Cloner le multipoint
            pGeomDiff = CType(pClone.Clone, IGeometry)

            'Vérifier si le multipoint est vide
            If Not (pGeometryBag.IsEmpty Or pGeometry.IsEmpty) Then
                'Vider la géométrie de différence
                pGeomDiff.SetEmpty()

                'Interface pour ajouter les géométries de différence
                pDiffColl = CType(pGeomDiff, IGeometryCollection)

                'Interface pour traiter toutes les géométries du Bag
                pGeomColl = CType(pGeometryBag, IGeometryCollection)

                'Traiter toutes les géométries du Bag
                For i = 0 To pGeomColl.GeometryCount - 1
                    'Vérifier si la géométrie du Bag est un Point
                    If pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPoint Then
                        'Extraire la différence entre un point et une géométrie
                        pDifference = CType(fpDifferencePoint(CType(pGeomColl.Geometry(i), IPoint), pGeometry), IGeometry)

                        'Vérifier si la géométrie est absente
                        If Not pDifference Is Nothing Then
                            'vérifier si la géométrie est vide
                            If Not pDifference.IsEmpty Then
                                'Ajouter la géométrie non vide
                                pDiffColl.AddGeometry(pDifference)
                            End If
                        End If

                        'Vérifier si la géométrie du Bag est un Multipoint
                    ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryMultipoint Then
                        'Extraire la différence entre un point et une géométrie
                        pDifference = CType(fpDifferenceMultiPoint(CType(pGeomColl.Geometry(i), IMultipoint), pGeometry), IGeometry)

                        'Vérifier si la géométrie est absente
                        If Not pDifference Is Nothing Then
                            'vérifier si la géométrie est vide
                            If Not pDifference.IsEmpty Then
                                'Ajouter la géométrie non vide
                                pDiffColl.AddGeometry(pDifference)
                            End If
                        End If

                        'Vérifier si la géométrie du Bag est une Polyline
                    ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPolyline Then
                        'Extraire la différence entre un point et une géométrie
                        pDifference = CType(fpDifferencePolyline(CType(pGeomColl.Geometry(i), IPolyline), pGeometry), IGeometry)

                        'Vérifier si la géométrie est absente
                        If Not pDifference Is Nothing Then
                            'vérifier si la géométrie est vide
                            If Not pDifference.IsEmpty Then
                                'Ajouter la géométrie non vide
                                pDiffColl.AddGeometry(pDifference)
                            End If
                        End If

                        'Vérifier si la géométrie du Bag est un Polygon
                    ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPolygon Then
                        'Extraire la différence entre un point et une géométrie
                        pDifference = CType(fpDifferencePolygon(CType(pGeomColl.Geometry(i), IPolygon), pGeometry), IGeometry)

                        'Vérifier si la géométrie est absente
                        If Not pDifference Is Nothing Then
                            'vérifier si la géométrie est vide
                            If Not pDifference.IsEmpty Then
                                'Ajouter la géométrie non vide
                                pDiffColl.AddGeometry(pDifference)
                            End If
                        End If

                        'Vérifier si la géométrie du Bag est un autre Bag
                    ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryBag Then
                        'Extraire la différence entre un point et une géométrie
                        pDifference = CType(fpDifferenceBag(CType(pGeomColl.Geometry(i), IGeometryBag), pGeometry), IGeometry)

                        'Vérifier si la géométrie est absente
                        If Not pDifference Is Nothing Then
                            'vérifier si la géométrie est vide
                            If Not pDifference.IsEmpty Then
                                'Ajouter la géométrie non vide
                                pDiffColl.AddGeometry(pDifference)
                            End If
                        End If
                    End If
                Next i
            End If

            'Retour du résultat
            fpDifferenceBag = CType(pGeomDiff, IGeometryBag)

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pClone = Nothing
            pGeomDiff = Nothing
            pGeomColl = Nothing
            pDiffColl = Nothing
            pDifference = Nothing
        End Try
    End Function

    '''<summary>
    ''' Routine qui permet d'effectuer l'intersection multiple dimension sur les géométries de travail
    ''' selon les éléments sélectionnés.
    '''</summary>
    '''   
    Public Sub IntersectionMultiple()
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                        'Interface ESRI contenant la fenêtre de données active.
        Dim pEnumFeature As IEnumFeature = Nothing        'Interface ESRI pour traiter tous les éléments sélectionnés.
        Dim pFeature As IFeature = Nothing                'Interface ESRI contenant un élément.
        Dim pGeometry As IGeometry = Nothing              'Interface ESRI utilisé pour assigner la référence spatiale.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisé pour ajouter des géométries.

        Try
            'Définir un nouveau GeometryBag vide
            pGeometry = CType(New GeometryBag, IGeometry)

            'Interface pour ajouter des géométries
            pGeomColl = CType(pGeometry, IGeometryCollection)

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Définir la référence spatiale
            pGeometry.SpatialReference = pMap.SpatialReference

            'MInterface pour extraire les éléments sélectionnés
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)

            'Trouver le premier élément
            pFeature = pEnumFeature.Next

            'Trouver les autres éléments
            Do Until pFeature Is Nothing
                'Copie de la géométrie originale
                pGeomColl.AddGeometry(pFeature.ShapeCopy)

                'Trouver le prochain élément
                pFeature = pEnumFeature.Next
            Loop

            'Trouver l'intersection entre les éléments sélectionnés et les géométries de travail
            mpGeometrieTravail = CType(fpIntersectMultiDimension(pGeometry, CType(mpGeometrieTravail, IGeometry), 40), IGeometryCollection)

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometry = Nothing
            pGeomColl = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Fonction qui permet de retourner les intersections points, lignes et surfaces entre deux
    ''' géométries. Le GeometryBag résultant contient trois géométries, une géométrie MultiPoint pour
    ''' les intersections de points, une géométrie Polyline les intersections de lignes et une géométrie
    ''' Polygon pour les intersections de surfaces. Les trois géométries sont toujours présentes même s'il
    ''' n'y aucune intersection, le nombre de composante sera égale à zéro. Les Polylines qui ne
    ''' respectent pas la longueur minimum seront changées en Points et les surfaces qui ne respectent pas
    ''' la superficie minimale seront changées en points également.
    '''</summary>
    '''
    '''<param name=" pGeometryA "> Interface ESRI de la Géométrie de base.</param>
    '''<param name=" pGeometryB "> Interface ESRI de la géométrie à comparer.</param>
    '''<param name=" dLongueur "> Longueur minimum à respecter pour un segment de ligne.</param>
    '''<param name=" dSuperficie "> Superficie minimum à respecter pour une surface.</param>
    '''
    '''<returns> La fonction va retourner un "IGeometryBag". Si le traitement n'a pas réussi le "IGeometryBag" est "Nothing".</returns>
    ''' 
    Public Function fpIntersectMultiDimension(ByVal pGeometryA As IGeometry, ByVal pGeometryB As IGeometry, _
    Optional ByVal dLongueur As Double = 0, Optional ByVal dSuperficie As Double = 0) As IGeometryBag
        'Définir la valeur de retour par défaut
        fpIntersectMultiDimension = Nothing

        'Vérifier si les géométrie sont valide
        If pGeometryA Is Nothing Or pGeometryB Is Nothing Then Exit Function

        'Déclarer les variables de travail
        Dim pGeometry As IGeometry = Nothing             'Interface ESRI utiliséé pour changer la référence spatiale.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes du Bag.
        Dim pPointColl As IPointCollection3 = Nothing     'Interface ESRI utilisée pour ajouter des points.
        Dim pMultiPoint As IGeometryCollection = Nothing  'Interface ESRI contenant les intersections Points.
        Dim pPolyline As IGeometryCollection = Nothing    'Interface ESRI contenant les intersectrions Lignes.
        Dim pPolygon As IGeometryCollection = Nothing     'Interface ESRI contenant les intersectrions Surfaces.
        Dim pRelOp As IRelationalOperator = Nothing       'Interface ESRI pour vérifier la relation entre 2 géométries.

        Try
            'Définir le résultat vide
            pMultiPoint = CType(New Multipoint, IGeometryCollection)
            pPolyline = CType(New Polyline, IGeometryCollection)
            pPolygon = CType(New Polygon, IGeometryCollection)

            'Définir la référence spatiale
            pGeometry = CType(pMultiPoint, IGeometry)
            pGeometry.SpatialReference = pGeometryA.SpatialReference
            pGeometry = CType(pPolyline, IGeometry)
            pGeometry.SpatialReference = pGeometryA.SpatialReference
            pGeometry = CType(pPolygon, IGeometry)
            pGeometry.SpatialReference = pGeometryA.SpatialReference

            'Arrondir les coordonnées des géométries A et B
            '    pGeometryA.SnapToSpatialReference
            '    pGeometryB.SnapToSpatialReference

            'Simplifier les géométries
            pGeometryA = fpSimplifierGeometrie(pGeometryA)
            pGeometryB = fpSimplifierGeometrie(pGeometryB)

            'Vérifier si la géométrie est vide
            If Not (pGeometryA.IsEmpty Or pGeometryB.IsEmpty) Then
                'Si la géométrie A est un Point
                If pGeometryA.GeometryType = esriGeometryType.esriGeometryPoint Then
                    'Créer un nouveau GeometryBag vide
                    pGeomColl = CType(New GeometryBag, IGeometryCollection)

                    'Interface pour vérifier l'intersection
                    pRelOp = CType(pGeometryB, IRelationalOperator)

                    'Vérifier l'intersection du point point
                    If Not pRelOp.Disjoint(pGeometryA) Then
                        'Ajouter un point dans le Multipoint
                        pMultiPoint.AddGeometry(pGeometryA)
                    End If

                    'Si la géométrie A est un MultiPoint
                ElseIf pGeometryA.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                    'Extraire les intersections avec le MultiPoint
                    pGeomColl = CType(fpIntersectMultiPoint(CType(pGeometryA, IMultipoint), pGeometryB), IGeometryCollection)

                    'Définir les intersectyions multipoints
                    pMultiPoint = CType(pGeomColl.Geometry(0), IGeometryCollection)

                    'Si la géométrie A est une Polyline
                ElseIf pGeometryA.GeometryType = esriGeometryType.esriGeometryPolyline Then
                    'Extraire les intersections avec la Polyline
                    pGeomColl = CType(fpIntersectPolyline(CType(pGeometryA, IPolyline), pGeometryB), IGeometryCollection)

                    'Définir les intersectyions multipoints
                    pMultiPoint = CType(pGeomColl.Geometry(0), IGeometryCollection)

                    'Définir les intersectyions Polyline
                    pPolyline = CType(pGeomColl.Geometry(1), IGeometryCollection)

                    'Si la géométrie A est un Polygon
                ElseIf pGeometryA.GeometryType = esriGeometryType.esriGeometryPolygon Then
                    'Extraire les intersections avec le Polygon
                    pGeomColl = CType(fpIntersectPolygon(CType(pGeometryA, IPolygon), pGeometryB), IGeometryCollection)

                    'Définir les intersectyions multipoints
                    pMultiPoint = CType(pGeomColl.Geometry(0), IGeometryCollection)

                    'Définir les intersectyions Polyline
                    pPolyline = CType(pGeomColl.Geometry(1), IGeometryCollection)

                    'Définir les intersectyions polygon
                    pPolygon = CType(pGeomColl.Geometry(2), IGeometryCollection)

                    'Si la géométrie A est un Bag
                ElseIf pGeometryA.GeometryType = esriGeometryType.esriGeometryBag Then
                    'Extraire les intersections avec le Bag
                    pGeomColl = CType(fpIntersectBag(CType(pGeometryA, IGeometryBag), pGeometryB), IGeometryCollection)

                    'Définir les intersectyions multipoints
                    pMultiPoint = CType(pGeomColl.Geometry(0), IGeometryCollection)

                    'Définir les intersectyions Polyline
                    pPolyline = CType(pGeomColl.Geometry(1), IGeometryCollection)

                    'Définir les intersectyions polygon
                    pPolygon = CType(pGeomColl.Geometry(2), IGeometryCollection)
                End If

                'Vérifier si on doit transformer les lignes en points selon une longueur minimale
                If dLongueur > 0 Then
                    'Interface pour ajouter des points
                    pPointColl = CType(pMultiPoint, IPointCollection3)

                    'Transformer les lignes trop petites en points
                    pPointColl.AddPointCollection(fpChangerLigneEnPointSelonLongueur(CType(pPolyline, IPolyline2), dLongueur))
                End If

                'Vérifier si on doit transformer les surfaces en points selon une superficie minimale
                If dSuperficie > 0 Then
                    'Interface pour ajouter des points
                    pPointColl = CType(pMultiPoint, IPointCollection3)

                    'Transformer les surfaces trop petites en points
                    pPointColl.AddPointCollection(fpChangerSurfaceEnPointSelonSuperficie(CType(pPolygon, IPolygon2), dSuperficie))
                End If
            End If

            'Définir le résultat vide
            pGeomColl = CType(New GeometryBag, IGeometryCollection)

            'Simplifier les points d'intersections
            pMultiPoint = CType(fpSimplifierGeometrie(CType(pMultiPoint, IGeometry)), IGeometryCollection)

            'Simplifier les lignes d'intersections
            pPolyline = CType(fpSimplifierGeometrie(CType(pPolyline, IGeometry)), IGeometryCollection)

            'Simplifier les surfaces d'intersections
            pPolygon = CType(fpSimplifierGeometrie(CType(pPolygon, IGeometry)), IGeometryCollection)

            'Insérer l'intersection de points
            pGeomColl.AddGeometry(CType(pMultiPoint, IGeometry))

            'Insérer l'intersection de lignes
            pGeomColl.AddGeometry(CType(pPolyline, IGeometry))

            'Insérer l'intersection de surfaces
            pGeomColl.AddGeometry(CType(pPolygon, IGeometry))

            'Retourner l'intersection multidimension
            fpIntersectMultiDimension = CType(pGeomColl, IGeometryBag)

            'Définir la référence spatiale du résultat
            fpIntersectMultiDimension.SpatialReference = pGeometryA.SpatialReference

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pGeometry = Nothing
            pGeomColl = Nothing
            pPointColl = Nothing
            pMultiPoint = Nothing
            pPolyline = Nothing
            pPolygon = Nothing
            pRelOp = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner les intersections points entre une géométrie MultiPoint et une
    ''' autre géométrie. Le GeometryBag résultant contient une seule géométrie MultiPoint pour les
    ''' intersections de points. La géométrie MultiPoint est toujours présente même s'il n'y aucune
    ''' intersection, le nombre de composante sera égale à zéro. 
    '''</summary>
    '''
    '''<param name=" pMultiPoint "> Interface ESRI de la Géométrie MultiPoint de base.</param>
    '''<param name=" pGeometry "> Interface ESRI de la géométrie à comparer.</param>
    '''
    '''<returns> La fonction va retourner un "IGeometryBag". Si le traitement n'a pas réussi le "IGeometryBag" est "Nothing".</returns>
    ''' 
    Public Function fpIntersectMultiPoint(ByVal pMultiPoint As IMultipoint, ByVal pGeometry As IGeometry) As IGeometryBag
        'Déclarer les variables de travail
        Dim pRelOp As IRelationalOperator = Nothing       'Interface ESRI pour vérifier la relation entre 2 géométries.
        Dim pTopoOp As ITopologicalOperator2 = Nothing    'Interface ESRI pour les intersections entre 2 géométries.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes du Bag.
        Dim pPointColl As IPointCollection = Nothing      'Interface ESRI utilisée pour ajouter des points.
        Dim pNewGeomColl As IGeometryCollection = Nothing 'Interface ESRI utilisée pour le nouveau Bag d'intersections.
        Dim pNewMultiPoint As IMultipoint = Nothing       'Interface ESRI contenant les intersections Points

        'Définir la valeur de retour par défaut
        fpIntersectMultiPoint = Nothing

        Try
            'Créer les géométries d'intersections vides
            pNewMultiPoint = CType(New Multipoint, IMultipoint)
            pNewMultiPoint.SpatialReference = pMultiPoint.SpatialReference

            'Interface pour vérifier la présence d'une intersection
            pRelOp = CType(pGeometry, IRelationalOperator)

            'Vérifier si les deux géométries se touches
            If Not pRelOp.Disjoint(pMultiPoint) Then

                'Vérifier si la géométrie à comparer est de type point
                If pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                    'Interface pour ajouter des nouveaux points
                    pPointColl = CType(pNewMultiPoint, IPointCollection)

                    'Ajouter l'intersection des points
                    pPointColl.AddPoint(CType(pGeometry, IPoint))

                    'Vérifier si la géométrie à comparer est un GeometryBag
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryBag Then
                    'Interface pour traiter toutes les géométries
                    pGeomColl = CType(pGeometry, IGeometryCollection)

                    'Traiter toutes les géométries
                    For i = 0 To pGeomColl.GeometryCount - 1
                        'Trouver l'intersection entre la ligne et une composante du GeometryBag
                        pNewGeomColl = CType(fpIntersectMultiPoint(pMultiPoint, CType(pGeomColl.Geometry(i), IGeometry)), IGeometryCollection)

                        'Vérifier si la géométrie est vide
                        If pNewMultiPoint.IsEmpty Then
                            'Insérer les intersections points
                            pNewMultiPoint = CType(pNewGeomColl.Geometry(0), IMultipoint)
                        Else
                            'Simplifier les points d'intersections
                            pNewMultiPoint = CType(fpSimplifierGeometrie(CType(pNewMultiPoint, IGeometry)), IMultipoint)

                            'Insérer les intersections points
                            pTopoOp = CType(pNewGeomColl.Geometry(0), ITopologicalOperator2)
                            pTopoOp.IsKnownSimple_2 = False
                            pTopoOp.Simplify()
                            pNewMultiPoint = CType(pTopoOp.Union(pNewMultiPoint), IMultipoint)
                        End If
                    Next i

                    'Vérifier si les géométries sont de mêmes types
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                    'Interface pour calculer l'intersection entre les géométrie
                    pTopoOp = CType(pMultiPoint, ITopologicalOperator2)

                    'Extraire l'intersection des points
                    pNewMultiPoint = CType(pTopoOp.Intersect(pGeometry, esriGeometryDimension.esriGeometry0Dimension), IMultipoint)

                    'Arrondir les coordonnées à la précision du système de référence
                    pNewMultiPoint.SnapToSpatialReference()

                    'Si les géométries sont des types différents
                Else
                    'Interface pour calculer l'intersection entre les géométrie
                    pTopoOp = CType(pMultiPoint, ITopologicalOperator2)

                    'Extraire l'intersection multidimension
                    pGeomColl = CType(pTopoOp.IntersectMultidimension(pGeometry), IGeometryCollection)

                    'Trouver l'intersection des lignes
                    For i = 0 To pGeomColl.GeometryCount - 1
                        'Vérifier si la géométrie correspond à l'intersection des points
                        If pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPoint Then
                            'Interface pour ajouter un point
                            pPointColl = CType(pNewMultiPoint, IPointCollection)

                            'Ajouter un point au MultiPoint
                            pPointColl.AddPoint(CType(pGeomColl.Geometry(i), IPoint))

                            'Vérifier si la géométrie correspond à l'intersection des lignes
                        ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryMultipoint Then
                            'Définir l'intersection des points
                            pNewMultiPoint = CType(pGeomColl.Geometry(i), IMultipoint)
                        End If
                    Next i
                End If
            End If

            'Créer un nouveau GeometryBag vide
            pNewGeomColl = CType(New GeometryBag, IGeometryCollection)

            'Insérer les intersections points
            pNewGeomColl.AddGeometry(pNewMultiPoint)

            'Retourner le resultat
            fpIntersectMultiPoint = CType(pNewGeomColl, IGeometryBag)

            'Définir la référence spatiale du résultat
            fpIntersectMultiPoint.SpatialReference = pMultiPoint.SpatialReference

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pRelOp = Nothing
            pTopoOp = Nothing
            pGeomColl = Nothing
            pPointColl = Nothing
            pNewGeomColl = Nothing
            pNewMultiPoint = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner les intersections points et lignes entre une géométrie Polyline et une autre
    ''' géométrie. Le GeometryBag résultant contient une géométrie MultiPoint pour les intersections de points et une
    ''' géométrie Polyline pour les intersections lignes. La géométrie Bag résultante contient toujours une géométrie
    ''' MultiPoint et Polyline même s'il n'y aucune intersection, le nombre de composante sera égale à zéro.
    '''</summary>
    '''
    '''<param name=" pPolyline "> Interface ESRI de la Géométrie Polyline de base.</param>
    '''<param name=" pGeometry "> Interface ESRI de la géométrie à comparer.</param>
    '''
    '''<returns> La fonction va retourner un "IGeometryBag". Si le traitement n'a pas réussi le "IGeometryBag" est "Nothing".</returns>
    ''' 
    Public Function fpIntersectPolyline(ByVal pPolyline As IPolyline, ByVal pGeometry As IGeometry) As IGeometryBag
        'Déclarer les variables de travail
        Dim pRelOp As IRelationalOperator = Nothing       'Interface ESRI pour vérifier la relation entre 2 géométries.
        Dim pTopoOp As ITopologicalOperator2 = Nothing    'Interface ESRI pour les intersections entre 2 géométries.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes du Bag.
        Dim pPointColl As IPointCollection = Nothing      'Interface ESRI utilisée pour ajouter des points.
        Dim pNewGeomColl As IGeometryCollection = Nothing 'Interface ESRI utilisée pour le nouveau Bag d'intersections.
        Dim pNewMultiPoint As IGeometry = Nothing        'Interface ESRI contenant les intersections Points.
        Dim pNewPolyline As IGeometry = Nothing          'Interface ESRI contenant les intersections Lignes.

        'Définir la valeur de retour par défaut
        fpIntersectPolyline = Nothing

        Try
            'Créer les géométries d'intersections vides
            pNewMultiPoint = CType(New Multipoint, IGeometry)
            pNewPolyline = CType(New Polyline, IGeometry)

            'Définir la référence spatiale
            pNewMultiPoint.SpatialReference = pPolyline.SpatialReference
            pNewPolyline.SpatialReference = pPolyline.SpatialReference

            'Interface pour vérifier la présence d'une intersection
            pRelOp = CType(pGeometry, IRelationalOperator)

            'Vérifier si les deux géométries se touches
            If Not pRelOp.Disjoint(pPolyline) Then

                'Vérifier si la géométrie à comparer est de type point
                If pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                    'Interface pour ajouter des nouveaux points
                    pPointColl = CType(pNewMultiPoint, IPointCollection)

                    'Ajouter l'intersection des points
                    pPointColl.AddPoint(CType(pGeometry, IPoint))

                    'Vérifier si la géométrie à comparer est un GeometryBag
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryBag Then
                    'Interface pour traiter toutes les géométries
                    pGeomColl = CType(pGeometry, IGeometryCollection)

                    'Traiter toutes les géométries
                    For i = 0 To pGeomColl.GeometryCount - 1
                        'Trouver l'intersection entre la ligne et une composante du GeometryBag
                        pNewGeomColl = CType(fpIntersectPolyline(pPolyline, CType(pGeomColl.Geometry(i), IGeometry)), IGeometryCollection)

                        'Vérifier si la géométrie est vide
                        If pNewMultiPoint.IsEmpty Then
                            'Insérer les intersections points
                            pNewMultiPoint = CType(pNewGeomColl.Geometry(0), IGeometry)
                        Else
                            'Simplifier les points d'intersections
                            pNewMultiPoint = fpSimplifierGeometrie(pNewMultiPoint)

                            'Insérer les intersections points
                            pTopoOp = CType(pNewGeomColl.Geometry(0), ITopologicalOperator2)
                            pTopoOp.IsKnownSimple_2 = False
                            pTopoOp.Simplify()
                            pNewMultiPoint = CType(pTopoOp.Union(pNewMultiPoint), IGeometry)
                        End If

                        'Vérifier si la géométrie est vide
                        If pNewPolyline.IsEmpty Then
                            'Insérer les intersections lignes
                            pNewPolyline = CType(pNewGeomColl.Geometry(1), IGeometry)
                        Else
                            'Simplifier les lignes d'intersections
                            pNewPolyline = fpSimplifierGeometrie(pNewPolyline)

                            'Insérer les intersections lignes
                            pTopoOp = CType(pNewGeomColl.Geometry(1), ITopologicalOperator2)
                            pTopoOp.Simplify()
                            pNewPolyline = CType(pTopoOp.Union(pNewPolyline), IGeometry)
                        End If
                    Next i

                    'Vérifier si les géométries sont de mêmes types
                ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then
                    'Interface pour calculer l'intersection entre les géométrie
                    pTopoOp = CType(pPolyline, ITopologicalOperator2)

                    'Extraire l'intersection des points
                    '            Set pNewMultiPoint = pTopoOp.Intersect(pGeometry, esriGeometry0Dimension)
                    Call fbIntersectGeometrie(CType(pPolyline, IGeometry), pGeometry, pNewMultiPoint, esriGeometryDimension.esriGeometry0Dimension)

                    'Arrondir les coordonnées à la précision du système de référence
                    pNewMultiPoint.SnapToSpatialReference()

                    'Extraire l'intersection des lignes
                    '            Set pNewPolyline = pTopoOp.Intersect(pGeometry, esriGeometry1Dimension)
                    Call fbIntersectGeometrie(pPolyline, pGeometry, pNewPolyline, esriGeometryDimension.esriGeometry1Dimension)

                    'Arrondir les coordonnées à la précision du système de référence
                    pNewPolyline.SnapToSpatialReference()

                    'Vérifier si le résultat de l'intersection est vide (BUG)
                    If pNewMultiPoint.IsEmpty And pNewPolyline.IsEmpty Then
                        'Extraire les limites de la géométrieA
                        pPointColl = CType(pTopoOp.Boundary, IPointCollection)

                        'Interface pour vérifier l'intersection entre les limites de A et la Géométrie de B
                        pTopoOp = CType(pPointColl, ITopologicalOperator2)

                        'Vérifier l'intersection entre les limites de A et la Géométrie de B
                        '   Set pNewMultiPoint = pTopoOp.Intersect(pGeometry, esriGeometry0Dimension)
                        Call fbIntersectGeometrie(CType(pPointColl, IGeometry), pGeometry, pNewMultiPoint, esriGeometryDimension.esriGeometry0Dimension)
                    End If

                    'Traiter les autres types de géométries
                Else
                    'Interface pour calculer l'intersection entre les géométrie
                    pTopoOp = CType(pPolyline, ITopologicalOperator2)

                    'Extraire l'intersection multidimension
                    pGeomColl = CType(pTopoOp.IntersectMultidimension(pGeometry), IGeometryCollection)

                    'Trouver l'intersection des lignes
                    For i = 0 To pGeomColl.GeometryCount - 1
                        'Vérifier si la géométrie correspond à l'intersection des points
                        If pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPoint Then
                            'Interface pour ajouter un point
                            pPointColl = CType(pNewMultiPoint, IPointCollection)
                            'Ajouter un point au MultiPoint
                            pPointColl.AddPoint(CType(pGeomColl.Geometry(i), IPoint))

                            'Vérifier si la géométrie correspond à l'intersection des lignes
                        ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryMultipoint Then
                            'Définir l'intersection des points
                            pNewMultiPoint = CType(pGeomColl.Geometry(i), IGeometry)

                            'Vérifier si la géométrie correspond à l'intersection des lignes
                        ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPolyline Then
                            'Définir l'intersection des lignes
                            pNewPolyline = CType(pGeomColl.Geometry(i), IGeometry)
                        End If
                    Next i
                End If
            End If

            'Créer un nouveau GeometryBag vide
            pNewGeomColl = CType(New GeometryBag, IGeometryCollection)

            'Insérer les intersections points
            pNewGeomColl.AddGeometry(pNewMultiPoint)

            'Insérer les intersections lignes
            pNewGeomColl.AddGeometry(pNewPolyline)

            'Retourner le resultat
            fpIntersectPolyline = CType(pNewGeomColl, IGeometryBag)

            'Définir la référence spatiale du resultat
            fpIntersectPolyline.SpatialReference = pPolyline.SpatialReference

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pRelOp = Nothing
            pTopoOp = Nothing
            pGeomColl = Nothing
            pPointColl = Nothing
            pNewGeomColl = Nothing
            pNewMultiPoint = Nothing
            pNewPolyline = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner les intersections points, lignes et surfaces entre une géométrie
    ''' Polygon et une autre géométrie. Le GeometryBag résultant contient une géométrie MultiPoint pour
    ''' les intersections de points, une géométrie Polyline pour les intersections lignes et une géométrie
    ''' Polygon pours les intersections surfaces. La géométrie Bag résultante contient toujours une
    ''' géométrie MultiPoint, Polyline et Polygon même s'il n'y aucune intersection, le nombre de
    ''' composante sera égale à zéro.
    '''</summary>
    '''
    '''<param name=" pPolygon "> Interface ESRI de la Géométrie Polygon de base.</param>
    '''<param name=" pGeometry "> Interface ESRI de la géométrie à comparer.</param>
    '''
    '''<returns> La fonction va retourner un "IGeometryBag". Si le traitement n'a pas réussi le "IGeometryBag" est "Nothing".</returns>
    ''' 
    Public Function fpIntersectPolygon(ByVal pPolygon As IPolygon, ByVal pGeometry As IGeometry) As IGeometryBag
        'Déclarer les variables de travail
        Dim pRelOp As IRelationalOperator = Nothing       'Interface ESRI pour vérifier la relation entre 2 géométries.
        Dim pTopoOp As ITopologicalOperator2 = Nothing    'Interface ESRI pour les intersections entre 2 géométries.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes du Bag.
        Dim pPointColl As IPointCollection = Nothing      'Interface ESRI utilisée pour ajouter des points.
        Dim pNewGeomColl As IGeometryCollection = Nothing 'Interface ESRI utilisée pour le nouveau Bag d'intersections.
        Dim pNewMultiPoint As IMultipoint = Nothing       'Interface ESRI contenant les intersections Points.
        Dim pNewPolyline As IPolyline = Nothing           'Interface ESRI contenant les intersectrions Lignes.
        Dim pNewPolygon As IPolygon = Nothing             'Interface ESRI contenant les intersectrions Surfaces.

        'Définir la valeur de retour par défaut
        fpIntersectPolygon = Nothing

        Try
            'Créer les géométries d'intersections vides
            pNewMultiPoint = CType(New Multipoint, IMultipoint)
            pNewPolyline = CType(New Polyline, IPolyline)
            pNewPolygon = CType(New Polygon, IPolygon)

            'Définir la référence spatiale
            pNewMultiPoint.SpatialReference = pPolygon.SpatialReference
            pNewPolyline.SpatialReference = pPolygon.SpatialReference
            pNewPolygon.SpatialReference = pPolygon.SpatialReference

            'Interface pour vérifier la présence d'une intersection
            pRelOp = CType(pGeometry, IRelationalOperator)

            'Vérifier si les deux géométries se touches
            '    If Not pRelOp.Disjoint(pPolygon) Then

            'Vérifier si la géométrie à comparer est de type point
            If pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                'Interface pour ajouter des nouveaux points
                pPointColl = CType(pNewMultiPoint, IPointCollection)

                'Ajouter l'intersection des points
                pPointColl.AddPoint(CType(pGeometry, IPoint))

                'Vérifier si la géométrie à comparer est un GeometryBag
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryBag Then
                'Interface pour traiter toutes les géométries
                pGeomColl = CType(pGeometry, IGeometryCollection)

                'Traiter toutes les géométries
                For i = 0 To pGeomColl.GeometryCount - 1
                    'Trouver l'intersection entre la ligne et une composante du GeometryBag
                    pNewGeomColl = CType(fpIntersectPolygon(pPolygon, CType(pGeomColl.Geometry(i), IGeometry)), IGeometryCollection)

                    'Vérifier si la géométrie est vide
                    If pNewMultiPoint.IsEmpty Then
                        'Insérer les intersections points
                        pNewMultiPoint = CType(pNewGeomColl.Geometry(0), IMultipoint)
                    Else
                        'Simplifier les points d'intersections
                        pNewMultiPoint = CType(fpSimplifierGeometrie(CType(pNewMultiPoint, IGeometry)), IMultipoint)

                        'Insérer les intersections points
                        pTopoOp = CType(pNewGeomColl.Geometry(0), ITopologicalOperator2)
                        pTopoOp.Simplify()
                        pNewMultiPoint = CType(pTopoOp.Union(pNewMultiPoint), IMultipoint)
                    End If

                    'Vérifier si la géométrie est vide
                    If pNewPolyline.IsEmpty Then
                        'Insérer les intersections lignes
                        pNewPolyline = CType(pNewGeomColl.Geometry(1), IPolyline)
                    Else
                        'Simplifier les lignes d'intersections
                        pNewPolyline = CType(fpSimplifierGeometrie(CType(pNewPolyline, IGeometry)), IPolyline)

                        'Insérer les intersections lignes
                        pTopoOp = CType(pNewGeomColl.Geometry(1), ITopologicalOperator2)
                        pTopoOp.Simplify()
                        pNewPolyline = CType(pTopoOp.Union(pNewPolyline), IPolyline)
                    End If

                    'Vérifier si la géométrie est vide
                    If pNewPolygon.IsEmpty Then
                        'Insérer les intersections surfaces
                        pNewPolygon = CType(pNewGeomColl.Geometry(2), IPolygon)
                    Else
                        'Simplifier les lignes d'intersections
                        pNewPolygon = CType(fpSimplifierGeometrie(CType(pNewPolygon, IGeometry)), IPolygon)

                        'Insérer les intersections surfaces
                        pTopoOp = CType(pNewGeomColl.Geometry(2), ITopologicalOperator2)
                        pTopoOp.Simplify()
                        pNewPolygon = CType(pTopoOp.Union(pNewPolygon), IPolygon)
                    End If
                Next i

                'Vérifier si les géométries sont de mêmes types
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryPolygon Then
                'Interface pour calculer l'intersection entre les géométrie
                pTopoOp = CType(pPolygon, ITopologicalOperator2)

                'Extraire l'intersection des points
                pNewMultiPoint = CType(pTopoOp.Intersect(pGeometry, esriGeometryDimension.esriGeometry0Dimension), IMultipoint)

                'Extraire l'intersection des lignes
                pNewPolyline = CType(pTopoOp.Intersect(pGeometry, esriGeometryDimension.esriGeometry1Dimension), IPolyline)

                'Extraire l'intersection des lignes
                pNewPolygon = CType(pTopoOp.Intersect(pGeometry, esriGeometryDimension.esriGeometry2Dimension), IPolygon)

                'Vérifier si le polygone est vide
                If Not pNewPolygon.IsEmpty Then
                    'Arrondir les coordonnées
                    pNewMultiPoint.SnapToSpatialReference()
                    pNewPolygon.SnapToSpatialReference()

                    'Interface pour détruire les points communs avec les polygones d'intersections
                    pTopoOp = CType(pNewMultiPoint, ITopologicalOperator2)

                    'Trouver les points commun entre les points et les polygones
                    pPointColl = CType(pTopoOp.Intersect(pNewPolygon, esriGeometryDimension.esriGeometry0Dimension), IPointCollection)

                    'Détruire les les points communs avec les polygones d'intersections
                    pNewMultiPoint = CType(pTopoOp.Difference(CType(pPointColl, IGeometry)), IMultipoint)
                End If

                'Traiter les autres types de géométries
            Else
                'Interface pour calculer l'intersection entre les géométrie
                pTopoOp = CType(pPolygon, ITopologicalOperator2)

                'Extraire l'intersection multidimension
                pGeomColl = CType(pTopoOp.IntersectMultidimension(pGeometry), IGeometryCollection)

                'Trouver l'intersection des lignes
                For i = 0 To pGeomColl.GeometryCount - 1
                    'Vérifier si la géométrie correspond à l'intersection des points
                    If pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPoint Then
                        'Interface pour ajouter un point
                        pPointColl = CType(pNewMultiPoint, IPointCollection)
                        'Ajouter un point au MultiPoint
                        pPointColl.AddPoint(CType(pGeomColl.Geometry(i), IPoint))

                        'Vérifier si la géométrie correspond à l'intersection des multipoint
                    ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryMultipoint Then
                        'Définir l'intersection des points
                        pNewMultiPoint = CType(pGeomColl.Geometry(i), IMultipoint)

                        'Vérifier si la géométrie correspond à l'intersection des lignes
                    ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPolyline Then
                        'Définir l'intersection des lignes
                        pNewPolyline = CType(pGeomColl.Geometry(i), IPolyline)

                        'Vérifier si la géométrie correspond à l'intersection des surfaces
                    ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPolygon Then
                        'Définir l'intersection des surafces
                        pNewPolygon = CType(pGeomColl.Geometry(i), IPolygon)
                    End If
                Next i
            End If
            '    End If

            'Créer un nouveau GeometryBag vide
            pNewGeomColl = CType(New GeometryBag, IGeometryCollection)

            'Insérer les intersections points
            pNewGeomColl.AddGeometry(pNewMultiPoint)

            'Insérer les intersections lignes
            pNewGeomColl.AddGeometry(pNewPolyline)

            'Insérer les intersections surfaces
            pNewGeomColl.AddGeometry(pNewPolygon)

            'Retourner le resultat
            fpIntersectPolygon = CType(pNewGeomColl, IGeometryBag)

            'Définir la référence spatiale du resultat
            fpIntersectPolygon.SpatialReference = pPolygon.SpatialReference

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pRelOp = Nothing
            pTopoOp = Nothing
            pGeomColl = Nothing
            pPointColl = Nothing
            pNewGeomColl = Nothing
            pNewMultiPoint = Nothing
            pNewPolyline = Nothing
            pNewPolygon = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner les intersections points, lignes et surfaces entre une géométrie
    ''' Bag et une autre géométrie. Le GeometryBag résultant contient une géométrie MultiPoint pour les
    ''' intersections de points, une géométrie Polyline pour les intersections lignes et une géométrie
    ''' Polygon pour les intersections surfaces. La géométrie Bag résultante contient toujours une
    ''' géométrie MultiPoint, Polyline et Polygon même s'il n'y aucune intersection, le nombre de
    ''' composante sera égale à zéro.
    '''</summary>
    '''
    '''<param name=" pGeometryBag "> Interface ESRI de la Géométrie Bag de base.</param>
    '''<param name=" pGeometry "> Interface ESRI de la géométrie à comparer.</param>
    '''
    '''<returns> La fonction va retourner un "IGeometryBag". Si le traitement n'a pas réussi le "IGeometryBag" est "Nothing".</returns>
    ''' 
    Public Function fpIntersectBag(ByVal pGeometryBag As IGeometryBag, ByVal pGeometry As IGeometry) As IGeometryBag
        'Déclarer les variables de travail
        Dim pRelOp As IRelationalOperator = Nothing       'Interface ESRI pour vérifier la relation entre 2 géométries.
        Dim pTopoOp As ITopologicalOperator2 = Nothing    'Interface ESRI pour les intersections entre 2 géométries.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes du Bag.
        Dim pPointColl As IPointCollection = Nothing      'Interface ESRI utilisée pour ajouter des points.
        Dim pNewGeomColl As IGeometryCollection = Nothing 'Interface ESRI utilisée pour le nouveau Bag d'intersections.
        Dim pNewMultiPoint As IMultipoint = Nothing       'Interface ESRI contenant les intersections Points.
        Dim pNewPolyline As IPolyline = Nothing           'Interface ESRI contenant les intersectrions Lignes.
        Dim pNewPolygon As IPolygon = Nothing             'Interface ESRI contenant les intersectrions Surfaces.

        'Définir la valeur de retour par défaut
        fpIntersectBag = Nothing

        Try
            'Créer les géométries d'intersections vides
            pNewMultiPoint = CType(New Multipoint, IMultipoint)
            pNewPolyline = CType(New Polyline, IPolyline)
            pNewPolygon = CType(New Polygon, IPolygon)

            'Définir la référence spatiale
            pNewMultiPoint.SpatialReference = pGeometryBag.SpatialReference
            pNewPolyline.SpatialReference = pGeometryBag.SpatialReference
            pNewPolygon.SpatialReference = pGeometryBag.SpatialReference

            'Interface pour traiter toutes les géométries du GeometryBag
            pGeomColl = CType(pGeometryBag, IGeometryCollection)

            'Traiter toutes les composantes du GeometryBag
            For i = 0 To pGeomColl.GeometryCount - 1
                'Vérifier si la géométrie est un Point
                If pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPoint Then
                    'Interface pour vérifier l'intersection
                    pRelOp = CType(pGeometry, IRelationalOperator)

                    'Vérifier si le point intersecte
                    If Not pRelOp.Disjoint(pGeomColl.Geometry(i)) Then
                        'Ajouter les intersections de points
                        pPointColl = CType(pNewMultiPoint, IPointCollection)
                        pPointColl.AddPoint(CType(pGeomColl.Geometry(i), IPoint))

                        'Simplifier la géométrie
                        pTopoOp = CType(pNewMultiPoint, ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()
                    End If

                    'Vérifier si la géométrie est une ligne
                ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryMultipoint Then
                    'Extraire les intersections avec le MultiPoint
                    pNewGeomColl = CType(fpIntersectMultiPoint(CType(pGeomColl.Geometry(i), IMultipoint), pGeometry), IGeometryCollection)

                    'Vérifier si la géométrie est vide
                    If pNewMultiPoint.IsEmpty Then
                        'Ajouter les intersections de points
                        pNewMultiPoint = CType(pNewGeomColl.Geometry(0), IMultipoint)
                    Else
                        'Simplifier les points d'intersections
                        pNewMultiPoint = CType(fpSimplifierGeometrie(CType(pNewMultiPoint, IGeometry)), IMultipoint)

                        'Ajouter les intersections de points
                        pTopoOp = CType(pNewGeomColl.Geometry(0), ITopologicalOperator2)
                        pTopoOp.Simplify()
                        pNewMultiPoint = CType(pTopoOp.Union(pNewMultiPoint), IMultipoint)
                    End If

                    'Vérifier si la géométrie est une ligne
                ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPolyline Then
                    'Extraire les intersections avec la Polyline
                    pNewGeomColl = CType(fpIntersectPolyline(CType(pGeomColl.Geometry(i), IPolyline), pGeometry), IGeometryCollection)

                    'Vérifier si la géométrie est vide
                    If pNewMultiPoint.IsEmpty Then
                        'Ajouter les intersections de points
                        pNewMultiPoint = CType(pNewGeomColl.Geometry(0), IMultipoint)
                    Else
                        'Simplifier les points d'intersections
                        pNewMultiPoint = CType(fpSimplifierGeometrie(CType(pNewMultiPoint, IGeometry)), IMultipoint)

                        'Ajouter les intersections de points
                        pTopoOp = CType(pNewGeomColl.Geometry(0), ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()
                        pNewMultiPoint = CType(pTopoOp.Union(pNewMultiPoint), IMultipoint)
                    End If

                    'Vérifier si la géométrie est vide
                    If pNewPolyline.IsEmpty Then
                        'Ajouter les intersections de lignes
                        pNewPolyline = CType(pNewGeomColl.Geometry(1), IPolyline)
                    Else
                        'Simplifier les lignes d'intersections
                        pNewPolyline = CType(fpSimplifierGeometrie(CType(pNewPolyline, IGeometry)), IPolyline)

                        'Ajouter les intersections de lignes
                        pTopoOp = CType(pNewGeomColl.Geometry(1), ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()
                        pNewPolyline = CType(pTopoOp.Union(pNewPolyline), IPolyline)
                    End If

                    'Vérifier si la géométrie est une surface
                ElseIf pGeomColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPolygon Then
                    'Extraire les intersections avec le polygon
                    pNewGeomColl = CType(fpIntersectPolygon(CType(pGeomColl.Geometry(i), IPolygon), pGeometry), IGeometryCollection)

                    'Vérifier si la géométrie est vide
                    If pNewMultiPoint.IsEmpty Then
                        'Ajouter les intersections de points
                        pNewMultiPoint = CType(pNewGeomColl.Geometry(0), IMultipoint)
                    Else
                        'Simplifier les points d'intersections
                        pNewMultiPoint = CType(fpSimplifierGeometrie(CType(pNewMultiPoint, IGeometry)), IMultipoint)

                        'Ajouter les intersections de points
                        pTopoOp = CType(pNewGeomColl.Geometry(0), ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()
                        pNewMultiPoint = CType(pTopoOp.Union(pNewMultiPoint), IMultipoint)
                    End If

                    'Vérifier si la géométrie est vide
                    If pNewPolyline.IsEmpty Then
                        'Ajouter les intersections de lignes
                        pNewPolyline = CType(pNewGeomColl.Geometry(1), IPolyline)
                    Else
                        'Simplifier les lignes d'intersections
                        pNewPolyline = CType(fpSimplifierGeometrie(CType(pNewPolyline, IGeometry)), IPolyline)

                        'Ajouter les intersections de lignes
                        pTopoOp = CType(pNewGeomColl.Geometry(1), ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()
                        pNewPolyline = CType(pTopoOp.Union(pNewPolyline), IPolyline)
                    End If

                    'Vérifier si la géométrie est vide
                    If pNewPolygon.IsEmpty Then
                        'Ajouter les intersections de surfaces
                        pNewPolygon = CType(pNewGeomColl.Geometry(2), IPolygon)
                    Else
                        'Simplifier les lignes d'intersections
                        pNewPolygon = CType(fpSimplifierGeometrie(CType(pNewPolygon, IGeometry)), IPolygon)

                        'Ajouter les intersections de surfaces
                        pTopoOp = CType(pNewGeomColl.Geometry(2), ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()
                        pNewPolygon = CType(pTopoOp.Union(pNewPolygon), IPolygon)
                    End If
                End If
            Next i

            'Créer un nouveau GeometryBag vide
            pNewGeomColl = CType(New GeometryBag, IGeometryCollection)

            'Insérer les intersections points
            pNewGeomColl.AddGeometry(pNewMultiPoint)

            'Insérer les intersections lignes
            pNewGeomColl.AddGeometry(pNewPolyline)

            'Insérer les intersections surfaces
            pNewGeomColl.AddGeometry(pNewPolygon)

            'Retourner le resultat
            fpIntersectBag = CType(pNewGeomColl, IGeometryBag)

            'Définir la référence spatiale du resultat
            fpIntersectBag.SpatialReference = pGeometryBag.SpatialReference

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pRelOp = Nothing
            pTopoOp = Nothing
            pGeomColl = Nothing
            pPointColl = Nothing
            pNewGeomColl = Nothing
            pNewMultiPoint = Nothing
            pNewPolyline = Nothing
            pNewPolygon = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de changer les géométries surfaces inférieures à la superficie minimale spécifiée en géométries
    ''' points. Les surfaces données en paramètres qui sont inférieures à la superficie minimale seront enlevées de cette
    ''' géométrie, elles seront transformées en points et retournées via le PointCollection.
    '''</summary>
    '''
    '''<param name=" pPolygon "> Interface ESRI contenant des surfaces.</param>
    '''<param name=" dSuperficie "> Superficie minimale des surfaces à respecter.</param>
    '''
    '''<returns> La fonction va retourner un "IPointCollection". Si le traitement n'a pas réussi le "IPointCollection" est "Nothing".</returns>
    ''' 
    Public Function fpChangerSurfaceEnPointSelonSuperficie(ByVal pPolygon As IPolygon2, ByVal dSuperficie As Double) As IPointCollection
        'Déclarer les variables de travail
        Dim pGeometry As IGeometry = Nothing             'Interface ESRI utiliséé pour changer la référence spatiale.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes du Bag.
        Dim pNewPolygon As IGeometryCollection = Nothing  'Interface ESRI contenant les intersections Points.
        Dim pArea As IArea = Nothing                      'Interface ESRI contenant des lignes.
        Dim pNewMultiPoint As IPointCollection = Nothing  'Interface ESRI utilisée pour vérifier la superficie.

        'Définir la valeur de retour par défaut
        fpChangerSurfaceEnPointSelonSuperficie = Nothing

        Try
            'Initialiser les variables de travail
            pNewPolygon = CType(New Polygon, IGeometryCollection)
            pNewMultiPoint = CType(New Multipoint, IPointCollection)

            'Définir la référence spatiale
            pGeometry = CType(pNewPolygon, IGeometry)
            pGeometry.SpatialReference = pPolygon.SpatialReference
            pGeometry = CType(pNewMultiPoint, IGeometry)
            pGeometry.SpatialReference = pPolygon.SpatialReference

            'Interface pour traiter les composantes de la surface
            pGeomColl = CType(pPolygon, IGeometryCollection)

            'Traiter toutes les composantes du polygon
            For i = 0 To pGeomColl.GeometryCount - 1
                'Interface pour vérifier la longueur
                pArea = CType(pGeomColl.Geometry(i), IArea)

                'Vérifier la longueur d'une composantes
                If pArea.Area < dSuperficie Then
                    'Ajouter le point dans la géométrie de points
                    pNewMultiPoint.AddPoint(pArea.LabelPoint)
                Else
                    'Conserver la composante dans le polygon
                    pNewPolygon.AddGeometry(CType(pArea, IGeometry))
                End If
            Next i

            'Retourner le nouveau polygon
            pPolygon = CType(pNewPolygon, IPolygon2)

            'Retourner le résultat du traitement
            fpChangerSurfaceEnPointSelonSuperficie = pNewMultiPoint

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pGeometry = Nothing
            pGeomColl = Nothing
            pNewPolygon = Nothing
            pArea = Nothing
            pNewMultiPoint = Nothing
            pNewPolygon = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de changer les géométries lignes inférieures à la longueur minimale spécifiée
    ''' en géométries points. Les lignes données en paramètres qui sont inférieures à la longueur
    ''' minimale seront enlevées de cette géométrie, elles seront transformées en points et retournées
    ''' via le PointCollection.
    '''</summary>
    '''
    '''<param name=" pPolyline "> Interface ESRI contenant des lignes.</param>
    '''<param name=" dLongueur "> Longueur minimale des lignes à respecter.</param>
    '''<param name=" nNbSommet "> Nombre de sommets minimale des lignes à respecter.</param>
    '''
    '''<returns> La fonction va retourner un "IPointCollection". Si le traitement n'a pas réussi le "IPointCollection" est "Nothing".</returns>
    ''' 
    Public Function fpChangerLigneEnPointSelonLongueur(ByVal pPolyline As IPolyline2, ByVal dLongueur As Double, Optional ByVal nNbSommet As Long = 0) As IPointCollection
        'Déclarer les variables de travail
        Dim pGeometry As IGeometry = Nothing              'Interface ESRI utiliséé pour changer la référence spatiale.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes du Bag.
        Dim pNewPolyline As IGeometryCollection = Nothing 'Interface ESRI contenant les intersections Points.
        Dim pPath As IPath = Nothing                      'Interface ESRI contenant des lignes.
        Dim pPointColl As IPointCollection = Nothing      'Interface pour vérifier le nombre de sommets présents.
        Dim pNewMultiPoint As IPointCollection = Nothing  'Interface ESRI contenant une composante de ligne.
        Dim pPoint As IPoint = Nothing                    'Interface ESRI contenant une géométrie point.

        'Définir la valeur de retour par défaut
        fpChangerLigneEnPointSelonLongueur = Nothing

        Try
            'Initialiser les variables de travail
            If nNbSommet = 0 Then nNbSommet = 2147000000
            pNewPolyline = CType(New Polyline, IGeometryCollection)
            pNewMultiPoint = CType(New Multipoint, IPointCollection)

            'Définir la référence spatiale
            pGeometry = CType(pNewPolyline, IGeometry)
            pGeometry.SpatialReference = pPolyline.SpatialReference
            pGeometry = CType(pNewMultiPoint, IGeometry)
            pGeometry.SpatialReference = pPolyline.SpatialReference

            'Interface pour traiter les composantes de la ligne
            pGeomColl = CType(pPolyline, IGeometryCollection)

            'Traiter toutes les composantes de la polyline
            For i = 0 To pGeomColl.GeometryCount - 1
                'Interface pour vérifier la longueur
                pPath = CType(pGeomColl.Geometry(i), IPath)

                'Interface pour vérifier le nombre de sommet
                pPointColl = CType(pGeomColl.Geometry(i), IPointCollection)

                'Vérifier la longueur d'une composantes
                If pPath.Length < dLongueur And pPointColl.PointCount < nNbSommet Then
                    'Créer un nouveau point vide
                    pPoint = New ESRI.ArcGIS.Geometry.Point

                    'Extraire le point au centre de la composante
                    pPath.QueryPoint(esriSegmentExtension.esriNoExtension, pPath.Length / 2, False, pPoint)

                    'Ajouter le point dans la géométrie de points
                    pNewMultiPoint.AddPoint(pPoint)
                Else
                    'Conserver la composante dans la Polyline
                    pNewPolyline.AddGeometry(pPath)
                End If
            Next i

            'Retourner la nouvelle polyline
            pPolyline = CType(pNewPolyline, IPolyline2)

            'Retourner le résultat du traitement
            fpChangerLigneEnPointSelonLongueur = pNewMultiPoint

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pGeometry = Nothing
            pGeomColl = Nothing
            pNewPolyline = Nothing
            pPath = Nothing
            pNewMultiPoint = Nothing
            pPoint = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner l'intersection entre deux géométries selon une dimension.
    '''</summary>
    '''
    '''<param name=" pGeometryA "> Interface ESRI contenant la première géométrie.</param>
    '''<param name=" pGeometryB "> Interface ESRI contenant la deuxième géométrie.</param>
    '''<param name=" pIntersectGeometry "> Interface ESRI contenant la géométrie résultante.</param>
    '''<param name=" ResultDimension "> Indique la dimension du résultat désiré (0, 1 ou 2 dimension).</param>
    '''
    '''<returns> La fonction va retourner un "Boolean". Si le traitement n'a pas réussi le "Boolean" est "false".</returns>
    ''' 
    Public Function fbIntersectGeometrie(ByVal pGeometryA As IGeometry, ByVal pGeometryB As IGeometry, _
    ByRef pIntersectGeometry As IGeometry, ByVal ResultDimension As esriGeometryDimension) As Boolean
        'Déclarer les variables de travail
        Dim pClone As IClone = Nothing                  'Interface ESRI utilisée pour cloner une géométrie
        Dim pTopoOp As ITopologicalOperator2 = Nothing  'Interface ESRI utilisée pour traiter la topologie des géométries
        Dim pProxOp As IProximityOperator = Nothing     'Interface ESRI utilisée pour calculer la proximité entre deux géométries

        Try
            'Arrondir les corrdonnées selon la référence spatiale
            pGeometryA.SnapToSpatialReference()
            pGeometryB.SnapToSpatialReference()

            'Vérifier si la géométrie A est de plus petite dimension
            If pGeometryA.Dimension <= pGeometryB.Dimension Then
                'Interface pour clone la géométrie
                pClone = CType(pGeometryA, IClone)
            Else
                'Interface pour clone la géométrie
                pClone = CType(pGeometryB, IClone)
            End If

            'Cloner la géométrie
            pIntersectGeometry = CType(pClone.Clone, IGeometry)

            'Interface pour calculer la distance
            pProxOp = CType(pGeometryA, IProximityOperator)

            'Vérifier si la géométrie n'est pas de type point
            If pGeometryA.GeometryType = esriGeometryType.esriGeometryPoint Or pGeometryB.GeometryType = esriGeometryType.esriGeometryPoint Then
                'Vérifier si il y a intersection
                If Not (pProxOp.ReturnDistance(pGeometryB) = 0 And ResultDimension = esriGeometryDimension.esriGeometry0Dimension) Then
                    'Vider la géométrie
                    pIntersectGeometry.SetEmpty()
                End If

                'Vérifier si les géométries ne sont pas de type point
            ElseIf pProxOp.ReturnDistance(pGeometryB) = 0 Then
                'Simplifier la géométrie B
                pTopoOp = CType(pGeometryB, ITopologicalOperator2)
                pTopoOp.IsKnownSimple_2 = False
                pTopoOp.Simplify()

                'Simplifier la géométrie A
                pTopoOp = CType(pGeometryA, ITopologicalOperator2)
                pTopoOp.IsKnownSimple_2 = False
                pTopoOp.Simplify()

                'Union de la géométrie B dans A
                pIntersectGeometry = CType(pTopoOp.Intersect(pGeometryB, ResultDimension), IGeometry)
            End If

            'Retourner le résultat
            fbIntersectGeometrie = True

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pTopoOp = Nothing
            pProxOp = Nothing
            pClone = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de simplifer une géométrie afin de la rendre non simple. On enlève tous
    ''' les croisements et les superpositions de géométries.
    '''</summary>
    '''
    '''<param name=" pGeometry "> Interface ESRI contenant une géométrie à simplifier.</param>
    '''
    '''<returns> La fonction va retourner un "IGeometry". Si le traitement n'a pas réussi le "IGeometry" est "Nothing".</returns>
    ''' 
    Public Function fpSimplifierGeometrie(ByVal pGeometry As IGeometry) As IGeometry
        'Déclarer les variables de travail
        Dim pTopoOp As ITopologicalOperator2 = Nothing    'Interface ESRI utilisée pour simplifier la géométrie.
        Dim pNewGeometry As IGeometry = Nothing           'Interface ESRI contenant une géométrie simplifiée.
        Dim pGeomColl As IGeometryCollection = Nothing    'Interface ESRI utilisée pour les composantes du vieux Bag.
        Dim pNewGeomColl As IGeometryCollection = Nothing 'Interface ESRI utilisée pour les composantes du nouveau Bag.

        'Définir la valeur de retour par défaut
        fpSimplifierGeometrie = Nothing

        Try
            'Vérifier si c'est un Point
            If pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                pNewGeometry = pGeometry

                'Vérifier si c'est un GeometryBag
            ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryBag Then
                'Créer un nouveau GeometryBag
                pNewGeomColl = CType(New GeometryBag, IGeometryCollection)

                'Interface pour traiter toutes les géométries
                pGeomColl = CType(pGeometry, IGeometryCollection)

                'Traiter toutes les géométries du GeometryBag
                For i = 0 To pGeomColl.GeometryCount - 1
                    'Simplifier un géométrie
                    pNewGeometry = fpSimplifierGeometrie(pGeomColl.Geometry(i))

                    'Ajouter la géométrie simplifier dans le nouveau GeometryBag
                    pNewGeomColl.AddGeometry(pNewGeometry)
                Next i

                'Définir le nouveau GeometryBag
                pNewGeometry = CType(pNewGeomColl, IGeometry)

                'Définir la référence spatiale
                pNewGeometry.SpatialReference = pGeometry.SpatialReference

                'Vérifier si c'est un autre type de géométrie
            Else
                'Interface pour simplifier la géométrie
                pTopoOp = CType(pGeometry, ITopologicalOperator2)

                'On ne connait pas l'état de la géométrie
                pTopoOp.IsKnownSimple_2 = False

                'Rendre la géométrie simple
                pTopoOp.Simplify()

                'Définir la nouvelle géométrie
                pNewGeometry = CType(pTopoOp, IGeometry)
            End If

            'Retourner la géométrie simplifiée
            fpSimplifierGeometrie = pNewGeometry

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pNewGeometry = Nothing
            pTopoOp = Nothing
            pGeomColl = Nothing
            pNewGeomColl = Nothing
        End Try

    End Function

    '''<summary>
    ''' Routine qui permet de remplacer la géométrie d'un élément sélectionné selon la géométrie de
    ''' la géométrie de travail. Le mode édition doit être actif. la géométrie de travait courant doit être actif. 
    '''</summary>
    '''  
    Public Sub RemplacerGeometrie()
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                      'Interface ESRI contenant les Layers de la vue active.
        Dim pEnumFeature As IEnumFeature = Nothing      'Interface ESRI pour traiter les éléments sélectionnés.
        Dim pFeatureClass As IFeatureClass = Nothing    'Interface ESRI contenant la classe de l'élément à créer.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant l'élément traité.
        Dim pGeometry As IGeometry = Nothing            'Interface ESRI contenant la géométrie à remplacer.
        Dim pEditor As IEditor = Nothing                'Interface ESRI utilisée pour effectuer l'édition.
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.
        Dim pMsgBoxResult As MsgBoxResult = Nothing     'Contient le résultat d'une question.
        Dim bRemplacer As Boolean = False               'Indique si la géométrie a été remplacée.

        Try
            'Interface pour vérifer si on est en mode édition
            pEditor = CType(m_Application.FindExtensionByName("ESRI Object Editor"), IEditor)

            'Vérifier si on est en mode édition
            If pEditor.EditState = esriEditState.esriStateNotEditing Then
                'Message d'erreur
                MsgBox("ATTENTION : Vous devez être en mode édition")
                pEditor = Nothing
                Exit Try
            End If

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vérifier la présece d'un seul élément
            If pMap.SelectionCount <> 1 Then
                MsgBox("ATTENTION : Vous devez sélectionner un seul élément")
                Exit Sub
            End If

            'Débuter l'opération UnDo
            pEditor.StartOperation()

            'Trouver le premier élément
            pEnumFeature = CType(pEditor.EditSelection, IEnumFeature)
            pFeature = pEnumFeature.Next

            'Trouver les autres éléments
            Do Until pFeature Is Nothing
                'Traiter toutes les géométries
                For i = 0 To mpGeometrieTravail.GeometryCount - 1
                    'Définir la géométry de la géométrie de travail active
                    pGeometry = mpGeometrieTravail.Geometry(i)

                    'Définir la classe de l'élément
                    pFeatureClass = CType(pFeature.Class, IFeatureClass)

                    'Vérifier si la géométrie de l'élément est de même type
                    If pFeatureClass.ShapeType = pGeometry.GeometryType Then
                        'Mettre la géométrie de la géométrie de travail selon la référence de l'élément
                        pGeometry.Project(pFeature.Shape.SpatialReference)

                        'Interface pour vérifier la présence du Z et du M
                        pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                        'Vérifier la présence du Z
                        If pGeometryDef.HasZ Then
                            'Traiter le Z
                            Call TraiterZ(pGeometry)
                        End If
                        'Vérifier la présence du M
                        If pGeometryDef.HasM Then
                            'Traiter le M
                            Call TraiterM(pGeometry)
                        End If

                        'Vérifier si la géométrie est vide
                        If pGeometry.IsEmpty Then
                            'Message d'erreur
                            pMsgBoxResult = MsgBox("ATTENTION : La géométrie est vide, voulez-vous modifier l'élément quand même ? ", MsgBoxStyle.YesNo, "Remplacer la géométrie de l'élément")

                            'Si la géométrie n'est pas vide
                        Else
                            'Répondre oui à la question
                            pMsgBoxResult = MsgBoxResult.Yes
                        End If

                        'Vérifier si on doit créer quand même
                        If pMsgBoxResult = MsgBoxResult.Yes Then
                            'Changer la géométrie de l'élément sélectionné
                            pFeature.Shape = pGeometry

                            'Sauver le traitement
                            pFeature.Store()

                            'Indique que la géométrie a été remplacée
                            bRemplacer = True
                        End If

                        'Sortir de la boucle
                        Exit For
                    End If
                Next

                'Trouver le prochain élément
                pFeature = pEnumFeature.Next
            Loop

            'Vérifier si la géométrie a été remplacée
            If bRemplacer Then
                'Terminer l'opération UnDo
                pEditor.StopOperation("Remplacer la géométrie selon les géométries de travail")

                'Permet d'indiquer que l'édition s'est bien terminée.
                pEditor = Nothing

                'Rafraîchir l'affichage
                m_MxDocument.ActiveView.Refresh()
            Else
                'Message d'erreur
                MsgBox("ATTENTION : La géométrie n'a pas été remplacée")
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vérifier si une erreur est survenue et qu'on est en mode Undo
            If Not pEditor Is Nothing Then
                'Annuler l'opération UnDo
                pEditor.AbortOperation()
            End If
            'Vider la mémoire
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometry = Nothing
            pEditor = Nothing
            pFeatureClass = Nothing
            pGeometryDef = Nothing
            pMsgBoxResult = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet de créer un élément selon la géométrie de la géométrie de travail et selon le Layer de construction.
    ''' Le mode édition doit être actif. la géométrie de travait courant doit être actif. 
    '''</summary>
    '''
    Public Sub CreerElement()
        'Déclarer les variables de travail
        Dim pEditLayers As IEditLayers = Nothing        'Interface ESRI utilisée pour extraire le Layer de construction.
        Dim pFeatureLayer As IFeatureLayer = Nothing    'Interface ESRI contenant l'affichage de la classe de l'élément à créer.
        Dim pFeatureClass As IFeatureClass = Nothing    'Interface ESRI contenant la classe de l'élément à créer.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant l'élément à créer.
        Dim pGeometry As IGeometry = Nothing            'Interface ESRI contenant la géométrie de l'élément à créer.
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.
        Dim pPointColl As IPointCollection = Nothing    'Interface qui permet d'extraire un sommet
        Dim pEditor As IEditor = Nothing                'Interface ESRI utilisée pour effectuer l'édition.
        Dim pGeodataset As IGeoDataset = Nothing        'Interface contenant la référence spatiale
        Dim pMsgBoxResult As MsgBoxResult = Nothing     'Contient le résultat d'une question.

        Try
            'Interface pour vérifer si on est en mode édition
            pEditor = CType(m_Application.FindExtensionByName("ESRI Object Editor"), IEditor)

            'Vérifier si on est en mode édition
            If pEditor.EditState = esriEditState.esriStateNotEditing Then
                'Message d'erreur
                MsgBox("ATTENTION : Vous devez être en mode édition")
                pEditor = Nothing
                Exit Try
            End If

            'Débuter l'opération UnDo
            pEditor.StartOperation()

            'Interface pour extraire le Layer de construction
            pEditLayers = CType(pEditor, IEditLayers)

            'Extraire le Layer de construction
            pFeatureLayer = pEditLayers.CurrentLayer

            'Vérifier si on est en mode édition
            If pFeatureLayer Is Nothing Then
                'Message d'erreur
                MsgBox("ATTENTION : Vous devez sélectionner un FeatureLayer de construction")
                pEditor = Nothing
                Exit Try
            End If

            'Définir la FeatureClass de Layer de construction
            pFeatureClass = pFeatureLayer.FeatureClass

            'Interface utilisé pour extraire la référence spatiale de la classe
            pGeodataset = CType(pFeatureClass, IGeoDataset)

            'Traiter toutes les géométries
            For i = 0 To mpGeometrieTravail.GeometryCount - 1
                'Définir la géométrie de l'élément
                pGeometry = mpGeometrieTravail.Geometry(i)

                'Vérifier si le type de géométrie est un Point versus Multipoint
                If pFeatureClass.ShapeType = esriGeometryType.esriGeometryPoint _
                And pGeometry.GeometryType = esriGeometryType.esriGeometryMultipoint Then
                    'Interface pour extraire un sommet
                    pPointColl = CType(pGeometry, IPointCollection)
                    'Vérifier si le multipoint possède seulement 1 sommet
                    If pPointColl.PointCount = 1 Then
                        'Définir le premier point
                        pGeometry = pPointColl.Point(0)
                    End If

                    'Vérifier si le type de géométrie est un Multipoint versus Point
                ElseIf pFeatureClass.ShapeType = esriGeometryType.esriGeometryMultipoint _
                And pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                    'Interface pour extraire un sommet
                    pPointColl = CType(New Multipoint, IPointCollection)
                    'Ajouter le point
                    pPointColl.AddPoint(CType(pGeometry, IPoint))
                    'Définir le premier point
                    pGeometry = CType(pPointColl, IGeometry)
                    'Définir la référence spatiale
                    pGeometry.SpatialReference = mpGeometrieTravail.Geometry(i).SpatialReference
                End If

                'Vérifier si la géométrie de la FeatureClass est de même type
                If pFeatureClass.ShapeType = pGeometry.GeometryType Then
                    'Créer un nouvel élément
                    pFeature = pFeatureClass.CreateFeature

                    'Mettre la géométrie de la géométrie de travail selon la référence de l'élément
                    pGeometry.Project(pGeodataset.SpatialReference)

                    'Interface pour vérifier la présence du Z et du M
                    pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                    'Vérifier la présence du Z
                    If pGeometryDef.HasZ Then
                        'Traiter le Z
                        Call TraiterZ(pGeometry)
                    End If
                    'Vérifier la présence du M
                    If pGeometryDef.HasM Then
                        'Traiter le M
                        Call TraiterM(pGeometry)
                    End If

                    'Vérifier si la géométrie est vide
                    If pGeometry.IsEmpty Then
                        'Poser et répondre à une question
                        pMsgBoxResult = MsgBox("ATTENTION : La géométrie est vide, voulez-vous créer l'élément quand même ? ", MsgBoxStyle.YesNo, "Créer un nouvel élément")

                        'Si la géométrie n'est pas vide
                    Else
                        'Répondre oui à la question
                        pMsgBoxResult = MsgBoxResult.Yes
                    End If

                    'Vérifier si on doit créer quand même
                    If pMsgBoxResult = MsgBoxResult.Yes Then
                        'Ajouter la géométrie dans l'élément
                        pFeature.Shape = pGeometry

                        'Ajouter les valeur d'attribut par défaut
                        For j = 0 To pFeature.Fields.FieldCount - 1
                            'Ajouter la valeur par défaut
                            Call fbModifierValeurAttributSelonDefaut(pFeature, j)
                        Next

                        'Sauver le traiter
                        pFeature.Store()
                    End If
                End If
            Next

            'Vérifier si une édition a été effectuée
            If pFeature IsNot Nothing Then
                'Terminer l'opération UnDo
                pEditor.StopOperation("Créer un nouvel élément selon les géométries de travail et le target")
                pEditor = Nothing
                'Rafraîchir l'affichage
                m_MxDocument.ActiveView.Refresh()

                'Si aucun élément n'a été créé
            ElseIf mpGeometrieTravail.GeometryCount > 0 Then
                'Message d'erreur
                MsgBox("ATTENTION : Le Layer de construction n'est pas du même type qu'une des géométries de travail")
            End If

        Catch erreur As Exception
            'Message d'erreur
            Throw erreur
        Finally
            'Vérifier si une erreur est survenue et qu'on est en mode Undo
            If Not pEditor Is Nothing Then
                'Annuler l'opération UnDo
                pEditor.AbortOperation()
            End If
            'Vider la mémoire
            pEditor = Nothing
            pEditLayers = Nothing
            pFeatureLayer = Nothing
            pFeatureClass = Nothing
            pFeature = Nothing
            pGeometry = Nothing
            pGeodataset = Nothing
            pGeometryDef = Nothing
            pMsgBoxResult = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Fonction qui permet de modifier la valeur d'attribut selon la valeur par défaut de la classe ou la
    ''' sous-classe. Si l'élément possède des sous-classes, la valeur par défaut associée à la sous-classe
    ''' sera utilisée mais si l'élément ne possède pas de sous-classe, alors ce sera la valeur par défaut
    ''' de classe qui sera utilisée.
    '''</summary>
    '''
    '''<param name=" pFeature "> Élément pour lequel on veut modifier un attribut.</param>
    '''<param name=" nIndex "> Position de l'attribut à modifier.</param>
    '''
    '''<returns> La fonction va retourner un "Boolean". Si le traitement n'a pas réussi le "Boolean" est à "False".</returns>
    ''' 
    Public Function fbModifierValeurAttributSelonDefaut(ByRef pFeature As IFeature, ByVal nIndex As Integer) As Boolean
        'Déclarer les variables de travail
        Dim pObjectClass As IObjectClass = Nothing    'Interface ESRI contenant une classe.
        Dim pFeatureClass As IFeatureClass = Nothing  'Interface ESRI contenant des éléments sur disque.
        Dim pSubtypes As ISubtypes = Nothing          'Interface ESRI contenant les subtypes (sous-classes).
        Dim pField As IField = Nothing                'Interface ESRI contenant un attribut d'un élément.
        Dim nIndexSousClasse As Integer = Nothing     'Contient la position de l'attribut de la sous-classe.

        Try
            'Définir les subtypes
            pObjectClass = pFeature.Class
            pFeatureClass = CType(pObjectClass, IFeatureClass)
            pSubtypes = CType(pFeatureClass, ISubtypes)

            'Définir l'attribut à corriger
            pField = pFeatureClass.Fields.Field(nIndex)

            'Vérifier si c'est un attribut réservé
            If pField.Editable And pField.Name <> pFeatureClass.ShapeFieldName Then
                'Ajouter la valeur d'attribut à l'élément selon la position de l'attribut
                If pSubtypes.HasSubtype Then
                    'Définir la position de l'attribut de sous-classe
                    nIndexSousClasse = pSubtypes.SubtypeFieldIndex

                    'Vérifier la présence de l'attribut de sous-classe
                    If nIndexSousClasse >= 0 Then
                        'Ajouter la valeur d'attribut par défaut selon la sous-classe
                        pFeature.Value(nIndex) = pSubtypes.DefaultValue(pSubtypes.DefaultSubtypeCode, pField.Name)
                        'Sinon
                    Else
                        'Ajouter la valeur d'attribut par défaut selon la classe
                        pFeature.Value(nIndex) = pField.DefaultValue
                    End If
                    'Sinon
                Else
                    pFeature.Value(nIndex) = pField.DefaultValue
                End If
            End If

            'Retourner le succès de la function
            fbModifierValeurAttributSelonDefaut = True

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pObjectClass = Nothing
            pFeatureClass = Nothing
            pSubtypes = Nothing
            pField = Nothing
        End Try
    End Function

    '''<summary>
    ''' Routine qui permet de modifier la géométrie d'un élément à partir des lignes contenues dans les
    ''' géométries de travail. La partie la plus grande (Gauche ou droite) sera conservée. 
    '''</summary>
    '''
    Public Sub Reshape()
        'Déclarer les variables de travail
        Dim pEditor As IEditor = Nothing            'Interface ESRI pour effectuer l'édition des éléments.
        Dim pMap As IMap = Nothing                  'Interface ESRI contenant la fenêtre de données active.
        Dim pEnumFeature As IEnumFeature = Nothing  'Interface ESRI pour traiter tous les éléments sélectionnés.
        Dim pFeature As IFeature = Nothing          'Interface ESRI contenant un élément.
        Dim pGeometry As IGeometry = Nothing        'Interface contenant la géométrie
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.
        Dim pTopoOp As ITopologicalOperator2 = Nothing   'Interface ESRI qui permet de changer la topologie.
        Dim pGeomCollA As IGeometryCollection = Nothing  'Interface ESRI pour manipuler les composantes d'une géométrie.
        Dim pGeomCollB As IGeometryCollection = Nothing  'Interface ESRI pour manipuler les composantes d'une géométrie.
        Dim pPolyline As IPolyline = Nothing        'Interface ESRI contenant une géométrie ligne.
        Dim pRing As IRing = Nothing                'Interface ESRI contenent une composante d'un polygon.
        Dim pPath As IPath = Nothing                'Interface ESRI contenent une composante d'une polyline.
        Dim bModif As Boolean = Nothing             'Indique si une modification a été effectuée.

        Try
            'Interface pour vérifer si on est en mode édition
            pEditor = CType(m_Application.FindExtensionByName("ESRI Object Editor"), IEditor)

            'Vérifier si on est en mode édition
            If pEditor.EditState = esriEditState.esriStateNotEditing Then
                'Message d'erreur
                MsgBox("ATTENTION : Vous devez être en mode édition")
                pEditor = Nothing
                Exit Try
            End If

            'Débuter l'opération UnDo
            pEditor.StartOperation()

            'Initialiser les variable de travail
            pRing = CType(New Ring, IRing)
            pPath = CType(New Path, IPath)

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Trouver le premier élément
            pEnumFeature = CType(pEditor.EditSelection, IEnumFeature)
            pFeature = pEnumFeature.Next

            'Trouver les autres éléments
            Do Until pFeature Is Nothing
                'Vérifier s'il s'agit d'un polygone
                If pFeature.Shape.GeometryType = esriGeometryType.esriGeometryPolygon Then
                    'Initialiser un élément A
                    pGeomCollA = CType(pFeature.ShapeCopy, IGeometryCollection)

                    'Rendre la géométrie valide
                    pTopoOp = CType(pGeomCollA, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Traiter chaque composante géométrique de élément A
                    For i = 0 To pGeomCollA.GeometryCount - 1
                        'Définir un Ring de l'élément A
                        pRing = CType(pGeomCollA.Geometry(i), IRing)

                        'Comparer tous les éléments A avec tous les éléments de B
                        For k = 0 To mpGeometrieTravail.GeometryCount - 1
                            'Vérifier si la géométrie est une ligne
                            If mpGeometrieTravail.Geometry(k).GeometryType = esriGeometryType.esriGeometryPolyline Then
                                'Initialiser un élément B
                                pGeomCollB = CType(mpGeometrieTravail.Geometry(k), IGeometryCollection)

                                'Rendre la géométrie valide
                                pTopoOp = CType(pGeomCollB, ITopologicalOperator2)
                                pTopoOp.IsKnownSimple_2 = False
                                pTopoOp.Simplify()

                                'Traiter chaque composante géométrique de B
                                For n = 0 To pGeomCollB.GeometryCount - 1
                                    'Définir le Path utilisé pour le reshape
                                    pPath = CType(pGeomCollB.Geometry(n), IPath)

                                    'Reshape de la polyligne de a selon B
                                    pRing.Reshape(pPath)
                                    bModif = True
                                Next n
                            End If
                        Next k
                    Next i

                    'Rendre la géométrie valide
                    pTopoOp = CType(pGeomCollA, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Définir la géométrie de l'élément
                    pGeometry = CType(pGeomCollA, IGeometry)

                    'Interface pour vérifier la présence du Z et du M
                    pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                    'Vérifier la présence du Z
                    If pGeometryDef.HasZ Then
                        'Traiter le Z
                        Call TraiterZ(pGeometry)
                    End If
                    'Vérifier la présence du M
                    If pGeometryDef.HasM Then
                        'Traiter le M
                        Call TraiterM(pGeometry)
                    End If

                    'Changer la géométrie de l'élément A
                    pFeature.Shape = pGeometry

                    'Conserver le résultat
                    pFeature.Store()

                    'S'il s'agit d'une polyligne
                ElseIf pFeature.Shape.GeometryType = esriGeometryType.esriGeometryPolyline Then
                    'Définir la polyligne à traiter
                    pPolyline = CType(pFeature.ShapeCopy, IPolyline)

                    'Comparer tous les éléments A avec tous les éléments de B
                    For k = 0 To mpGeometrieTravail.GeometryCount - 1
                        'Vérifier si l'élément B est une Polyligne
                        If mpGeometrieTravail.Geometry(k).GeometryType = esriGeometryType.esriGeometryPolyline Then
                            'Initialiser un élément B
                            pGeomCollB = CType(mpGeometrieTravail.Geometry(k), IGeometryCollection)

                            'Traiter chaque composante géométrique de élément B
                            For n = 0 To pGeomCollB.GeometryCount - 1
                                'Définir le Path utilisé pour le reshape
                                pPath = CType(pGeomCollB.Geometry(n), IPath)

                                'Reshape de la polyligne de a selon B
                                pPolyline.Reshape(pPath)
                                bModif = True
                            Next n
                        End If
                    Next k

                    'Rendre la géométrie valide
                    pTopoOp = CType(pPolyline, ITopologicalOperator2)
                    pTopoOp.Simplify()

                    'Définir la géométrie de l'élément
                    pGeometry = CType(pPolyline, IGeometry)

                    'Interface pour vérifier la présence du Z et du M
                    pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                    'Vérifier la présence du Z
                    If pGeometryDef.HasZ Then
                        'Traiter le Z
                        Call TraiterZ(pGeometry)
                    End If
                    'Vérifier la présence du M
                    If pGeometryDef.HasM Then
                        'Traiter le M
                        Call TraiterM(pGeometry)
                    End If

                    'Changer la géométrie de l'élément
                    pFeature.Shape = pGeometry

                    'Conserver le résultat
                    pFeature.Store()
                End If

                'Trouver le prochain élément
                pFeature = pEnumFeature.Next
            Loop

            'Rafraîchir la fenêtre d'affichage
            m_MxDocument.ActiveView.Refresh()

            'Vérifier la présecense d'une modification
            If bModif Then
                'Terminer l'opération UnDo
                pEditor.StopOperation("Reshape des éléments")
                'Sinon
            Else
                'Annuler l'opération UnDo
                pEditor.AbortOperation()
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Annuler l'opération UnDo
            If Not pEditor Is Nothing Then pEditor.AbortOperation()
            'Vider la mémoire
            pEditor = Nothing
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometry = Nothing
            pGeometryDef = Nothing
            pTopoOp = Nothing
            pGeomCollA = Nothing
            pGeomCollB = Nothing
            pPolyline = Nothing
            pRing = Nothing
            pPath = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet de Couper la géométrie d'un élément selon une ligne de travail et garder la partie gauche
    ''' et/ou droite. Un message d'erreur de la liste des OID d'éléments non-coupés est affiché à l'écran. 
    '''</summary>
    '''
    '''<param name=" bLeft "> Indique si on garde la partie gauche de la géométrie.</param>
    '''<param name=" bRight "> Indique si on garde la partie droite de la géométrie.</param>
    '''<param name=" nNbModif "> Nombre de modifications total effectuées.</param>
    ''' 
    Public Sub Couper(ByVal bLeft As Boolean, ByVal bRight As Boolean, Optional ByRef nNbModif As Integer = 0)
        'Déclarer les variables de travail
        Dim pEditor As IEditor = Nothing              'Interface ESRI pour effectuer l'édition des éléments.
        Dim pMap As IMap = Nothing                    'Interface ESRI contenant la liste des Layers actifs.
        Dim pEnumFeature As IEnumFeature = Nothing    'Interface ESRI pour traiter les éléments sélectionnés.
        Dim pFeature As IFeature = Nothing            'Interface ESRI contenant les éléments à traiter.

        Try
            'Interface pour vérifer si on est en mode édition
            pEditor = CType(m_Application.FindExtensionByName("ESRI Object Editor"), IEditor)

            'Vérifier si on est en mode édition
            If pEditor.EditState = esriEditState.esriStateNotEditing Then
                'Message d'erreur
                MsgBox("ATTENTION : Vous devez être en mode édition")
                pEditor = Nothing
                Exit Try
            End If

            'Débuter l'opération UnDo
            pEditor.StartOperation()

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Trouver le premier élément
            pEnumFeature = CType(pEditor.EditSelection, IEnumFeature)
            pFeature = pEnumFeature.Next

            'Trouver les autres éléments
            Do Until pFeature Is Nothing
                'Couper un élément selon tous les éléments de travail
                For i = 0 To mpGeometrieTravail.GeometryCount - 1
                    'Couper l'élément en deux
                    If bLeft And bRight Then
                        'Couper la géométrie de l'élément en deux
                        If fbCouperGeometrie(pFeature, mpGeometrieTravail.Geometry(i)) Then
                            nNbModif = nNbModif + 1
                        End If

                    ElseIf bLeft Then
                        'Couper l'élément et garder la partie gauche
                        If fbCouperGarderPartieGauche(pFeature, mpGeometrieTravail.Geometry(i)) Then
                            nNbModif = nNbModif + 1
                        End If

                    ElseIf bRight Then
                        'Couper l'élément et garder la partie droite
                        If fbCouperGarderPartieDroite(pFeature, mpGeometrieTravail.Geometry(i)) Then
                            nNbModif = nNbModif + 1
                        End If
                    End If
                Next i

                'Trouver le prochain élément
                pFeature = pEnumFeature.Next
            Loop

            'Rafraîchir la fenêtre d'affichage
            m_MxDocument.ActiveView.Refresh()

            'Vérifier le nombre de modifications
            If nNbModif = 0 Then
                'Annuler l'opération UnDo
                pEditor.AbortOperation()
            Else
                'Terminer l'opération UnDo
                pEditor.StopOperation("Couper selon les géométries de travail")
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Annuler l'opération UnDo
            If Not pEditor Is Nothing Then pEditor.AbortOperation()
            'Vider la mémoire
            pEditor = Nothing
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Fonction qui permet de couper et conserver la partie gauche et droite de la géométrie d'un élément
    ''' en fonction d'une ligne de travail. L'élément est coupé seulement si la ligne de travail croise la
    ''' géométrie de l'élément. La partie gauche est conservée dans l'élément traité et la partie droite
    ''' est ajoutée dans la géométrie d'un nouvel élément avec les même attributs que l'élément traité.
    '''</summary>
    '''
    '''<param name=" pFeature "> Interface ESRI contenant l'élément à traiter.</param>
    '''<param name=" pGeometry "> Interface ESRI contenant la ligne utilisée pour couper les éléments.</param>
    '''
    '''<returns> La fonction va retourner un "Boolean". Si le traitement n'a pas réussi le "Boolean" est à "False".</returns>
    ''' 
    Public Function fbCouperGeometrie(ByVal pFeature As IFeature, ByVal pGeometry As IGeometry) As Boolean
        'Déclarer les variables de travail
        Dim pObjectClass As IObjectClass = Nothing     'Interface ESRI contenant une classe.
        Dim pFeatureClass As IFeatureClass = Nothing   'Interface ESRI contenant les éléments sur disque.
        Dim pNewFeature As IFeature = Nothing          'Interface ESRI contenant un nouvel élément.
        Dim pRelOp As IRelationalOperator = Nothing    'Interface ESRI pour vérifier si la ligne croise l'élément.
        Dim pTopoOp As ITopologicalOperator2 = Nothing 'Interface ESRI utilisée pour couper la géométrie.
        Dim pGeometryRight As IGeometry = Nothing      'Interface ESRI contenant la géométrie droite.
        Dim pGeometryLeft As IGeometry = Nothing       'Interface ESRI contenant la géométrie gauche.
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.

        Try
            'Interface pour simplifier la géométrie de travail
            pTopoOp = CType(pGeometry, ITopologicalOperator2)
            pTopoOp.IsKnownSimple_2 = False
            pTopoOp.Simplify()

            'Interface pour simplifier et couper les éléments sélectionnés
            pTopoOp = CType(pFeature.ShapeCopy, ITopologicalOperator2)
            pTopoOp.IsKnownSimple_2 = False
            pTopoOp.Simplify()

            'Interface utilisé pour vérifier si les éléments se touche
            pRelOp = CType(pTopoOp, IRelationalOperator)

            'Vérifier si l'élément de travail est une polyligne
            If pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then
                '        'Vérifier si les éléments se touche
                '        If pRelOp.Crosses(pGeometry) Then
                'Couper la géométrie de l'élément une une polyligne
                pTopoOp.Cut(CType(pGeometry, IPolyline), pGeometryLeft, pGeometryRight)

                'Vérifier si les deux géométries ne sont pas vide
                If Not (pGeometryLeft Is Nothing) And Not (pGeometryRight Is Nothing) Then
                    'Vérifier si la géométrie résultante gauche est valide
                    If pGeometryLeft.GeometryType = pFeature.Shape.GeometryType Then
                        'Retourner le résultat
                        fbCouperGeometrie = True

                        'Rendre la géométrie valide
                        pTopoOp = CType(pGeometryLeft, ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()

                        'Interface pour vérifier la présence du Z et du M
                        pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                        'Vérifier la présence du Z
                        If pGeometryDef.HasZ Then
                            'Traiter le Z
                            Call TraiterZ(pGeometryLeft)
                        End If
                        'Vérifier la présence du M
                        If pGeometryDef.HasM Then
                            'Traiter le M
                            Call TraiterM(pGeometryLeft)
                        End If

                        'Remplacer l'acienne géométrie par la nouvelle
                        pFeature.Shape = pGeometryLeft

                        'Sauver le traitement
                        pFeature.Store()
                    End If

                    'Vérifier si la géométrie résultante droite est valide
                    If pGeometryRight.GeometryType = pFeature.Shape.GeometryType Then
                        'Retourner le résultat
                        fbCouperGeometrie = True

                        'Rendre la géométrie valide
                        pTopoOp = CType(pGeometryRight, ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()

                        'Définir la classe de l'élément
                        pObjectClass = pFeature.Class
                        pFeatureClass = CType(pObjectClass, IFeatureClass)

                        'Créer un nouvel élément
                        pNewFeature = pFeatureClass.CreateFeature

                        'Copier les attributs à partir d'un autres élément
                        Call fbCopierValeurAttributElementIdentique(pNewFeature, pFeature)

                        'Interface pour vérifier la présence du Z et du M
                        pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                        'Vérifier la présence du Z
                        If pGeometryDef.HasZ Then
                            'Traiter le Z
                            Call TraiterZ(pGeometryRight)
                        End If
                        'Vérifier la présence du M
                        If pGeometryDef.HasM Then
                            'Traiter le M
                            Call TraiterM(pGeometryRight)
                        End If

                        'Remplacer l'acienne géométrie par la nouvelle
                        pNewFeature.Shape = pGeometryRight

                        'Sauver le traitement
                        pNewFeature.Store()
                    End If
                End If
                '        End If
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pObjectClass = Nothing
            pFeatureClass = Nothing
            pNewFeature = Nothing
            pRelOp = Nothing
            pTopoOp = Nothing
            pGeometryLeft = Nothing
            pGeometryRight = Nothing
            pGeometryDef = Nothing
        End Try
    End Function

    '''<summary>
    '''  Fonction qui permet de copier les valeurs d'attributs d'un élément à un autre. Les attributs
    ''' OBJECTID, SHAPE, SHAPE_Length et SHAPE_Area ne sont pas traité. Le traitement n'est pas sauvé
    ''' à la fin. Il faut effectuer un store à la suite de ce traitement si on veut que les valeurs
    ''' d'attributs soient sauvées.
    '''</summary>
    '''
    '''<param name=" pFeatureTo "> Interface ESRI de l'élément avec les valeurs de destination.</param>
    '''<param name=" pFeatureFrom "> Interface ESRI de l'élément avec les valeurs d'origine.</param>
    '''
    '''<returns> La fonction va retourner un "Boolean". Si le traitement n'a pas réussi, le "Boolean" sera à "False".</returns>
    ''' 
    Public Function fbCopierValeurAttributElementIdentique(ByRef pFeatureTo As IFeature, ByVal pFeatureFrom As IFeature) As Boolean
        'Déclarer les variable des travail
        Dim pFields As IFields = Nothing       'Interface ESRI contenant les attributs d'un élément.
        Dim pField As IField = Nothing         'Interface ESRI contenat un attribut d'élément.
        Dim nPosition As Integer = Nothing     'Position de l'attribut recherché.

        Try
            'Interface pour trouver le nom de tous les attributs
            pFields = pFeatureTo.Fields

            'Traiter tous les attributs de l'élément
            For i = 0 To pFields.FieldCount - 1

                'Interface pour trouvé le nom des attributs
                pField = pFields.Field(i)

                'Vérifier si l'attribut est un attribut réservé
                If pField.Editable And pField.Type <> esriFieldType.esriFieldTypeGeometry Then
                    'Trouver la position de l'attribut correspondante
                    nPosition = pFeatureFrom.Fields.FindField(pField.Name)

                    'Vérifier la position a été trouvée
                    If nPosition >= 0 Then
                        'Copier la valeur d'attribut
                        pFeatureTo.Value(i) = pFeatureFrom.Value(nPosition)
                    End If
                End If
            Next i

            'Retourner le résultat
            fbCopierValeurAttributElementIdentique = True

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pFields = Nothing
            pField = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de couper et conserver la partie gauche de la géométrie d'un élément en fonction d'une ligne
    '''de travail. L'élément est coupé seulement si la ligne de travail croise la géométrie de l'élément. 
    '''</summary>
    '''
    '''<param name=" pFeature "> Interface ESRI contenant l'élément à traiter.</param>
    '''<param name=" pGeometry "> Interface ESRI contenant la ligne pour couper.</param>
    '''
    '''<returns> La fonction va retourner un "Boolean". Si le traitement n'a pas réussi le "Boolean" est à "False".</returns>
    ''' 
    Public Function fbCouperGarderPartieGauche(ByVal pFeature As IFeature, ByVal pGeometry As IGeometry) As Boolean
        'Déclarer les variables de travail
        Dim pRelOp As IRelationalOperator = Nothing    'Interface ESRI pour vérifier si la ligne croise l'élément.
        Dim pTopoOp As ITopologicalOperator2 = Nothing 'Interface ESRI utilisée pour couper la géométrie.
        Dim pGeometryRight As IGeometry = Nothing      'Interface ESRI contenant la géométrie droite.
        Dim pGeometryLeft As IGeometry = Nothing       'Interface ESRI contenant la géométrie gauche.
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.

        Try
            'Interface pour couper les éléments
            pTopoOp = CType(pFeature.ShapeCopy, ITopologicalOperator2)
            pTopoOp.IsKnownSimple_2 = False
            pTopoOp.Simplify()

            'Interface utilisé pour vérifier si les éléments se touche
            pRelOp = CType(pTopoOp, IRelationalOperator)

            'Vérifier si l'élément de travail est une polyligne
            If pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then

                'Vérifier si les éléments se touche
                If pRelOp.Crosses(pGeometry) Then

                    'Couper la géométrie de l'élément une une polyligne
                    pTopoOp.Cut(CType(pGeometry, IPolyline), pGeometryLeft, pGeometryRight)

                    'Vérifier si la géométrie n'est pas vide
                    If Not (pGeometryLeft Is Nothing) Then
                        'Vérifier si la géométrie résultante est valide
                        If pGeometryLeft.GeometryType = pFeature.Shape.GeometryType Then
                            'Retourner le résultat
                            fbCouperGarderPartieGauche = True

                            'Rendre la géométrie valide
                            pTopoOp = CType(pGeometryLeft, ITopologicalOperator2)
                            pTopoOp.IsKnownSimple_2 = False
                            pTopoOp.Simplify()

                            'Interface pour vérifier la présence du Z et du M
                            pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                            'Vérifier la présence du Z
                            If pGeometryDef.HasZ Then
                                'Traiter le Z
                                Call TraiterZ(pGeometryLeft)
                            End If
                            'Vérifier la présence du M
                            If pGeometryDef.HasM Then
                                'Traiter le M
                                Call TraiterM(pGeometryLeft)
                            End If

                            'Remplacer l'acienne géométrie par la nouvelle
                            pFeature.Shape = pGeometryLeft

                            'Sauver le traitement
                            pFeature.Store()
                        End If
                    End If
                End If
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pRelOp = Nothing
            pTopoOp = Nothing
            pGeometryLeft = Nothing
            pGeometryRight = Nothing
            pGeometryDef = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de couper et conserver la partie droite de la géométrie d'un élément en fonction d'une ligne
    ''' de travail. L'élément est coupé seulement si la ligne de travail croise la géométrie de l'élément.
    '''</summary>
    '''
    '''<param name=" pFeature "> Interface ESRI contenant l'élément à traiter.</param>
    '''<param name=" pGeometry"> Interface ESRI contenant la ligne pour couper.</param>
    '''
    '''<returns> La fonction va retourner un "Boolean". Si le traitement n'a pas réussi le "Boolean" est à "False".</returns>
    ''' 
    Public Function fbCouperGarderPartieDroite(ByVal pFeature As IFeature, ByVal pGeometry As IGeometry) As Boolean
        'Déclarer les variables de travail
        Dim pRelOp As IRelationalOperator = Nothing    'Interface ESRI pour vérifier si la ligne croise l'élément.
        Dim pTopoOp As ITopologicalOperator2 = Nothing 'Interface ESRI utilisée pour couper la géométrie.
        Dim pGeometryRight As IGeometry = Nothing      'Interface ESRI contenant la géométrie droite.
        Dim pGeometryLeft As IGeometry = Nothing       'Interface ESRI contenant la géométrie gauche.
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.

        Try
            'Interface pour couper les éléments
            pTopoOp = CType(pFeature.ShapeCopy, ITopologicalOperator2)
            pTopoOp.IsKnownSimple_2 = False
            pTopoOp.Simplify()

            'Interface utilisé pour vérifier si les éléments se touche
            pRelOp = CType(pTopoOp, IRelationalOperator)

            'Vérifier si l'élément de travail est une polyligne
            If pGeometry.GeometryType = esriGeometryType.esriGeometryPolyline Then

                'Vérifier si les éléments se touche
                If pRelOp.Crosses(pGeometry) Then

                    'Couper la géométrie de l'élément avec une polyligne
                    pTopoOp.Cut(CType(pGeometry, IPolyline), pGeometryLeft, pGeometryRight)

                    'Vérifier si la géométrie n'est pas vide
                    If Not (pGeometryRight Is Nothing) Then
                        'Vérifier si la géométrie résultante est valide
                        If pGeometryRight.GeometryType = pFeature.Shape.GeometryType Then
                            'Retourner le résultat
                            fbCouperGarderPartieDroite = True

                            'Rendre la géométrie valide
                            pTopoOp = CType(pGeometryRight, ITopologicalOperator2)
                            pTopoOp.IsKnownSimple_2 = False
                            pTopoOp.Simplify()

                            'Interface pour vérifier la présence du Z et du M
                            pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                            'Vérifier la présence du Z
                            If pGeometryDef.HasZ Then
                                'Traiter le Z
                                Call TraiterZ(pGeometryRight)
                            End If
                            'Vérifier la présence du M
                            If pGeometryDef.HasM Then
                                'Traiter le M
                                Call TraiterM(pGeometryRight)
                            End If

                            'Remplacer l'acienne géométrie par la nouvelle
                            pFeature.Shape = pGeometryRight

                            'Sauver le traitement
                            pFeature.Store()
                        End If
                    End If
                End If
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pRelOp = Nothing
            pTopoOp = Nothing
            pGeometryLeft = Nothing
            pGeometryRight = Nothing
            pGeometryDef = Nothing
        End Try
    End Function

    '''<summary>
    ''' Routine qui permet d'effectuer le traitement de symétrie des éléments sélectionnés avec les
    ''' géométries de travail. Seulement les éléments qui ont la contraite "Overlap" avec les géométries
    ''' de travail de même type seront traités. 
    '''</summary>
    '''
    '''<param name=" nNbModif "> Nombre de modifications total effectuées.</param>
    '''  
    Public Sub Intersection(Optional ByRef nNbModif As Integer = 0)
        'Déclarer les variables de travail
        Dim pEditor As IEditor = Nothing                'Interface ESRI pour effectuer l'édition des éléments.
        Dim pMap As IMap = Nothing                      'Interface ESRI contenant la liste des Layers actifs.
        Dim pEnumFeature As IEnumFeature = Nothing      'Interface ESRI pour traiter les éléments sélectionnés.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant les éléments à traiter.
        Dim pGeometryA As IGeometry = Nothing          'Interface ESRI contenant la géométrie à traiter.
        Dim pGeometryB As IGeometry = Nothing          'Interface ESRI contenant la géométrie de travail.
        Dim pGeometryC As IGeometry = Nothing          'Interface ESRI contenant la géométrie de travail.
        Dim pNewGeometry As IGeometry = Nothing        'Interface ESRI contenant la nouvelle géométrie traitée.
        Dim pRelOp As IRelationalOperator = Nothing     'Interface ESRI pour vérifier une contraite d'intégrité.
        Dim pTopoOp As ITopologicalOperator2 = Nothing  'Interface ESRI pour effectuer le traitement des géométries.
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.
        Dim j As Integer = Nothing       'Compteur

        Try
            'Interface pour vérifer si on est en mode édition
            pEditor = CType(m_Application.FindExtensionByName("ESRI Object Editor"), IEditor)

            'Vérifier si on est en mode édition
            If pEditor.EditState = esriEditState.esriStateNotEditing Then
                'Message d'erreur
                MsgBox("ATTENTION : Vous devez être en mode édition")
                pEditor = Nothing
                Exit Try
            End If

            'Débuter l'opération UnDo
            pEditor.StartOperation()

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Trouver le premier élément
            pEnumFeature = CType(pEditor.EditSelection, IEnumFeature)
            pFeature = pEnumFeature.Next

            'Trouver les autres éléments
            Do Until pFeature Is Nothing
                'Copie de la géométrie originale
                pGeometryA = CType(pFeature.ShapeCopy, IGeometry)

                'Interface pour couper les éléments
                pTopoOp = CType(pGeometryA, ITopologicalOperator2)
                pTopoOp.IsKnownSimple_2 = False
                pTopoOp.Simplify()

                'Interface pour vérifer la contrainte entre les éléments
                pRelOp = CType(pGeometryA, IRelationalOperator)

                'Traiter tous les éléments en mémoire
                For i = 0 To mpGeometrieTravail.GeometryCount - 1
                    'Définir la géométrie B
                    pGeometryB = CType(mpGeometrieTravail.Geometry(i), IGeometry)

                    'Interface pour simplifier la géométrie
                    pTopoOp = CType(pGeometryB, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Vérifier si l'élément de B est une polyligne
                    If pGeometryA.GeometryType = pGeometryB.GeometryType Then
                        'Vérifier si la contrainte Overlap est valide
                        If Not pRelOp.Disjoint(pGeometryB) Then
                            'Intersection de la géométrie de l'élément
                            Call fbIntersectGeometrie(pGeometryA, pGeometryB, pNewGeometry, pGeometryA.Dimension)

                            'Vérifier si le résultat est vide
                            If Not pNewGeometry Is Nothing Then
                                'Compter le nombre de mofifications
                                nNbModif = nNbModif + 1

                                'Interface pour simplifier la géométrie
                                pTopoOp = CType(pNewGeometry, ITopologicalOperator2)
                                pTopoOp.IsKnownSimple_2 = False
                                pTopoOp.Simplify()

                                'Vérifier si la géométrie C est valide
                                If pGeometryC Is Nothing Then
                                    'définir la nouvelle géométrie
                                    pGeometryC = pNewGeometry
                                    'Sinon
                                Else
                                    'Union des nouvelles géométries
                                    pGeometryC = CType(pTopoOp.Union(pGeometryC), IGeometry)
                                End If
                            End If
                        End If
                    End If
                Next i

                'Vérifier si la géométrie C est valide
                If Not pGeometryC Is Nothing Then
                    'Interface pour vérifier la présence du Z et du M
                    pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                    'Vérifier la présence du Z
                    If pGeometryDef.HasZ Then
                        'Traiter le Z
                        Call TraiterZ(pGeometryC)
                    End If
                    'Vérifier la présence du M
                    If pGeometryDef.HasM Then
                        'Traiter le M
                        Call TraiterM(pGeometryC)
                    End If

                    'Remplacer l'acienne géométrie par la nouvelle
                    pFeature.Shape = pGeometryC

                    'Sauver le traitement
                    pFeature.Store()
                End If

                'Afficher le pourcentage du traitement effectué
                j = j + 1
                m_Application.StatusBar.Message(0) = CInt((j * 100) / (pEditor.SelectionCount)) & "% "

                'Trouver le prochain élément
                pFeature = pEnumFeature.Next
            Loop

            'Rafraîchir la fenêtre d'affichage
            m_MxDocument.ActiveView.Refresh()

            'Vérifier le nombre de modifications
            If nNbModif = 0 Then
                'Annuler l'opération UnDo
                pEditor.AbortOperation()
            Else
                'Terminer l'opération UnDo
                pEditor.StopOperation("Intersection selon les géométries de travail")
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Annuler l'opération UnDo
            If Not pEditor Is Nothing Then pEditor.AbortOperation()
            'Vider la mémoire
            pEditor = Nothing
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryA = Nothing
            pGeometryB = Nothing
            pGeometryC = Nothing
            pNewGeometry = Nothing
            pRelOp = Nothing
            pTopoOp = Nothing
            pGeometryDef = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet d'effectuer le traitement de différence des éléments sélectionnés avec les
    ''' géométries de travail. Seulement les éléments qui ont la contraite "Overlap" avec les géométries
    ''' de travail de même type seront traités.
    '''</summary>
    '''
    '''<param name=" nNbModif "> Nombre de modifications total effectuées.</param>
    '''  
    Public Sub Difference(Optional ByRef nNbModif As Integer = 0)
        'Déclarer les variables de travail
        Dim pEditor As IEditor = Nothing               'Interface ESRI pour effectuer l'édition des éléments.
        Dim pMap As IMap = Nothing                     'Interface ESRI contenant la liste des Layers actifs.
        Dim pEnumFeature As IEnumFeature = Nothing     'Interface ESRI pour traiter les éléments sélectionnés.
        Dim pFeature As IFeature = Nothing             'Interface ESRI contenant les éléments à traiter.
        Dim pGeometryA As IGeometry = Nothing          'Interface ESRI contenant la géométrie à traiter.
        Dim pGeometryB As IGeometry = Nothing          'Interface ESRI contenant la géométrie de travail.
        Dim pNewGeometry As IGeometry = Nothing        'Interface ESRI contenant la nouvelle géométrie traitée.
        Dim pRelOp As IRelationalOperator = Nothing    'Interface ESRI pour vérifier une contraite d'intégrité.
        Dim pTopoOp As ITopologicalOperator2 = Nothing 'Interface ESRI pour effectuer le traitement des géométries.
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.
        Dim j As Integer = Nothing   'Compteur

        Try
            'Interface pour vérifer si on est en mode édition
            pEditor = CType(m_Application.FindExtensionByName("ESRI Object Editor"), IEditor)

            'Vérifier si on est en mode édition
            If pEditor.EditState = esriEditState.esriStateNotEditing Then
                'Message d'erreur
                MsgBox("ATTENTION : Vous devez être en mode édition")
                pEditor = Nothing
                Exit Try
            End If

            'Débuter l'opération UnDo
            pEditor.StartOperation()

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Trouver le premier élément
            pEnumFeature = CType(pEditor.EditSelection, IEnumFeature)
            pFeature = pEnumFeature.Next

            'Trouver les autres éléments
            Do Until pFeature Is Nothing
                'Copie de la géométrie originale
                pGeometryA = pFeature.ShapeCopy

                'Traiter tous les éléments en mémoire
                For i = 0 To mpGeometrieTravail.GeometryCount - 1

                    'Interface pour couper les éléments
                    pTopoOp = CType(pGeometryA, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Interface pour vérifer la contrainte entre les éléments
                    pRelOp = CType(pGeometryA, IRelationalOperator)

                    'Simplifier la géométrie en relation
                    pGeometryB = mpGeometrieTravail.Geometry(i)
                    pTopoOp = CType(pGeometryB, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Vérifier si la dimension de la géométrie de l'élément n'est pas plus grande que celle de la géométrie de travail
                    If pGeometryA.Dimension <= pGeometryB.Dimension Then
                        'Vérifier si la contrainte Overlap est valide
                        If Not pRelOp.Disjoint(pGeometryB) Then
                            'Compter le nombre de mofifications
                            nNbModif = nNbModif + 1

                            'Différence de la géométrie de l'élément
                            pTopoOp = CType(pGeometryA, ITopologicalOperator2)
                            pNewGeometry = pTopoOp.Difference(pGeometryB)

                            'Vérifier si le résultat est vide
                            If pNewGeometry Is Nothing Then
                                'Détruire l'élément
                                pFeature.Delete()
                                GoTo Suivant
                            Else
                                'Remplacer l'acienne géométrie par la nouvelle
                                pGeometryA = pNewGeometry
                            End If
                        End If
                    End If
                Next i

                'Interface pour vérifier la présence du Z et du M
                pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                'Vérifier la présence du Z
                If pGeometryDef.HasZ Then
                    'Traiter le Z
                    Call TraiterZ(pGeometryA)
                End If
                'Vérifier la présence du M
                If pGeometryDef.HasM Then
                    'Traiter le M
                    Call TraiterM(pGeometryA)
                End If

                'Remplacer l'acienne géométrie par la nouvelle
                pFeature.Shape = pGeometryA

                'Sauver le traitement
                pFeature.Store()

Suivant:
                'Afficher le pourcentage du traitement effectué
                j = j + 1
                m_Application.StatusBar.Message(0) = CInt((j * 100) / (pEditor.SelectionCount)) & "% "

                'Trouver le prochain élément
                pFeature = pEnumFeature.Next
            Loop

            'Rafraîchir la fenêtre d'affichage
            m_MxDocument.ActiveView.Refresh()

            'Vérifier le nombre de modifications
            If nNbModif = 0 Then
                'Annuler l'opération UnDo
                pEditor.AbortOperation()
            Else
                'Terminer l'opération UnDo
                pEditor.StopOperation("Différence selon les géométries de travail")
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Annuler l'opération UnDo
            If Not pEditor Is Nothing Then pEditor.AbortOperation()
            'Vider la mémoire
            pEditor = Nothing
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryA = Nothing
            pGeometryB = Nothing
            pNewGeometry = Nothing
            pRelOp = Nothing
            pTopoOp = Nothing
            pGeometryDef = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Fonction qui permet d'effectuer le traitement de différence inversée des éléments sélectionnés
    ''' avec les géométries de travail. Seulement les éléments qui ont la contraite "Overlap" avec les
    '''géométries de travail de même type seront traités.
    '''</summary>
    '''
    '''<param name=" nNbModif "> Nombre de modifications total effectuées.</param>
    '''
    '''<returns> La fonction va retourner un "Boolean". Si le traitement n'a pas réussi le "Boolean" est à "False".</returns>
    ''' 
    Public Function DifferenceInverser(Optional ByRef nNbModif As Integer = 0) As Boolean
        'Déclarer les variables de travail
        Dim pEditor As IEditor = Nothing               'Interface ESRI pour effectuer l'édition des éléments.
        Dim pMap As IMap = Nothing                     'Interface ESRI contenant le document de ArcMap.
        Dim pEnumFeature As IEnumFeature = Nothing     'Interface ESRI contenant la liste des Layers actifs.
        Dim pFeature As IFeature = Nothing             'Interface ESRI pour traiter les éléments sélectionnés.
        Dim pGeometryA As IGeometry = Nothing          'Interface ESRI contenant les éléments à traiter.
        Dim pGeometryB As IGeometry = Nothing          'Interface ESRI contenant la géométrie à traiter.
        Dim pNewGeometry As IGeometry = Nothing        'Interface ESRI contenant la géométrie de travail.
        Dim pRelOp As IRelationalOperator = Nothing    'Interface ESRI contenant la nouvelle géométrie traitée.
        Dim pTopoOp As ITopologicalOperator2 = Nothing 'Interface ESRI pour vérifier une contraite d'intégrité.
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.
        Dim j As Integer = Nothing       'Compteur

        Try
            'Interface pour vérifer si on est en mode édition
            pEditor = CType(m_Application.FindExtensionByName("ESRI Object Editor"), IEditor)

            'Vérifier si on est en mode édition
            If pEditor.EditState = esriEditState.esriStateNotEditing Then
                'Message d'erreur
                MsgBox("ATTENTION : Vous devez être en mode édition")
                pEditor = Nothing
                Exit Try
            End If

            'Débuter l'opération UnDo
            pEditor.StartOperation()

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Trouver le premier élément
            pEnumFeature = CType(pEditor.EditSelection, IEnumFeature)
            pFeature = pEnumFeature.Next

            'Trouver les autres éléments
            Do Until pFeature Is Nothing
                'Copie de la géométrie originale
                pGeometryA = pFeature.ShapeCopy

                'Interface pour vérifer la contrainte entre les éléments
                pRelOp = CType(pGeometryA, IRelationalOperator)

                'Valider la géométrie A
                pTopoOp = CType(pGeometryA, ITopologicalOperator2)
                pTopoOp.IsKnownSimple_2 = False
                pTopoOp.Simplify()

                'Traiter tous les éléments en mémoire
                For i = 0 To mpGeometrieTravail.GeometryCount - 1
                    pGeometryB = mpGeometrieTravail.Geometry(i)
                    pTopoOp = CType(pGeometryB, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Vérifier si l'élément de B est une polyligne
                    If pGeometryA.GeometryType = pGeometryB.GeometryType Then
                        'Vérifier si la contrainte not disjoint est valide
                        If Not pRelOp.Disjoint(pGeometryB) Then
                            'Compter le nombre de mofifications
                            nNbModif = nNbModif + 1

                            'Différence inversée de la géométrie de l'élément
                            pNewGeometry = pTopoOp.Difference(pGeometryA)

                            'Vérifier si le résultat est vide
                            If pNewGeometry Is Nothing Then
                                'Détruire l'élément
                                pFeature.Delete()
                                GoTo Suivant
                                'Sinon
                            Else
                                'Rendre la géométrie valide
                                pTopoOp = CType(pNewGeometry, ITopologicalOperator2)
                                pTopoOp.IsKnownSimple_2 = False
                                pTopoOp.Simplify()
                            End If
                        End If
                    End If
                Next i

                'Interface pour vérifier la présence du Z et du M
                pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                'Vérifier la présence du Z
                If pGeometryDef.HasZ Then
                    'Traiter le Z
                    Call TraiterZ(pNewGeometry)
                End If
                'Vérifier la présence du M
                If pGeometryDef.HasM Then
                    'Traiter le M
                    Call TraiterM(pNewGeometry)
                End If

                'Remplacer l'acienne géométrie par la nouvelle
                pFeature.Shape = pNewGeometry

                'Sauver le traitement
                pFeature.Store()

Suivant:
                'Afficher le pourcentage du traitement effectué
                j = j + 1
                m_Application.StatusBar.Message(0) = CInt((j * 100) / (pEditor.SelectionCount)) & "% "

                'Trouver le prochain élément
                pFeature = pEnumFeature.Next
            Loop

            'Rafraîchir la fenêtre d'affichage
            m_MxDocument.ActiveView.Refresh()

            'Vérifier le nombre de modifications
            If nNbModif = 0 Then
                'Annuler l'opération UnDo
                pEditor.AbortOperation()
                'Sinon
            Else
                'Terminer l'opération UnDo
                pEditor.StopOperation("Différence inverser selon les géométries de travail")
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Annuler l'opération UnDo
            If Not pEditor Is Nothing Then pEditor.AbortOperation()
            'Vider la mémoire
            pEditor = Nothing
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryA = Nothing
            pGeometryB = Nothing
            pNewGeometry = Nothing
            pRelOp = Nothing
            pTopoOp = Nothing
            pGeometryDef = Nothing
        End Try
    End Function

    '''<summary>
    ''' Routine qui permet d'effectuer le traitement de symétrie des éléments sélectionnés avec les
    ''' géométries de travail. Seulement les éléments qui ont la contraite "Overlap" avec les géométries
    ''' de travail de même type seront traités.
    '''</summary>
    '''
    '''<param name=" nNbModif "> Nombre de modifications total effectuées.</param>
    '''   
    Public Sub Symetrie(Optional ByRef nNbModif As Integer = 0)
        'Déclarer les variables de travail
        Dim pEditor As IEditor = Nothing               'Interface ESRI pour effectuer l'édition des éléments.
        Dim pMap As IMap = Nothing                     'Interface ESRI contenant la liste des Layers actifs.
        Dim pEnumFeature As IEnumFeature = Nothing     'Interface ESRI pour traiter les éléments sélectionnés.
        Dim pFeature As IFeature = Nothing             'Interface ESRI contenant les éléments à traiter.
        Dim pGeometryA As IGeometry = Nothing          'Interface ESRI contenant la géométrie à traiter.
        Dim pGeometryB As IGeometry = Nothing          'Interface ESRI contenant la géométrie de travail.
        Dim pNewGeometry As IGeometry = Nothing        'Interface ESRI contenant la nouvelle géométrie traitée.
        Dim pRelOp As IRelationalOperator = Nothing    'Interface ESRI pour vérifier une contraite d'intégrité.
        Dim pTopoOp As ITopologicalOperator2 = Nothing 'Interface ESRI pour effectuer le traitement des géométries.
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.
        Dim j As Integer = Nothing   'Compteur

        Try
            'Interface pour vérifer si on est en mode édition
            pEditor = CType(m_Application.FindExtensionByName("ESRI Object Editor"), IEditor)

            'Vérifier si on est en mode édition
            If pEditor.EditState = esriEditState.esriStateNotEditing Then
                'Message d'erreur
                MsgBox("ATTENTION : Vous devez être en mode édition")
                pEditor = Nothing
                Exit Try
            End If

            'Débuter l'opération UnDo
            pEditor.StartOperation()

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Trouver le premier élément
            pEnumFeature = CType(pEditor.EditSelection, IEnumFeature)
            pFeature = pEnumFeature.Next

            'Trouver les autres éléments
            Do Until pFeature Is Nothing
                'Copie de la géométrie originale
                pGeometryA = pFeature.ShapeCopy

                'Traiter tous les éléments en mémoire
                For i = 0 To mpGeometrieTravail.GeometryCount - 1
                    'Interface pour couper les éléments
                    pTopoOp = CType(pGeometryA, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Interface pour vérifer la contrainte entre les éléments
                    pRelOp = CType(pGeometryA, IRelationalOperator)

                    'Simplifier la géométrie en relation
                    pGeometryB = mpGeometrieTravail.Geometry(i)
                    pTopoOp = CType(pGeometryB, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Vérifier si l'élément de B est une polyligne
                    If pGeometryA.GeometryType = pGeometryB.GeometryType Then
                        'Vérifier si la contrainte Overlap est valide
                        If pRelOp.Overlaps(pGeometryB) Then
                            'Compter le nombre de mofifications
                            nNbModif = nNbModif + 1

                            'Symétrie de la géométrie de l'élément
                            pNewGeometry = pTopoOp.SymmetricDifference(pGeometryA)

                            'Vérifier si le résultat est vide
                            If pNewGeometry Is Nothing Then
                                'Détruire l'élément
                                pFeature.Delete()
                                GoTo Suivant
                            Else
                                'Remplacer l'acienne géométrie par la nouvelle
                                pGeometryA = pNewGeometry
                            End If
                        End If
                    End If
                Next i

                'Interface pour vérifier la présence du Z et du M
                pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                'Vérifier la présence du Z
                If pGeometryDef.HasZ Then
                    'Traiter le Z
                    Call TraiterZ(pNewGeometry)
                End If
                'Vérifier la présence du M
                If pGeometryDef.HasM Then
                    'Traiter le M
                    Call TraiterM(pNewGeometry)
                End If

                'Remplacer l'acienne géométrie par la nouvelle
                pFeature.Shape = pNewGeometry

                'Sauver le traitement
                pFeature.Store()

Suivant:
                'Afficher le pourcentage du traitement effectué
                j = j + 1
                m_Application.StatusBar.Message(0) = CInt((j * 100) / (pEditor.SelectionCount)) & "% "

                'Trouver le prochain élément
                pFeature = pEnumFeature.Next
            Loop

            'Rafraîchir la fenêtre d'affichage
            m_MxDocument.ActiveView.Refresh()

            'Vérifier le nombre de modifications
            If nNbModif = 0 Then
                'Annuler l'opération UnDo
                pEditor.AbortOperation()
            Else
                'Terminer l'opération UnDo
                pEditor.StopOperation("Symétrie entre les géométries de travail")
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Annuler l'opération UnDo
            If Not pEditor Is Nothing Then pEditor.AbortOperation()
            'Vider la mémoire
            pEditor = Nothing
            pMap = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryA = Nothing
            pGeometryB = Nothing
            pNewGeometry = Nothing
            pRelOp = Nothing
            pTopoOp = Nothing
            pGeometryDef = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet d'effectuer le traitement d'union des éléments sélectionnés avec les
    ''' géométries de travail. Seulement les éléments qui touches ou superposes les géométries de travail. 
    '''</summary>
    '''
    '''<param name=" nNbModif "> Nombre de modifications total effectuées.</param>
    '''   
    Public Sub Union(Optional ByRef nNbModif As Integer = 0)
        'Déclarer les variables de travail
        Dim pEditor As IEditor = Nothing               'Interface ESRI pour effectuer l'édition des éléments.
        Dim pEnumFeature As IEnumFeature = Nothing     'Interface ESRI pour traiter les éléments sélectionnés.
        Dim pFeature As IFeature = Nothing             'Interface ESRI contenant les éléments à traiter.
        Dim pGeometryA As IGeometry = Nothing          'Interface ESRI contenant la géométrie à traiter.
        Dim pGeometryB As IGeometry = Nothing          'Interface ESRI contenant la géométrie de travail.
        Dim pNewGeometry As IGeometry = Nothing        'Interface ESRI contenant la nouvelle géométrie traitée.
        Dim pRelOp As IRelationalOperator = Nothing    'Interface ESRI pour vérifier une contraite d'intégrité.
        Dim pTopoOp As ITopologicalOperator2 = Nothing 'Interface ESRI pour effectuer le traitement des géométries.
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.
        Dim j As Integer = Nothing   'Compteur

        Try
            'Interface pour vérifer si on est en mode édition
            pEditor = CType(m_Application.FindExtensionByName("ESRI Object Editor"), IEditor)

            'Vérifier si on est en mode édition
            If pEditor.EditState = esriEditState.esriStateNotEditing Then
                'Message d'erreur
                MsgBox("ATTENTION : Vous devez être en mode édition")
                pEditor = Nothing
                Exit Try
            End If

            'Débuter l'opération UnDo
            pEditor.StartOperation()

            'Interface pour les éléments sélectionnés
            pEnumFeature = pEditor.EditSelection

            'Trouver le premier élément
            pFeature = pEnumFeature.Next

            'Trouver les autres éléments
            Do Until pFeature Is Nothing
                'Copie de la géométrie originale
                pGeometryA = pFeature.ShapeCopy

                'Traiter tous les éléments en mémoire
                For i = 0 To mpGeometrieTravail.GeometryCount - 1
                    'Interface pour couper les éléments
                    pTopoOp = CType(pGeometryA, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Interface pour vérifer la contrainte entre les éléments
                    pRelOp = CType(pGeometryA, IRelationalOperator)

                    'Simplifier la géométrie B
                    pGeometryB = mpGeometrieTravail.Geometry(i)
                    pTopoOp = CType(pGeometryB, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Vérifier si l'élément de B est une polyligne
                    If pGeometryA.GeometryType = pGeometryB.GeometryType Then
                        'Vérifier si la contrainte Overlap est valide
                        If Not pRelOp.Disjoint(pGeometryB) Then
                            'Union de la géométrie de l'élément
                            pNewGeometry = pTopoOp.Union(pGeometryA)

                            'Vérifier si le résultat est vide
                            If Not pNewGeometry Is Nothing Then
                                'Compter le nombre de mofifications
                                nNbModif = nNbModif + 1

                                'Remplacer l'acienne géométrie par la nouvelle
                                pGeometryA = pNewGeometry
                            End If
                        End If
                    End If
                Next i

                'Interface pour vérifier la présence du Z et du M
                pGeometryDef = RetournerGeometryDef(CType(pFeature.Class, IFeatureClass))
                'Vérifier la présence du Z
                If pGeometryDef.HasZ Then
                    'Traiter le Z
                    Call TraiterZ(pNewGeometry)
                End If
                'Vérifier la présence du M
                If pGeometryDef.HasM Then
                    'Traiter le M
                    Call TraiterM(pNewGeometry)
                End If

                'Remplacer l'acienne géométrie par la nouvelle
                pFeature.Shape = pNewGeometry

                'Sauver le traitement
                pFeature.Store()

                'Afficher le pourcentage du traitement effectué
                j = j + 1
                m_Application.StatusBar.Message(0) = CInt((j * 100) / (pEditor.SelectionCount)) & "% "

                'Trouver le prochain élément
                pFeature = pEnumFeature.Next
            Loop

            'Rafraîchir la fenêtre d'affichage
            m_MxDocument.ActiveView.Refresh()

            'Vérifier le nombre de modifications
            If nNbModif = 0 Then
                'Annuler l'opération UnDo
                pEditor.AbortOperation()
                'Sinon
            Else
                'Terminer l'opération UnDo
                pEditor.StopOperation("Union selon les géométries de travail")
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Annuler l'opération UnDo
            If Not pEditor Is Nothing Then pEditor.AbortOperation()
            'Vider la mémoire
            pEditor = Nothing
            pEnumFeature = Nothing
            pFeature = Nothing
            pGeometryA = Nothing
            pGeometryB = Nothing
            pNewGeometry = Nothing
            pRelOp = Nothing
            pTopoOp = Nothing
            pGeometryDef = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet d'exporter les géométries de travail dans le projet afin de les conserver.
    '''</summary>
    '''
    Public Sub Importer()
        'Déclarer les variables de travail
        Dim pGraphicsContainerSelect As IGraphicsContainerSelect = Nothing    'Interface pour sélectionner les éléments.
        Dim pEnumElement As IEnumElement = Nothing        'Interface pour traiter tous les éléments graphiques.
        Dim pElement As IElement = Nothing                'Interafce contenant un élément graphique.

        Try
            'Interface pour sélectionner des éléments graphiques
            pGraphicsContainerSelect = CType(m_MxDocument.FocusMap, IGraphicsContainerSelect)

            'Vérifier si des éléments sont sélectionnés
            If pGraphicsContainerSelect.ElementSelectionCount = 0 Then
                'Sélectionner tous les éléments
                pGraphicsContainerSelect.SelectAllElements()
            End If

            'Extraire tous les éléments graphiques sélectionnés
            pEnumElement = pGraphicsContainerSelect.SelectedElements

            'Extraire le premier élément
            pElement = pEnumElement.Next

            'Traiter tous les éléments
            Do Until pElement Is Nothing
                'Ajouter la géométrie de l'élément
                mpGeometrieTravail.AddGeometry(pElement.Geometry)

                'Extraire le prochain élément
                pElement = pEnumElement.Next
            Loop

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pGraphicsContainerSelect = Nothing
            pEnumElement = Nothing
            pElement = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet d'exporter les géométries de travail dans le projet afin de les conserver.
    '''</summary>
    '''
    Public Sub Exporter()
        'Déclarer les variables de travail
        Dim pGraphicsContainer As IGraphicsContainer = Nothing  'Interface pour ajouter une collection d'éléments.
        Dim pElementColl As IElementCollection = Nothing  'Collection d'éléments générés à partir des géométries.

        Try
            'Interface pour ajouter des éléments graphiques
            pGraphicsContainer = CType(m_MxDocument.FocusMap, IGraphicsContainer)

            'Transformer la collection des géométries de travail en collection d'éléments graphiques
            Call Geometry2ElementCollection(mpGeometrieTravail, pElementColl)

            'Ajouter l'élément dans la Map du projet
            pGraphicsContainer.AddElements(pElementColl, 0)

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pGraphicsContainer = Nothing
            pElementColl = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet de transformer une collection de géométrie en une collection d'éléments graphiques. 
    '''</summary>
    '''
    '''<param name=" pGeometryColl "> Collection de géométries à transférer.</param>
    '''<param name=" pElementColl "> Collection d'éléments générés à partir des géométries.</param>
    '''   
    Public Sub Geometry2ElementCollection(ByVal pGeometryColl As IGeometryCollection, ByRef pElementColl As IElementCollection)
        'Déclarer les variables de travail
        Dim pElement As IElement = Nothing              'Interface contenant un élément graphique.
        Dim pPointColl As IPointCollection = Nothing    'Interface qui permet d'accéder à chaque sommet

        Try
            'Vérifier si la collection d'élément est invalide
            If pElementColl Is Nothing Then
                'Créer une collection d'élément vide
                pElementColl = New ElementCollection
            End If

            'Traiter toutes les géométries de travail
            For i = 0 To mpGeometrieTravail.GeometryCount - 1
                'Vider la mémoire de l'élément
                pElement = Nothing

                'Vérifier si la géométrie est de type point
                If pGeometryColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPoint Then
                    'Créer un nouvel élément de type point
                    pElement = New MarkerElement
                    'Définir la géométrie de l'élément
                    pElement.Geometry = mpGeometrieTravail.Geometry(i)
                    'Ajouter l'élément dans la Map du projet
                    pElementColl.Add(pElement)

                    'Vérifier si la géométrie est de type multipoint
                ElseIf pGeometryColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryMultipoint Then
                    'Interface pour accéder à chaque sommet
                    pPointColl = CType(pGeometryColl.Geometry(i), IPointCollection)
                    'Traiter tous les sommets
                    For j = 0 To pPointColl.PointCount - 1
                        'Créer un nouvel élément de type point
                        pElement = New MarkerElement
                        'Définir la géométrie de l'élément
                        pElement.Geometry = pPointColl.Point(j)
                        'Ajouter l'élément dans la Map du projet
                        pElementColl.Add(pElement)
                    Next j

                    'Vérifier si la géométrie est de type ligne
                ElseIf pGeometryColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPolyline Then
                    'Créer un nouvel élément de type ligne
                    pElement = New LineElement
                    'Définir la géométrie de l'élément
                    pElement.Geometry = mpGeometrieTravail.Geometry(i)
                    'Ajouter l'élément dans la Map du projet
                    pElementColl.Add(pElement)

                    'Vérifier si la géométrie est de type surface
                ElseIf pGeometryColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryPolygon Then
                    'Créer un nouvel élément de type surface
                    pElement = New PolygonElement
                    'Définir la géométrie de l'élément
                    pElement.Geometry = mpGeometrieTravail.Geometry(i)
                    'Ajouter l'élément dans la Map du projet
                    pElementColl.Add(pElement)

                    'Vérifier si la géométrie est de type surface
                ElseIf pGeometryColl.Geometry(i).GeometryType = esriGeometryType.esriGeometryBag Then
                    'Transformer la collection des géométries de travail en collection d'éléments graphiques
                    Call Geometry2ElementCollection(pGeometryColl, pElementColl)
                End If
            Next i

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pElement = Nothing
            pPointColl = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet de traiter le Z d'une Géométrie.
    '''</summary>
    '''
    '''<param name=" pGeometry "> Interface contenant la géométrie à traiter.</param>
    '''<param name=" dZ "> Contient la valeur du Z.</param>
    '''
    Public Sub TraiterZ(ByRef pGeometry As IGeometry, Optional ByVal dZ As Double = 0)
        'Déclarer les variables de travail
        Dim pZAware As IZAware = Nothing                'Interface ESRI utilisée pour traiter le Z.
        Dim pGeomColl As IGeometryCollection = Nothing  'Interface qui permet d'accéder aux géométries
        Dim pPointColl As IPointCollection = Nothing    'Interface qui permet d'accéder aux sommets de la géométrie
        Dim pPoint As IPoint = Nothing                  'Interface qui permet de modifier le Z

        Try
            'Interface pour traiter le Z
            pZAware = CType(pGeometry, IZAware)

            'Vérifier la présence du Z
            If pGeometry.SpatialReference.HasZPrecision Then
                'Ajouter le 3D
                pZAware.ZAware = True
                'Corriger le Z au besoin
                If pZAware.ZSimple = False Then
                    'Vérifier si la géométrie est un Point
                    If pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                        'Définir le point
                        pPoint = CType(pGeometry, IPoint)
                        'Définir le Z du point
                        pPoint.Z = dZ

                        'Vérifier si la géométrie est un Bag
                    ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryBag Then
                        'Interface utilisé pour accéder aux géométries
                        pGeomColl = CType(pGeometry, IGeometryCollection)
                        'Traiter toutes les géométries
                        For i = 0 To pGeomColl.GeometryCount - 1
                            'Traiter le Z
                            Call TraiterZ(pGeomColl.Geometry(i), dZ)
                        Next

                        'Vérifier si la géométrie est un Multipoint,une Polyline ou un Polygon
                    Else
                        'Interface utilisé pour accéder aux sommets de la géométrie
                        pPointColl = CType(pGeometry, IPointCollection)
                        'Traiter tous les sommets de la géométrie
                        For i = 0 To pPointColl.PointCount - 1
                            'Interface pour définir le Z
                            pPoint = pPointColl.Point(i)
                            'Définir le Z du point
                            pPoint.Z = dZ
                            'Conserver les modifications
                            pPointColl.UpdatePoint(i, pPoint)
                        Next
                    End If
                End If

                'Si aucun Z
            Else
                'Enlever le 3D
                pZAware.ZAware = True
                pZAware.DropZs()
                pZAware.ZAware = False
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pZAware = Nothing
            pGeomColl = Nothing
            pPointColl = Nothing
            pPoint = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet de traiter le M d'une Géométrie.
    '''</summary>
    '''
    '''<param name=" pGeometry "> Interface contenant la géométrie à traiter.</param>
    '''<param name=" dM "> Contient la valeur du M.</param>
    '''
    Public Sub TraiterM(ByRef pGeometry As IGeometry, Optional ByVal dM As Double = 0)
        'Déclarer les variables de travail
        Dim pMAware As IMAware = Nothing                'Interface ESRI utilisée pour traiter le M.
        Dim pGeomColl As IGeometryCollection = Nothing  'Interface qui permet d'accéder aux géométries
        Dim pPointColl As IPointCollection = Nothing    'Interface qui permet d'accéder aux sommets de la géométrie
        Dim pPoint As IPoint = Nothing                  'Interface qui permet de modifier le M

        Try
            'Interface pour traiter le M
            pMAware = CType(pGeometry, IMAware)

            'Vérifier la présence du M
            If pGeometry.SpatialReference.HasMPrecision Then
                'Ajouter le M
                pMAware.MAware = True
                'Corriger le M au besoin
                If pMAware.MSimple = False Then
                    'Vérifier si la géométrie est un Point
                    If pGeometry.GeometryType = esriGeometryType.esriGeometryPoint Then
                        'Définir le point
                        pPoint = CType(pGeometry, IPoint)
                        'Définir le M du point
                        pPoint.M = dM

                        'Vérifier si la géométrie est un Bag
                    ElseIf pGeometry.GeometryType = esriGeometryType.esriGeometryBag Then
                        'Interface utilisé pour accéder aux géométries
                        pGeomColl = CType(pGeometry, IGeometryCollection)
                        'Traiter toutes les géométries
                        For i = 0 To pGeomColl.GeometryCount - 1
                            'Traiter le M
                            Call TraiterM(pGeomColl.Geometry(i), dM)
                        Next

                        'Vérifier si la géométrie est un Multipoint,une Polyline ou un Polygon
                    Else
                        'Interface utilisé pour accéder aux sommets de la géométrie
                        pPointColl = CType(pGeometry, IPointCollection)
                        'Traiter tous les sommets de la géométrie
                        For i = 0 To pPointColl.PointCount - 1
                            'Interface pour définir le M
                            pPoint = pPointColl.Point(i)
                            'Définir le M du point
                            pPoint.M = dM
                            'Conserver les modifications
                            pPointColl.UpdatePoint(i, pPoint)
                        Next
                    End If
                End If

                'Si aucun M
            Else
                'Enlever le M
                pMAware.MAware = True
                pMAware.DropMs()
                pMAware.MAware = False
            End If

        Catch erreur As Exception
            Throw erreur
        Finally
            'Vider la mémoire
            pMAware = Nothing
            pGeomColl = Nothing
            pPointColl = Nothing
            pPoint = Nothing
        End Try
    End Sub

    ''' <summary> 
    ''' Cette fonction permet de retourner la définition de la géométrie à partir de la classe afin de vérifier la présence du Z et M.
    ''' </summary>
    ''' <param name="pFeatureClass"></param>
    ''' <returns>IGeometryDef</returns>
    Public Function RetournerGeometryDef(ByVal pFeatureClass As IFeatureClass) As IGeometryDef
        Dim shapeFieldName As String = pFeatureClass.ShapeFieldName
        Dim fields As IFields = pFeatureClass.Fields
        Dim geometryIndex As Integer = fields.FindField(shapeFieldName)
        Dim field As IField = fields.Field(geometryIndex)
        Dim geometryDef As IGeometryDef = field.GeometryDef
        Return geometryDef
    End Function

    '''<summary>
    '''Fonction qui permet de créer et retourner la topologie en mémoire des éléments entre une collection de FeatureLayer.
    '''</summary>
    '''
    '''<param name="pEnvelope">Interface ESRI contenant l'enveloppe de création de la topologie traitée.</param>
    '''<param name="qFeatureLayerColl">Interface ESRI contenant les FeatureLayers pour traiter la topologie.</param> 
    '''<param name="dTolerance">Tolerance de proximité.</param> 
    '''
    '''<returns>"ITopologyGraph" contenant la topologie des classes de données, "Nothing" sinon.</returns>
    '''
    Private Function CreerTopologyGraph(ByVal pEnvelope As IEnvelope, ByVal qFeatureLayerColl As Collection, ByVal dTolerance As Double) As ITopologyGraph4
        'Déclarer les variables de travail
        Dim qType As Type = Nothing                         'Interface contenant le type d'objet à créer.
        Dim oObjet As System.Object = Nothing               'Interface contenant l'objet correspondant à l'application.
        Dim pTopologyExt As ITopologyExtension = Nothing    'Interface contenant l'extension de la topologie.
        Dim pMapTopology As IMapTopology2 = Nothing         'Interface utilisé pour créer la topologie.
        Dim pTopologyGraph As ITopologyGraph4 = Nothing     'Interface contenant la topologie.
        Dim pFeatureLayer As IFeatureLayer = Nothing        'Interface contenant la classe de données.

        'Définir la valeur de retour par défaut
        CreerTopologyGraph = Nothing

        Try
            'Définir l'extension de topologie
            qType = Type.GetTypeFromProgID("esriEditorExt.TopologyExtension")
            oObjet = Activator.CreateInstance(qType)
            pTopologyExt = CType(oObjet, ITopologyExtension)

            'Définir l'interface pour créer la topologie
            pMapTopology = CType(pTopologyExt.MapTopology, IMapTopology2)

            'S'assurer que laliste des Layers est vide
            pMapTopology.ClearLayers()

            'Traiter tous les FeatureLayer présents
            For Each pFeatureLayer In qFeatureLayerColl
                'Ajouter le FeatureLayer à la topologie
                pMapTopology.AddLayer(pFeatureLayer)
            Next

            'Changer la référence spatiale selon l'enveloppe
            pMapTopology.SpatialReference = pEnvelope.SpatialReference

            'Définir la tolérance de connexion et de partage
            pMapTopology.ClusterTolerance = dTolerance

            'Interface pour construre la topologie
            pTopologyGraph = CType(pMapTopology.Cache, ITopologyGraph4)
            pTopologyGraph.SetEmpty()

            Try
                'Construire la topologie
                pTopologyGraph.Build(pEnvelope, False)
            Catch ex As OutOfMemoryException
                'Retourner une erreur de création de la topologie
                Throw New Exception("Incapable de créer la topologie : OutOfMemoryException")
            Catch ex As Exception
                'Retourner une erreur de création de la topologie
                Throw New Exception("Incapable de créer la topologie : " & ex.Message)
            End Try

            'Retourner la topologie
            CreerTopologyGraph = pTopologyGraph

        Catch ex As Exception
            'Message d'erreur
            Throw
        Finally
            'Vider la mémoire
            qType = Nothing
            oObjet = Nothing
            pTopologyExt = Nothing
            pTopologyGraph = Nothing
            pMapTopology = Nothing
            pFeatureLayer = Nothing
            'Récupération de la mémoire disponible
            GC.Collect()
        End Try
    End Function
End Module
