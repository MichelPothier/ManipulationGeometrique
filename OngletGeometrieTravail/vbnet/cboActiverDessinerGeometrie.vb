Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts

''' <summary>
''' Represents the ComboBox
''' </summary>
''' <remarks></remarks>
Public Class cboActiverDessinerGeometrie
    Inherits ComboBox

    'Permet d'indiquer si le combobox a été initialisé
    Private _isInitialized As Boolean

    '''<summary>
    ''' Constructeur
    '''</summary>
    Public Sub New()
        'Mise à jour des items du combobox
        UpdateCombo()
    End Sub

    '''<summary>
    ''' Mise à jour des items du combobox
    '''</summary>
    Private Sub UpdateCombo()
        'Vérifier si le ComboBox est initialisé
        If _isInitialized Then
            'Définir le premier item par défaut
            SelectedItem = ItemCollection.FirstOrDefault() 'set the default item in the comboBox
        End If

        'Vérifier si le ComboBox est initialisé
        If Not _isInitialized Then
            'Détruire les items du combobox
            Clear()
            'Ajouter les 2 items du combobox
            Add(New ComboBoxItem("Dessiner"))
            Add(New ComboBoxItem("Pas dessiner"))
            'Indiquer que le ComboBox a été initialisé
            _isInitialized = True
        End If

        'Activer le ComboBox
        Enabled = True
        'Définir le premier item par défaut
        SelectedItem = ItemCollection.FirstOrDefault()
    End Sub

    ''' <summary>
    ''' Événement activé lorsque la sélection d'un item du combobox change
    ''' </summary>
    ''' <param name="item">Contient le nouvel item sélectionné.</param>
    Protected Overrides Sub OnSelectionChange(item As ComboBoxItem)
        'Sortir si l'item est invalide
        If (item Is Nothing) Then
            Return
        End If

        'Sortir si l'item est nulle
        If (String.IsNullOrEmpty(item.Text)) Then
            Return
        End If

        'Définir le paramètre qui indique qu'on doit dessiner les géométries
        modGeometrieTravail.ActiverDessinerGeometrie = True
        'Vérifier si on ne doit pas dessiner les géométries
        If item.Text = "Pas dessiner" Then
            'Définir le paramètre qui indique qu'on ne doit pas dessiner les géométries
            modGeometrieTravail.ActiverDessinerGeometrie = False
        End If
    End Sub
End Class
