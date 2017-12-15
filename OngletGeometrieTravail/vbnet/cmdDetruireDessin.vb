Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports ArcGIS.Desktop.Framework
Imports ArcGIS.Desktop.Framework.Contracts
Imports ArcGIS.Desktop.Mapping

Friend Class cmdDetruireDessin
    Inherits Button

    Protected Overrides Sub OnClick()
        'Détruire l'affichage des géométries de type point, ligne, surface et sommet.
        modGeometrieTravail.DetruireAffichage()
    End Sub

    Protected Overrides Sub OnUpdate()
        'Désactiver la commande par défaut
        Enabled = False
        'Vérifier si le DockPane est valide
        If MapView.Active IsNot Nothing Then
            'Vérifier si l'affichage des géométries de type point est présente
            If modGeometrieTravail.PointOverlay IsNot Nothing Or modGeometrieTravail.LineOverlay IsNot Nothing Or modGeometrieTravail.PolygonOverlay IsNot Nothing Then
                'Activer la commande
                Enabled = True
            End If
        End If
    End Sub
End Class

