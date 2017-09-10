Public Class Alphametik
    'Diese Klasse modelliert eine Alphametik

    'Konstanten
    Public Const OperatorAddition As Integer = 1
    Public Const OperatorSubtraktion As Integer = 2
    Public Const OperatorMultiplikation As Integer = 3
    Public Const OperatorDivision As Integer = 4

    'Struktur für die Lösung
    Public Structure LoesungsZuordnung
        Dim Symbol As String
        Dim Wert As Integer
    End Structure

    'Attribute
    Private aOperanden As List(Of String)
    Private aOperatoren As List(Of Integer)

    'Getter und Setter
    Public ReadOnly Property Solveble As Boolean 'Ist die Alphametik strukturell lösbar?
        Get
            'Nein, wenn Verhältnis zwischen Operanden und Operatoren nicht gültig ist.
            Return aOperanden.Count > 1 And aOperatoren.Count = aOperanden.Count - 2
        End Get
    End Property
    Public ReadOnly Property Operanden As List(Of String) 'Liste der Operanden
        Get
            Return aOperanden
        End Get
    End Property
    Public ReadOnly Property Operatoren As List(Of Integer) 'Liste der Operatoren
        Get
            Return aOperatoren
        End Get
    End Property

    'Einen Operant hinzufügen
    Public Sub add(ByVal pOperant As String)
        aOperanden.Add(pOperant.ToUpper)
    End Sub
    'Einen Operator hinzufügen
    Public Sub add(ByVal pOperator As Integer)
        aOperatoren.Add(pOperator)
    End Sub
    'Einen Operant und einen Operator hinzufügen
    Public Sub add(ByVal pOperant As String, ByVal pOperator As Integer)
        If pOperator = 0 Then
            add(pOperant)
        ElseIf pOperator > 0 And pOperator < 5 Then 'Nur 4 Operatoren möglich: + - * /
            aOperanden.Add(pOperant.ToUpper()) 'Wir arbeiten nur mit Grossbuchstaben
            aOperatoren.Add(pOperator)
        Else
            Throw New Exception("Ungültiger Operator")
        End If
    End Sub
    'Liste von Operanden und Operatoren hinzufügen
    Public Sub add(ByVal pOperanden As List(Of String), ByVal pOperatoren As List(Of Integer))
        aOperanden.AddRange(pOperanden)
        aOperatoren.AddRange(pOperatoren)
    End Sub

    'Konstruktor
    Public Sub New()
        aOperanden = New List(Of String)
        aOperatoren = New List(Of Integer)
    End Sub

    'Freigebene Funktion, um einen Operator als String zu erhalten
    Public Shared Function OperatorIntToStr(ByVal OperatorInt As Integer) As String
        If OperatorInt = OperatorAddition Then : Return "+"
        ElseIf OperatorInt = OperatorSubtraktion Then : Return "-"
        ElseIf OperatorInt = OperatorMultiplikation Then : Return "*"
        ElseIf OperatorInt = OperatorDivision Then : Return "/"
        Else : Return ""
        End If
    End Function
    'Freigebene Funktion, um einen Term ohne Klammern
    'unter Beachtung der Regel Punkt vor Strich zu lösen
    Public Shared Function ausrechnen(ByVal pOperanden As List(Of Integer), ByVal pOperatoren As List(Of Integer)) As Integer
        While pOperatoren.Count > 0
            Dim toSolve As Integer = 0 'Von Links nach Recht
            For i As Integer = 0 To pOperatoren.Count - 1
                If pOperatoren(i) > 2 Then 'Punkt vor Strich
                    toSolve = i
                    Exit For
                End If
            Next
            Dim NeuerOperant As Integer = 0
            If pOperatoren(toSolve) = 1 Then
                NeuerOperant = pOperanden(toSolve) + pOperanden(toSolve + 1)
            ElseIf pOperatoren(toSolve) = 2 Then
                NeuerOperant = pOperanden(toSolve) - pOperanden(toSolve + 1)
            ElseIf pOperatoren(toSolve) = 3 Then
                NeuerOperant = pOperanden(toSolve) * pOperanden(toSolve + 1)
            Else
                Dim Wert As Double = pOperanden(toSolve) / pOperanden(toSolve + 1)
                If Math.Round(Wert) = Wert Then
                    NeuerOperant = CInt(Math.Round(Wert))
                Else
                    Throw New Exception("Alphametik: Beschränkt auf ganze Zahlen")
                End If
            End If
            'Den Operator löschen
            pOperatoren.RemoveAt(toSolve)
            'Die Operanden löschen
            pOperanden.RemoveAt(toSolve)
            pOperanden.RemoveAt(toSolve)
            'Neuer Operant
            pOperanden.Insert(toSolve, NeuerOperant)
        End While
        'Der Übrigbleibende Operant ist die Lösung
        Return pOperanden(0)
    End Function

    'Methoden zum Lösen

    'Rekursive Tiefensuche (BackTracking):
    Private Function solveRecursiv(ByVal pRestSymbole As List(Of String), ByVal pZugewieseneZiffern As List(Of Integer), ByVal pOperanden As List(Of String), ByVal pOperatoren As List(Of Integer), ByVal Loesung As List(Of LoesungsZuordnung)) As List(Of LoesungsZuordnung)
        If pRestSymbole.Count > 0 And pZugewieseneZiffern.Count < 10 Then 'Alle Symbole zugewiesen?
            'Erstes nicht zugewiesenes Symbol auswählen:
            Dim Symbol As String = pRestSymbole(0)
            'Alle Ziffern durchlaufen
            For i As Integer = 0 To 9
                If Not pZugewieseneZiffern.Contains(i) Then 'Ist die Ziffer bereits zugewiesen?
                    'Führende Nullen sind unzulässig
                    Dim WareGueltig As Boolean = True
                    If i = 0 Then 'Aktuelle Ziffer gleich 0?
                        For Each O As String In pOperanden 'Operanden durchlaufen?
                            'Ist das aktuelle Symbol iregendwo am Anfang?
                            If O.Substring(0, 1) = Symbol Then
                                WareGueltig = False
                            End If
                        Next
                    End If
                    If WareGueltig Then
                        'Keine Führende 0
                        'Klonen:
                        Dim NeueOperanden As New List(Of String)
                        Dim NeueRestSymbole As New List(Of String)
                        Dim NeueZugewieseneZiffern As New List(Of Integer)
                        Dim NeueLoesung As New List(Of LoesungsZuordnung)
                        'Aktuelles Symbol durch aktuelle Ziffer ersetzen:
                        For Each O As String In pOperanden
                            NeueOperanden.Add(O.Replace(Symbol, i.ToString()))
                        Next
                        'Das aktuelle Symbol steht nicht mehr zur Verfügung:
                        For Each S As String In pRestSymbole
                            If S <> Symbol Then
                                NeueRestSymbole.Add(S)
                            End If
                        Next
                        For Each Z As Integer In pZugewieseneZiffern
                            NeueZugewieseneZiffern.Add(Z)
                        Next
                        For Each L As LoesungsZuordnung In Loesung
                            NeueLoesung.Add(L)
                        Next
                        NeueLoesung.Add(New LoesungsZuordnung With {.Symbol = Symbol, .Wert = i})
                        'Aktuelle Ziffer ist jetzt zugewiesen:
                        NeueZugewieseneZiffern.Add(i)
                        'Nächster Schritt durch Rekursion:
                        Dim Ergebnis As List(Of LoesungsZuordnung) = solveRecursiv(NeueRestSymbole, NeueZugewieseneZiffern, NeueOperanden, pOperatoren, NeueLoesung)
                        'Nach Rücksprung:
                        If Ergebnis IsNot Nothing Then 'Lösung gefunden?
                            'Ja, also zurückgeben:
                            Return Ergebnis
                        End If
                    End If
                End If
            Next
            'Keine Lösung gefunden, also nichts zurückgeben:
            Return Nothing
        ElseIf pRestSymbole.Count = 0 Then
            'Alles zugewiesen, also auf gültigkeit prüfen:
            'Dazu Klonen, da beim Ausrechnen aus einer Liste gelöscht wird:
            Dim IntOperanden As New List(Of Integer)
            For Each O As String In pOperanden
                IntOperanden.Add(CInt(O))
            Next
            Dim OperatorenClone As New List(Of Integer)
            For Each O As Integer In pOperatoren
                OperatorenClone.Add(O)
            Next
            'Ergebnis rechts des =
            Dim ausgerechnetesErgebnis As Integer = IntOperanden.Last()
            IntOperanden.RemoveAt(IntOperanden.Count - 1)
            'Ergebnis des Terms links des =
            Dim termErgebnis As Integer = ausrechnen(IntOperanden, OperatorenClone)
            'Gleichung erfüllt?
            If termErgebnis = ausgerechnetesErgebnis Then
                'Ja, also Lösung gefunden
                Return Loesung
            Else
                'Nein
                Return Nothing
            End If
        Else
            'Ausnahme
            Throw New Exception("Irgendwas ist schief gelaufen")
        End If
    End Function

    'Bereitet die Tiefensuche vor und startet diese
    Public Function Solve() As List(Of LoesungsZuordnung)
        If Solveble Then 'Überhaupt strukturell lösbar?
            'Liste der Symbole erzeugen:
            Dim Symbole As New List(Of String)
            For Each O As String In aOperanden
                For i As Integer = 0 To O.Length - 1
                    If Not Symbole.Contains(O.Substring(i, 1)) Then
                        Symbole.Add(O.Substring(i, 1))
                    End If
                Next
            Next
            'Zu viele oder zu wenige Symbole?
            If Symbole.Count > 0 And Symbole.Count < 11 Then
                'Nein, also Lösung suchen
                Return solveRecursiv(Symbole, New List(Of Integer), aOperanden, aOperatoren, New List(Of LoesungsZuordnung))
            Else
                'Ja, also nicht lösbar.
                Throw New Exception("Alphametik: Zu wenige oder zu viele Symbole")
            End If
        Else
            'Strukturell nicht lösbar
            Throw New Exception("Alphametik: Alphametik ungültig")
        End If
    End Function

End Class
