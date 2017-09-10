Public Class Behaelter
    'Diese Klasse modelliert einen Behälter eines Rätsels

    Private aFuellstand As Integer
    Private aVolumen As Integer

    'Konstruktor
    Public Sub New(ByVal pFuellstand As Integer, ByVal pVolumen As Integer)
        aFuellstand = pFuellstand
        aVolumen = pVolumen
    End Sub

    'Eigenschaft (Getter + Setter) für den Füllstand
    Public Property Fuellstand As Integer
        Get
            Return aFuellstand
        End Get
        Set(ByVal value As Integer)
            If value > aVolumen Then 'Ein Füllstand größer als das Volumen ist unzulässig
                Throw New Exception("Der Fuellstand kann nicht groesser als das Volumen sein!")
            Else
                aFuellstand = value
            End If
        End Set
    End Property

    'Eigenschaft (Getter) für das Volumen. Das Volumen kann nicht geändert werden.
    Public ReadOnly Property Volumen As Integer
        Get
            Return aVolumen
        End Get
    End Property
End Class
