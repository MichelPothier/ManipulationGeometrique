Imports ArcGIS.Core.Data
Imports ArcGIS.Core.Geometry
Imports ArcGIS.Desktop.Editing
Imports ArcGIS.Desktop.Editing.Templates
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Mapping

'**
'Nom de la composante : clsSNRC.vb
'
'''<summary>
''' Classe qui contient l'information de l'unité de travail du SNRC (Système National de Référence Cartographique du Canada). 
''' Le SNRC est utilisé sous deux échelle 50000 et 250000.
''' Le numéro du SNRC au 50000 se divise en trois parties, tandis que celui au 250000 se divise seulement en deux parties. 
''' La longueur du numéro SNRC au 50000 est de six lettres et celui du 250000 est de quatres lettres. 
''' Les deux premières parties du numéro de ses deux échelles sont identiques. 
''' La première partie comprend trois chiffres entre 001 et 520. 
''' La deuxième partie comprend une lettre de A à P.
''' La troisième partie qui s'applique seulement à l'échelle du 50000 comprend deux chiffres de 01 à 16.
'''</summary>
'''
'''<remarks>
'''Auteur : Michel Pothier
'''Date : 8 décembre 2009
'''</remarks>
''' 
Public Class clsSNRC
    'Permet d'hériter des fonctions et valeurs de la classe de base UniteTravail
    Inherits clsUniteTravail

    'Définition des variables globales de travail
    Protected gsNumeroPartie(3) As String     'Numéro de chacune des trois parties du SNRC.
    Protected gyZoneUtmEst As Byte            'Zone UTM Est du SNRC.
    Protected gyZoneUtmOuest As Byte          'Zone UTM Ouest du SNRC.
    Protected gbDeuxZones As Boolean          'Indiquer si le numéro Snrc est présent dans deux zones.

#Region " Routines et fonctions publiques"
    '''<summary>
    ''' Retourner le Delta de la latitude pour n'importe lequel SNRC.
    '''</summary>
    ''' 
    '''<returns>Delat de la latitude pour n'importe lequel SNRC.</returns>
    ''' 
    Public ReadOnly Property DeltaLatitude(ByVal dLatitude As Double, ByVal nEchelle As Integer) As Double
        Get
            'Définir la valeur par défaut
            DeltaLatitude = 0.25

            'Si l'échelle est 50000
            If nEchelle = 50000 Then
                If dLatitude < 68 Then
                    DeltaLatitude = 0.25
                ElseIf dLatitude < 80 Then
                    DeltaLatitude = 0.25
                Else
                    DeltaLatitude = 0.25
                End If

                'Si l'échelle est 250000
            ElseIf nEchelle = 250000 Then
                If dLatitude < 68 Then
                    DeltaLatitude = 1
                ElseIf dLatitude < 80 Then
                    DeltaLatitude = 1
                Else
                    DeltaLatitude = 1
                End If
            End If
        End Get
    End Property

    '''<summary>
    ''' Retourner le Delta de la longitude pour n'importe lequel SNRC.
    '''</summary>
    ''' 
    '''<returns>Delta de la longitude pour n'importe lequel SNRC.</returns>
    ''' 
    Public ReadOnly Property DeltaLongitude(ByVal dLatitude As Double, ByVal nEchelle As Integer) As Double
        Get
            'Définir la valeur par défaut
            DeltaLongitude = 0.5

            'Si l'échelle est 50000
            If nEchelle = 50000 Then
                If dLatitude < 68 Then
                    DeltaLongitude = 0.5
                ElseIf dLatitude < 80 Then
                    DeltaLongitude = 1
                Else
                    DeltaLongitude = 2
                End If

                'Si l'échelle est 250000
            ElseIf nEchelle = 250000 Then
                If dLatitude < 68 Then
                    DeltaLongitude = 2
                ElseIf dLatitude < 80 Then
                    DeltaLongitude = 4
                Else
                    DeltaLongitude = 8
                End If
            End If
        End Get
    End Property

    '''<summary>
    ''' Retourner la latitude minimum pour n'importe lequel SNRC.
    '''</summary>
    ''' 
    '''<returns>Latitude minimum pour n'importe lequel SNRC</returns>
    ''' 
    Public ReadOnly Property LatitudeMinimum() As Double
        Get
            LatitudeMinimum = 40
        End Get
    End Property

    '''<summary>
    ''' Retourner la latitude maximum pour n'importe lequel SNRC.
    '''</summary>
    ''' 
    '''<returns>Latitude maximum pour n'importe lequel SNRC</returns>
    ''' 
    Public ReadOnly Property LatitudeMaximum() As Double
        Get
            LatitudeMaximum = 86.5
        End Get
    End Property

    '''<summary>
    ''' Retourner la longitude minimum pour n'importe lequel SNRC.
    '''</summary>
    ''' 
    '''<returns>Longitude minimum pour n'importe lequel SNRC</returns>
    ''' 
    Public ReadOnly Property LongitudeMinimum() As Double
        Get
            LongitudeMinimum = -48
        End Get
    End Property

    '''<summary>
    ''' Retourner la longitude maximum pour n'importe lequel SNRC.
    '''</summary>
    ''' 
    '''<returns>Longitude maximum pour n'importe lequel SNRC</returns>
    ''' 
    Public ReadOnly Property LongitudeMaximum() As Double
        Get
            LongitudeMaximum = -142
        End Get
    End Property

    '''<summary>
    '''Retourner le numéro de zone UTM Est du SNRC.
    '''</summary>
    ''' 
    '''<returns> Numéro de zone UTM Est du SNRC</returns>
    ''' 
    Public ReadOnly Property ZoneUtmEst() As Byte
        Get
            ZoneUtmEst = gyZoneUtmEst
        End Get
    End Property

    '''<summary>
    '''Retourner le numéro de zone UTM Ouest du SNRC.
    '''</summary>
    '''
    '''<returns> Numéro de zone UTM Ouest du SNRC.</returns>
    ''' 
    Public ReadOnly Property ZoneUtmOuest() As Byte
        Get
            ZoneUtmOuest = gyZoneUtmOuest
        End Get
    End Property

    '''<summary>
    '''Indiquer si le SNRC est présent dans deux zones.
    '''</summary>
    ''' 
    '''<returns>Vrai si le SNRC est présent dans deux zones, faux sinon</returns>
    ''' 
    Public ReadOnly Property DeuxZones() As Boolean
        Get
            DeuxZones = gbDeuxZones
        End Get
    End Property

    '''<summary>
    '''Retourner l'information pour le SNRC.
    '''</summary>
    ''' 
    '''<returns>Information pour le SNRC.</returns>
    ''' 
    Public Overloads ReadOnly Property Information() As String
        Get
            If EstValide Then
                'Définir la description du SNRC
                gsInformation = "Unité de travail : " & Numero & vbCrLf
                gsInformation = gsInformation & "Catégorie : " & Categorie & vbCrLf
                gsInformation = gsInformation & "Échelle : " & Echelle & vbCrLf
                gsInformation = gsInformation & "Zone UTM : " & ZoneUTM & vbCrLf
                gsInformation = gsInformation & "Deux zones UTM : " & gbDeuxZones & vbCrLf
                'Vérifier la présence de deux zones
                If gbDeuxZones Then
                    gsInformation = gsInformation & "  Zone UTM Est : " & gyZoneUtmEst & vbCrLf
                    gsInformation = gsInformation & "  Zone UTM Ouest : " & gyZoneUtmOuest & vbCrLf
                End If
                gsInformation = gsInformation & "Méridien central : Longitude=" & Format(MeridienCentral, "###0.00") & vbCrLf
                gsInformation = gsInformation & "Latitude SE :" & Format(LatitudeSE, "###0.00") & vbCrLf
                gsInformation = gsInformation & "Longitude SE :" & Format(LongitudeSE, "###0.00") & vbCrLf
                gsInformation = gsInformation & "Latitude NW :" & Format(LatitudeNW, "###0.00") & vbCrLf
                gsInformation = gsInformation & "Longitude NW :" & Format(LongitudeNW, "###0.00") & vbCrLf
                gsInformation = gsInformation & "Indice de validité : " & EstValide & vbCrLf
            End If
            Information = gsInformation
        End Get
    End Property

    '''<summary>
    '''Définir un nouveau SNRC lors de l'instanciation à partir d'un numéro.
    '''</summary>
    ''' 
    '''<param name="sNumero">Numéro du SNRC à définir.</param>
    ''' 
    Public Sub New(Optional ByVal sNumero As String = "")
        'Définir la catégorie
        gsCategorie = "SNRC"
        'Définir le numéro Snrc
        Call Definir(sNumero)
    End Sub

    '''<summary>
    '''Fonction qui permet de définir le numéro de l'unité de travail correspondant à un SNRC. 
    '''Le numéro SNRC est décomposer en trois parties et chaque partie est validé. 
    '''À partir de chaque partie, on détermine certaine information nécessaire 
    '''pour le calcul du polygone et son système de coordonnées.
    '''</summary>
    '''
    '''<param name="sNumero">Numéro du SNRC à définir.</param>
    ''' 
    '''<returns>Vrai si le SNRC est valide, faux sinon.</returns>
    '''
    Public Function Definir(ByVal sNumero As String) As Boolean
        Try
            'Écrire une trace de début
            ''Call citsMod_FichierTrace.EcrireMessageTrace("Debut")

            'Initialiser les variables
            Definir = False
            gbEstValide = True
            gsNumero = UCase(sNumero)

            'Décomposer le Numero en trois parties
            Call DecomposerNumero()
            If EstValide = False Then Exit Try

            'Valider la première partie
            Call ValiderPartieUn()
            If EstValide = False Then Exit Try

            'Valider la deuxième partie
            Call ValiderPartieDeux()
            If EstValide = False Then Exit Try

            'Définir l'échelle et valider la troisième partie
            Call ValiderPartieTrois()
            If EstValide = False Then Exit Try

            'Calcul du coin SE et NW en géographique selon l'échelle 50K ou 250K et la Zone UTM
            Call CalculCoin_SE_NW()

            'Calcul du méridien centrale
            gdMeridienCentral = ((gyZoneUtm - 1) * 6) - 177

            'Succès du traitement
            gsNumero = gsNumeroPartie(1) & gsNumeroPartie(2) & gsNumeroPartie(3)
            gbEstValide = True
            Definir = True

        Catch ex As Exception
            gsInformation = ex.ToString
            gbEstValide = False
            Definir = False

        Finally
            'Écrire une trace de fin
            ''Call citsMod_FichierTrace.EcrireMessageTrace("Fin")
        End Try
    End Function

    '''<summary>
    '''Définir un nouveau SNRC lors de l'instanciation à partir d'un point et d'une échelle.
    '''</summary>
    ''' 
    '''<param name="pPoint">Point utilisé pour définir le SNRC.</param>
    '''<param name="nEchelle">Échelle utilisée pour définir le SNRC.</param>
    ''' 
    Public Sub New(ByVal pPoint As MapPoint, Optional ByVal nEchelle As Integer = 50000)
        'Définir la catégorie
        gsCategorie = "SNRC"
        'Définir le numéro Snrc
        Call Definir(pPoint, nEchelle)
    End Sub

    '''<summary>
    '''Fonction qui permet de définir le numéro de l'unité de travail correspondant à un SNRC. 
    '''Le numéro SNRC est décomposer en trois parties et chaque partie est validé. 
    '''À partir de chaque partie, on détermine certaine information nécessaire 
    '''pour le calcul du polygone et son système de coordonnées.
    '''</summary>
    '''
    '''<param name="pPoint">Point utilisé pour définir le SNRC.</param>
    '''<param name="nEchelle">Échelle utilisée pour définir le SNRC.</param>
    ''' 
    '''<returns>Vrai si le SNRC est valide, faux sinon.</returns>
    '''
    Public Function Definir(ByVal pPoint As MapPoint, Optional ByVal nEchelle As Integer = 50000) As Boolean
        Try
            'Écrire une trace de début
            Definir = True

            'Vérifier si le point est valide
            If Not pPoint Is Nothing Then
                'Vérifier si le point n'est pas vide 
                If Not pPoint.IsEmpty Then
                    'Vérifier si la référence spatial du point est valide
                    If Not pPoint.SpatialReference Is Nothing Then
                        'Projeter le point en géographique
                        GeometryEngine.Instance.Project(pPoint, ReferenceSpatialeGeoNad83)

                        'Définir le numéro SNRC en fonction d'un point et d'une échelle
                        Call Definir(GET_SHEET_GEOG(pPoint, nEchelle))
                    End If
                End If
            End If

        Catch ex As Exception
            gsInformation = ex.ToString
            gbEstValide = False
            Definir = False
        End Try
    End Function
#End Region

#Region " Routines et fonctions privées utilisé pour le calcul du SNRC à partir du numéro"
    '''<summary>
    '''Routine qui permet de décomposer le numero SNRC en trois parties. La première lettre trouvée dans le numéro SNRC 
    '''est utilisée pour décomposer le numéro. Tout ce qui vient avant la lettre trouvée est placé dans la partie un.
    '''</summary>
    '''
    Private Sub DecomposerNumero()
        'Définir les variables de travail
        Dim iLongueur As Integer = Nothing  'Longueur du numéro SNRC spécifié.
        Dim i As Integer = Nothing          'Compteur

        Try
            'Trouver la longeur du Numero
            iLongueur = Len(gsNumero)

            'Valider la longueur du Numero est valide
            If iLongueur > 1 And iLongueur < 7 Then
                'Décomposer le Numero en trois parties
                For i = 1 To iLongueur
                    If Not IsNumeric(Mid(gsNumero, i, 1)) Then
                        gsNumeroPartie(1) = Mid(gsNumero, 1, i - 1)
                        gsNumeroPartie(2) = StrConv(Mid(gsNumero, i, 1), vbUpperCase)
                        gsNumeroPartie(3) = Mid(gsNumero, i + 1, iLongueur)
                        Exit For
                    End If
                Next i

                'Si la longueur du Numero est invalide
            Else
                'Indiquer l'erreur
                gbEstValide = False
                gsInformation = "ERREUR : Le Numero est invalide (" & gsNumero & ")"
            End If

        Finally
            'Vider la mémoire
            iLongueur = Nothing
            i = Nothing
        End Try
    End Sub

    '''<summary>
    '''Routine qui permet de valider la première partie du numéro SNRC. On vérifie que la valeur de la première partie est
    '''entre 1 et 120, ou 340 ou 520. On ajoute un zéro au début si le chiffre est inférieur à 100.
    '''</summary>
    '''
    Private Sub ValiderPartieUn()
        'Valider la longueur
        If Len(gsNumeroPartie(1)) = 0 Then
            gbEstValide = False
            gsInformation = "ERREUR : La première partie du Numéro est vide (" & gsNumero & ")"

            'Valider si c'est un entier
        ElseIf IsNumeric(gsNumeroPartie(1)) = False Then
            gbEstValide = False
            gsInformation = "ERREUR : La première partie du Numéro n'est pas un entier (" & gsNumeroPartie(1) & ")"

            'Valider la valeur
        ElseIf Not ((CInt(gsNumeroPartie(1)) >= 1 And CInt(gsNumeroPartie(1)) <= 120) _
        Or CInt(gsNumeroPartie(1)) = 340 Or CInt(gsNumeroPartie(1)) = 560) Then
            gbEstValide = False
            gsInformation = "ERREUR : La première partie du Numéro doit être entre 1 à 100, 120, 340 ou 560 (" & gsNumeroPartie(1) & ")"

            'Compléter la première partie avec des 0
        ElseIf gsNumeroPartie(1).Length < 3 Then
            gsNumeroPartie(1) = Format(CInt(gsNumeroPartie(1)), "000")
        End If
    End Sub

    '''<summary>
    '''Routine qui permet de valider la deuxième partie du Numero SNRC. On vérifie si la lettre est entre A et P inclusivement.
    '''</summary>
    '''
    Private Sub ValiderPartieDeux()
        'Vérifier si la première partie correspond à la latitude 80 degrés
        If CInt(gsNumeroPartie(1)) = 120 Or CInt(gsNumeroPartie(1)) = 340 Or CInt(gsNumeroPartie(1)) = 560 Then
            'Définir la latitude à 80 degrés selon la première partie
            gdLatitudeSE = 80
            'Définir la longitude selon la première partie
            gdLongitudeSE = CInt((CInt(Math.Truncate(CInt(gsNumeroPartie(1)) / 100)) * 8) + 48)

            'Sinon
        Else
            'Définir la latitude selon la première partie
            gdLatitudeSE = CInt((CInt(CInt(gsNumeroPartie(1)) - CInt(Math.Truncate(CInt(gsNumeroPartie(1)) / 10)) * 10) * 4) + 40)
            'Définir la longitude selon la première partie
            gdLongitudeSE = CInt((CInt(Math.Truncate(CInt(gsNumeroPartie(1)) / 10)) * 8) + 48)
        End If

        'Valider la longueur
        If Len(gsNumeroPartie(2)) = 0 Then
            gbEstValide = False
            gsInformation = "ERREUR : La deuxième partie du Numero est vide (" & gsNumero & ")"

            'Valider la valeur si la latitude est inférieure à 68 dégrés
        ElseIf gdLatitudeSE < 68 Then
            'Vérifier si la partie 2 se situe entre la lettre A et P
            If gsNumeroPartie(2) < "A" Or gsNumeroPartie(2) > "P" Then
                'Définir une erreur
                gbEstValide = False
                gsInformation = "ERREUR : La deuxième partie du Numero doit être entre A à P (" & gsNumeroPartie(2) & ")"
            End If

            'Valider la valeur si la latitude est supérieure ou égale à 68 dégrés
        Else
            'Vérifier si la partie 2 se situe entre la lettre A et H
            If gsNumeroPartie(2) < "A" Or gsNumeroPartie(2) > "H" Then
                'Définir une erreur
                gbEstValide = False
                gsInformation = "ERREUR : La deuxième partie du Numero doit être entre A à H (" & gsNumeroPartie(2) & ")"
            End If
        End If
    End Sub

    '''<summary>
    '''Routine qui permet de valider la troisième partie du numéro SNRC et définir son échelle. 
    '''On vérifie que la valeur est entre 1 et 16 inclusivement. On ajoute un zéro si la valeur est inférieur à 10.
    '''</summary>
    '''
    Private Sub ValiderPartieTrois()
        'Définir l'échelle selon la troisième partie
        If Len(gsNumeroPartie(3)) = 0 Then
            'Définir l'échelle du 250000
            giEchelle = 250000
        Else
            'Valider si c'est un entier
            If IsNumeric(gsNumeroPartie(3)) = False Then
                gbEstValide = False
                gsInformation = "ERREUR : La Troisième partie du Numero doit n'est pas un entier (" & gsNumeroPartie(3) & ")"

                'Valider la valeur
            ElseIf CInt(gsNumeroPartie(3)) < 1 Or CInt(gsNumeroPartie(3)) > 16 Then
                gbEstValide = False
                gsInformation = "ERREUR : La troixième partie du Numero doit être entre 1 à 16 (" & gsNumeroPartie(3) & ")"

                'Compléter la troisième partie incomplete et définir l'échelle
            Else
                If Len(gsNumeroPartie(3)) = 1 Then
                    'Complèter la troisième partie
                    gsNumeroPartie(3) = "0" & gsNumeroPartie(3)
                End If

                'Définir l'échelle du 50000
                giEchelle = 50000
            End If
        End If
    End Sub

    '''<summary>
    '''Routine qui permet de calculer du coin SE et NW en géographique selon l'échelle 50K ou 250K et la Zone UTM.
    '''</summary>
    '''
    Private Sub CalculCoin_SE_NW()
        'Définir les variables de travail
        Dim dDeltaLongitude As Double = Nothing 'Distance entre chaque sommet en longitude.
        Dim dDeltaLatitude As Double = Nothing  'Distance entre chaque sommet en latitude.
        Dim iCaractere As Integer = Nothing     'Valeur numérique associé à la lettre a=0, b=1, etc ...

        Try
            'Deuxième et troisième partie selon l'échelle
            iCaractere = Asc(gsNumeroPartie(2)) - 64

            'Si l'échelle est 50000
            If giEchelle = 50000 Then
                If gdLatitudeSE < 68 Then
                    dDeltaLatitude = 0.25
                    dDeltaLongitude = 0.5
                    Call CoinBasGauche(iCaractere, dDeltaLatitude * 4, dDeltaLongitude * 4)
                ElseIf gdLatitudeSE < 80 Then
                    dDeltaLatitude = 0.25
                    dDeltaLongitude = 1
                    Call Coin68et80(iCaractere, dDeltaLatitude * 4, dDeltaLongitude * 4)
                Else
                    dDeltaLatitude = 0.25
                    dDeltaLongitude = 2
                    Call Coin68et80(iCaractere, dDeltaLatitude * 4, dDeltaLongitude * 4)
                End If
                Call CoinBasGauche(CInt(gsNumeroPartie(3)), dDeltaLatitude, dDeltaLongitude)

                'Si l'échelle est 250000
            ElseIf giEchelle = 250000 Then
                If gdLatitudeSE < 68 Then
                    dDeltaLatitude = 1
                    dDeltaLongitude = 2
                    Call CoinBasGauche(iCaractere, dDeltaLatitude, dDeltaLongitude)
                ElseIf gdLatitudeSE < 80 Then
                    dDeltaLatitude = 1
                    dDeltaLongitude = 4
                    Call Coin68et80(iCaractere, dDeltaLatitude, dDeltaLongitude)
                Else
                    dDeltaLatitude = 1
                    dDeltaLongitude = 8
                    Call Coin68et80(iCaractere, dDeltaLatitude, dDeltaLongitude)
                End If
            End If

            'Calcul de la longitude Sud-Est
            gdLongitudeSE = gdLongitudeSE * -1

            'Calcul de la latitude Nord-Ouest
            gdLatitudeNW = gdLatitudeSE + dDeltaLatitude

            'Calcul de la longitude Nord-Ouest
            gdLongitudeNW = gdLongitudeSE - dDeltaLongitude

            'Calcul de la zone UTM
            gyZoneUtm = CByte(Math.Truncate((186 + (gdLongitudeSE - (dDeltaLongitude / 2) - 0.05)) / 6))

            'Calcul de la zone UTM Est
            gyZoneUtmEst = CByte(Math.Truncate((186 + (gdLongitudeSE - 0.05)) / 6))

            'Calcul de la zone UTM Ouest
            gyZoneUtmOuest = CByte(Math.Truncate((186 + (gdLongitudeNW + 0.05)) / 6))

            'Vérifier si ce n'est pas une carte deux Zones
            If gyZoneUtmEst = gyZoneUtmOuest Then
                'Indiquer qu'il n'y a seulement qu'une zone UTM
                gbDeuxZones = False
                'si c'est une carte deux Zones
            Else
                'Indiquer qu'il y a deux zones UTM
                gbDeuxZones = True
            End If

        Finally
            'Vider la mémoire
            dDeltaLongitude = Nothing
            dDeltaLatitude = Nothing
            iCaractere = Nothing
        End Try
    End Sub

    '''
    '''<summary>
    '''Riutine qui permet de calculer la Latitude et la Longitude du coin en bas à gauche pour le polygone du SNRC.
    '''</summary>
    '''
    '''<param name="position">Numéro de position de la lettre (Partie #2).</param>
    '''<param name="DeltaLatitude">Distance en degrés entre les points pour la latitude.</param>
    '''<param name="DeltaLongitude">Distance en degrés entre les points pour la longtude.</param>
    '''
    Private Sub CoinBasGauche(ByVal position As Integer, ByVal DeltaLatitude As Double, ByVal DeltaLongitude As Double)
        'Traite les positions 1 à 4 ou A à D
        If position <= 4 Then
            gdLongitudeSE = gdLongitudeSE + (DeltaLongitude * (position - 1))

            'Traite les positions 5 à 8 ou E à H
        ElseIf position <= 8 Then
            gdLatitudeSE = gdLatitudeSE + DeltaLatitude
            gdLongitudeSE = gdLongitudeSE + (DeltaLongitude * (8 - position))

            'Traite les positions 9 à 12 ou I à L
        ElseIf position <= 12 Then
            gdLatitudeSE = gdLatitudeSE + (DeltaLatitude * 2)
            gdLongitudeSE = gdLongitudeSE + (DeltaLongitude * (position - 9))

            'Traite les positions 13 à 16 ou M à P
        ElseIf position <= 16 Then
            gdLatitudeSE = gdLatitudeSE + (DeltaLatitude * 3)
            gdLongitudeSE = gdLongitudeSE + (DeltaLongitude * (16 - position))
        End If
    End Sub

    '''<summary>
    '''Riutine qui permet de calculer la Latitude et la Longitude du coin selon la latitude 68 et 80.
    '''</summary>
    '''
    '''<param name="position">Numéro de position de la lettre (Partie #2).</param>
    '''<param name="DeltaLatitude">Distance en degrés entre les points pour la latitude.</param>
    '''<param name="DeltaLongitude">Distance en degrés entre les points pour la longtude.</param>
    '''
    Private Sub Coin68et80(ByVal position As Integer, ByVal DeltaLatitude As Double, ByVal DeltaLongitude As Double)
        'Traite les positions 1 à 4 ou A à D
        If position <= 2 Then
            gdLongitudeSE = gdLongitudeSE + (DeltaLongitude * (position - 1))

            'Traite les positions 5 à 8 ou E à H
        ElseIf position <= 4 Then
            gdLatitudeSE = gdLatitudeSE + DeltaLatitude
            gdLongitudeSE = gdLongitudeSE + (DeltaLongitude * (4 - position))

            'Traite les positions 9 à 12 ou I à L
        ElseIf position <= 6 Then
            gdLatitudeSE = gdLatitudeSE + (DeltaLatitude * 2)
            gdLongitudeSE = gdLongitudeSE + (DeltaLongitude * (position - 5))

            'Traite les positions 13 à 16 ou M à P
        ElseIf position <= 8 Then
            gdLatitudeSE = gdLatitudeSE + (DeltaLatitude * 3)
            gdLongitudeSE = gdLongitudeSE + (DeltaLongitude * (8 - position))
        End If
    End Sub
#End Region

#Region " Routines et fonctions privées utilisé pour le calcul du SNRC à partir d'une point géographique et d'une échelle"
    '''<summary>
    ''' ROUTINE PERMETTANT DE DONNER LE NUMERO SNRC D'UN FEUILLET CARTOGRAPHIQUE AU 1:20000, 1:50000, 1:250000, 1:1000000 
    ''' A PARTIR D'UNE COORDONNEE GEOGRAPHIQUE ET L'ECHELLE DE LA CARTE.
    '''</summary>
    '''
    '''<param name="pPoint">COORDONNEES GEOGRAPHIQUE D'UN POINT (LAT. LONG.).</param>
    '''<param name="ECHELLE">FACTEUR ECHELLE DU FEUILLET.</param>
    ''' 
    '''<returns>"String" contenant le numéro SNRC si le point et l'échelle sont valident, sinon "".</returns>
    ''' 
    '''<remarks>
    ''' SNRC : AAABCCD
    '''        |  || |
    '''        |  || Partie du SNRC au 20000 (A-D)
    '''        |  |Partie du SNRC au 50000 (1-16)
    '''        |  Partie du SNRC au 250000 (A-P)
    '''        Partie du SNRC au 1000000 (1-116)
    '''</remarks>
    ''' 
    Private Function GET_SHEET_GEOG(ByVal pPoint As MapPoint, ByVal ECHELLE As Integer) As String
        'Déclarer les variables de travail
        Dim NTSA(1) As Integer  'Secteur en lat.(0-6) et long.(1-11) pour la partie A du SNRC
        Dim NTSB(1) As Integer  'Secteur de 0-3 en lat. et long pour la partie B du SNRC
        Dim NTSC(1) As Integer  'Secteur de 0-3 en lat. et long pour la partie C du SNRC
        Dim NTSD(1) As Integer  'Secteur de 0-1 en lat. et long pour la partie D du SNRC

        'Définir la valeur de retour du numéro SNRC par défaut
        GET_SHEET_GEOG = ""

        Try
            'Vérifier si la latitude est valide
            If ((pPoint.Y >= LatitudeMinimum) And (pPoint.Y <= LatitudeMaximum)) Then
                'Vérifier si la longitude est valide
                If ((Math.Abs(pPoint.X) >= Math.Abs(LongitudeMinimum)) And (Math.Abs(pPoint.X) <= Math.Abs(LongitudeMaximum))) Then
                    'Vérifier si la position est valide
                    If (pPoint.Y > 80) And (Math.Abs(pPoint.X) > 104) Then
                        'Définir une erreur de position illégale
                        gbEstValide = False
                        gsInformation = "ERREUR : Position illégale : " & CStr(pPoint.X) & "," & CStr(pPoint.Y)
                    Else
                        'Définir les Secteurs pour toutes les échelles
                        Call SECTEUR(pPoint, NTSA, NTSB, NTSC, NTSD)
                        'Si l'échelle est 1000000, définir le numéro SNRC pour cette échelle
                        If (ECHELLE <= 1000000) Then Call NTS1000K(GET_SHEET_GEOG, NTSA)
                        'Si l'échelle est 250000, définir le numéro SNRC pour cette échelle
                        If (ECHELLE <= 250000) Then Call NTS250K(GET_SHEET_GEOG, NTSB, pPoint)
                        'Si l'échelle est 50000, définir le numéro SNRC pour cette échelle
                        If (ECHELLE <= 50000) Then Call NTS50K(GET_SHEET_GEOG, NTSC)
                        'Si l'échelle est 20000, définir le numéro SNRC pour cette échelle
                        If (ECHELLE <= 20000) Then Call NTS20K(GET_SHEET_GEOG, NTSD)
                    End If
                Else
                    'Définir une erreur de longitude illégale
                    gbEstValide = False
                    gsInformation = "ERREUR : Longitude illégale : " & CStr(pPoint.X)
                End If
            Else
                'Définir une erreur de latitude illégale
                gbEstValide = False
                gsInformation = "ERREUR : Latitude illégale : " & CStr(pPoint.Y)
            End If

        Catch e As Exception
            'Message d'erreur
            Err.Raise(vbObjectError + 1, "", e.ToString)
        End Try
    End Function

    '''<summary>
    ''' ROUTINE PERMETTANT DE DONNER LES SECTEURS EN LAT. ET LONG. POUR LES FEUILLETS CARTOGRAPHIQUE DU SNRC.
    '''</summary>
    '''
    '''<param name="pPoint">Coordonnee geographique (latitude et longitude).</param>
    '''<param name="NTSA">Secteur en lat.(0-6) et long.(1-11) pour la partie A (1000000) du SNRC.</param>
    '''<param name="NTSB">Secteur de 0-3 en lat. et long pour la partie B (250000) du SNRC.</param>
    '''<param name="NTSC">Secteur de 0-3 en lat. et long pour la partie C (50000) du SNRC.</param>
    '''<param name="NTSD">Secteur de 0-1 en lat. et long pour la partie D (20000) du SNRC.</param>
    ''' 
    Private Sub SECTEUR(ByVal pPoint As MapPoint, ByRef NTSA() As Integer, ByRef NTSB() As Integer, ByRef NTSC() As Integer, ByRef NTSD() As Integer)
        'Déclarer les variables de travail
        Dim pPointA As New Point        'Coordonnées géographique du coin inférieur droit au 1000000.
        Dim pPointB As New Point        'Coordonnées géographique du coin inférieur droit au 250000.
        Dim pPointC As New Point        'Coordonnées géographique du coin inférieur droit au 50000.
        Dim pPointD As New Point        'Coordonnées géographique du coin inférieur droit au 20000.
        Dim dTmp As Double = Nothing    'Variable temporaire de travail

        Try
            'Vérifier si la latitude est inférieure à 68 degré
            If (pPoint.Y < 68) Then
                'Secteur pour l'échelle 1000000
                NTSA(0) = CInt(Math.Truncate((Math.Abs(pPoint.X) - 48) / 8))
                NTSA(1) = CInt(Math.Truncate((pPoint.Y - 40) / 4))
                'Coordonnées géographique du coin inférieur droit au 1000000.
                pPointA.X = (NTSA(0) * 8) + 48
                pPointA.Y = (NTSA(1) * 4) + 40

                'Secteur pour l'échelle 250000
                NTSB(0) = CInt(Math.Truncate(pPoint.Y - pPointA.Y))
                NTSB(1) = CInt(Math.Truncate((Math.Abs(pPoint.X) - pPointA.X) / 2))
                'Coordonnées géographique du coin inférieur droit au 250000.
                pPointB.X = pPointA.X + (NTSB(1) * 2)
                pPointB.Y = pPointA.Y + NTSB(0)

                'Secteur pour l'échelle 50000
                NTSC(0) = CInt(Math.Truncate((pPoint.Y - pPointB.Y) * 4))
                NTSC(1) = CInt(Math.Truncate((Math.Abs(pPoint.X) - pPointB.X) * 2))
                'Coordonnées géographique du coin inférieur droit au 50000.
                dTmp = NTSC(0)
                pPointC.Y = pPointB.Y + (dTmp / 4)
                dTmp = NTSC(1)
                pPointC.X = pPointB.X + (dTmp / 2)

                'Secteur pour l'échelle 20000
                NTSD(0) = CInt(Math.Truncate((pPoint.Y - pPointC.Y) * 8))
                NTSD(1) = CInt(Math.Truncate((Math.Abs(pPoint.X) - pPointC.X) * 4))
                'Coordonnées géographique du coin inférieur droit au 20000.
                dTmp = NTSD(0)
                pPointD.Y = pPointC.Y + (dTmp / 8)
                dTmp = NTSD(1)
                pPointD.X = pPointC.X + (dTmp / 4)

                'Vérifier si la latitude est supérieure ou égale à 68 degré et est inférieure à 80 degré
            ElseIf ((pPoint.Y >= 68) And (pPoint.Y < 80)) Then
                'Secteur pour l'échelle 1000000
                NTSA(0) = CInt(Math.Truncate((Math.Abs(pPoint.X) - 48) / 8))
                NTSA(1) = CInt(Math.Truncate((pPoint.Y - 40) / 4))
                'Coordonnées géographique du coin inférieur droit au 1000000.
                pPointA.X = (NTSA(0) * 8) + 48
                pPointA.Y = (NTSA(1) * 4) + 40

                'Secteur pour l'échelle 250000
                NTSB(0) = CInt(Math.Truncate(pPoint.Y - pPointA.Y))
                NTSB(1) = CInt(Math.Truncate((Math.Abs(pPoint.X) - pPointA.X) / 4))
                'Coordonnées géographique du coin inférieur droit au 250000.
                pPointB.X = pPointA.X + (NTSB(1) * 4)
                pPointB.Y = pPointA.Y + NTSB(0)

                'Secteur pour l'échelle 50000
                NTSC(0) = CInt(Math.Truncate((pPoint.Y - pPointB.Y) * 4))
                NTSC(1) = CInt(Math.Truncate((Math.Abs(pPoint.X) - pPointB.X) * 1))
                'Coordonnées géographique du coin inférieur droit au 50000.
                dTmp = NTSC(1)
                pPointC.X = pPointB.X + (dTmp / 1)
                dTmp = NTSC(0)
                pPointC.Y = pPointB.Y + (dTmp / 4)

                'Secteur pour l'échelle 20000
                NTSD(0) = CInt(Math.Truncate((pPoint.Y - pPointC.Y) * 8))
                NTSD(1) = CInt(Math.Truncate((Math.Abs(pPoint.X) - pPointC.X) * 2))
                'Coordonnées géographique du coin inférieur droit au 20000.
                dTmp = NTSD(1)
                pPointD.X = pPointC.X + (dTmp / 2)
                dTmp = NTSD(0)
                pPointD.Y = pPointC.Y + (dTmp / 8)

                'Vérifier si la latitude est supérieure ou égale à 80 degré et est inférieure à 86.5 degré
            ElseIf ((pPoint.Y >= 80) And (pPoint.Y <= 86.5)) Then
                'Secteur pour l'échelle 1000000
                NTSA(0) = CInt(Math.Truncate((Math.Abs(pPoint.X) - 56) / 16))
                NTSA(0) = (NTSA(0) * 22) + 12
                NTSA(1) = 0
                'Coordonnées géographique du coin inférieur droit au 1000000.
                pPointA.X = (((NTSA(0) - 12) / 22) * 16) + 56
                pPointA.Y = 80

                'Secteur pour l'échelle 250000
                NTSB(0) = CInt(Math.Truncate(pPoint.Y - pPointA.Y))
                NTSB(1) = CInt(Math.Truncate((Math.Abs(pPoint.X) - pPointA.X) / 8))
                'Coordonnées géographique du coin inférieur droit au 250000.
                pPointB.X = pPointA.X + (NTSB(1) * 8)
                pPointB.Y = pPointA.Y + NTSB(0)

                'Secteur pour l'échelle 50000
                NTSC(0) = CInt(Math.Truncate((pPoint.Y - pPointB.Y) * 4))
                NTSC(1) = CInt(Math.Truncate((Math.Abs(pPoint.X) - pPointB.X) * 0.5))
                'Coordonnées géographique du coin inférieur droit au 50000.
                dTmp = NTSC(1)
                pPointC.X = pPointB.X + (dTmp / 0.5)
                dTmp = NTSC(0)
                pPointC.Y = pPointB.Y + (dTmp / 4)

                'Secteur pour l'échelle 20000
                NTSD(0) = CInt(Math.Truncate((pPoint.Y - pPointC.Y) * 8))
                NTSD(1) = CInt(Math.Truncate((Math.Abs(pPoint.X) - pPointC.X) * 1))
                'Coordonnées géographique du coin inférieur droit au 20000.
                dTmp = NTSD(1)
                pPointD.X = pPointC.X + (dTmp / 1)
                dTmp = NTSD(0)
                pPointD.Y = pPointC.Y + (dTmp / 8)
            End If

        Catch e As Exception
            'Message d'erreur
            Err.Raise(vbObjectError + 1, "", e.ToString)
        Finally
            'Vider la mémoire
            pPointA = Nothing
            pPointB = Nothing
            pPointC = Nothing
            pPointD = Nothing
        End Try
    End Sub

    '''<summary>
    ''' ROUTINE PERMETTANT DE DONNER LA PARTIE DU NUMERO SNRC POUR LES FEUILLETS CARTOGRAPHIQUE AU 1000000.
    '''</summary>
    '''
    '''<param name="NTS">Nom de la carte selon le SNRC.</param>
    '''<param name="NTSA">Secteur en lat.(0-6) et long.(1-11) pour la partie A (1000000) du SNRC.</param>
    ''' 
    Private Sub NTS1000K(ByRef NTS As String, ByVal NTSA() As Integer)
        Try
            'Définir la première partie du numéro SNRC selon le premier secteur
            NTS = CStr(NTSA(0))
            'Ajouter un 0 au début du numéro SNRC si un seul caractère est présent
            If NTS.Length = 1 Then NTS = "0" & NTS
            'Définir la deuxième partie du numéro SNRC selon le deuxième secteur
            NTS = NTS & CStr(NTSA(1))

        Catch e As Exception
            'Message d'erreur
            Err.Raise(vbObjectError + 1, "", e.ToString)
        End Try
    End Sub

    '''<summary>
    ''' ROUTINE PERMETTANT DE DONNER LA PARTIE DU NUMERO SNRC POUR LES FEUILLETS CARTOGRAPHIQUE AU 250000.
    '''</summary>
    '''
    '''<param name="NTS">Nom de la carte selon le SNRC.</param>
    '''<param name="NTSB">Secteur de 0-3 en lat. et long pour la partie B (250000) du SNRC.</param>
    '''<param name="pPoint">Coordonnée géographique (latitude et longitude).</param>
    ''' 
    Private Sub NTS250K(ByRef NTS As String, ByVal NTSB() As Integer, ByVal pPoint As MapPoint)
        Try
            'Vérifier si la latitude est inférieure à 68 degré
            If (pPoint.Y < 68) Then
                'Vérifier si la première partie du secteur correspond à la première ligne
                If (NTSB(0) = 0) Then
                    If (NTSB(1) = 0) Then NTS = Mid(NTS, 1, 4) & "A"
                    If (NTSB(1) = 1) Then NTS = Mid(NTS, 1, 4) & "B"
                    If (NTSB(1) = 2) Then NTS = Mid(NTS, 1, 4) & "C"
                    If (NTSB(1) = 3) Then NTS = Mid(NTS, 1, 4) & "D"
                    'Vérifier si la première partie du secteur correspond à la deuxième ligne
                ElseIf (NTSB(0) = 1) Then
                    If (NTSB(1) = 0) Then NTS = Mid(NTS, 1, 4) & "H"
                    If (NTSB(1) = 1) Then NTS = Mid(NTS, 1, 4) & "G"
                    If (NTSB(1) = 2) Then NTS = Mid(NTS, 1, 4) & "F"
                    If (NTSB(1) = 3) Then NTS = Mid(NTS, 1, 4) & "E"
                    'Vérifier si la première partie du secteur correspond à la troisième ligne
                ElseIf (NTSB(0) = 2) Then
                    If (NTSB(1) = 0) Then NTS = Mid(NTS, 1, 4) & "I"
                    If (NTSB(1) = 1) Then NTS = Mid(NTS, 1, 4) & "J"
                    If (NTSB(1) = 2) Then NTS = Mid(NTS, 1, 4) & "K"
                    If (NTSB(1) = 3) Then NTS = Mid(NTS, 1, 4) & "L"
                    'Vérifier si la première partie du secteur correspond à la quatrième ligne
                ElseIf (NTSB(0) = 3) Then
                    If (NTSB(1) = 0) Then NTS = Mid(NTS, 1, 4) & "P"
                    If (NTSB(1) = 1) Then NTS = Mid(NTS, 1, 4) & "O"
                    If (NTSB(1) = 2) Then NTS = Mid(NTS, 1, 4) & "N"
                    If (NTSB(1) = 3) Then NTS = Mid(NTS, 1, 4) & "M"
                End If

                'Vérifier si la latitude est supérieure ou égale à 68 degré
            ElseIf (pPoint.Y >= 68) Then
                'Vérifier si la première partie du secteur correspond à la première ligne
                If (NTSB(0) = 0) Then
                    If (NTSB(1) = 0) Then NTS = Mid(NTS, 1, 4) & "A"
                    If (NTSB(1) = 1) Then NTS = Mid(NTS, 1, 4) & "B"
                    'Vérifier si la première partie du secteur correspond à la deuxième ligne
                ElseIf (NTSB(0) = 1) Then
                    If (NTSB(1) = 0) Then NTS = Mid(NTS, 1, 4) & "D"
                    If (NTSB(1) = 1) Then NTS = Mid(NTS, 1, 4) & "C"
                    'Vérifier si la première partie du secteur correspond à la troisième ligne
                ElseIf (NTSB(0) = 2) Then
                    If (NTSB(1) = 0) Then NTS = Mid(NTS, 1, 4) & "E"
                    If (NTSB(1) = 1) Then NTS = Mid(NTS, 1, 4) & "F"
                    'Vérifier si la première partie du secteur correspond à la quatrième ligne
                ElseIf (NTSB(0) = 3) Then
                    If (NTSB(1) = 0) Then NTS = Mid(NTS, 1, 4) & "H"
                    If (NTSB(1) = 1) Then NTS = Mid(NTS, 1, 3) & "G"
                End If
            End If

        Catch e As Exception
            'Message d'erreur
            Err.Raise(vbObjectError + 1, "", e.ToString)
        End Try
    End Sub

    '''<summary>
    ''' ROUTINE PERMETTANT DE DONNER LA PARTIE DU NUMERO SNRC POUR LES FEUILLETS CARTOGRAPHIQUE AU 50000.
    '''</summary>
    '''
    '''<param name="NTS">Nom de la carte selon le SNRC.</param>
    '''<param name="NTSC">Secteur de 0-3 en lat. et long pour la partie C (50000) du SNRC.</param>
    ''' 
    Private Sub NTS50K(ByRef NTS As String, ByVal NTSC() As Integer)
        Try
            'Vérifier si la première partie du secteur correspond à la première ligne
            If (NTSC(0) = 0) Then
                If (NTSC(1) = 0) Then NTS = Mid(NTS, 1, 4) & "01"
                If (NTSC(1) = 1) Then NTS = Mid(NTS, 1, 4) & "02"
                If (NTSC(1) = 2) Then NTS = Mid(NTS, 1, 4) & "03"
                If (NTSC(1) = 3) Then NTS = Mid(NTS, 1, 4) & "04"
                'Vérifier si la première partie du secteur correspond à la deuxième ligne
            ElseIf (NTSC(0) = 1) Then
                If (NTSC(1) = 0) Then NTS = Mid(NTS, 1, 4) & "08"
                If (NTSC(1) = 1) Then NTS = Mid(NTS, 1, 4) & "07"
                If (NTSC(1) = 2) Then NTS = Mid(NTS, 1, 4) & "06"
                If (NTSC(1) = 3) Then NTS = Mid(NTS, 1, 4) & "05"
                'Vérifier si la première partie du secteur correspond à la troisième ligne
            ElseIf (NTSC(0) = 2) Then
                If (NTSC(1) = 0) Then NTS = Mid(NTS, 1, 4) & "09"
                If (NTSC(1) = 1) Then NTS = Mid(NTS, 1, 4) & "10"
                If (NTSC(1) = 2) Then NTS = Mid(NTS, 1, 4) & "11"
                If (NTSC(1) = 3) Then NTS = Mid(NTS, 1, 4) & "12"
                'Vérifier si la première partie du secteur correspond à la quatrième ligne
            ElseIf (NTSC(0) = 3) Then
                If (NTSC(1) = 0) Then NTS = Mid(NTS, 1, 4) & "16"
                If (NTSC(1) = 1) Then NTS = Mid(NTS, 1, 4) & "15"
                If (NTSC(1) = 2) Then NTS = Mid(NTS, 1, 4) & "14"
                If (NTSC(1) = 3) Then NTS = Mid(NTS, 1, 4) & "13"
            End If

        Catch e As Exception
            'Message d'erreur
            Err.Raise(vbObjectError + 1, "", e.ToString)
        End Try
    End Sub

    '''<summary>
    ''' ROUTINE PERMETTANT DE DONNER LA PARTIE DU NUMERO SNRC POUR LES FEUILLETS CARTOGRAPHIQUE AU 20000.
    '''</summary>
    '''
    '''<param name="NTS">Nom de la carte selon le SNRC.</param>
    '''<param name="NTSD">Secteur de 0-1 en lat. et long pour la partie D (20000) du SNRC.</param>
    ''' 
    Private Sub NTS20K(ByRef NTS As String, ByVal NTSD() As Integer)
        Try
            'Vérifier si la première partie du secteur correspond à la première ligne
            If (NTSD(0) = 0) Then
                If (NTSD(1) = 0) Then NTS = Mid(NTS, 1, 6) & "A"
                If (NTSD(1) = 1) Then NTS = Mid(NTS, 1, 6) & "B"
                'Vérifier si la première partie du secteur correspond à la deuxième ligne
            ElseIf (NTSD(0) = 1) Then
                If (NTSD(1) = 0) Then NTS = Mid(NTS, 1, 6) & "D"
                If (NTSD(1) = 1) Then NTS = Mid(NTS, 1, 6) & "C"
            End If

        Catch e As Exception
            'Message d'erreur
            Err.Raise(vbObjectError + 1, "", e.ToString)
        End Try
    End Sub
#End Region
End Class
