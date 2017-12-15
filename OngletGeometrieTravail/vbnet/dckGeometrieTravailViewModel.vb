Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts

Friend Class dckGeometrieTravailViewModel
    Inherits DockPane

    Private Const _dockPaneID As String = "MpoGeometrieTravail_dckGeometrieTravail"     'Contient l'identifiant du DockPane des géométries

    Protected Sub New()
        InitializeAsync()
    End Sub

    ''' <summary>
    ''' Called when the pane is first created to give it the opportunity to initialize itself asynchronously.
    ''' </summary>
    ''' 
    ''' <returns>A task that represents the work queued to execute in the ThreadPool.</returns>
    ''' 
    Protected Overrides Function InitializeAsync() As Task
        Return MyBase.InitializeAsync()
    End Function

    ''' <summary>
    ''' Permet d'afficher le DockPane des Géométries.
    ''' </summary>
    ''' 
    Friend Shared Sub Show()
        Dim pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID)
        If IsNothing(pane) Then
            Return
        End If

        pane.Activate()
    End Sub

    Private Shared Property _this As Object     'Contient le module "MpoGeometrieTravail_dckGeometrieTravail".
    ''' <summary>
    ''' Retrieve the singleton instance to this module here
    ''' </summary>
    ''' 
    Public Shared ReadOnly Property Current() As dckGeometrieTravailViewModel
        Get
            If (_this Is Nothing) Then
                _this = FrameworkApplication.DockPaneManager.Find(_dockPaneID)
            End If

            Return _this
        End Get
    End Property

    Private _Information As String = ""  'Contient le texte à afficher dans le TextBox 'txtInformation'.
    ''' <summary>
    ''' Texte à afficher dans le TextBox nommé 'txtInformation'
    ''' </summary>
    ''' 
    Public Property Information() As String
        Get
            Return _Information
        End Get
        Set(value As String)
            SetProperty(_Information, value, Function() Information)
        End Set
    End Property

    Private _items As TreeViewItem = Nothing
    ''' <summary>
    ''' Treeview à afficher nommé 'TreeViewItems'
    ''' </summary>
    ''' 
    Public Property TreeViewItems() As TreeViewItem
        Get
            Return _items
        End Get
        Set(value As TreeViewItem)
            SetProperty(_items, value, Function() TreeViewItems)
        End Set
    End Property

    'Contient le texte pour le ToolTip de l'action.
    Private Const _ActionToolTip As String = "Permet de définir une action à effectuer sur la géométrie sélectionnée."
    ''' <summary>
    ''' Permet de définir et retourner le ToolTip de l'action.
    ''' </summary>
    ''' 
    Public Property ActionToolTip() As String
        Get
            Return _ActionToolTip
        End Get
        Set(value As String)
            SetProperty(_ActionToolTip, value, Function() ActionToolTip)
        End Set
    End Property

    'Contient le texte pour le ToolTip de l'information.
    Private Const _InformationToolTip As String = "Permet d'afficher la description de la géométrie sélectionnée."
    ''' <summary>
    ''' Permet de définir et retourner le ToolTip de l'information.
    ''' </summary>
    ''' 
    Public Property InformationToolTip() As String
        Get
            Return _InformationToolTip
        End Get
        Set(value As String)
            SetProperty(_InformationToolTip, value, Function() InformationToolTip)
        End Set
    End Property

    'Contient le texte pour le ToolTip des géométries.
    Private Const _GeometrieToolTip As String = "Permet de visualiser les géométries ou les composantes des géométries." & vbCrLf &
                                                "Attention : Un double-clic est nécessaire pour accéder aux géométries et leurs composantes."
    ''' <summary>
    ''' Permet de définir et retourner le ToolTip des géométries.
    ''' </summary>
    ''' 
    Public Property GeometrieToolTip() As String
        Get
            Return _GeometrieToolTip
        End Get
        Set(value As String)
            SetProperty(_GeometrieToolTip, value, Function() GeometrieToolTip)
        End Set
    End Property
End Class

''' <summary>
''' Button implementation to create a new instance of the pane and activate it.
''' </summary>
''' 
Friend Class dckGeometrieTravail_ShowButton
    Inherits Button

    Protected Overrides Sub OnClick()
        dckGeometrieTravailViewModel.Show()
    End Sub
End Class
