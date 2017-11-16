Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Editor
'**
'Nom de la composante : modUniteTravail.vb
'
'''<summary>
''' Ce module contient des constantes, variables, fonctions et routines utilisée pour manipuler 
''' les divers unités de travail (CANADA,SNRC,...).
'''</summary>
'''
'''<remarks>
'''Auteur : Michel Pothier
'''</remarks>
''' 
Module modUniteTravail
    '''<summary> Classe contenant le formulaire pour l'initialisation d'un unité de travail. </summary>
    Public m_MenuUniteTravail As dckMenuUniteTravail
    '''<summary> Objet contenant l'information de l'unité de travail. </summary>
    Public m_Application As IApplication
    '''<summary> Objet contenant l'information de l'unité de travail. </summary>
    Public m_MxDocument As IMxDocument
    '''<summary> Objet contenant l'information de l'unité de travail. </summary>
    Public m_UniteTravail As clsUniteTravail
    '''<summary> 'Interface ESRI contenant le symbol pour le texte. </summary>
    Public m_SymboleTexte As ISymbol
    '''<summary> 'Interface ESRI contenant le symbol pour le point. </summary>
    Public m_SymbolePoint As ISymbol
    '''<summary> 'Interface ESRI contenant le symbol pour la géométrie. </summary>
    Public m_SymboleSurface As ISymbol
    '''<summary> 'Interface ESRI contenant la couleur RGB pour le symbol pour la géométrie. </summary>
    Public m_RgbColor As IRgbColor
    '''<summary> 'Indique si la géométrie doit être précise ou non. </summary>
    Public m_GeometriePrecise As Boolean

    '''<summary>Valeur initiale de la dimension en hauteur du menu.</summary>
    Public m_Height As Integer = 300
    '''<summary>Valeur initiale de la dimension en largeur du menu.</summary>
    Public m_Width As Integer = 300

    '''<summary>
    '''Routine qui permet de créer et valider une unité de travail selon un numéro.
    '''L'unité de travail est conservé via un variant qui pointe vers un objet qui
    '''contient l'information de l'unité de travail.
    '''</summary>
    ''' 
    '''<param name="sNumero">Numéro de l'unité de travail à créer et valider.</param>
    ''' 
    '''<returns>
    '''Retourner l'objet contenant l'information et la géométrie de l'unité  de travail.
    '''</returns>
    Public Function CreerUniteTravail(ByVal sNumero As String) As clsUniteTravail
        'Déclarer les variables de travail
        Dim oSnrc As clsSNRC = Nothing             'Objet contenant l'information d'un SNRC
        Dim oCanada As clsUniteTravail = Nothing   'Objet contenant l'information d'un Unite de travail CANADA

        Try
            'Définir la valeur par défaut
            CreerUniteTravail = Nothing

            'Convertir le numéro en majuscule
            sNumero = UCase(sNumero)

            'Créer une nouvelle classe selon l'unité de travail
            If sNumero = "CANADA" Then
                'Définir l'unité de travail selon le CANADA
                oCanada = New clsUniteTravail

                'Conserver l'unité de travail
                m_UniteTravail = oCanada
            Else
                'Définir l'unité de travail selon le SNRC
                oSnrc = New clsSNRC(sNumero)

                'Conserver l'unité de travail
                m_UniteTravail = oSnrc
            End If

            'Retourner le résultat
            CreerUniteTravail = m_UniteTravail

        Finally
            'Vider la mémoire
            oCanada = Nothing
            oSnrc = Nothing
        End Try
    End Function

    '''<summary>
    '''Routine qui permet de dessiner dans la vue active la géométrie calculée à partir de l'unité de travail courant.
    '''</summary>
    '''
    Public Sub DessinerGeometrieUniteTravail()
        'Déclarer les variables de travail
        Dim pPolygon As IPolygon = Nothing              'Interface ESRI contenant le polygone de l'unité de travail.
        Dim pActiveView As IActiveView = Nothing        'Interface ESRI contenant la vue active.
        Dim pScreenDisplay As IScreenDisplay = Nothing  'Interface ESRI contenant la fenêtre d'affichage.

        Try
            'Initialiser les variables de travail
            pActiveView = CType(m_MxDocument.FocusMap, IActiveView)
            pScreenDisplay = pActiveView.ScreenDisplay

            'Créer le polygone de l'unité de travail
            pPolygon = CreerPolygoneUniteTravail()

            'Vérifier si le polygone est vide
            If pPolygon.IsEmpty Then Exit Try

            'Transformation du système de coordonnées selon la vue active
            pPolygon.Project(m_MxDocument.FocusMap.SpatialReference)

            'Afficher la géométrie avec sa symbologie dans la vue active
            With pScreenDisplay
                .StartDrawing(pScreenDisplay.hDC, CShort(esriScreenCache.esriNoScreenCache))
                .SetSymbol(m_SymboleSurface)
                .DrawPolygon(pPolygon)
                .FinishDrawing()
            End With

        Finally
            'Vider la mémoire
            pPolygon = Nothing
            pActiveView = Nothing
            pScreenDisplay = Nothing
        End Try
    End Sub

    '''<summary>
    '''Routine qui permet d'afficher la fenêtre graphique selon l'enveloppe de la géométrie de l'unité de
    '''travail  plus 10%.
    '''</summary>
    '''
    Public Sub ZoomGeometrieUniteTravail()
        'Déclarer les variables de travail
        Dim pPolygon As IPolygon = Nothing      'Interface ESRI contenant la surface de l'unité de travail.
        Dim pEnvelope As IEnvelope = Nothing    'Interface ESRI contenant l'enveloppe de l'unité de travail.

        Try
            'Créer le polygone de l'unité de travail
            pPolygon = CreerPolygoneUniteTravail()

            'Vérifier si le polygone est vide
            If pPolygon.IsEmpty Then Exit Try

            'Transformation du système de coordonnées selon la vue active
            pPolygon.Project(m_MxDocument.FocusMap.SpatialReference)

            'Définir l'enveloppe de l'élément en erreur qui n'est pas un point
            pEnvelope = pPolygon.Envelope

            'Agrandir l'enveloppe de 10% de l'élément en erreur
            pEnvelope.Expand(pEnvelope.Width / 10, pEnvelope.Height / 10, False)

            'Définir la nouvelle fenêtre de travail
            m_MxDocument.ActiveView.Extent = pEnvelope

            'Rafraîchier l'affichage
            m_MxDocument.ActiveView.Refresh()

            'Permet de vider la mémoire sur les évènements
            System.Windows.Forms.Application.DoEvents()

        Finally
            'Vider la mémoire
            pEnvelope = Nothing
            pPolygon = Nothing
        End Try
    End Sub

    '''<summary>
    '''Routine qui permet de dessiner   la géométrie de l'unité de travail dans la fenêtre graphique.
    '''</summary>
    '''
    Public Sub DessinerLimiteGeometrieUniteTravail()
        'Déclarer les variables de travail
        Dim pPolygon As IPolygon = Nothing              'Interface ESRI contenant la surface de l'unité de travail.
        Dim pPolyline As IPolyline = Nothing            'Interface ESRI contenant la limite de la géométrie.
        Dim pGeometry As IGeometry = Nothing            'Interface ESRI contenant le buffer créé.
        Dim pTopoOp As ITopologicalOperator2 = Nothing  'Interface ESRI utilisée pour créer le buffer.
        Dim pActiveView As IActiveView = Nothing        'Interface ESRI contenant la vue active.
        Dim pScreenDisplay As IScreenDisplay = Nothing  'Interface ESRI contenant la fenêtre d'affichage.
        Dim sBufferDistance As String = Nothing         'Texte contenant la dimension du buffer à créer.

        Try
            'Définir les variables de travail
            pActiveView = CType(m_MxDocument.FocusMap, IActiveView)
            pScreenDisplay = pActiveView.ScreenDisplay

            'Créer le polygone de l'unité de travail
            pPolygon = CreerPolygoneUniteTravail()

            'Transformation du système de coordonnées selon la vue active
            pPolygon.Project(m_MxDocument.FocusMap.SpatialReference)

            'Get a buffer distance from the user
            sBufferDistance = InputBox("Entrer la distance : ", "Buffer", "1")
            If sBufferDistance = "" Or Not IsNumeric(sBufferDistance) Then Exit Try

            'Créer le buffer sur le polygon
            pTopoOp = CType(pPolygon, ITopologicalOperator2)
            pPolyline = CType(pTopoOp.Boundary, IPolyline)
            pTopoOp = CType(pPolyline, ITopologicalOperator2)
            pTopoOp.Simplify()
            pGeometry = pTopoOp.Buffer(CDbl(sBufferDistance))

            'Afficher la géométrie avec sa symbologie dans la vue active
            With pScreenDisplay
                .StartDrawing(pScreenDisplay.hDC, CShort(esriScreenCache.esriNoScreenCache))
                .SetSymbol(m_SymboleSurface)
                .DrawPolygon(pGeometry)
                .FinishDrawing()
            End With

        Finally
            'Vider la mémoire
            pPolygon = Nothing
            pPolyline = Nothing
            pGeometry = Nothing
            pTopoOp = Nothing
            pActiveView = Nothing
            pScreenDisplay = Nothing
        End Try
    End Sub

    '''<summary>
    '''Construire un élément selon la géométrie de l'unité de travail et selon le Layer de construction.
    '''Le mode édition doit être actif. L'unité de travait courant doit être actif.
    '''</summary>
    '''
    '''<returns>
    '''La fonction va retourner un "Boolean". Si le traitement n'a pas réussi le "Boolean" est à "False".
    '''</returns>
    Public Function ConstruireElementUniteTravail() As Boolean
        'Déclarer les variables de travail
        Dim pEditLayers As IEditLayers = Nothing        'Interface ESRI utilisée pour extraire le Layer de construction.
        Dim pFeatureLayer As IFeatureLayer = Nothing    'Interface ESRI contenant l'affichage de la classe de l'élément à créer.
        Dim pFeatureClass As IFeatureClass = Nothing    'Interface ESRI contenant la classe de l'élément à créer.
        Dim pFeature As IFeature = Nothing              'Interface ESRI contenant l'élément à créer.
        Dim pGeometry As IGeometry = Nothing              'Interface ESRI contenant la surface de l'unité de travail.
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.
        Dim pTopoOp As ITopologicalOperator = Nothing   'Interface ESRI utilisée pour extrire la limite de la surface.
        Dim pEditor As IEditor = Nothing                'Interface ESRI utilisée pour effectuer l'édition.
        Dim sResultat As String = Nothing               'Résultat de la question sur le nom de l'attribut contenant l'identifiant.
        Dim i As Integer = Nothing                      'Compteur

        Try
            'Définir le résultat par défaut
            ConstruireElementUniteTravail = False

            'Initialiser le mode d'édition
            pEditor = ModeEditor()

            'Vérifier si on est en mode édition
            If pEditor Is Nothing Then
                'Message d'erreur
                MsgBox("ATTENTION : Vous devez être en mode édition")
                Exit Function
            End If

            'Interface pour extraire le Layer de construction
            pEditLayers = CType(pEditor, IEditLayers)

            'Extraire le Layer de construction
            pFeatureLayer = pEditLayers.CurrentLayer

            'Définir la FeatureClass de Layer de construction
            pFeatureClass = pFeatureLayer.FeatureClass

            'Vérifier si la classe de construction est une ligne ou une surface
            If pFeatureClass.ShapeType = esriGeometryType.esriGeometryPolyline Or _
            pFeatureClass.ShapeType = esriGeometryType.esriGeometryPolygon Then
                'Débuter l'opération de construction avec un UnDo
                pEditor.StartOperation()

                'Créer un nouvel élément
                pFeature = pFeatureClass.CreateFeature

                'Créer le polygone de l'unité de travail
                pGeometry = CreerPolygoneUniteTravail()

                'Mettre la géométrie de l'unité de travail selon la référence de l'élément
                pGeometry.Project(m_MxDocument.FocusMap.SpatialReference)

                'Vérifier si le type de géométrie est une surface
                If pFeatureClass.ShapeType = esriGeometryType.esriGeometryPolyline Then
                    'Interface pour extraire la limite
                    pTopoOp = CType(pGeometry, ITopologicalOperator)
                    'Extraire la limite de la géométrie
                    pGeometry = pTopoOp.Boundary
                End If

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

                'Ajouter la géométrie dans l'élément
                pFeature.Shape = pGeometry

                'Ajouter les valeur d'attribut par défaut
                For i = 0 To pFeature.Fields.FieldCount - 1
                    'Ajouter la valeur par défaut
                    Call ModifierValeurAttributSelonDefaut(pFeature, i)
                Next i

                'Demander le nom de l'attribut contenant l'identifiant de l'unité de travail
                sResultat = InputBox("Nom de l'attribut contenant l'identifiant : ", "Identifiant", "IDENTIFIANT")
                If sResultat = "" Then Exit Function

                'Définir le numéro de l'unité de travail si l'identifiant est présent
                i = pFeature.Fields.FindField(sResultat)

                'Vérifier la présence de l'identifiant
                If i >= 0 Then
                    'Modifier le numéro de l'identifiant
                    pFeature.Value(i) = m_UniteTravail.Numero
                End If

                'Sauver le traiter
                pFeature.Store()

                'Terminer l'opération UnDo
                pEditor.StopOperation("Créer un nouvel unité de travail")
                pEditor = Nothing

                'Rafraîchir l'affichage
                m_MxDocument.ActiveView.Refresh()

                'Retourner le résultat
                ConstruireElementUniteTravail = True
                'Sinon
            Else
                'Message d'erreur
                MsgBox("Erreur : La classe de construction n'est pas une ligne ou une surface")
            End If

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
            pGeometryDef = Nothing
            pTopoOp = Nothing
        End Try
    End Function

    '''<summary>
    '''Routine qui permet de remplacer la géométrie d'un élément sélectionné selon la géométrie de
    '''l'unité de travail. Le mode édition doit être actif. L'unité de travait courant doit être actif.
    '''</summary>
    '''
    Public Sub RemplacerGeometrieUniteTravail()
        'Déclarer les variables de travail
        Dim pMap As IMap = Nothing                  'Interface ESRI contenant les Layers de la vue active.
        Dim pEnumFeature As IEnumFeature = Nothing  'Interface ESRI pour traiter les éléments sélectionnés.
        Dim pFeature As IFeature = Nothing          'Interface ESRI contenant l'élément traité.
        Dim pNewGeometry As IGeometry = Nothing     'Interface contenant la nouvelle géométrie.
        Dim pGeometryDef As IGeometryDef = Nothing  'Interface ESRI qui permet de vérifier la présence du Z et du M.
        Dim pPolygon As IPolygon = Nothing          'Interface ESRI contenant la géométrie à remplacer.
        Dim pTopoOp As ITopologicalOperator = Nothing 'Interface ESRI utilisée pour extrire la limite de la surface.
        Dim pEditor As IEditor = Nothing            'Interface ESRI utilisée pour effectuer l'édition.
        Dim pDatasetEdit As IDatasetEdit = Nothing  'Interface ESRI utilisé pour vérifier si un élément est en mode Edit
        Dim pSnrc As clsSNRC = Nothing              'Interface ESRI utilisée pour effectuer l'édition.

        Try
            'Initialiser le mode d'édition
            pEditor = ModeEditor()

            'Vérifier si on est en mode édition
            If pEditor Is Nothing Then
                'Message d'erreur
                MsgBox("ATTENTION : Vous devez être en mode édition")
                Exit Sub
            End If

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Vérifier la présece d'un seul élément
            If pMap.SelectionCount <> 1 Then
                MsgBox("ATTENTION : Vous devez sélectionner un seul élément")
                Exit Try
            End If

            'Débuter l'opération UnDo
            pEditor.StartOperation()

            'Créer le polygone de l'unité de travail
            pPolygon = CreerPolygoneUniteTravail()

            'Mettre la géométrie de l'unité de travail selon la référence de l'élément
            pPolygon.Project(pMap.SpatialReference)

            'Interface pour extraire la limite
            pTopoOp = CType(pPolygon, ITopologicalOperator)

            'Trouver le premier élément
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)
            pFeature = pEnumFeature.Next

            'Trouver les autres éléments
            Do Until pFeature Is Nothing
                'Interface qui permet d'indiquer si l'élément est en mode édition
                pDatasetEdit = CType(pFeature.Class, IDatasetEdit)
                'Vérifier si l'élément est en mode édition
                If pDatasetEdit.IsBeingEdited Then
                    'Vérifier si la géométrie de l'élément est une ligne
                    If pFeature.Shape.GeometryType = esriGeometryType.esriGeometryPolyline Then
                        'Définir la nouvelle géométrie
                        pNewGeometry = pTopoOp.Boundary
                        'Traiter le Z et le M
                        Call TraiterZ(pNewGeometry)
                        Call TraiterM(pNewGeometry)
                        'Changer la géométrie de l'élément sélectionné
                        pFeature.Shape = pNewGeometry
                        'Sauver le traitement
                        pFeature.Store()
                        'Vérifier si la géométrie de l'élément est un polygone
                    ElseIf pFeature.Shape.GeometryType = esriGeometryType.esriGeometryPolygon Then
                        'Définir la nouvelle géométrie
                        pNewGeometry = pPolygon

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
                        'Changer la géométrie de l'élément sélectionné
                        pFeature.Shape = pNewGeometry
                        'Sauver le traitement
                        pFeature.Store()
                    Else
                        MsgBox("ATTENTION : La géométrie de l'élément sélectionné n'est pas une surface")
                        Exit Try
                    End If
                Else
                    MsgBox("ATTENTION : L'élément sélectionné n'est pas en mode édition")
                    Exit Try
                End If

                'Trouver le prochain élément
                pFeature = pEnumFeature.Next
            Loop

            'Rafraîchir l'affichage
            m_MxDocument.ActiveView.Refresh()

            'Terminer l'opération UnDo
            pEditor.StopOperation("Remplacer la géométrie selon l'unité de travail")
            pEditor = Nothing

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
            pNewGeometry = Nothing
            pPolygon = Nothing
            pTopoOp = Nothing
            pEditor = Nothing
            pGeometryDef = Nothing
            pDatasetEdit = Nothing
        End Try
    End Sub

    '''<summary>
    ''' Routine qui permet d'effectuer le traitement d'intersection des éléments sélectionnés avec l'unité de travail. 
    ''' Seulement les éléments qui ont la contraite "Overlap" avec l'unité de travail de même type seront traités. 
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
        Dim pGeometryA As IGeometry = Nothing           'Interface ESRI contenant la géométrie à traiter.
        Dim pGeometryB As IGeometry = Nothing           'Interface ESRI contenant la géométrie de travail.
        Dim pGeometryC As IGeometry = Nothing           'Interface ESRI contenant la géométrie de travail.
        Dim pNewGeometry As IGeometry = Nothing         'Interface ESRI contenant la nouvelle géométrie traitée.
        Dim pGeometryDef As IGeometryDef = Nothing      'Interface ESRI qui permet de vérifier la présence du Z et du M.
        Dim pRelOp As IRelationalOperator = Nothing     'Interface ESRI pour vérifier une contraite d'intégrité.
        Dim pTopoOp As ITopologicalOperator2 = Nothing  'Interface ESRI pour effectuer le traitement des géométries.
        Dim pDatasetEdit As IDatasetEdit = Nothing      'Interface ESRI utilisé pour vérifier si un élément est en mode Edit
        Dim i As Integer = Nothing       'Compteur
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

            'Vérifier si aucun élément est en mode édition
            If pEditor.SelectionCount = 0 Then
                'Message d'erreur
                MsgBox("ATTENTION : Aucun élément sélectionné n'est en mode édition")
                pEditor = Nothing
                Exit Try
            End If

            'Débuter l'opération UnDo
            pEditor.StartOperation()

            'Définir la Map courante
            pMap = m_MxDocument.FocusMap

            'Trouver le premier élément
            pEnumFeature = CType(pMap.FeatureSelection, IEnumFeature)
            pFeature = pEnumFeature.Next

            'Trouver les autres éléments
            Do Until pFeature Is Nothing
                'Interface qui permet d'indiquer si l'élément est en mode édition
                pDatasetEdit = CType(pFeature.Class, IDatasetEdit)
                'Vérifier si l'élément est en mode édition
                If pDatasetEdit.IsBeingEdited Then
                    'Copie de la géométrie originale
                    pGeometryA = CType(pFeature.ShapeCopy, IGeometry)

                    If pGeometryA.GeometryType <> esriGeometryType.esriGeometryPoint Then
                        'Interface pour couper les éléments
                        pTopoOp = CType(pGeometryA, ITopologicalOperator2)
                        pTopoOp.IsKnownSimple_2 = False
                        pTopoOp.Simplify()
                    End If

                    'Interface pour vérifer la contrainte entre les éléments
                    pRelOp = CType(pGeometryA, IRelationalOperator)

                    'Définir la géométrie B
                    pGeometryB = CreerPolygoneUniteTravail()

                    'Mettre la géométrie de l'unité de travail selon la référence de l'élément
                    pGeometryB.Project(pMap.SpatialReference)

                    'Interface pour simplifier la géométrie
                    pTopoOp = CType(pGeometryB, ITopologicalOperator2)
                    pTopoOp.IsKnownSimple_2 = False
                    pTopoOp.Simplify()

                    'Vérifier si la contrainte Overlap est valide
                    If pRelOp.Disjoint(pGeometryB) Then
                        'Détruire l'élément
                        pFeature.Delete()

                        'Sinon si la géométrie est différence d'un point
                    ElseIf pGeometryA.GeometryType <> esriGeometryType.esriGeometryPoint Then
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
                        End If
                    End If
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
                pEditor.StopOperation("Intersection selon l'unité de travail")
            End If

        Catch erreur As Exception
            MsgBox(erreur.ToString)
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
            pDatasetEdit = Nothing
            pGeometryDef = Nothing
        End Try
    End Sub

    '''<summary>
    '''Fonction qui permet de créer le polygone de l'unité de travail selon l'indice de précision initialisé.
    '''L'indice de précision indique si la distance entre les sommets doit être plus grande (moins précis 15 ou 60 minutes)
    '''ou plus petite (plus précis 1 ou 30 minutes) tout dépendant de la catégorie SNRC ou CANADA.
    '''</summary>
    '''
    '''<returns>
    '''La fonction va retourner un "IPolygon" correspondant à l'unité de travail. "Nothing" sinon.
    '''</returns>
    Public Function CreerPolygoneUniteTravail() As IPolygon
        'Déclarer les variables de travail
        Dim pPolygon As IPolygon = Nothing  'Interface ESRI contenant la surface de l'unité de travail.
        Dim oSnrc As clsSNRC = Nothing 'Interface ESRI contenant la surface du SNRC.

        Try
            'Définir le résultat par défaut
            CreerPolygoneUniteTravail = Nothing

            'Vérifier si la catégorie de l'unité de travail est CANADA
            If m_UniteTravail.Categorie = "CANADA" Then
                'Vérifier si la géométrie est précise
                If m_GeometriePrecise Then
                    'Définir le polygone de l'unité de travail précis
                    pPolygon = m_UniteTravail.PolygoneGeo(30)
                Else
                    'Définir le polygone de l'unité de travail non précis
                    pPolygon = m_UniteTravail.PolygoneGeo(60)
                End If
                'Si c'est SNRC
            Else
                'Interface pour afficher l'information sur le SNRC
                oSnrc = CType(modUniteTravail.m_UniteTravail, clsSNRC)
                'Vérifier si la géométrie est précise
                If m_GeometriePrecise Then
                    'Définir le polygone de l'unité de travail précis
                    pPolygon = oSnrc.PolygoneGeo(1)
                Else
                    'Définir le polygone de l'unité de travail non précis
                    pPolygon = oSnrc.PolygoneGeo(15)
                End If
            End If

            'Retourner le résultat
            CreerPolygoneUniteTravail = pPolygon

        Finally
            'Vider la mémoire
            pPolygon = Nothing
            oSnrc = Nothing
        End Try
    End Function

    '''<summary>
    '''Fonction qui permet de modifier la valeur d'attribut selon la valeur par défaut de la classe ou la sous-classe.
    '''Si l'élément possède des sous-classes, la valeur par défaut associée à la sous-classe sera utilisée mais
    '''si l'élément ne possède pas de sous-classe, alors ce sera la valeur par défaut de classe qui sera utilisée.
    '''</summary>
    ''' 
    '''<param name="pFeature">Élément pour lequel on veut modifier un attribut.</param>
    '''<param name="iIndexAttribut">Position de l'attribut à modifier.</param>
    ''' 
    '''<returns>
    '''La fonction va retourner un "Boolean". Si le traitement n'a pas réussi le "Boolean" est à "False".
    '''</returns>
    Public Function ModifierValeurAttributSelonDefaut(ByVal pFeature As IFeature, ByVal iIndexAttribut As Integer) As Boolean
        'Déclarer les variables de travail
        Dim pObjectClass As IObjectClass = Nothing  'Interface ESRI contenant une classe.
        Dim pFeatureClass As IFeatureClass = Nothing 'Interface ESRI contenant des éléments sur disque.
        Dim pSubtypes As ISubtypes = Nothing        'Interface ESRI contenant les subtypes (sous-classes).
        Dim pField As IField = Nothing              'Interface ESRI contenant un attribut d'un élément.
        Dim iIndexSousClasse As Integer = Nothing   'Contient la position de l'attribut de la sous-classe.

        Try
            'Définir les subtypes
            pObjectClass = pFeature.Class
            pFeatureClass = CType(pObjectClass, IFeatureClass)
            'Attrapper les erreurs (BUG)
            Try
                pSubtypes = CType(pFeatureClass, ISubtypes)
            Catch
                pSubtypes = Nothing
            End Try

            'Définir l'attribut à corriger
            pField = pFeatureClass.Fields.Field(iIndexAttribut)

            'Vérifier si on peut modifier l'attribut et si ce n'est pas la géométrie
            If pField.Editable And pField.Name <> pFeatureClass.ShapeFieldName Then
                'Ajouter la valeur d'attribut à l'élément selon la position de l'attribut
                If Not pSubtypes Is Nothing Then
                    'Ajouter la valeur d'attribut à l'élément selon la position de l'attribut
                    If pSubtypes.HasSubtype Then
                        'Définir la position de l'attribut de sous-classe
                        iIndexSousClasse = pSubtypes.SubtypeFieldIndex

                        'Vérifier la présence de l'attribut de sous-classe
                        If iIndexSousClasse >= 0 Then
                            If Not IsDBNull(pSubtypes.DefaultValue(pSubtypes.DefaultSubtypeCode, pField.Name)) Then
                                'Ajouter la valeur d'attribut par défaut selon la sous-classe
                                pFeature.Value(iIndexAttribut) = pSubtypes.DefaultValue(pSubtypes.DefaultSubtypeCode, pField.Name)
                            End If
                            'Si l'attribut des sous-classes est absent
                        ElseIf Not IsDBNull(pField.DefaultValue) Then
                            'Ajouter la valeur d'attribut par défaut selon la classe
                            pFeature.Value(iIndexAttribut) = pField.DefaultValue
                        End If
                        'S'il n'y a pas de sous-classe
                    ElseIf Not IsDBNull(pField.DefaultValue) Then
                        pFeature.Value(iIndexAttribut) = pField.DefaultValue
                    End If
                ElseIf Not IsDBNull(pField.DefaultValue) Then
                    pFeature.Value(iIndexAttribut) = pField.DefaultValue
                End If
            End If

            'Accepter le résultat
            pFeature.Store()

            'Retourner le succès de la function
            ModifierValeurAttributSelonDefaut = True

        Finally
            'Vider la mémoire
            pObjectClass = Nothing
            pFeatureClass = Nothing
            pSubtypes = Nothing
            pField = Nothing
        End Try
    End Function

    '''<summary>
    '''Fonction qui permet de vérifier si on est en mode Edition afin de pouvoir effectuer un "Do/UnDo".
    '''</summary>
    ''' 
    '''<returns>
    '''La fonction va retourner un "IEditor". Si le traitement n'a pas réussi, le "IEditor" sera à "Nothing".
    '''</returns>
    Public Function ModeEditor() As IEditor
        'Déclarer les variables de travail
        Dim pEditor As IEditor  'Interface ESRI contenant une édition (Do/UnDo).

        Try
            'Interface pour vérifer si on est en mode édition
            pEditor = CType(m_Application.FindExtensionByName("ESRI Object Editor"), IEditor)

            'Vérifier si on est en mode édition
            If pEditor.EditState = esriEditState.esriStateNotEditing Then
                'Indiquer qu'il n'y a aucune modification possible
                pEditor = Nothing
            End If

            'Retourner le mode Editor
            ModeEditor = pEditor

        Finally
            'Vider la mémoire
            pEditor = Nothing
        End Try
    End Function

    '''<summary>
    '''Fonction qui permet de retourner une référence spatiale en coordonnées géographique avec le datum Nad83.
    '''</summary>
    ''' 
    '''<returns>
    '''La fonction va retourner un "ISpatialReference". Si la traitement n'a pas réussi, le
    '''"SpatialReference" sera à "Nothing".
    '''</returns>
    Public Function ReferenceSpatialeGeoNad83() As ISpatialReference
        'Déclarer les variables de travail
        Dim pSpRFc As SpatialReferenceEnvironment   'Interface ESRI pour créer une référence spatiale vide.
        Dim pGCS As IGeographicCoordinateSystem     'Interface ESRI contenant le système de coorsonnées Géog.

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
            MsgBox(erreur.ToString)
        Finally
            'Vider la mémoire
            pTopoOp = Nothing
            pProxOp = Nothing
            pClone = Nothing
        End Try
    End Function

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
        Dim i As Integer = Nothing                      'Compteur

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
            MsgBox(erreur.ToString)
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
        Dim i As Integer = Nothing                      'Compteur

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
            MsgBox(erreur.ToString)
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
End Module
