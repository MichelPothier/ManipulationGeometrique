Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Input
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Core.Geometry
Imports ArcGIS.Desktop.Framework.Threading.Tasks
Imports ArcGIS.Desktop.Mapping
Imports ArcGIS.Core.CIM

Friend Class modGeometrieTravail
    Inherits ArcGIS.Desktop.Framework.Contracts.Module

    'Contient l'énumération des actions de visualisation possibles
    Public Enum EnumActionVisualisation
        AucuneAction = 0
        CentreGeometrie = 1
        ZoomGeometrie = 2
    End Enum

#Region "Propriétés"
    'Contient le ComboBox du SNRC actif.
    Private Shared Property _cboSNRC As ComboBox
    ''' <summary>
    ''' Définir et retourner le ComboBoxdu SNRC actif.
    ''' </summary>
    Public Shared Property cboSNRC() As ComboBox
        Get
            Return _cboSNRC
        End Get
        Set(ByVal value As ComboBox)
            _cboSNRC = value
        End Set
    End Property

    'Contient le module "MpoGeometrieTravail_Module".
    Private Shared Property _this As Object
    ''' <summary>
    ''' Retourner l'instance unique du module courant
    ''' </summary>
    Public Shared ReadOnly Property Current() As modGeometrieTravail
        Get
            If (_this Is Nothing) Then
                _this = DirectCast(FrameworkApplication.FindModule("MpoGeometrieTravail_Module"), modGeometrieTravail)
            End If

            Return _this
        End Get
    End Property

    'Permet d'indiquer si on doit détruire les géométries avant d'en ajouter des nouveaux.
    Private Shared Property _ActiverDetruireGeometrie As Boolean
    ''' <summary>
    ''' Définir et retourner si on doit détruire les géométries avant d'en ajouter des nouveaux.
    ''' </summary>
    Public Shared Property ActiverDetruireGeometrie() As Boolean
        Get
            Return _ActiverDetruireGeometrie
        End Get
        Set(ByVal value As Boolean)
            _ActiverDetruireGeometrie = value
        End Set
    End Property

    'Permet d'indiquer si on doit dessiner les géométries.
    Private Shared Property _ActiverDessinerGeometrie As Boolean
    ''' <summary>
    ''' Définir et retourner si on doit dessiner les géométries.
    ''' </summary>
    Public Shared Property ActiverDessinerGeometrie() As Boolean
        Get
            Return _ActiverDessinerGeometrie
        End Get
        Set(ByVal value As Boolean)
            _ActiverDessinerGeometrie = value
        End Set
    End Property

    'Permet d'indiquer l'action de visualisation
    Private Shared Property _ActionVisualisation As EnumActionVisualisation
    ''' <summary>
    ''' Définir et retourner l'action de visualisation
    ''' </summary>
    Public Shared Property ActionVisualisation() As EnumActionVisualisation
        Get
            Return _ActionVisualisation
        End Get
        Set(ByVal value As EnumActionVisualisation)
            _ActionVisualisation = value
        End Set
    End Property

    'Liste des géométries de travail.
    Private Shared Property _GeometrieTravail As List(Of Geometry) = New List(Of Geometry)
    ''' <summary>
    ''' Définir et retourner la liste des géométries de travail
    ''' </summary>
    Public Shared Property GeometrieTravail() As List(Of Geometry)
        Get
            Return _GeometrieTravail
        End Get
        Set(ByVal value As List(Of Geometry))
            _GeometrieTravail = value
        End Set
    End Property

    'Symbole des sommets des géométries.
    Private Shared Property _VertexSymbol As CIMPointSymbol
    ''' <summary>
    ''' Définir et retourner le symbole pour les sommets des géométries
    ''' </summary>
    Public Shared Property VertexSymbolSymbol() As CIMPointSymbol
        Get
            Return _VertexSymbol
        End Get
        Set(ByVal value As CIMPointSymbol)
            _VertexSymbol = value
        End Set
    End Property

    'Symbole des points.
    Private Shared Property _PointSymbol As CIMPointSymbol
    ''' <summary>
    ''' Définir et retourner le symbole pour les points
    ''' </summary>
    Public Shared Property PointSymbol() As CIMPointSymbol
        Get
            Return _PointSymbol
        End Get
        Set(ByVal value As CIMPointSymbol)
            _PointSymbol = value
        End Set
    End Property

    'Symbole des lignes. 
    Private Shared Property _LineSymbol As CIMLineSymbol
    ''' <summary>
    ''' Définir et retourner le symbole pour les lignes
    ''' </summary>
    Public Shared Property LineSymbol() As CIMLineSymbol
        Get
            Return _LineSymbol
        End Get
        Set(ByVal value As CIMLineSymbol)
            _LineSymbol = value
        End Set
    End Property

    'Symbole des surfaces.
    Private Shared Property _PolygonSymbol As CIMPolygonSymbol
    ''' <summary>
    ''' Définir et retourner le symbole pour les surfaces
    ''' </summary>
    Public Shared Property PolygonSymbol() As CIMPolygonSymbol
        Get
            Return _PolygonSymbol
        End Get
        Set(ByVal value As CIMPolygonSymbol)
            _PolygonSymbol = value
        End Set
    End Property

    'Overlay contenant l'affichage des sommets des géométries.
    Private Shared Property _VertexOverlay As IDisposable
    ''' <summary>
    ''' Définir et retourner l'affichage des géométries de type point ou multipoint.
    ''' </summary>
    Public Shared Property VertexOverlay() As IDisposable
        Get
            Return _VertexOverlay
        End Get
        Set(ByVal value As IDisposable)
            _VertexOverlay = value
        End Set
    End Property

    'Overlay contenant l'affichage des géométries de type point ou multipoint.
    Private Shared Property _PointOverlay As IDisposable
    ''' <summary>
    ''' Définir et retourner l'affichage des géométries de type point ou multipoint.
    ''' </summary>
    Public Shared Property PointOverlay() As IDisposable
        Get
            Return _PointOverlay
        End Get
        Set(ByVal value As IDisposable)
            _PointOverlay = value
        End Set
    End Property

    'Overlay contenant l'affichage des géométries de type polyline.
    Private Shared Property _LineOverlay As IDisposable
    ''' <summary>
    ''' Définir et retourner l'affichage des géométries de type polyline.
    ''' </summary>
    Public Shared Property LineOverlay() As IDisposable
        Get
            Return _LineOverlay
        End Get
        Set(ByVal value As IDisposable)
            _LineOverlay = value
        End Set
    End Property

    'Overlay contenant l'affichage des géométries de type polygon.
    Private Shared Property _PolygonOverlay As IDisposable
    ''' <summary>
    ''' Définir et retourner l'affichage des géométries de type polygon.
    ''' </summary>
    Public Shared Property PolygonOverlay() As IDisposable
        Get
            Return _PolygonOverlay
        End Get
        Set(ByVal value As IDisposable)
            _PolygonOverlay = value
        End Set
    End Property

    'TreeView contenant les géométries de travail.
    Private Shared Property _TreeViewGeometrieTravail As TreeView
    ''' <summary>
    ''' Définir et retourner la liste des géométries de travail
    ''' </summary>
    Public Shared Property TreeViewGeometrieTravail() As TreeView
        Get
            Return _TreeViewGeometrieTravail
        End Get
        Set(ByVal value As TreeView)
            _TreeViewGeometrieTravail = value
        End Set
    End Property
#End Region

#Region "Overrides"
    ''' <summary>
    ''' Called by Framework when ArcGIS Pro is closing
    ''' </summary>
    ''' <returns>False to prevent Pro from closing, otherwise True</returns>
    Protected Overrides Function CanUnload() As Boolean
        'TODO - add your business logic
        'return false to ~cancel~ Application close
        Return True
    End Function
#End Region

#Region "Routines et fonctions publiques"
    '''<summary>
    ''' Fonction qui permet de modifier toutes les géométries d'une liste selon une distance de tampon (buffer).
    ''' Une géométrie de type point ou ligne sera transformée en surface. Une surface sera agrandit.
    '''</summary>
    '''
    '''<param name="geometries">Liste des géométries à traiter.</param>
    '''<param name="distance">Distance utilisée pour créer le tampon (buffer) d'une géométrie.</param>
    '''
    '''<returns>List(Of Geometry) contenant les géométries pour lesquelles un tampon (buffer) a été appliqué.</returns>
    '''
    Public Shared Function BufferListeGeometries(geometries As List(Of Geometry), distance As Double) As List(Of Geometry)
        'Créer une novelle liste de géométries vide
        BufferListeGeometries = New List(Of Geometry)

        'Vérifier la présence de géométries
        If geometries.Count > 0 Then
            'Déclarer les variables de travail
            Dim spatialReference = MapView.Active.Map.SpatialReference

            'Traiter toutes les géométries de travail
            For Each geometrie In geometries
                'Projeter la géométrie
                geometrie = GeometryEngine.Instance.Project(geometrie, spatialReference)

                'Buffer de la géométrie
                Dim geometrieBuffer = GeometryEngine.Instance.Buffer(geometrie, distance)

                'Vérifier si la géométrie n'est pas vide
                If Not geometrieBuffer.IsEmpty Then
                    'Ajouter la géométrie dans la liste
                    BufferListeGeometries.Add(geometrieBuffer)
                End If
            Next
        End If
    End Function

    '''<summary>
    ''' Fonction qui permet l'union des géométries de même type à partir d'une liste de géométries.
    '''</summary>
    '''
    '''<param name="geometries">Liste des géométries à traiter.</param>
    '''<param name="dessiner">Indique si on doit dessiner les géométries.</param>
    '''
    '''<returns>List(Of Geometry) contenant les géométries de même type fusionnées dans trois géométries Multipoint, Polyline et Polygon.</returns>
    '''
    '''<remarks>Si un type de géométrie est vide, il sera absent de la liste des géométries résultante.</remarks>
    '''
    Public Shared Function UnionListeGeometries(geometries As List(Of Geometry), Optional dessiner As Boolean = False) As List(Of Geometry)
        'Créer une novelle liste de géométries vide
        UnionListeGeometries = New List(Of Geometry)

        'Vérifier la présence de géométries
        If geometries.Count > 0 Then
            'Déclarer les variables de travail
            Dim point As MapPoint = Nothing
            Dim multipoint As Multipoint = Nothing
            Dim polyline As Polyline = Nothing
            Dim polygon As Polygon = Nothing
            Dim spatialReference = MapView.Active.Map.SpatialReference
            Dim multipointList = New List(Of Multipoint)
            Dim polylineList = New List(Of Polyline)
            Dim polygonList = New List(Of Polygon)

            'Traiter toutes les géométries de travail
            For Each geometrie In geometries
                'Projeter la géométrie
                geometrie = GeometryEngine.Instance.Project(geometrie, spatialReference)
                'Vérifier si la géométrie est de type point
                If geometrie.GeometryType = GeometryType.Point Then
                    'Définir le point
                    point = geometrie
                    'Ajouter le point dans la liste des multipoints
                    multipointList.Add(MultipointBuilder.CreateMultipoint(point, point.SpatialReference))
                    'Si la géométrie est de type multipoint
                ElseIf geometrie.GeometryType = GeometryType.Multipoint Then
                    'Définir le multipoint
                    multipoint = geometrie
                    'Ajouter le multipoint dans la liste
                    multipointList.Add(multipoint)
                    'Si la géométrie est de type polyline
                ElseIf geometrie.GeometryType = GeometryType.Polyline Then
                    'Définir la polyligne
                    polyline = geometrie
                    'Ajouter la polyline dans la liste
                    polylineList.Add(polyline)
                    'Si la géométrie est de type polygon
                ElseIf geometrie.GeometryType = GeometryType.Polygon Then
                    'Définir le polygone
                    polygon = geometrie
                    'Ajouter le polygone dans la liste
                    polygonList.Add(polygon)
                End If
            Next

            'Vérifier la présence de Multipoints
            If multipointList.Count > 0 Then
                'Union des multipoints
                multipoint = GeometryEngine.Instance.Union(multipointList)
                'Ajouter le multipoint dans la liste
                UnionListeGeometries.Add(multipoint)
            End If

            'Vérifier la présence de Polylignes
            If polylineList.Count > 0 Then
                'Union des Polylignes
                polyline = GeometryEngine.Instance.Union(polylineList)
                'Ajouter la polyligne dans la liste
                UnionListeGeometries.Add(polyline)
            End If

            'Vérifier la présence de Polygones
            If polygonList.Count > 0 Then
                'Union des Polygones
                polygon = GeometryEngine.Instance.Union(polygonList)
                'Ajouter le polygone dans la liste
                UnionListeGeometries.Add(polygon)
            End If

            'Vérifier s'il faut dessiner les géométrie
            If dessiner Then
                'Détruire l'affichage des sommets
                Call DetruireAffichageSommet()

                'Dessiner les géométries de type point
                Call DessinerGeometriePoint(multipoint)

                'Dessiner les géométries de type ligne
                Call DessinerGeometrieLigne(polyline)

                'Dessiner les géométries de type surface
                Call DessinerGeometrieSurface(polygon)
            End If
        End If
    End Function

    '''<summary>
    ''' Fonction qui permet le regroupement des géométries de même type à partir d'une liste de géométries.
    '''</summary>
    '''
    '''<param name="geometries">Liste des géométries à traiter.</param>
    '''<param name="dessiner">Indique si on doit dessiner les géométries.</param>
    '''
    '''<returns>List(Of Geometry) contenant les géométries de même type fusionnées dans trois géométries Multipoint,Polyline et Polygon.</returns>
    '''
    '''<remarks>Si un type de géométrie est vide, il sera absent de la liste des géométries résultante.</remarks>
    '''
    Public Shared Function RegrouperListeGeometries(geometries As List(Of Geometry), Optional dessiner As Boolean = False) As List(Of Geometry)
        'Créer une novelle liste de géométries vide
        RegrouperListeGeometries = New List(Of Geometry)

        'Vérifier la présence de géométries
        If geometries.Count > 0 Then
            'Déclarer les variables de travail
            Dim point As MapPoint = Nothing
                Dim multipoint As Multipoint = Nothing
                Dim polyline As Polyline = Nothing
                Dim polygon As Polygon = Nothing
                Dim spatialReference = MapView.Active.Map.SpatialReference
                Dim multipointBuilder = New MultipointBuilder(spatialReference)
                Dim polylineBuilder = New PolylineBuilder(spatialReference)
                Dim polygonBuilder = New PolygonBuilder(spatialReference)

                'Traiter toutes les géométries de travail
                For Each geometrie In geometries
                    'Projeter la géométrie
                    geometrie = GeometryEngine.Instance.Project(geometrie, spatialReference)
                    'Vérifier si la géométrie est de type point
                    If geometrie.GeometryType = GeometryType.Point Then
                        'Définir le point
                        point = geometrie
                        'Ajouter le point dans la liste
                        multipointBuilder.Add(point)
                        'Si la géométrie est de type multipoint
                    ElseIf geometrie.GeometryType = GeometryType.Multipoint Then
                        'Définir le multipoint
                        multipoint = geometrie
                        'Ajouter les points du multipoint dans la liste
                        multipointBuilder.Add(multipoint.Points)
                        'Si la géométrie est de type polyline
                    ElseIf geometrie.GeometryType = GeometryType.Polyline Then
                        'Définir la polyligne
                        polyline = geometrie
                        'Ajouter les parties de la polyline
                        polylineBuilder.AddParts(polyline.Parts)
                        'Si la géométrie est de type polygon
                    ElseIf geometrie.GeometryType = GeometryType.Polygon Then
                        'Définir le polygone
                        polygon = geometrie
                        'Ajouter les parties du polygone
                        polygonBuilder.AddParts(polygon.Parts)
                    End If
                Next

                'Créer le multipoint
                multipoint = multipointBuilder.ToGeometry
                'Vérifier la présence de points
                If Not multipoint.IsEmpty Then
                    'Ajouter le multipoint dans la liste
                    RegrouperListeGeometries.Add(multipoint)
                End If

                'Créer la polyligne
                polyline = polylineBuilder.ToGeometry
                'Vérifier la présence de lignes
                If Not polyline.IsEmpty Then
                    'Ajouter le polyligne dans la liste
                    RegrouperListeGeometries.Add(polyline)
                End If

                'Créer le polygone
                polygon = polygonBuilder.ToGeometry
                'Vérifier la présence de surfaces
                If Not polygon.IsEmpty Then
                    'Ajouter le polygon dans la liste
                    RegrouperListeGeometries.Add(polygon)
                End If

            'Vérifier s'il faut dessiner les géométrie
            If dessiner Then
                'Détruire l'affichage des sommets
                Call DetruireAffichageSommet()

                'Dessiner les géométries de type point
                Call DessinerGeometriePoint(multipoint)

                'Dessiner les géométries de type ligne
                Call DessinerGeometrieLigne(polyline)

                'Dessiner les géométries de type surface
                Call DessinerGeometrieSurface(polygon)
            End If
        End If
    End Function

    '''<summary>
    ''' Fonction qui permet de séparer les parties de chaque gémétrie d'une liste de géométries.
    ''' Un polygone contenant deux parties extérieures sera transformée en deux polygones. 
    ''' Une polyligne contenant deux lignes sera transformée en deux polylignes. 
    ''' Un multipoint contenant deux points sera transformé en deux points.
    '''</summary>
    '''
    '''<param name="geometries">Liste des géométries à traiter.</param>
    '''
    '''<returns>List(Of Geometry) contenant les parties des géométries séparées.</returns>
    '''
    Public Shared Function SeparerListeGeometries(geometries As List(Of Geometry)) As List(Of Geometry)
        'Créer une novelle liste de géométries vide
        SeparerListeGeometries = New List(Of Geometry)

        'Vérifier la présence de géométries
        If geometries.Count > 0 Then
            'Définir la référence spatiale
            Dim spatialReference = MapView.Active.Map.SpatialReference

            'Traiter toutes les géométries de travail
            For Each geometrie In geometries
                'Projeter la géométrie
                geometrie = GeometryEngine.Instance.Project(geometrie, spatialReference)

                'Vérifier si la géométrie est de type point
                If geometrie.GeometryType = GeometryType.Point Then
                    'Définir le point
                    Dim point As MapPoint = geometrie
                    'Ajouter le point dans la liste
                    SeparerListeGeometries.Add(point)

                    'Si la géométrie est de type multipoint
                ElseIf geometrie.GeometryType = GeometryType.Multipoint Then
                    'Définir le multipoint
                    Dim multipoint As Multipoint = geometrie
                    'Traiter tous les points
                    For Each point In multipoint.Points
                        'Ajouter le point dans la liste
                        SeparerListeGeometries.Add(point)
                    Next

                    'Si la géométrie est de type polyline
                ElseIf geometrie.GeometryType = GeometryType.Polyline Then
                    'Définir la polyligne
                    Dim polyline As Polyline = geometrie
                    'Définir la ligne
                    Dim ligne As Polyline = Nothing
                    'Définir le constructeur de ligne
                    Dim polylineBuilder = Nothing
                    'Traiter toutes les lignes
                    For Each part In polyline.Parts
                        'Créer une polyligne vide
                        polylineBuilder = New PolylineBuilder(spatialReference)
                        'Ajouter la partie de la polyline
                        polylineBuilder.AddPart(part)
                        'Définir la ligne
                        ligne = polylineBuilder.ToGeometry
                        'Ajouter la ligne dans la liste
                        SeparerListeGeometries.Add(ligne)
                    Next

                    'Si la géométrie est de type polygon
                ElseIf geometrie.GeometryType = GeometryType.Polygon Then
                    'Définir le polygone
                    Dim polygon As Polygon = geometrie
                    'Définir l'anneau
                    Dim anneau As Polygon = Nothing
                    'Définir le constructeur de surface
                    Dim polygonBuilder = New PolygonBuilder(spatialReference)

                    'Transformer chaque anneau en polygon
                    Dim rings = polygon.Parts.[Select](Function(p) PolygonBuilder.CreatePolygon(p, spatialReference))
                    'Dim ext = polygon.Parts.Where(Function(p) PolygonBuilder.CreatePolygon(p).Area > 0).OrderByDescending(Function(p) PolygonBuilder.CreatePolygon(p).Area)
                    'Aucun polygon n'est construit
                    polygonBuilder = Nothing
                    'Traiter tous les anneaux
                    For Each ring In rings
                        'Si c'est un anneau extérieur
                        If ring.Area >= 0 Then
                            'Vérifier si un polygone a été construit
                            If polygonBuilder IsNot Nothing Then
                                'Ajouter l'anneau dans la liste
                                SeparerListeGeometries.Add(polygonBuilder.ToGeometry)
                            End If
                            'Créer une polyligne vide
                            polygonBuilder = New PolygonBuilder(ring)

                            'Si c'est un anneau intérieur
                        Else
                            'Ajouter l'anneau intérieur à l'anneau extérieur
                            polygonBuilder.AddPart(ring.Parts.Item(0))
                        End If
                    Next
                    'Vérifier si un polygone a été construit
                    If polygonBuilder IsNot Nothing Then
                        'Ajouter l'anneau dans la liste
                        SeparerListeGeometries.Add(polygonBuilder.ToGeometry)
                    End If
                End If
            Next
        End If
    End Function

    '''<summary>
    ''' Routine qui permet de dessiner la liste de géométries dans la Map active.
    ''' 
    ''' La fenêtre de la Map active peut être déplacée ou recadrée avant de dessiner les géométries selon les paramètres de visualisation.
    '''</summary>
    '''
    '''<param name="geometries">Liste des géométries à afficher.</param>
    '''<param name="dessiner">Indique si on veut forcer pour dessiner les géométries.</param>
    '''<param name="zoomer">Indique si on veut forcer le zoom des géométries.</param>
    '''
    '''<remarks>
    '''Si le paramètre pour centrer les géométries est actif, la fenêtre de la Map active sera déplacée selon le centre de l'enveloppe des géométries.
    '''Si le paramètre pour zoomer les géométries est actif, la fenêtre de la Map active sera recadrée selon l'enveloppe des géométries.
    '''Les géométries seront dessinées si un des paramètres pour dessiner est actif.
    '''</remarks> 
    Public Shared Sub DessinerListeGeometries(geometries As List(Of Geometry), Optional dessiner As Boolean = False, Optional zoomer As Boolean = False)
        'Détruire l'affichage
        Call DetruireAffichage()

        'Vérifier la présence de géométries
        If GeometrieTravail.Count > 0 Then
            'Vérifier si l'action de visialisation est "ZoomGeometrie" ou si on force le zoom
            If ActionVisualisation = EnumActionVisualisation.ZoomGeometrie Or zoomer Then
                'Définir l'enveloppe de la liste des géométries de travail
                Dim envelope = modGeometrieTravail.CalculerEnveloppe(geometries)
                'Agrandir l'envelope
                envelope = envelope.Expand(1.5, 1.5, True)
                'Zoom selon l'envelope agrandit
                MapView.Active.ZoomTo(envelope)

                'Vérifier si l'action de visialisation est "CentreGeometrie"
            ElseIf ActionVisualisation = EnumActionVisualisation.CentreGeometrie Then
                'Définir l'enveloppe de la liste des géométries de travail
                Dim envelope = modGeometrieTravail.CalculerEnveloppe(geometries)
                'Centrer selon le centre de l'envelope
                MapView.Active.PanTo(envelope.Center)
            End If

            'Vérifier si on doit dessiner les géométries
            If modGeometrieTravail.ActiverDessinerGeometrie Or dessiner Then
                'Regrouper et dessiner la liste des géométries
                Call RegrouperListeGeometries(geometries, True)
            End If
        End If
    End Sub

    '''<summary>
    ''' Routine qui permet de dessiner une géométrie dans la Map active.  
    '''</summary>
    '''
    '''<param name="geometrie">Géométrie à afficher.</param>
    ''' 
    Public Shared Sub DessinerGeometrie(geometrie As Geometry)
        'Détruire l'affichage
        modGeometrieTravail.DetruireAffichage()

        'Vérifier si la géométrie n'est pas vide
        If Not geometrie.IsEmpty Then
            'Vérifier si l'action de visialisation est "CentreGeometrie"
            If ActionVisualisation = EnumActionVisualisation.CentreGeometrie Then
                'Définir l'enveloppe de la liste des géométries de travail
                Dim envelope = geometrie.Extent
                'Centrer selon le centre de l'envelope
                MapView.Active.PanTo(envelope.Center)

                'Vérifier si l'action de visialisation est "ZoomGeometrie"
            ElseIf ActionVisualisation = EnumActionVisualisation.ZoomGeometrie Then
                'Définir l'enveloppe de la liste des géométries de travail
                Dim envelope = geometrie.Extent
                'Agrandir l'envelope
                envelope = envelope.Expand(1.5, 1.5, True)
                'Zoom selon l'envelope agrandit
                MapView.Active.ZoomTo(envelope)
            End If

            'Vérifier si on doit dessiner les géométries
            If modGeometrieTravail.ActiverDessinerGeometrie Then
                'Vérifier si la géométrie est un point ou un multipoint
                If geometrie.GeometryType = GeometryType.Point Or geometrie.GeometryType = GeometryType.Multipoint Then
                    'Dessiner les géométries de type point
                    Call modGeometrieTravail.DessinerGeometriePoint(geometrie)

                    'Si la géométrie est de type polyline
                ElseIf geometrie.GeometryType = GeometryType.Polyline Then
                    'Dessiner les géométries de type ligne
                    Call modGeometrieTravail.DessinerGeometrieLigne(geometrie)

                    'Si la géométrie est de type polygon
                ElseIf geometrie.GeometryType = GeometryType.Polygon Then
                    'Dessiner les géométries de type surface
                    Call modGeometrieTravail.DessinerGeometrieSurface(geometrie)
                End If

                'Afficher les sommets des géométries
                Call modGeometrieTravail.DessinerGeometrieSommet(geometrie)
            End If
        End If
    End Sub

    '''<summary>
    ''' Routine qui permet de dessiner les sommets des géométries de travail dans la Map active. 
    '''</summary>
    '''
    '''<param name="geometrie">Géométrie pour lequel on veut afficher ses sommets.</param>
    '''<param name="symbol">Symbole utilisé pour afficher les sommets des géométries.</param>
    ''' 
    Public Shared Async Sub DessinerGeometrieSommet(geometrie As Geometry, Optional symbol As CIMPointSymbol = Nothing)
        'Vérifier si l'overlay des sommets est spécifié
        If _VertexOverlay IsNot Nothing Then
            'Désactiver l'affichage des sommets des géométries de travail
            _VertexOverlay.Dispose()
            'Détruire l'overlay des sommets
            _VertexOverlay = Nothing
        End If

        'Vérifier si la géométrie n'est pas vide
        If Not geometrie.IsEmpty Then
            'Déclarer les variables de travail
            Dim sommet As Geometry = Nothing     'Contient les sommets à afficher.

            'Vérifier si la géométrie est un point ou un multipoint
            If geometrie.GeometryType = GeometryType.Point Or geometrie.GeometryType = GeometryType.Multipoint Then
                'Définir le point à afficher
                sommet = geometrie
                'Si la géométrie est de type polyline
            ElseIf geometrie.GeometryType = GeometryType.Polyline Then
                'Définir la polyligne
                Dim polyline As Polyline = geometrie
                'Créer le multipoint des sommets
                sommet = MultipointBuilder.CreateMultipoint(polyline)
                'Si la géométrie est de type polygon
            ElseIf geometrie.GeometryType = GeometryType.Polygon Then
                'Définir la polylgon
                Dim polygon As Polygon = geometrie
                'Créer le multipoint des sommets
                sommet = MultipointBuilder.CreateMultipoint(polygon)
            End If
            'Définir la Map Active
            Dim activeMapView = MapView.Active
            'Vérifier si le symbol est spécifié
            If symbol Is Nothing Then
                'Définir le symbole pour afficher les points
                symbol = Await QueuedTask.Run(Function() SymbolFactory.Instance.ConstructPointSymbol(ColorFactory.Instance.BlueRGB, 3, SimpleMarkerStyle.Circle))
                'Conserver le symbole des sommets par défaut
                _VertexSymbol = symbol
            End If
            'Créer un Overlay contenant les sommets à afficher
            _VertexOverlay = Await QueuedTask.Run(Function() activeMapView.AddOverlay(sommet, symbol.MakeSymbolReference()))
        End If
    End Sub

    '''<summary>
    ''' Routine qui permet de dessiner une géométrie de type point ou multipoint dans la Map active. 
    '''</summary>
    '''
    '''<param name="point">Géométrie de type point ou multipoint à afficher.</param>
    '''<param name="symbol">Symbole utilisé pour afficher la géométrie de type point ou multipoint.</param>
    ''' 
    Public Shared Async Sub DessinerGeometriePoint(point As Geometry, Optional symbol As CIMPointSymbol = Nothing)
        'Vérifier si l'overlay des points est spécifié
        If _PointOverlay IsNot Nothing Then
            'Désactiver l'affichage des géométries de travail de type point
            _PointOverlay.Dispose()
            'Détruire l'overlay de type point
            _PointOverlay = Nothing
        End If

        'Vérifier si la géométrie n'est pas vide
        If Not point.IsEmpty Then
            'Vérifier si la géométrie est un point ou un multipoint
            If point.GeometryType = GeometryType.Point Or point.GeometryType = GeometryType.Multipoint Then
                'Définir la Map Active
                Dim activeMapView = MapView.Active
                'Vérifier si le symbol est spécifié
                If symbol Is Nothing Then
                    'Définir le symbole pour afficher les points
                    symbol = Await QueuedTask.Run(Function() SymbolFactory.Instance.ConstructPointSymbol(ColorFactory.Instance.RedRGB, 5, SimpleMarkerStyle.Square))
                    'Conserver le symbole de point par défaut
                    _PointSymbol = symbol
                End If
                'Créer un Overlay contenant les points à afficher
                _PointOverlay = Await QueuedTask.Run(Function() activeMapView.AddOverlay(point, symbol.MakeSymbolReference()))
            End If
        End If
    End Sub

    '''<summary>
    ''' Routine qui permet de dessiner une géométrie de type polyline dans la Map active. 
    '''</summary>
    '''
    '''<param name="polyline">Géométrie de type polyline à afficher.</param>
    '''<param name="symbol">Symbole utilisé pour afficher la géométrie de type polyline.</param>
    ''' 
    Public Shared Async Sub DessinerGeometrieLigne(polyline As Polyline, Optional symbol As CIMLineSymbol = Nothing)
        'Vérifier si l'overlay des lignes est spécifié
        If _LineOverlay IsNot Nothing Then
            'Désactiver l'affichage des géométries de travail de type polyline
            _LineOverlay.Dispose()
            'Détruire l'overlay de type polyline
            _LineOverlay = Nothing
        End If

        'Vérifier si la géométrie n'est pas vide
        If Not polyline.IsEmpty Then
            'Définir la Map Active
            Dim activeMapView = MapView.Active
            'Vérifier si le symbol est spécifié
            If symbol Is Nothing Then
                'Définir le symbole pour afficher les polylignes
                symbol = Await QueuedTask.Run(Function() SymbolFactory.Instance.ConstructLineSymbol(ColorFactory.Instance.RedRGB, 1, SimpleLineStyle.Solid))
                'Conserver le symbole de ligne par défaut
                _LineSymbol = symbol
            End If
            'Créer un Overlay contenant les polylignes à afficher
            _LineOverlay = Await QueuedTask.Run(Function() activeMapView.AddOverlay(polyline, symbol.MakeSymbolReference()))
        End If
    End Sub

    '''<summary>
    ''' Routine qui permet de dessiner une géométrie de type polygon dans la Map active. 
    '''</summary>
    '''
    '''<param name="polygon">Géométrie de type polygon à afficher.</param>
    '''<param name="symbol">Symbole utilisé pour afficher la géométrie de type polygon.</param>
    ''' 
    Public Shared Async Sub DessinerGeometrieSurface(polygon As Polygon, Optional symbol As CIMPolygonSymbol = Nothing)
        'Vérifier si l'overlay des surfaces est spécifié
        If _PolygonOverlay IsNot Nothing Then
            'Désactiver l'affichage des géométries de travail de type surface
            _PolygonOverlay.Dispose()
            'Détruire l'overlay de type surface
            _PolygonOverlay = Nothing
        End If

        'Vérifier si la géométrie n'est pas vide
        If Not polygon.IsEmpty Then
            'Définir la Map Active
            Dim activeMapView = MapView.Active
            'Vérifier si le symbol est spécifié
            If symbol Is Nothing Then
                'Définir le symbole pour afficher les polygones
                symbol = Await QueuedTask.Run(Function() SymbolFactory.Instance.ConstructPolygonSymbol(ColorFactory.Instance.RedRGB, SimpleFillStyle.BackwardDiagonal, SymbolFactory.Instance.ConstructStroke(ColorFactory.Instance.RedRGB, 1.0, SimpleLineStyle.Solid)))
                'Conserver le symbole de surface par défaut
                _PolygonSymbol = symbol
            End If
            'Créer un Overlay contenant les polygones à afficher
            _PolygonOverlay = Await QueuedTask.Run(Function() activeMapView.AddOverlay(polygon, symbol.MakeSymbolReference()))
        End If
    End Sub

    '''<summary>
    ''' Routine qui permet de détruire l'affichage des géométries de travail dans la Map active.
    '''</summary>
    Public Shared Sub DetruireAffichage()
        'Détruire l'affichage des sommets des géométries dans la Map active.
        Call DetruireAffichageSommet()

        'Détruire l'affichage des points et multipoints dans la Map active.
        Call DetruireAffichagePoint()

        'Détruire l'affichage des lignes dans la Map active.
        Call DetruireAffichageLigne()

        'Détruire l'affichage des sulfaces dans la Map active.
        Call DetruireAffichageSurface()
    End Sub

    '''<summary>
    ''' Routine qui permet de détruire l'affichage des sommets des géométries de travail dans la Map active.
    '''</summary>
    Public Shared Sub DetruireAffichageSommet()
        'Vérifier si l'affichage des sommets des géométries
        If _VertexOverlay IsNot Nothing Then
            'Désactiver l'affichage
            _VertexOverlay.Dispose()
            'Détruire l'overlay
            _VertexOverlay = Nothing
        End If
    End Sub

    '''<summary>
    ''' Routine qui permet de détruire l'affichage des points et multipoints dans la Map active.
    '''</summary>
    Public Shared Sub DetruireAffichagePoint()
        'Vérifier si l'affichage des géométries de type point est présente
        If _PointOverlay IsNot Nothing Then
            'Désactiver l'affichage
            _PointOverlay.Dispose()
            'Détruire l'overlay
            _PointOverlay = Nothing
        End If
    End Sub

    '''<summary>
    ''' Routine qui permet de détruire l'affichage des lignes dans la Map active.
    '''</summary>
    Public Shared Sub DetruireAffichageLigne()
        'Vérifier si l'affichage des géométries de type ligne est présente
        If _LineOverlay IsNot Nothing Then
            'Désactiver l'affichage
            _LineOverlay.Dispose()
            'Détruire l'overlay
            _LineOverlay = Nothing
        End If
        'Vérifier si l'affichage des géométries de type surface est présente
        If _PolygonOverlay IsNot Nothing Then
            'Désactiver l'affichage
            _PolygonOverlay.Dispose()
            'Détruire l'overlay
            _PolygonOverlay = Nothing
        End If
    End Sub

    '''<summary>
    ''' Routine qui permet de détruire l'affichage des surfaces dans la Map active.
    '''</summary>
    Public Shared Sub DetruireAffichageSurface()
        'Vérifier si l'affichage des géométries de type surface est présente
        If _PolygonOverlay IsNot Nothing Then
            'Désactiver l'affichage
            _PolygonOverlay.Dispose()
            'Détruire l'overlay
            _PolygonOverlay = Nothing
        End If
    End Sub

    '''<summary>
    ''' Fonction qui permet de calculer et retourner l'enveloppe d'une liste de géométries. 
    '''</summary>
    '''
    '''<param name="geometries">Objet contenant une liste de géométries</param>
    ''' 
    '''<returns>Envelope de la liste des géométries. Nothing si aucune géométrie.</returns>
    ''' 
    Public Shared Function CalculerEnveloppe(ByVal geometries As List(Of Geometry)) As Envelope
        'Par défaut, l'envelope est vide
        CalculerEnveloppe = Nothing

        'Traiter toutes les géométries
        For Each geometrie In geometries
            'Projeter la géométrie
            geometrie = GeometryEngine.Instance.Project(geometrie, MapView.Active.Map.SpatialReference)
            'Si aucun enveloppe n'est présent
            If CalculerEnveloppe Is Nothing Then
                'Définir l'enveloppe initiale selon l'enveloppe de la première géométrie
                CalculerEnveloppe = geometrie.Extent
                'Si l'enveloppe est présent
            Else
                'Union avec l'enveloppe existante
                CalculerEnveloppe = CalculerEnveloppe.Union(geometrie.Extent)
            End If
        Next
    End Function

    '''<summary>
    ''' Routine qui permet de remplir un item d'un TreeView à partir d'une liste de géométries, d'une géométrie ou une de ses composantes. 
    '''</summary>
    '''
    '''<param name="geometrie">Objet contenant une liste de géométries, une géométrie ou une de ses composantes.</param>
    ''' 
    Public Shared Sub RemplirTreeViewItemGeometrie(geometrie As Object, ByRef treeViewItem As TreeViewItem)
        Try
            'Vérifier si l'item est valide
            If treeViewItem IsNot Nothing Then
                'Vider les items
                treeViewItem.Items.Clear()
                'Vérifier si l'item n'a pas été traité
                If treeViewItem.Items.Count = 0 Then
                    'Vérifier si l'item sélectionné est une liste
                    If TypeOf (geometrie) Is List(Of Geometry) Then
                        'Traiter toutes les géométries
                        For Each geom In geometrie
                            'Créer un nouvel item
                            Dim item As New TreeViewItem
                            'Définir le nouvel item
                            item.Header = (treeViewItem.Items.Count + 1).ToString & " - " & geom.GeometryType.ToString
                            item.Tag = "GEOMETRY"
                            'Ajouter le nouvel item dans le TreeView
                            treeViewItem.Items.Add(item)
                        Next

                        'Vérifier si l'item sélectionné est un POLYGON
                    ElseIf TypeOf (geometrie) Is Polygon Then
                        'Définir le polygone
                        Dim polygon As Polygon = geometrie
                        'Traiter toutes les parties de la géométrie
                        For Each part In polygon.Parts
                            'Créer un nouvel item
                            Dim item As New TreeViewItem
                            'Définir le nouvel item
                            item.Header = (treeViewItem.Items.Count + 1).ToString & " - Ring"
                            item.Tag = "RING"
                            'Ajouter le nouvel item dans le TreeView
                            treeViewItem.Items.Add(item)
                        Next

                        'Vérifier si l'item sélectionné est une POLYLINE
                    ElseIf TypeOf (geometrie) Is Polyline Then
                        'Définir le polygone
                        Dim polyline As Polyline = geometrie
                        'Traiter toutes les parties de la géométrie
                        For Each part In polyline.Parts
                            'Créer un nouvel item
                            Dim item As New TreeViewItem
                            'Définir le nouvel item
                            item.Header = (treeViewItem.Items.Count + 1).ToString & " - Line"
                            item.Tag = "LINE"
                            'Ajouter le nouvel item dans le TreeView
                            treeViewItem.Items.Add(item)
                        Next

                        'Vérifier si l'item sélectionné est un RING ou une LINE
                    ElseIf TypeOf (geometrie) Is ReadOnlySegmentCollection Then
                        'Définir les segments
                        Dim segments As ReadOnlySegmentCollection = geometrie
                        'Traiter tous les segments
                        For Each segment As Segment In segments
                            'Créer un nouvel item
                            Dim item As New TreeViewItem
                            'Définir le nouvel item
                            item.Header = (treeViewItem.Items.Count + 1).ToString & " - Segment"
                            item.Tag = "SEGMENT"
                            'Ajouter le nouvel item dans le TreeView
                            treeViewItem.Items.Add(item)
                        Next

                        'Vérifier si l'item sélectionné est un SEGMENT
                    ElseIf TypeOf (geometrie) Is Segment Then
                        'Définir les segments
                        Dim segment As Segment = geometrie
                        'Créer un nouvel item
                        Dim item As New TreeViewItem
                        'Définir le nouvel item
                        item.Header = (treeViewItem.Items.Count + 1).ToString & " - Point"
                        item.Tag = "POINT"
                        item.Name = "FIRST"
                        'Ajouter le nouvel item dans le TreeView
                        treeViewItem.Items.Add(item)
                        'Créer un nouvel item
                        item = New TreeViewItem
                        'Définir le nouvel item
                        item.Header = (treeViewItem.Items.Count + 1).ToString & " - Point"
                        item.Tag = "POINT"
                        item.Name = "LAST"
                        'Ajouter le nouvel item dans le TreeView
                        treeViewItem.Items.Add(item)

                        'Vérifier si l'item sélectionné est un MULTIPOINT
                    ElseIf TypeOf (geometrie) Is Multipoint Then
                        'Définir le multipoint
                        Dim multipoint As Multipoint = geometrie
                        'Traiter toutes les parties de la géométrie
                        For Each part In multipoint.Points
                            'Créer un nouvel item
                            Dim item As New TreeViewItem
                            'Définir le nouvel item
                            item.Header = (treeViewItem.Items.Count + 1).ToString & " - Point"
                            item.Tag = "POINT"
                            'Ajouter le nouvel item dans le TreeView
                            treeViewItem.Items.Add(item)
                        Next
                    End If
                End If
            End If

        Catch ex As Exception
            Throw
        End Try
    End Sub

    '''<summary>
    ''' Fonction qui permet de retourner l'information d'une liste de géométries. 
    ''' L'information retournée est le nombre totale de géométries, le nombre de points, le nombre de lignes, 
    ''' la longueur totale des lignes, le nombre de surfaces, la superficie totale des surfaces.
    '''</summary>
    '''
    '''<param name="geometries"> Liste de géométries.</param>
    '''
    '''<returns>La fonction va retourner un "String". Si le traitement n'a pas réussi le "String" est vide. </returns>
    ''' 
    Public Function InfoListeGeometries(geometries As List(Of Geometry)) As String
        'Déclarer les variables de travail
        Dim geometry As Geometry = Nothing      'Interface ESRI contenant une géométrie.
        Dim multipoint As Multipoint = Nothing  'Contient un Multipoint
        Dim polyline As Polyline = Nothing      'Interface ESRI contenant une ligne.
        Dim polygon As Polygon = Nothing        'Interface ESRI contenant une surface.

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
        InfoListeGeometries = ""

        Try
            'Traiter toutes les géométries
            For Each geometry In geometries
                'Vérifier si la géométrie est un point
                If geometry.GeometryType = GeometryType.Point Then
                    'Compter le nombre total de point
                    lNbPoint = lNbPoint + 1

                    'Vérifier si la géométrie est un MultiPoint
                ElseIf geometry.GeometryType = GeometryType.Multipoint Then
                    'Compter le nombre total de MultiPoint
                    lNbMultiPoint = lNbMultiPoint + 1
                    'Définir le multipoint
                    multipoint = geometry

                    'Compter le nombre total de MultiPoint
                    lNbSommetPoint = lNbSommetPoint + multipoint.PointCount

                    'Vérifier si la géométrie est une ligne
                ElseIf geometry.GeometryType = GeometryType.Polyline Then
                    'Compter le nombre total de ligne
                    lNbPolyligne = lNbPolyligne + 1
                    'Définir la polyligne
                    polyline = geometry

                    'Compter le nombre total de lignes
                    lNbLigne = lNbLigne + polyline.PartCount
                    'Compter le nombre total de sommets
                    lNbSommetLigne = lNbSommetLigne + polyline.PointCount
                    'Calculer la longueur totale
                    dLongueurLigne = dLongueurLigne + polyline.Length

                    'Vérifier si la géométrie est une surface
                ElseIf geometry.GeometryType = GeometryType.Polygon Then
                    'Compter le nombre total de surface
                    lNbPolygone = lNbPolygone + 1
                    'Définir le polygone
                    polygon = geometry

                    'Transformer chaque anneau en polygon
                    Dim rings = polygon.Parts.[Select](Function(p) PolygonBuilder.CreatePolygon(p))
                    'Traiter tous les anneaux
                    For Each ring In rings
                        'Si c'est un anneau extérieur
                        If ring.Area >= 0 Then
                            'Compter le nombre d'anneau extérieur
                            lNbExtRing = lNbExtRing + 1

                            'Si c'est un anneau intérieur
                        Else
                            'Compter le nombre d'anneau intérieur
                            lNbIntRing = lNbIntRing + 1
                        End If
                    Next

                    'Compter le nombre total de sommets
                    lNbSommetSurface = lNbSommetSurface + polygon.PointCount
                    'Calculer la longueur des surfaces
                    dLongueurSurface = dLongueurSurface + polygon.Length
                    'Calculer la superficie
                    dSuperficie = dSuperficie + polygon.Area
                End If
            Next

            'Retourner le nombre de géométries présentes
            InfoListeGeometries = "Nombre total de géométries : " & geometries.Count & vbCrLf & vbCrLf

            'Retourner l'information plus complète
            If geometries.Count > 0 Then
                'Retourner l'information pour les points
                If lNbPoint > 0 Then
                    InfoListeGeometries = InfoListeGeometries & "Nombre de points (MapPoint) : " & lNbPoint & vbCrLf & vbCrLf
                End If

                'Retourner l'information pour les multipoints
                If lNbMultiPoint > 0 Then
                    InfoListeGeometries = InfoListeGeometries & "Nombre de multipoints (Multipoint) : " & lNbMultiPoint & vbCrLf
                    InfoListeGeometries = InfoListeGeometries & "Nombre de sommets (MapPoint) : " & lNbSommetPoint & vbCrLf & vbCrLf
                End If

                'Retourner l'information pour les lignes
                If lNbPolyligne > 0 Then
                    InfoListeGeometries = InfoListeGeometries & "Nombre de polylignes (Polyline) : " & lNbPolyligne & vbCrLf
                    InfoListeGeometries = InfoListeGeometries & "Nombre de lignes (Line) : " & lNbLigne & vbCrLf
                    InfoListeGeometries = InfoListeGeometries & "Nombre de sommets (MapPoint) : " & lNbSommetLigne & vbCrLf
                    InfoListeGeometries = InfoListeGeometries & "Longueur totale : " & dLongueurLigne.ToString("F2") & vbCrLf & vbCrLf
                End If

                'Retourner l'information pour les surfaces
                If lNbPolygone > 0 Then
                    InfoListeGeometries = InfoListeGeometries & "Nombre de polygones (Polygon) : " & lNbPolygone & vbCrLf
                    InfoListeGeometries = InfoListeGeometries & "Nombre d'anneaux extérieurs (ExteriorRing) : " & lNbExtRing & vbCrLf
                    InfoListeGeometries = InfoListeGeometries & "Nombre d'anneaux intérieurs (InteriorRing) : " & lNbIntRing & vbCrLf
                    InfoListeGeometries = InfoListeGeometries & "Nombre de sommets (MapPoint) : " & lNbSommetSurface & vbCrLf
                    InfoListeGeometries = InfoListeGeometries & "Longueur totale : " & dLongueurSurface.ToString("F2") & vbCrLf
                    InfoListeGeometries = InfoListeGeometries & "Superficie totale : " & dSuperficie.ToString("F2") & vbCrLf
                End If
            End If

        Catch ex As Exception
            'Retourner l'erreur
            InfoListeGeometries = ex.Message
        Finally
            'Vider la mémoire
            geometry = Nothing
            multipoint = Nothing
            polyline = Nothing
            polygon = Nothing
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner l'information d'une seule géométrie. 
    ''' L'information retournée est le nombre totale de géométries, le nombre de points, le nombre de lignes, 
    ''' la longueur totale des lignes, le nombre de surfaces, la superficie totale des surfaces.
    '''</summary>
    '''
    '''<param name="geometry"> Géométrie pour laquelle on veut retourne l'information.</param>
    '''
    '''<returns>La fonction va retourner un "String". Si le traitement n'a pas réussi le "String" est vide. </returns>
    ''' 
    Public Function InfoGeometrie(geometry As Geometry) As String
        'Initialiser la valeur de retour
        InfoGeometrie = ""

        Try
            'Retourner l'information plus complète
            If geometry IsNot Nothing Then
                'Retourner le type de la géométrie
                InfoGeometrie = "Type de géométrie : " & geometry.GeometryType.ToString & vbCrLf & vbCrLf

                'Vérifier si la géométrie est un point
                If geometry.GeometryType = GeometryType.Point Then
                    'Définir le point
                    Dim point As MapPoint = geometry
                    'Retourner la valeur X
                    InfoGeometrie = InfoGeometrie & "Coordonnée X : " & point.X.ToString & vbCrLf
                    'Retourner la valeur Y
                    InfoGeometrie = InfoGeometrie & "Coordonnée Y : " & point.Y.ToString & vbCrLf
                    'Retourner la valeur Z
                    InfoGeometrie = InfoGeometrie & "Coordonnée Z : " & point.Z.ToString & vbCrLf
                    'Retourner la valeur M
                    InfoGeometrie = InfoGeometrie & "Mesure M : " & point.M.ToString & vbCrLf

                    'Vérifier si la géométrie est un MultiPoint
                ElseIf geometry.GeometryType = GeometryType.Multipoint Then
                    'Définir le multipoint
                    Dim multipoint As Multipoint = geometry
                    'Retourner le nombre de sommtes
                    InfoGeometrie = InfoGeometrie & "Nombre de sommets (MapPoint) : " & multipoint.PointCount & vbCrLf & vbCrLf

                    'Vérifier si la géométrie est une ligne
                ElseIf geometry.GeometryType = GeometryType.Polyline Then
                    'Définir la polyligne
                    Dim polyline As Polyline = geometry
                    'Retourner le nombre de lignes
                    InfoGeometrie = InfoGeometrie & "Nombre de lignes (Line) : " & polyline.PartCount & vbCrLf
                    'Retourner le nombre de sommets
                    InfoGeometrie = InfoGeometrie & "Nombre de sommets (MapPoint) : " & polyline.PointCount & vbCrLf
                    'Retourner la longueur totale
                    InfoGeometrie = InfoGeometrie & "Longueur totale : " & polyline.Length.ToString("F2") & vbCrLf & vbCrLf

                    'Vérifier si la géométrie est une surface
                ElseIf geometry.GeometryType = GeometryType.Polygon Then
                    'Définir le polygone
                    Dim polygon As Polygon = geometry
                    Dim nbExtRing As Integer = 0          'Nombre total d'anneaux extérieurs.
                    Dim nbIntRing As Integer = 0          'Nombre total d'anneaux intérieurs.

                    'Transformer chaque anneau en polygon
                    Dim rings = polygon.Parts.[Select](Function(p) PolygonBuilder.CreatePolygon(p))
                    'Traiter tous les anneaux
                    For Each ring In rings
                        'Si c'est un anneau extérieur
                        If ring.Area >= 0 Then
                            'Compter le nombre d'anneau extérieur
                            nbExtRing = nbExtRing + 1

                            'Si c'est un anneau intérieur
                        Else
                            'Compter le nombre d'anneau intérieur
                            nbIntRing = nbIntRing + 1
                        End If
                    Next

                    'Retourner le nombre d'anneaux extérieurs
                    InfoGeometrie = InfoGeometrie & "Nombre d'anneaux extérieurs (ExteriorRing) : " & nbExtRing & vbCrLf
                    'Retourner le nombre d'anneaux intérieurs
                    InfoGeometrie = InfoGeometrie & "Nombre d'anneaux intérieurs (InteriorRing) : " & nbIntRing & vbCrLf
                    'Retourner le nombre de sommets
                    InfoGeometrie = InfoGeometrie & "Nombre de sommets (MapPoint) : " & polygon.PointCount & vbCrLf
                    'Retourner la longueur totale
                    InfoGeometrie = InfoGeometrie & "Longueur totale : " & polygon.Length.ToString("F2") & vbCrLf
                    'Retourner la superficie totale
                    InfoGeometrie = InfoGeometrie & "Superficie totale : " & polygon.Area.ToString("F2") & vbCrLf
                End If
            End If

        Catch ex As Exception
            'Retourner l'erreur
            InfoGeometrie = ex.Message
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner l'information d'une seule composante de géométrie. 
    ''' L'information retournée est le numéro de composante, le nombre de sommets, la longueur et/ou la superficie.
    '''</summary>
    '''
    '''<param name="geometry"> Géométrie pour laquelle on veut retourner l'information d'une composante.</param>
    '''<param name="index"> Numéro de la composante.</param>
    '''
    '''<returns>La fonction va retourner un "String". Si le traitement n'a pas réussi le "String" est vide. </returns>
    ''' 
    Public Function InfoComposante(geometry As Geometry, index As Integer) As String
        'Initialiser la valeur de retour
        InfoComposante = ""

        Try
            'Retourner l'information plus complète
            If geometry IsNot Nothing Then
                'Retourner le type de la géométrie
                InfoComposante = "Type de géométrie : " & geometry.GeometryType.ToString & vbCrLf & vbCrLf

                'Vérifier si la géométrie est un MultiPoint
                If geometry.GeometryType = GeometryType.Multipoint Then
                    'Définir le multipoint
                    Dim multipoint As Multipoint = geometry
                    'Définir la composante
                    Dim point As MapPoint = multipoint.Points.Item(index)
                    'Retourner le type de la composante
                    InfoComposante = InfoComposante & "Type de composante : " & point.GeometryType.ToString & vbCrLf
                    'Retourner le numéro de la composante
                    InfoComposante = InfoComposante & "Numéro de la composante : " & (index + 1).ToString & "/" & multipoint.Points.Count.ToString & vbCrLf
                    'Retourner la valeur X
                    InfoComposante = InfoComposante & "Coordonnée X : " & point.X.ToString & vbCrLf
                    'Retourner la valeur Y
                    InfoComposante = InfoComposante & "Coordonnée Y : " & point.Y.ToString & vbCrLf
                    'Retourner la valeur Z
                    InfoComposante = InfoComposante & "Coordonnée Z : " & point.Z.ToString & vbCrLf
                    'Retourner la valeur M
                    InfoComposante = InfoComposante & "Mesure M : " & point.M.ToString & vbCrLf

                    'Vérifier si la géométrie est une ligne
                ElseIf geometry.GeometryType = GeometryType.Polyline Then
                    'Définir la polyligne
                    Dim polyline As Polyline = geometry
                    'Définir la ligne
                    Dim line As Polyline = PolylineBuilder.CreatePolyline(polyline.Parts.Item(index))
                    'Retourner le type de la composante
                    InfoComposante = InfoComposante & "Type de composante : Line" & vbCrLf
                    'Retourner le numéro de la composante
                    InfoComposante = InfoComposante & "Numéro de la composante : " & (index + 1).ToString & "/" & polyline.Parts.Count.ToString & vbCrLf
                    'Retourner le nombre de sommets
                    InfoComposante = InfoComposante & "Nombre de sommets (MapPoint) : " & line.PointCount & vbCrLf
                    'Retourner la longueur totale
                    InfoComposante = InfoComposante & "Longueur totale : " & line.Length.ToString("F2") & vbCrLf & vbCrLf

                    'Vérifier si la géométrie est une surface
                ElseIf geometry.GeometryType = GeometryType.Polygon Then
                    'Définir le polygone
                    Dim polygon As Polygon = geometry
                    'Définir l'anneau
                    Dim ring As Polygon = PolygonBuilder.CreatePolygon(polygon.Parts.Item(index))
                    'Vérifier si c'est un anneau extérieur
                    If ring.Area >= 0 Then
                        'Retourner le type de la composante
                        InfoComposante = InfoComposante & "Type de composante : ExteriorRing" & vbCrLf
                        'Si c'est un anneau interieur
                    Else
                        'Retourner le type de la composante
                        InfoComposante = InfoComposante & "Type de composante : InteriorRing" & vbCrLf
                    End If
                    'Retourner le numéro de la composante
                    InfoComposante = InfoComposante & "Numéro de la composante : " & (index + 1).ToString & "/" & polygon.Parts.Count.ToString & vbCrLf
                    'Retourner le nombre de sommets
                    InfoComposante = InfoComposante & "Nombre de sommets (MapPoint) : " & ring.PointCount & vbCrLf
                    'Retourner la longueur totale
                    InfoComposante = InfoComposante & "Longueur totale : " & ring.Length.ToString("F2") & vbCrLf
                    'Retourner la superficie totale
                    InfoComposante = InfoComposante & "Superficie totale : " & ring.Area.ToString("F2") & vbCrLf
                End If
            End If
        Catch ex As Exception
            'Retourner l'erreur
            InfoComposante = ex.Message
        End Try
    End Function

    '''<summary>
    ''' Fonction qui permet de retourner l'information d'un seul segment d'une composante de géométrie. 
    ''' L'information retournée est le numéro de composanteméro de segment, le nombre de sommets, la longueur et/ou la superficie.
    '''</summary>
    '''
    '''<param name="geometry"> Géométrie pour laquelle on veut retourner l'information d'une composante.</param>
    '''<param name="index"> Numéro de la composante.</param>
    '''
    '''<returns>La fonction va retourner un "String". Si le traitement n'a pas réussi le "String" est vide. </returns>
    ''' 
    Public Function InfoSegment(geometry As Geometry, index As Integer, no As Integer) As String
        'Initialiser la valeur de retour
        InfoSegment = ""

        Try
            'Retourner l'information plus complète
            If geometry IsNot Nothing Then
                'Retourner le type de la géométrie
                InfoSegment = "Type de géométrie : " & geometry.GeometryType.ToString & vbCrLf & vbCrLf

                'Vérifier si la géométrie est une ligne
                If geometry.GeometryType = GeometryType.Polyline Then
                    'Définir la polyligne
                    Dim polyline As Polyline = geometry
                    'Définir le segment
                    Dim segment As Segment = polyline.Parts.Item(index).Item(no)
                    'Retourner le type de la composante
                    InfoSegment = InfoSegment & "Type de composante : Segment" & vbCrLf
                    'Retourner le numéro de la composante
                    InfoSegment = InfoSegment & "Numéro de la composante : " & (index + 1).ToString & "/" & polyline.Parts.Count.ToString & vbCrLf
                    'Retourner le numéro du segment
                    InfoSegment = InfoSegment & "Numéro du segment : " & (no + 1).ToString & "/" & polyline.Parts.Item(index).Count.ToString & vbCrLf
                    'Retourner le nombre de sommets
                    InfoSegment = InfoSegment & "Nombre de sommets (MapPoint) : 2" & vbCrLf
                    'Retourner la longueur totale
                    InfoSegment = InfoSegment & "Longueur totale : " & segment.Length.ToString("F2") & vbCrLf & vbCrLf

                    'Vérifier si la géométrie est une surface
                ElseIf geometry.GeometryType = GeometryType.Polygon Then
                    'Définir le polygone
                    Dim polygon As Polygon = geometry
                    'Définir le segment
                    Dim segment As Segment = polygon.Parts.Item(index).Item(no)
                    'Retourner le type de la composante
                    InfoSegment = InfoSegment & "Type de composante : Segment" & vbCrLf
                    'Retourner le numéro de la composante
                    InfoSegment = InfoSegment & "Numéro de la composante : " & (index + 1).ToString & "/" & polygon.Parts.Count.ToString & vbCrLf
                    'Retourner le numéro du segment
                    InfoSegment = InfoSegment & "Numéro du segment : " & (no + 1).ToString & "/" & polygon.Parts.Item(index).Count.ToString & vbCrLf
                    'Retourner le nombre de sommets
                    InfoSegment = InfoSegment & "Nombre de sommets (MapPoint) : 2" & vbCrLf
                    'Retourner la longueur totale
                    InfoSegment = InfoSegment & "Longueur totale : " & segment.Length.ToString("F2") & vbCrLf & vbCrLf
                End If
            End If
        Catch ex As Exception
            'Retourner l'erreur
            InfoSegment = ex.Message
        End Try
    End Function
#End Region
End Class
