Imports ArcGIS.Core.Geometry
Imports ArcGIS.Desktop.Framework.Threading.Tasks

'**
'Nom de la composante : clsUniteTravail.vb
'
'''<summary>
''' Classe qui contient l'information sur l'unit� de travail. Cette information est calcul�e compl�tement.
''' Une unit� de travail peut �tre de diff�rente cat�gorie. Il peut �tre de cat�gorie CANADA, 
''' SNRC (Syst�me National de R�f�rence Cartographique du Canada) ou autres. 
''' Par d�faut, la cat�gorie est CANADA.
'''</summary>
'''
'''<remarks>
'''Auteur : Michel Pothier
'''Date : 16 d�cembre 2009
'''</remarks>
''' 
Public Class clsUniteTravail
    'D�finition des variables globales de travail
    Protected gsCategorie As String = "CANADA"  'Num�ro de l'unit� de travail.
    Protected gsNumero As String = "CANADA"     'Num�ro de l'unit� de travail.
    Protected gyZoneUtm As Byte = 14            'Zone UTM de l'unit� de travail.
    Protected giEchelle As Integer = 50000      '�chelle de l'unit� de travail.
    Protected gdMeridienCentral As Double = ((gyZoneUtm - 1) * 6) - 177 'M�ridien central de l'unit� de travail.
    Protected gdLatitudeSE As Double = 40       'Latitude Sud-Est du polygone de l'unit� de travail.
    Protected gdLongitudeSE As Double = -50     'Longitude Sud-Est du polygone de l'unit� de travail.
    Protected gdLatitudeNW As Double = 85       'Latitude Nord-Ouest du polygone de l'unit� de travail.
    Protected gdLongitudeNW As Double = -145    'Longitude Nord-Ouest du polygone de l'unit� de travail.
    Protected gbEstValide As Boolean = True     'Indiquer si l'unit� de travail est valide.
    Protected gsInformation As String = ""      'Texte contenant l'information qui d�crit l'unit� de travail.

#Region " Propri�t�s "
    '''<summary>
    '''Retourner la cat�gorie de l'unit� de travail.
    '''</summary>
    ''' 
    '''<returns>Cat�gorie de l'unit� de travail.</returns>
    ''' 
    Public ReadOnly Property Categorie() As String
        Get
            Categorie = gsCategorie
        End Get
    End Property

    '''<summary>
    '''Retourner le num�ro de l'unit� de travail.
    '''</summary>
    ''' 
    '''<returns>Num�ro de l'unit� de travail.</returns>
    ''' 
    Public ReadOnly Property Numero() As String
        Get
            Numero = gsNumero
        End Get
    End Property

    '''<summary>
    '''Retourner la zone UTM de l'unit� de travail.
    '''</summary>
    ''' 
    '''<returns>Zone UTM de l'unit� de travail.</returns>
    ''' 
    Public ReadOnly Property ZoneUTM() As Byte
        Get
            ZoneUTM = gyZoneUtm
        End Get
    End Property

    '''<summary>
    '''Retourner l'�chelle de l'unit� de travail.
    '''</summary>
    ''' 
    '''<returns>�chelle de l'unit� de travail.</returns>
    ''' 
    Public ReadOnly Property Echelle() As Integer
        Get
            Echelle = giEchelle
        End Get
    End Property

    '''<summary>
    '''Retourner le m�ridien central de l'unit� de travail.
    '''</summary>
    ''' 
    '''<returns>M�ridien central de l'unit� de travail.</returns>
    ''' 
    Public ReadOnly Property MeridienCentral() As Double
        Get
            MeridienCentral = gdMeridienCentral
        End Get
    End Property

    '''<summary>
    '''Retourner la latitude SE de l'unit� de travail.
    '''</summary>
    ''' 
    '''<returns>Latitude SE de l'unit� de travail.</returns>
    ''' 
    Public ReadOnly Property LatitudeSE() As Double
        Get
            LatitudeSE = gdLatitudeSE
        End Get
    End Property

    '''<summary>
    '''Retourner la longitude SE de l'unit� de travail.
    '''</summary>
    ''' 
    '''<returns>Longitude SE de l'unit� de travail.</returns>
    ''' 
    Public ReadOnly Property LongitudeSE() As Double
        Get
            LongitudeSE = gdLongitudeSE
        End Get
    End Property

    '''<summary>
    '''Retourner la latitude NW de l'unit� de travail.
    '''</summary>
    ''' 
    '''<returns>Latitude NW de l'unit� de travail.</returns>
    ''' 
    Public ReadOnly Property LatitudeNW() As Double
        Get
            LatitudeNW = gdLatitudeNW
        End Get
    End Property

    '''<summary>
    '''Retourner l'indice de validit� de l'unit� de travail.
    '''</summary>
    ''' 
    '''<returns>Indice de validit� de l'unit� de travail de l'unit� de travail.</returns>
    ''' 
    Public ReadOnly Property EstValide() As Boolean
        Get
            EstValide = gbEstValide
        End Get
    End Property

    '''<summary>
    '''Retourner la longitude NW de l'unit� de travail.
    '''</summary>
    ''' 
    '''<returns>Longitude NW de l'unit� de travail.</returns>
    ''' 
    Public ReadOnly Property LongitudeNW() As Double
        Get
            LongitudeNW = gdLongitudeNW
        End Get
    End Property

    '''<summary>
    '''Retourner l'information de l'unit� de travail.
    '''</summary>
    ''' 
    '''<returns>Information de l'unit� de travail.</returns>
    ''' 
    Public ReadOnly Property Information() As String
        Get
            'D�finir la description du SNRC
            gsInformation = "Unit� de travail : " & Numero & vbCrLf
            gsInformation = gsInformation & "Cat�gorie : " & Categorie() & vbCrLf
            gsInformation = gsInformation & "�chelle : " & Echelle & vbCrLf
            gsInformation = gsInformation & "Zone Utm : " & ZoneUTM & vbCrLf
            gsInformation = gsInformation & "M�ridien central : Longitude=" & Format(MeridienCentral, "###0.00") & vbCrLf
            gsInformation = gsInformation & "Latitude SE :" & Format(LatitudeSE, "###0.00") & vbCrLf
            gsInformation = gsInformation & "Longitude SE :" & Format(LongitudeSE, "###0.00") & vbCrLf
            gsInformation = gsInformation & "Latitude NW :" & Format(LatitudeNW, "###0.00") & vbCrLf
            gsInformation = gsInformation & "Longitude NW :" & Format(LongitudeNW, "###0.00") & vbCrLf
            gsInformation = gsInformation & "Indice de validit� : " & EstValide & vbCrLf
            'Retourner l'information
            Information = gsInformation
        End Get
    End Property
#End Region

#Region " Routines et fonctions publiques "
    '''<summary>
    '''Fonction qui permet de retourner la g�om�trie de type surface de l'unit� de travail en coordonn�es g�ographiques.
    '''</summary>
    '''
    '''<param name="iDeltaMinute">Distance entre chaque sommet du polygone en minute.</param>
    ''' 
    '''<returns>IPolygon correspondant � la g�om�trie de type surface de l'unit� de travail, Nothing sinon.</returns>
    '''
    Public Function PolygoneGeo(ByVal iDeltaMinute As Integer) As Polygon
        'D�clarer les variables de travail
        Dim dLatitude As Double = Nothing       'Latitude de travail.
        Dim dLongitude As Double = Nothing      'Longitude de travail.
        Dim dDeltaDegres As Double = Nothing    'Distance entre les sommets du polygone en degr�s.

        'D�finir la valeur de retour par d�faut
        PolygoneGeo = Nothing

        'Valider le DeltaMinute
        'If iDeltaMinute <> 30 And iDeltaMinute <> 60 Then
        '    Exit Function
        'End If

        'Red�finir le Delta en d�gr�s
        dDeltaDegres = iDeltaMinute / 60

        'D�finir une nouvelle liste de sommets vide
        Dim pListePoint = New List(Of MapPoint)

        'Calcul de la limite SUD
        dLatitude = gdLatitudeSE
        dLongitude = gdLongitudeSE
        Do While (dLongitude > gdLongitudeNW)
            'Ajouter le nouveau point
            pListePoint.Add(MapPointBuilder.CreateMapPoint(dLongitude, dLatitude))
            'D�finir la prochaine longitude
            dLongitude = dLongitude - dDeltaDegres
        Loop

        'Calcul de la limite OUEST
        dLatitude = gdLatitudeSE
        dLongitude = gdLongitudeNW
        Do While (dLatitude < gdLatitudeNW)
            'Ajouter le nouveau point
            pListePoint.Add(MapPointBuilder.CreateMapPoint(dLongitude, dLatitude))
            'D�finir la prochaine latitude
            dLatitude = dLatitude + dDeltaDegres
        Loop

        'Calcul de la limite NORD
        dLatitude = gdLatitudeNW
        dLongitude = gdLongitudeNW
        Do While (dLongitude < gdLongitudeSE)
            'Ajouter le nouveau point
            pListePoint.Add(MapPointBuilder.CreateMapPoint(dLongitude, dLatitude))
            'D�finir la prochaine longitude
            dLongitude = dLongitude + dDeltaDegres
        Loop

        'Calcul de la limite EST
        dLatitude = gdLatitudeNW
        dLongitude = gdLongitudeSE
        Do While (dLatitude > gdLatitudeSE)
            'Ajouter le nouveau point
            pListePoint.Add(MapPointBuilder.CreateMapPoint(dLongitude, dLatitude))
            'D�finir la prochaine latitude
            dLatitude = dLatitude - dDeltaDegres
        Loop

        'Retourner le r�sultat
        PolygoneGeo = PolygonBuilder.CreatePolygon(pListePoint, ReferenceSpatialeGeoNad83())
    End Function

    '''<summary>
    '''Fonction qui permet de retourner la limite SUD de l'unit� de travail en coordonn�es g�ographiques.
    '''</summary>
    '''
    '''<param name="iDeltaMinute">Distance entre chaque sommet de la ligne en minute.</param>
    ''' 
    '''<returns>IPolyline correspondant � la g�om�trie de type ligne de la limite SUD, Nothing sinon.</returns>
    '''
    Public Function LimiteSud(ByVal iDeltaMinute As Integer) As Polyline
        'D�finir les varaibles de travail
        Dim dDeltaDegres As Double = Nothing    'Distance entre les sommets de la ligne en degr�s.
        Dim dLatitude As Double = Nothing       'Latitude de travail.
        Dim dLongitude As Double = Nothing      'Longitude de travail.

        'D�finir la valeur par d�faut � retourner
        LimiteSud = Nothing

        'Valider le DeltaMinute
        'If iDeltaMinute <> 30 And iDeltaMinute <> 60 Then
        '    Exit Function
        'End If

        'Red�finir le Delta en d�gr�s
        dDeltaDegres = iDeltaMinute / 60

        'D�finir une nouvelle liste de sommets vide
        Dim pListePoint = New List(Of MapPoint)

        'Calcul de la limite SUD
        dLatitude = gdLatitudeSE
        dLongitude = gdLongitudeSE
        Do While (dLongitude >= gdLongitudeNW)
            'Ajouter le nouveau point
            pListePoint.Add(MapPointBuilder.CreateMapPoint(dLongitude, dLatitude))
            'D�finir la prochaine longitude
            dLongitude = dLongitude - dDeltaDegres
        Loop

        'Retourner la limite
        LimiteSud = PolylineBuilder.CreatePolyline(pListePoint, ReferenceSpatialeGeoNad83())
    End Function

    '''<summary>
    '''Fonction qui permet de retourner la limite OUEST de l'unit� de travail en coordonn�es g�ographiques.
    '''</summary>
    '''
    '''<param name="iDeltaMinute">Distance entre chaque sommet de la ligne en minute.</param>
    ''' 
    '''<returns>IPolyline correspondant � la g�om�trie de type ligne de la limite OUEST, Nothing sinon.</returns>
    '''
    Public Function LimiteOuest(ByVal iDeltaMinute As Integer) As Polyline
        'D�finir les varaibles de travail
        Dim dDeltaDegres As Double = Nothing    'Distance entre les sommets de la ligne en degr�s.
        Dim dLatitude As Double = Nothing       'Latitude de travail.
        Dim dLongitude As Double = Nothing      'Longitude de travail.

        'D�finir la valeur par d�faut � retourner
        LimiteOuest = Nothing

        'Valider le DeltaMinute
        'If iDeltaMinute <> 30 And iDeltaMinute <> 60 Then
        '    Exit Function
        'End If

        'Red�finir le Delta en d�gr�s
        dDeltaDegres = iDeltaMinute / 60

        'D�finir une nouvelle liste de sommets vide
        Dim pListePoint = New List(Of MapPoint)

        'Calcul de la limite OUEST
        dLatitude = gdLatitudeSE
        dLongitude = gdLongitudeNW
        Do While (dLatitude <= gdLatitudeNW)
            'Ajouter le nouveau point
            pListePoint.Add(MapPointBuilder.CreateMapPoint(dLongitude, dLatitude))
            'D�finir la prochaine latitude
            dLatitude = dLatitude + dDeltaDegres
        Loop

        'Retourner la limite
        LimiteOuest = PolylineBuilder.CreatePolyline(pListePoint, ReferenceSpatialeGeoNad83())
    End Function

    '''<summary>
    '''Fonction qui permet de retourner la limite NORD de l'unit� de travail en coordonn�es g�ographiques.
    '''</summary>
    '''
    '''<param name="iDeltaMinute">Distance entre chaque sommet de la ligne en minute.</param>
    ''' 
    '''<returns>IPolyline correspondant � la g�om�trie de type ligne de la limite NORD, Nothing sinon.</returns>
    '''
    Public Function LimiteNord(ByVal iDeltaMinute As Integer) As Polyline
        'D�finir les varaibles de travail
        Dim dDeltaDegres As Double = Nothing    'Distance entre les sommets de la ligne en degr�s.
        Dim dLatitude As Double = Nothing       'Latitude de travail.
        Dim dLongitude As Double = Nothing      'Longitude de travail.

        'D�finir la valeur par d�faut � retourner
        LimiteNord = Nothing

            'Valider le DeltaMinute
            'If iDeltaMinute <> 30 And iDeltaMinute <> 60 Then
            '    Exit Function
            'End If

            'Red�finir le Delta en d�gr�s
            dDeltaDegres = iDeltaMinute / 60

            'D�finir une nouvelle liste de sommets vide
            Dim pListePoint = New List(Of MapPoint)

            'Calcul de la limite NORD
            dLatitude = gdLatitudeNW
            dLongitude = gdLongitudeNW
            Do While (dLongitude <= gdLongitudeSE)
                'Ajouter le nouveau point
                pListePoint.Add(MapPointBuilder.CreateMapPoint(dLongitude, dLatitude))
                'D�finir la prochaine longitude
                dLongitude = dLongitude + dDeltaDegres
            Loop

        'Retourner la limite
        LimiteNord = PolylineBuilder.CreatePolyline(pListePoint, ReferenceSpatialeGeoNad83())
    End Function

    '''<summary>
    '''Fonction qui permet de retourner la limite EST de l'unit� de travail en coordonn�es g�ographiques.
    '''</summary>
    '''
    '''<param name="iDeltaMinute">Distance entre chaque sommet de la ligne en minute.</param>
    ''' 
    '''<returns>IPolyline correspondant � la g�om�trie de type ligne de la limite EST, Nothing sinon.</returns>
    '''
    Public Function LimiteEst(ByVal iDeltaMinute As Integer) As Polyline
        'D�finir les varaibles de travail
        Dim dDeltaDegres As Double = Nothing    'Distance entre les sommets de la ligne en degr�s.
        Dim dLatitude As Double = Nothing       'Latitude de travail.
        Dim dLongitude As Double = Nothing      'Longitude de travail.

        'D�finir la valeur par d�faut � retourner
        LimiteEst = Nothing

        'Valider le DeltaMinute
        'If iDeltaMinute <> 30 And iDeltaMinute <> 60 Then
        '    Exit Function
        'End If

        'Red�finir le Delta en d�gr�s
        dDeltaDegres = iDeltaMinute / 60

        'D�finir une nouvelle liste de sommets vide
        Dim pListePoint = New List(Of MapPoint)

        'Calcul de la limite EST
        dLatitude = gdLatitudeNW
        dLongitude = gdLongitudeSE
        Do While (dLatitude >= gdLatitudeSE)
            'Ajouter le nouveau point
            pListePoint.Add(MapPointBuilder.CreateMapPoint(dLongitude, dLatitude))
            'D�finir la prochaine latitude
            dLatitude = dLatitude - dDeltaDegres
        Loop

        'Retourner la limite
        LimiteEst = PolylineBuilder.CreatePolyline(pListePoint, ReferenceSpatialeGeoNad83())
    End Function
#End Region

#Region " Routines et fonctions publiques utilitaires"
    '''<summary>
    '''Fonction qui permet de retourner une r�f�rence spatiale en coordonn�es UTM avec le datum Nad83 
    '''et selon le num�ro ESRI de la projection.
    '''</summary>
    '''
    '''<param name="ZoneUTM">Num�ro de la zone UTM d�sir�. Par d�faut, la zone UTM sera celle calcul� du SNRC.</param>
    ''' 
    '''<returns>ISpatialReference si la zone UTM est valide, sinon "Nothing"</returns>
    '''
    Public Function ProjectionUtmNad83(Optional ByVal ZoneUTM As Byte = 0) As SpatialReference
        'Par d�faut, elle est invalide
        ProjectionUtmNad83 = Nothing

        'Ex�cuter la fonction qui permet d'ex�cuter l'affichage
        Dim identifyResult = QueuedTask.Run(
            Function()
                'Cr�ation de la r�f�rence spatiale "NAD 83 UTM Zone __N"
                Dim spatialReferenceBuilder = New SpatialReferenceBuilder(NumeroProjectionUtmESRI(ZoneUTM))

                'Retourner la r�f�rence spatiale
                ProjectionUtmNad83 = spatialReferenceBuilder.ToSpatialReference

                'Retourner le succ�s du traitement
                Return True
            End Function)
    End Function

    '''<summary>
    '''Fonction qui permet de retourner le num�ro de projection UTM de ESRI selon la zone sp�cifi�e.
    '''</summary>
    '''
    '''<param name="ZoneUTM">Num�ro de la zone UTM d�sir�.</param>
    ''' 
    '''<returns>Num�ro de projection UTM de ESRI selon la zone sp�cifi�e, ou 0 si la zone n'est pas entre 6 et 22</returns>
    '''
    Public Function NumeroProjectionUtmESRI(ByVal ZoneUTM As Byte) As Integer
        'Retourner 0 si zone invalide
        NumeroProjectionUtmESRI = 26906

        'D�finir la projection UTM selon la zone du Snrc
        Select Case ZoneUTM
            Case 6
                NumeroProjectionUtmESRI = 26906
            Case 7
                NumeroProjectionUtmESRI = 26907
            Case 8
                NumeroProjectionUtmESRI = 26908
            Case 9
                NumeroProjectionUtmESRI = 26909
            Case 10
                NumeroProjectionUtmESRI = 26910
            Case 11
                NumeroProjectionUtmESRI = 26911
            Case 12
                NumeroProjectionUtmESRI = 26912
            Case 13
                NumeroProjectionUtmESRI = 26913
            Case 14
                NumeroProjectionUtmESRI = 26914
            Case 15
                NumeroProjectionUtmESRI = 26915
            Case 16
                NumeroProjectionUtmESRI = 26916
            Case 17
                NumeroProjectionUtmESRI = 26917
            Case 18
                NumeroProjectionUtmESRI = 26918
            Case 19
                NumeroProjectionUtmESRI = 26919
            Case 20
                NumeroProjectionUtmESRI = 26920
            Case 21
                NumeroProjectionUtmESRI = 26921
            Case 22
                NumeroProjectionUtmESRI = 26922
        End Select
    End Function

    '''<summary>
    '''Fonction qui permet de retourner la r�f�rence spatiale en coordonn�es g�ographique avec le datum Nad83 GCS.
    '''</summary>
    ''' 
    '''<returns>ISpatialReference correspondant au syst�me de coordonn�es g�ographique Nad83, Nothing sinon.</returns>
    '''
    Public Shared Function ReferenceSpatialeGeoNad83() As SpatialReference
        'Par d�faut, elle est invalide
        ReferenceSpatialeGeoNad83 = Nothing

        'Ex�cuter la fonction qui permet d'ex�cuter l'affichage
        Dim identifyResult = QueuedTask.Run(
            Function()
                'Cr�ation de la r�f�rence spatiale 4617:"GCS North American 1983 CSRS"
                Dim spatialReferenceBuilder = New SpatialReferenceBuilder(4617)

                'Retourner la r�f�rence spatiale
                ReferenceSpatialeGeoNad83 = spatialReferenceBuilder.ToSpatialReference

                'Retourner le succ�s du traitement
                Return True
            End Function)
    End Function
#End Region
End Class
