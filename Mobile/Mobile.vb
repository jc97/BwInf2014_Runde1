Public Class Mobile
    Implements IComparable(Of Mobile)
    'Diese Klasse modelliert ein einzelnes Mobile
    'Dabei ist ein einzelnes Gewicht auch ein Mobile: Ein Mobile ohne Kindelemente

    'Das Gewicht, falls es sich bei dem Mobile um ein einzelnes Gewicht handelt:
    Private aGewicht As Integer
    'Die Kinder, falls es sich um einen Balken handelt:
    Private aChildes(3) As Mobile
    'Die Anzahl der Kinder:
    Private numChildes As Integer
    'Das Mobile, an dem dieses Mobile hängt:
    Private aParent As Mobile
    'Der Abstand zum Mittelpunkt von aParent:
    Private aAbstand As Integer

    'Getter und Setter:
    Public Property Partent As Mobile
        Get
            Return aParent
        End Get
        Set(ByVal value As Mobile)
            aParent = value
        End Set
    End Property
    Public ReadOnly Property istBalken As Boolean 'Gibt an, ob das Mobile ein Balken ist
        Get
            Return aGewicht = 0
        End Get
    End Property
    Public ReadOnly Property Gewicht As Integer 'Gibt das gesamte Gewicht des Mobiles an.
        Get
            If aGewicht > 0 Then
                Return aGewicht
            Else
                Dim g As Integer = 0
                For i As Integer = 0 To 3
                    If aChildes(i) IsNot Nothing Then
                        g += aChildes(i).Gewicht
                    End If
                Next
                Return g
            End If
        End Get
    End Property
    Public Property Abstand As Integer 'Gibt den Abstand zum Eltern-Mobile.
        Set(ByVal value As Integer)
            aAbstand = value
        End Set
        Get
            Return aAbstand
        End Get
    End Property

    'Konstruktoren:
    Public Sub New(ByVal pGewicht As Integer, ByVal pAbstand As Integer)
        Me.New(pGewicht)
        aAbstand = pAbstand
    End Sub
    Public Sub New()
        aGewicht = 0
        numChildes = 0
        aAbstand = 0
    End Sub
    Public Sub New(ByVal ParamArray pChildes() As Mobile)
        Me.New()
        If pChildes.Count > 4 Then
            Throw New Exception("Mehr als 4 Strukturen pro Balken sind unzulässig!")
        Else
            For i As Integer = 0 To pChildes.Count - 1
                aChildes(i) = pChildes(i)
                aChildes(i).Partent = Me
            Next
            numChildes = pChildes.Count
        End If
    End Sub
    Public Sub New(ByVal pAbstand As Integer, ByVal ParamArray pChildes() As Mobile)
        Me.New(pChildes)
        aAbstand = pAbstand
    End Sub
    Public Sub New(ByVal pGewicht As Integer)
        Me.New()
        aGewicht = pGewicht
    End Sub

    'Diese Funktion liefert die textuelle Darstellung des Mobiles:
    Public Function Notation() As String
        If Not istBalken Then
            'Wenn das Mobile lediglich ein Gewicht ist, wird nur der Wert des Gewichtes
            'zurückgegeben:
            Return aGewicht.ToString()
        Else
            'Wenn das Mobile ein Balken ist, müssen die Notationen der Kindelemente
            'ermittelt werden:
            Dim Ret As String = "{" 'Die Variable für die textuelle Darstellung
            'Damit zu Beginn kein ; eingefügt wird:
            Dim StrichPunkt As Boolean = False
            For i As Integer = 0 To 3 'Alle Kinder durchlaufen
                If aChildes(i) IsNot Nothing Then
                    If StrichPunkt Then : Ret += ";" : End If
                    StrichPunkt = True
                    'Das Kind zur Notation hinzufügen:
                    Ret += "(" + aChildes(i).Notation + ";" + aChildes(i).Abstand.ToString() + ")"
                End If
            Next
            Ret += "}"
            Return Ret
        End If
    End Function

    'Diese Funktion ermittelt, ob das Mobile bereits ein Kind mit einem bestimmten Abstand enthält:
    Private Function istPositionBelegt(ByVal pAbstand As Integer) As Boolean
        Dim ret As Boolean = False
        'Alle Kinder durchlaufen:
        For i As Integer = 0 To 3
            If aChildes(i) IsNot Nothing Then
                If aChildes(i).aAbstand = pAbstand Then
                    ret = True
                End If
            End If
        Next
        Return ret
    End Function

    'Diese Prozedur balanciert ein Mobile mit seinen Kindern aus, indem es die Abstände anpasst.
    'Es sollen sich nach Möglichkeit immer zwei Gewichte gegenseitig Aufheben. Dazu werden die Gewichte
    'zunächst sotiert und paarweise durch das Setzen der Abstände ausbalanciert. Wenn die Anzahl
    'der Kinder ungerade ist, wird das schwerste Gewicht in die Mitte gehängt (Abstand 0).
    Public Sub ausbalancieren(Optional ByVal Start As Integer = 0)
        If numChildes > Start Then
            If Start = 0 Then
                'Gewichte sortieren
                Array.Sort(aChildes)
                Array.Reverse(aChildes) 'Das schwerste zu erst
            End If
            If (numChildes = 1 Or numChildes = 3) And Start = 0 Then
                'Bei ungerader anzahl von Kindern, das erste in die Mitte:
                aChildes(0).Abstand = 0
                ausbalancieren(1)
            Else
                'Das KgV der beiden Gewichte berechnen (aktuelles Gewicht und nächstes Gewicht)
                Dim Gewichte_KgV As Integer = kgV(aChildes(Start).Gewicht, aChildes(Start + 1).Gewicht)
                'Die Abstände mit dem KgV als Zugkraft berechenen, sodass sich beide Gewichte aufhaben:
                Dim AbstandA As Integer = CInt(Gewichte_KgV / aChildes(Start).Gewicht)
                Dim AbstandB As Integer = CInt(Gewichte_KgV / aChildes(Start + 1).Gewicht)
                'Ein Gewicht muss auf die negative Seite:
                If Not istPositionBelegt(AbstandA) And Not istPositionBelegt(AbstandB * -1) Then
                    'B kann auf die andere Seite
                    aChildes(Start).Abstand = AbstandA
                    aChildes(Start + 1).Abstand = AbstandB * -1
                ElseIf Not istPositionBelegt(AbstandA * -1) And Not istPositionBelegt(AbstandB) Then
                    'A muss auf die andere Seite
                    aChildes(Start).Abstand = AbstandA * -1
                    aChildes(Start + 1).Abstand = AbstandB
                Else
                    'Mit doppelten Abständen arbeiten, da andere Kombinationen nicht möglich sind
                    'Die Verwendung der der doppelten Abständen muss möglich sein, da die vorherigen
                    'Kombinationen nicht möglich sind, also mindestens 2 Positionen belegt sind und
                    'max. 4 Kindelemente pro Balken zulässig sind:
                    aChildes(Start).Abstand = AbstandA * 2
                    aChildes(Start + 1).Abstand = AbstandB * -2
                End If
                'Die nächsten beiden Gewichte ausbalancieren:
                ausbalancieren(Start + 2)
            End If
        End If
    End Sub

    'Diese Funktion berechnet den ggT nach dem Euklidischen Algorithmus
    Public Shared Function Euclid_ggT(ByVal m As Integer, ByVal n As Integer) As Integer
        If m = 0 Then
            Return n
        ElseIf n = 0 Then
            Return m
        ElseIf m > n Then
            Return Euclid_ggT(m - n, n)
        Else
            Return Euclid_ggT(m, n - m)
        End If
    End Function

    'Diese Funktion berechnet den KgV anhand des ggT:
    Public Shared Function kgV(ByVal m As Integer, ByVal n As Integer) As Integer
        Dim ggt As Integer = Euclid_ggT(m, n)
        Return CInt((m * n) / ggt)
    End Function

    'Diese Methode fügt dem Mobile ein weiteres Mobile hinzu
    Public Sub Add(ByVal pMobile As Mobile)
        If Not istBalken Then
            Throw New Exception("Einem Gewicht können keine Kinder angehängt werden")
        ElseIf numChildes > 4 Then
            Throw New Exception("Kein Platz mehr")
        Else
            aChildes(numChildes) = pMobile
            aChildes(numChildes).Partent = Me
            numChildes += 1
        End If
    End Sub

    'Diese Funktion liefert ein Mobile aus einer Liste von Gewichten:
    Public Shared Function MobileBerechnen(ByVal Gewichte As List(Of Integer)) As Mobile
        Dim Anzahl As Integer = Gewichte.Count
        'Die Anzahl der benötigten Mobiles berechnen:
        Dim AnzahlMobiles As Integer = CInt(Math.Round(Anzahl / 3, 0))
        If AnzahlMobiles = 0 Then : AnzahlMobiles = 1 : End If
        Gewichte.Sort() 'Die Gewichte sortieren
        'Die Anzahl der Ebenen, die die Baumartige Struktur des gesammten Mobiles haben muss:
        Dim AnzahlEbenen As Integer = CInt(Math.Ceiling(Math.Log(AnzahlMobiles + 1, 2)))
        'Die Anzahl der Mobiles, für die auf der untersten Ebene noch platz wäre:
        Dim FehlendeMobiles As Integer = CInt((Math.Pow(2, AnzahlEbenen) - 1) - AnzahlMobiles)

        'Gewichte pro Ebene Gleichmäßig verteilen, aber je weiter unten (untere Ebenen), desto leichetere Gewichte:
        Dim ZuverteilendeMobiles As New Queue(Of Mobile)
        Dim GesammtMobile As Mobile = Nothing 'Wird später zugewiesen
        For i As Integer = AnzahlEbenen To 1 Step -1 'Mit der untersten Ebene beginnen
            Dim AnzahlMobilesDieserEbene As Integer = CInt(Math.Pow(2, i - 1))
            Dim PlatzOberhalb As Integer = 0
            If i = AnzahlEbenen Then
                'Wie viele Platz ist überhalb dieser Ebene für Gewichte?
                PlatzOberhalb = (AnzahlMobilesDieserEbene - 1) * 2 + FehlendeMobiles
                AnzahlMobilesDieserEbene -= FehlendeMobiles
            End If
            Dim Mobiles(AnzahlMobilesDieserEbene) As Mobile 'Array für alle Mobiles der aktuellen Ebene
            For iM As Integer = 0 To AnzahlMobilesDieserEbene - 1 'Alle Mobiles dieser Ebene erstellen
                Mobiles(iM) = New Mobile() 'Neues Mobile
                For iG As Integer = 0 To 3 'Vier Gewichte bzw Mobiles können angehängt werden
                    'Ein Mobil soll zur Aestetik immer nur höchstens 2 Balken haben (iG = 0) und (iG = 1)
                    'und bis zu 2 Gewichte haben
                    If iG = 0 And ZuverteilendeMobiles.Count > 0 And i < AnzahlEbenen Then
                        'Auf der untersten Ebene sollen keine Mobiles angehängt werden (i < AnzahlEbenen)
                        Mobiles(iM).Add(ZuverteilendeMobiles.Dequeue())
                    ElseIf iG = 1 And ZuverteilendeMobiles.Count > ((AnzahlMobilesDieserEbene - iM) - 1) _
                        And i < AnzahlEbenen Then
                        Mobiles(iM).Add(ZuverteilendeMobiles.Dequeue())
                    Else
                        'Gewichte sollen soweit oben, wie möglich platziert werden:
                        If Gewichte.Count > PlatzOberhalb Then
                            'Index des anzuhängenden Gewichtes:
                            Dim GIndex As Integer = iG * (AnzahlMobilesDieserEbene - iM) - iG
                            If GIndex >= Gewichte.Count Then 'Gewicht existiert nicht, also das letzte Gewicht nehmen
                                Dim G As Integer = Gewichte.Last()
                                Dim GewichtMobile = New Mobile(G)
                                Mobiles(iM).Add(GewichtMobile) 'Hinzufügen
                                Gewichte.RemoveAt(Gewichte.Count - 1) 'Aus Liste löschen
                                Exit For
                            Else
                                Dim G As Integer = Gewichte(GIndex)
                                Dim GewichtMobile = New Mobile(G)
                                Mobiles(iM).Add(GewichtMobile) 'Hinzufügen
                                Gewichte.RemoveAt(GIndex) 'Aus Liste löschen
                            End If
                        End If
                    End If
                Next
                'Das erstelle Mobile ausbalancieren:
                Mobiles(iM).ausbalancieren()
                If i > 1 Then 'Oberste Ebene:
                    'Nein, also zur Schlange der zuverteilenden Mobiles hinzufügen
                    ZuverteilendeMobiles.Enqueue(Mobiles(iM))
                Else
                    'Ja, das erstelle Mobile ist also eine Lösung:
                    GesammtMobile = Mobiles(iM)
                End If
            Next
        Next
        Return GesammtMobile
    End Function

    'Ermöglicht das Vergleichen zweier Mobiles, um diese zu Sortieren
    Public Function CompareTo(ByVal other As Mobile) As Integer Implements System.IComparable(Of Mobile).CompareTo
        Return other.Gewicht - aGewicht
    End Function
End Class
