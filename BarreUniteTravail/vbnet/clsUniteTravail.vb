Imports ESRI.ArcGIS.Geometry
'**
'Nom de la composante : clsUniteTravail.vb
'
'''<summary>
''' Classe qui contient l'information sur l'unité de travail. Cette information est calculée complètement.
''' Une unité de travail peut être de différente catégorie. Il peut être de catégorie CANADA, 
''' SNRC (Système National de Référence Cartographique du Canada) ou autres. 
''' Par défaut, la catégorie est CANADA.
'''</summary>
'''
'''<remarks>
'''Auteur : Michel Pothier
'''Date : 16 décembre 2009
'''</remarks>
''' 
Public Class clsUniteTravail
    'Définition des variables globales de travail
    Protected gsCategorie As String = "CANADA"  'Numéro de l'unité de travail.
    Protected gsNumero As String = "CANADA"     'Numéro de l'unité de travail.
    Protected gyZoneUtm As Byte = 14            'Zone UTM de l'unité de travail.
    Protected giEchelle As Integer = 50000      'Échelle de l'unité de travail.
    Protected gdMeridienCentral As Double = ((gyZoneUtm - 1) * 6) - 177 'Méridien central de l'unité de travail.
    Protected gdLatitudeSE As Double = 40       'Latitude Sud-Est du polygone de l'unité de travail.
    Protected gdLongitudeSE As Double = -50     'Longitude Sud-Est du polygone de l'unité de travail.
    Protected gdLatitudeNW As Double = 85       'Latitude Nord-Ouest du polygone de l'unité de travail.
    Protected gdLongitudeNW As Double = -145    'Longitude Nord-Ouest du polygone de l'unité de travail.
    Protected gbEstValide As Boolean = True     'Indiquer si l'unité de travail est valide.
    Protected gsInformation As String = ""      'Texte contenant l'information qui décrit l'unité de travail.

#Region " Propriétés "
    '''<summary>
    '''Retourner la catégorie de l'unité de travail.
    '''</summary>
    ''' 
    '''<returns>Catégorie de l'unité de travail.</returns>
    ''' 
    Public ReadOnly Property Categorie() As String
        Get
            Categorie = gsCategorie
        End Get
    End Property

    '''<summary>
    '''Retourner le numéro de l'unité de travail.
    '''</summary>
    ''' 
    '''<returns>Numéro de l'unité de travail.</returns>
    ''' 
    Public ReadOnly Property Numero() As String
        Get
            Numero = gsNumero
        End Get
    End Property

    '''<summary>
    '''Retourner la zone UTM de l'unité de travail.
    '''</summary>
    ''' 
    '''<returns>Zone UTM de l'unité de travail.</returns>
    ''' 
    Public ReadOnly Property ZoneUTM() As Byte
        Get
            ZoneUTM = gyZoneUtm
        End Get
    End Property

    '''<summary>
    '''Retourner l'échelle de l'unité de travail.
    '''</summary>
    ''' 
    '''<returns>Échelle de l'unité de travail.</returns>
    ''' 
    Public ReadOnly Property Echelle() As Integer
        Get
            Echelle = giEchelle
        End Get
    End Property

    '''<summary>
    '''Retourner le méridien central de l'unité de travail.
    '''</summary>
    ''' 
    '''<returns>Méridien central de l'unité de travail.</returns>
    ''' 
    Public ReadOnly Property MeridienCentral() As Double
        Get
            MeridienCentral = gdMeridienCentral
        End Get
    End Property

    '''<summary>
    '''Retourner la latitude SE de l'unité de travail.
    '''</summary>
    ''' 
    '''<returns>Latitude SE de l'unité de travail.</returns>
    ''' 
    Public ReadOnly Property LatitudeSE() As Double
        Get
            LatitudeSE = gdLatitudeSE
        End Get
    End Property

    '''<summary>
    '''Retourner la longitude SE de l'unité de travail.
    '''</summary>
    ''' 
    '''<returns>Longitude SE de l'unité de travail.</returns>
    ''' 
    Public ReadOnly Property LongitudeSE() As Double
        Get
            LongitudeSE = gdLongitudeSE
        End Get
    End Property

    '''<summary>
    '''Retourner la latitude NW de l'unité de travail.
    '''</summary>
    ''' 
    '''<returns>Latitude NW de l'unité de travail.</returns>
    ''' 
    Public ReadOnly Property LatitudeNW() As Double
        Get
            LatitudeNW = gdLatitudeNW
        End Get
    End Property

    '''<summary>
    '''Retourner l'indice de validité de l'unité de travail.
    '''</summary>
    ''' 
    '''<returns>Indice de validité de l'unité de travail de l'unité de travail.</returns>
    ''' 
    Public ReadOnly Property EstValide() As Boolean
        Get
            EstValide = gbEstValide
        End Get
    End Property

    '''<summary>
    '''Retourner la longitude NW de l'unité de travail.
    '''</summary>
    ''' 
    '''<returns>Longitude NW de l'unité de travail.</returns>
    ''' 
    Public ReadOnly Property LongitudeNW() As Double
        Get
            LongitudeNW = gdLongitudeNW
        End Get
    End Property

    '''<summary>
    '''Retourner l'information de l'unité de travail.
    '''</summary>
    ''' 
    '''<returns>Information de l'unité de travail.</returns>
    ''' 
    Public ReadOnly Property Information() As String
        Get
            'Définir la description du SNRC
            gsInformation = "Unité de travail : " & Numero & vbCrLf
            gsInformation = gsInformation & "Catégorie : " & Categorie() & vbCrLf
            gsInformation = gsInformation & "Échelle : " & Echelle & vbCrLf
            gsInformation = gsInformation & "Zone Utm : " & ZoneUTM & vbCrLf
            gsInformation = gsInformation & "Méridien central : Longitude=" & Format(MeridienCentral, "###0.00") & vbCrLf
            gsInformation = gsInformation & "Latitude SE :" & Format(LatitudeSE, "###0.00") & vbCrLf
            gsInformation = gsInformation & "Longitude SE :" & Format(LongitudeSE, "###0.00") & vbCrLf
            gsInformation = gsInformation & "Latitude NW :" & Format(LatitudeNW, "###0.00") & vbCrLf
            gsInformation = gsInformation & "Longitude NW :" & Format(LongitudeNW, "###0.00") & vbCrLf
            gsInformation = gsInformation & "Indice de validité : " & EstValide & vbCrLf
            'Retourner l'information
            Information = gsInformation
        End Get
    End Property
#End Region

#Region " Routines et fonctions publiques "
    '''<summary>
    '''Fonction qui permet de retourner la géométrie de type surface de l'unité de travail en coordonnées géographiques.
    '''</summary>
    '''
    '''<param name="iDeltaMinute">Distance entre chaque sommet du polygone en minute.</param>
    ''' 
    '''<returns>IPolygon correspondant à la géométrie de type surface de l'unité de travail, Nothing sinon.</returns>
    '''
    Public Function PolygoneGeo(ByVal iDeltaMinute As Integer) As IPolygon
        'Déclarer les variables de travail
        Dim pPolygon As IPolygon = Nothing      'Interface ESRI contenant le polygone de l'unité de travail.
        Dim pPointColl As IPointCollection = Nothing    'Interface ESRI utilisée pour créer le polygone.
        Dim pPoint As ESRI.ArcGIS.Geometry.IPoint = Nothing          'Interface ESRI utilisée pour ajouter un point dans le poygone.
        Dim dLatitude As Double = Nothing       'Latitude de travail.
        Dim dLongitude As Double = Nothing      'Longitude de travail.
        Dim dDeltaDegres As Double = Nothing    'Distance entre les sommets du polygone en degrés.

        Try
            'Définir la valeur de retour par défaut
            PolygoneGeo = Nothing

            'Valider le DeltaMinute
            'If iDeltaMinute <> 30 And iDeltaMinute <> 60 Then
            '    Exit Function
            'End If

            'Redéfinir le Delta en dégrés
            dDeltaDegres = iDeltaMinute / 60

            'Définir un nouveau polygone vide
            pPointColl = New Polygon

            'Calcul de la limite SUD
            dLatitude = gdLatitudeSE
            dLongitude = gdLongitudeSE
            Do While (dLongitude > gdLongitudeNW)
                'Ajouter le nouveau point
                pPoint = New Point
                pPoint.PutCoords(dLongitude, dLatitude)
                pPointColl.AddPoint(pPoint)
                dLongitude = dLongitude - dDeltaDegres
            Loop

            'Calcul de la limite OUEST
            dLatitude = gdLatitudeSE
            dLongitude = gdLongitudeNW
            Do While (dLatitude < gdLatitudeNW)
                'Ajouter le nouveau point
                pPoint = New Point
                pPoint.PutCoords(dLongitude, dLatitude)
                pPointColl.AddPoint(pPoint)
                dLatitude = dLatitude + dDeltaDegres
            Loop

            'Calcul de la limite NORD
            dLatitude = gdLatitudeNW
            dLongitude = gdLongitudeNW
            Do While (dLongitude < gdLongitudeSE)
                'Ajouter le nouveau point
                pPoint = New Point
                pPoint.PutCoords(dLongitude, dLatitude)
                pPointColl.AddPoint(pPoint)
                dLongitude = dLongitude + dDeltaDegres
            Loop

            'Calcul de la limite EST
            dLatitude = gdLatitudeNW
            dLongitude = gdLongitudeSE
            Do While (dLatitude > gdLatitudeSE)
                'Ajouter le nouveau point
                pPoint = New Point
                pPoint.PutCoords(dLongitude, dLatitude)
                pPointColl.AddPoint(pPoint)
                dLatitude = dLatitude - dDeltaDegres
            Loop

            'Définir et fermer le polygone en coordonnées géographique
            pPolygon = CType(pPointColl, IPolygon)
            pPolygon.Close()

            'Définir la référence spatiale
            pPolygon.SpatialReference = ReferenceSpatialeGeoNad83()

            'Retourner le résultat
            PolygoneGeo = pPolygon

        Finally
            'Vider la mémoire
            pPolygon = Nothing
            pPointColl = Nothing
            pPoint = Nothing
            dDeltaDegres = Nothing
            dLatitude = Nothing
            dLongitude = Nothing
        End Try
    End Function

    '''<summary>
    '''Fonction qui permet de retourner la limite SUD de l'unité de travail en coordonnées géographiques.
    '''</summary>
    '''
    '''<param name="iDeltaMinute">Distance entre chaque sommet de la ligne en minute.</param>
    ''' 
    '''<returns>IPolyline correspondant à la géométrie de type ligne de la limite SUD, Nothing sinon.</returns>
    '''
    Public Function LimiteSud(ByVal iDeltaMinute As Integer) As IPolyline
        'Définir les varaibles de travail
        Dim pPolyline As IPolyline = Nothing    'Interface ESRI contenant la ligne EST de l'unité de travail.
        Dim pPointColl As IPointCollection = Nothing 'Interface ESRI utilisée pour créer la ligne.
        Dim pPoint As ESRI.ArcGIS.Geometry.IPoint = Nothing          'Interface ESRI utilisée pour ajouter un point dans la ligne.
        Dim dDeltaDegres As Double = Nothing    'Distance entre les sommets de la ligne en degrés.
        Dim dLatitude As Double = Nothing       'Latitude de travail.
        Dim dLongitude As Double = Nothing      'Longitude de travail.

        Try
            'Définir la valeur par défaut à retourner
            LimiteSud = Nothing

            'Valider le DeltaMinute
            'If iDeltaMinute <> 30 And iDeltaMinute <> 60 Then
            '    Exit Function
            'End If

            'Redéfinir le Delta en dégrés
            dDeltaDegres = iDeltaMinute / 60

            'Définir une nouvelle polyligne vide
            pPointColl = New Polyline

            'Calcul de la limite SUD
            dLatitude = gdLatitudeSE
            dLongitude = gdLongitudeSE
            Do While (dLongitude >= gdLongitudeNW)
                'Ajouter le nouveau point
                pPoint = New Point
                pPoint.PutCoords(dLongitude, dLatitude)
                pPointColl.AddPoint(pPoint)
                dLongitude = dLongitude - dDeltaDegres
            Loop

            'Définir la référence spatiale
            pPolyline = CType(pPointColl, IPolyline)
            pPolyline.SpatialReference = ReferenceSpatialeGeoNad83()

            'Retourner la limite
            LimiteSud = pPolyline

        Finally
            'Vider la mémoire
            pPolyline = Nothing
            pPointColl = Nothing
            pPoint = Nothing
            dDeltaDegres = Nothing
            dLatitude = Nothing
            dLongitude = Nothing
        End Try
    End Function

    '''<summary>
    '''Fonction qui permet de retourner la limite OUEST de l'unité de travail en coordonnées géographiques.
    '''</summary>
    '''
    '''<param name="iDeltaMinute">Distance entre chaque sommet de la ligne en minute.</param>
    ''' 
    '''<returns>IPolyline correspondant à la géométrie de type ligne de la limite OUEST, Nothing sinon.</returns>
    '''
    Public Function LimiteOuest(ByVal iDeltaMinute As Integer) As IPolyline
        'Définir les varaibles de travail
        Dim pPolyline As IPolyline = Nothing    'Interface ESRI contenant la ligne EST de l'unité de travail.
        Dim pPointColl As IPointCollection = Nothing   'Interface ESRI utilisée pour créer la ligne.
        Dim pPoint As ESRI.ArcGIS.Geometry.IPoint = Nothing          'Interface ESRI utilisée pour ajouter un point dans la ligne.
        Dim dDeltaDegres As Double = Nothing    'Distance entre les sommets de la ligne en degrés.
        Dim dLatitude As Double = Nothing       'Latitude de travail.
        Dim dLongitude As Double = Nothing      'Longitude de travail.

        Try
            'Définir la valeur par défaut à retourner
            LimiteOuest = Nothing

            'Valider le DeltaMinute
            'If iDeltaMinute <> 30 And iDeltaMinute <> 60 Then
            '    Exit Function
            'End If

            'Redéfinir le Delta en dégrés
            dDeltaDegres = iDeltaMinute / 60

            'Définir une nouvelle polyligne vide
            pPointColl = New Polyline

            'Calcul de la limite OUEST
            dLatitude = gdLatitudeSE
            dLongitude = gdLongitudeNW
            Do While (dLatitude <= gdLatitudeNW)
                'Ajouter le nouveau point
                pPoint = New Point
                pPoint.PutCoords(dLongitude, dLatitude)
                pPointColl.AddPoint(pPoint)
                dLatitude = dLatitude + dDeltaDegres
            Loop

            'Définir la référence spatiale
            pPolyline = CType(pPointColl, IPolyline)
            pPolyline.SpatialReference = ReferenceSpatialeGeoNad83()

            'Retourner la limite
            LimiteOuest = pPolyline

        Finally
            'Vider la mémoire
            pPolyline = Nothing
            pPointColl = Nothing
            pPoint = Nothing
            dDeltaDegres = Nothing
            dLatitude = Nothing
            dLongitude = Nothing
        End Try
    End Function

    '''<summary>
    '''Fonction qui permet de retourner la limite NORD de l'unité de travail en coordonnées géographiques.
    '''</summary>
    '''
    '''<param name="iDeltaMinute">Distance entre chaque sommet de la ligne en minute.</param>
    ''' 
    '''<returns>IPolyline correspondant à la géométrie de type ligne de la limite NORD, Nothing sinon.</returns>
    '''
    Public Function LimiteNord(ByVal iDeltaMinute As Integer) As IPolyline
        'Définir les varaibles de travail
        Dim pPolyline As IPolyline = Nothing    'Interface ESRI contenant la ligne EST de l'unité de travail.
        Dim pPointColl As IPointCollection = Nothing   'Interface ESRI utilisée pour créer la ligne.
        Dim pPoint As ESRI.ArcGIS.Geometry.IPoint = Nothing          'Interface ESRI utilisée pour ajouter un point dans la ligne.
        Dim dDeltaDegres As Double = Nothing    'Distance entre les sommets de la ligne en degrés.
        Dim dLatitude As Double = Nothing       'Latitude de travail.
        Dim dLongitude As Double = Nothing      'Longitude de travail.

        Try
            'Définir la valeur par défaut à retourner
            LimiteNord = Nothing

            'Valider le DeltaMinute
            'If iDeltaMinute <> 30 And iDeltaMinute <> 60 Then
            '    Exit Function
            'End If

            'Redéfinir le Delta en dégrés
            dDeltaDegres = iDeltaMinute / 60

            'Définir une nouvelle polyligne vide
            pPointColl = New Polyline

            'Calcul de la limite NORD
            dLatitude = gdLatitudeNW
            dLongitude = gdLongitudeNW
            Do While (dLongitude <= gdLongitudeSE)
                'Ajouter le nouveau point
                pPoint = New Point
                pPoint.PutCoords(dLongitude, dLatitude)
                pPointColl.AddPoint(pPoint)
                dLongitude = dLongitude + dDeltaDegres
            Loop

            'Définir la référence spatiale
            pPolyline = CType(pPointColl, IPolyline)
            pPolyline.SpatialReference = ReferenceSpatialeGeoNad83()

            'Retourner la limite
            LimiteNord = pPolyline

        Finally
            'Vider la mémoire
            pPolyline = Nothing
            pPointColl = Nothing
            pPoint = Nothing
            dDeltaDegres = Nothing
            dLatitude = Nothing
            dLongitude = Nothing
        End Try
    End Function

    '''<summary>
    '''Fonction qui permet de retourner la limite EST de l'unité de travail en coordonnées géographiques.
    '''</summary>
    '''
    '''<param name="iDeltaMinute">Distance entre chaque sommet de la ligne en minute.</param>
    ''' 
    '''<returns>IPolyline correspondant à la géométrie de type ligne de la limite EST, Nothing sinon.</returns>
    '''
    Public Function LimiteEst(ByVal iDeltaMinute As Integer) As IPolyline
        'Définir les varaibles de travail
        Dim pPolyline As IPolyline = Nothing            'Interface ESRI contenant la ligne EST de l'unité de travail.
        Dim pPointColl As IPointCollection = Nothing    'Interface ESRI utilisée pour créer la ligne.
        Dim pPoint As ESRI.ArcGIS.Geometry.IPoint = Nothing 'Interface ESRI utilisée pour ajouter un point dans la ligne.
        Dim dDeltaDegres As Double = Nothing    'Distance entre les sommets de la ligne en degrés.
        Dim dLatitude As Double = Nothing       'Latitude de travail.
        Dim dLongitude As Double = Nothing      'Longitude de travail.

        Try
            'Définir la valeur par défaut à retourner
            LimiteEst = Nothing

            'Valider le DeltaMinute
            'If iDeltaMinute <> 30 And iDeltaMinute <> 60 Then
            '    Exit Function
            'End If

            'Redéfinir le Delta en dégrés
            dDeltaDegres = iDeltaMinute / 60

            'Définir une nouvelle polyligne vide
            pPointColl = New Polyline

            'Calcul de la limite EST
            dLatitude = gdLatitudeNW
            dLongitude = gdLongitudeSE
            Do While (dLatitude >= gdLatitudeSE)
                'Ajouter le nouveau point
                pPoint = New Point
                pPoint.PutCoords(dLongitude, dLatitude)
                pPointColl.AddPoint(pPoint)
                dLatitude = dLatitude - dDeltaDegres
            Loop

            'Définir la référence spatiale
            pPolyline = CType(pPointColl, IPolyline)
            pPolyline.SpatialReference = ReferenceSpatialeGeoNad83()

            'Retourner la limite
            LimiteEst = pPolyline

        Finally
            'Vider la mémoire
            pPolyline = Nothing
            pPointColl = Nothing
            pPoint = Nothing
            dDeltaDegres = Nothing
            dLatitude = Nothing
            dLongitude = Nothing
        End Try
    End Function
#End Region

#Region " Routines et fonctions publiques utilitaires"
    '''<summary>
    '''Fonction qui permet de retourner une référence spatiale en coordonnées UTM avec le datum Nad83 
    '''et selon le numéro ESRI de la projection.
    '''</summary>
    '''
    '''<param name="ZoneUTM">Numéro de la zone UTM désiré. Par défaut, la zone UTM sera celle calculé du SNRC.</param>
    ''' 
    '''<returns>ISpatialReference si la zone UTM est valide, sinon "Nothing"</returns>
    '''
    Public Function ProjectionUtmNad83(Optional ByVal ZoneUTM As Byte = 0) As ISpatialReference
        'Déclarer les variables de travail
        Dim pSpRFc As SpatialReferenceEnvironment = Nothing 'Interface ESRI pour créer une référence spatiale vide.
        Dim pPCS As IProjectedCoordinateSystem = Nothing    'Interface ESRI contenant le système de coorsonnées UTM.
        Dim esriSRProjCSType As Integer = Nothing           'Numéro ESRI de la projection UTM en NAD83.

        Try
            'Définir la valeur de retour par défaut
            ProjectionUtmNad83 = Nothing

            'Définir la zone UTM du SNRC par défaut
            If ZoneUTM = 0 Then ZoneUTM = gyZoneUtm

            'Retourner le numéro de projection ESRI selon la zone UTM spécifiée
            esriSRProjCSType = NumeroProjectionUtmESRI(ZoneUTM)

            'Vérifier si le numéro de projection est valide
            If esriSRProjCSType > 0 Then
                'Interface pour effectuer la création des références spatiales
                pSpRFc = New SpatialReferenceEnvironment

                'Définir la projection selon la zone UTM spécifiée
                pPCS = pSpRFc.CreateProjectedCoordinateSystem(esriSRProjCSType)

                'Retourner la référence spatiale
                ProjectionUtmNad83 = pPCS
            End If

        Finally
            'Vider la mémoire
            pSpRFc = Nothing
            pPCS = Nothing
            esriSRProjCSType = Nothing
        End Try
    End Function

    '''<summary>
    '''Fonction qui permet de retourner le numéro de projection UTM de ESRI selon la zone spécifiée.
    '''</summary>
    '''
    '''<param name="ZoneUTM">Numéro de la zone UTM désiré.</param>
    ''' 
    '''<returns>Numéro de projection UTM de ESRI selon la zone spécifiée, ou 0 si la zone n'est pas entre 6 et 22</returns>
    '''
    Public Function NumeroProjectionUtmESRI(ByVal ZoneUTM As Byte) As Integer
        'Retourner 0 si zone invalide
        NumeroProjectionUtmESRI = 0

        'Définir la projection UTM selon la zone du Snrc
        Select Case ZoneUTM
            Case 6
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_6N
            Case 7
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_7N
            Case 8
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_8N
            Case 9
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_9N
            Case 10
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_10N
            Case 11
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_11N
            Case 12
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_12N
            Case 13
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_13N
            Case 14
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_14N
            Case 15
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_15N
            Case 16
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_16N
            Case 17
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_17N
            Case 18
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_18N
            Case 19
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_19N
            Case 20
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_20N
            Case 21
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_21N
            Case 22
                NumeroProjectionUtmESRI = esriSRProjCSType.esriSRProjCS_NAD1983UTM_22N
        End Select
    End Function

    '''<summary>
    '''Fonction qui permet de retourner la référence spatiale en coordonnées géographique avec le datum Nad83.
    '''</summary>
    ''' 
    '''<returns>ISpatialReference correspondant au système de coordonnées géographique Nad83, Nothing sinon.</returns>
    '''
    Public Function ReferenceSpatialeGeoNad83() As ISpatialReference
        'Déclarer les variables de travail
        Dim pSpRFc As SpatialReferenceEnvironment = Nothing 'Interface ESRI pour créer une référence spatiale vide.
        Dim pGCS As IGeographicCoordinateSystem = Nothing   'Interface ESRI contenant le système de coorsonnées Géographique.

        Try
            'Interface pour effectuer la création des références spatiales
            pSpRFc = New SpatialReferenceEnvironment

            'Définir le système de coordonnée géographique
            pGCS = pSpRFc.CreateGeographicCoordinateSystem(esriSRGeoCSType.esriSRGeoCS_NAD1983)

            'Retourner la référence spatiale
            ReferenceSpatialeGeoNad83 = pGCS

        Finally
            'Vider la mémoire
            pSpRFc = Nothing
            pGCS = Nothing
        End Try
    End Function
#End Region
End Class

