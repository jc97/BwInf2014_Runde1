Module Module1

    Sub Main()
        If Environment.GetCommandLineArgs().Count() > 2 Then 'Mehr als 1 Parameter
            'Aufgabenteil 1:
            Dim A As New Alphametik()
            For i As Integer = 1 To Environment.GetCommandLineArgs().Count - 1
                Dim arg As String = Environment.GetCommandLineArgs(i)
                If arg = "+" Then
                    A.add(Alphametik.OperatorAddition)
                ElseIf arg = "-" Then
                    A.add(Alphametik.OperatorSubtraktion)
                ElseIf arg = "*" Then
                    A.add(Alphametik.OperatorMultiplikation)
                ElseIf arg = "/" Then
                    A.add(Alphametik.OperatorDivision)
                ElseIf arg <> "=" Then
                    'Operand
                    A.add(arg)
                End If
            Next
            Try
                Console.WriteLine("Rechne...")
                'Lösen:
                Dim Loesung As List(Of Alphametik.LoesungsZuordnung) = A.Solve()
                If Loesung Is Nothing Then
                    Console.WriteLine("Keine Lösung gefunden")
                Else
                    Dim orig As String = "" 'Die Alphametik als Text
                    Dim solved As String = "" 'Die gelöste Alphametik mit Zahlen
                    For i As Integer = 0 To A.Operanden.Count - 1
                        orig &= A.Operanden(i)
                        Dim OperantSolved As String = A.Operanden(i)
                        'Für die Lösung: Alle Zuordnungen auf den Operanden anwenden
                        For Each Zuordung As Alphametik.LoesungsZuordnung In Loesung
                            OperantSolved = OperantSolved.Replace(Zuordung.Symbol, Zuordung.Wert.ToString())
                        Next
                        solved &= OperantSolved
                        If i = A.Operanden.Count - 2 Then
                            'Vorletzter Operand, also jetzt das =
                            orig &= " = "
                            solved &= " = "
                        ElseIf i < A.Operanden.Count - 2 Then
                            'Weder letzter noch vorletzter Operand, also Operator einfügen:
                            orig &= " " & Alphametik.OperatorIntToStr(A.Operatoren(i)) & " "
                            solved &= " " & Alphametik.OperatorIntToStr(A.Operatoren(i)) & " "
                        End If
                    Next
                    Console.WriteLine(orig)
                    Console.WriteLine(solved)
                End If
            Catch ex As Exception
                Console.Error.WriteLine(ex.Message)
            End Try
        ElseIf Environment.GetCommandLineArgs.Count = 2 Then 'Genau 1 Parameter
            'Aufgabenteil 2:
            Try
                Dim AnzOperanden As Integer = CInt(Environment.GetCommandLineArgs(1))
                If AnzOperanden > 2 Then
                    Console.WriteLine(AnzOperanden.ToString() & " Operanden")
                    Console.WriteLine("Rechne...")
                    'Nach einer Zahl-Alphametik suchen:
                    'Wir verwende die Ziffern 1 bis 9
                    Dim A As AlphametikMitLosung = GeneriereAlphametik(AnzOperanden,
                                                    New List(Of Integer), 1, 9)
                    Console.WriteLine(A.Symbole)
                    Console.WriteLine(A.Zahlen)
                Else
                    Console.Error.WriteLine("Es werden mindestens 3 Operanden benötigt." &
                                            " Das Ergebnis rechts des Gleichhaltezeichen ist auch ein Operant")
                End If
            Catch Ex As Exception
                Console.Error.WriteLine(Ex.Message)
            End Try
        Else
            Console.Error.WriteLine("Nicht genügend Parameter")
        End If

        'Damit die Konsole in der IDE offen bleibt:
#If DEBUG Then
        Console.ReadKey()
#End If
    End Sub

    Public Class AlphametikMitLosung
        Public Symbole As String
        Public Zahlen As String
    End Class

    'Funktion, um nach einer Zahl-Alphametik zu suchen
    Function GeneriereAlphametik(ByVal NumOperanden As Integer, ByVal prefix As List(Of Integer), Optional ByVal RangeStart As Integer = 0, Optional ByVal RangeEnd As Integer = 0) As AlphametikMitLosung
        Dim Range As New List(Of Integer) 'Der Zahlenbereich
        'Alle Zahlen des Zahlenbereichs in zufälliger Reihenfolge hinzufügen:
        While Range.Count < RangeEnd - RangeStart
            Dim rand As New Random
            Dim randInt As Integer = rand.Next(RangeStart, RangeEnd)
            If Not Range.Contains(randInt) Then
                Range.Add(randInt)
            End If
        End While

        'Die Zufallszahlen der Reihe nach durchlaufen
        For Each i As Integer In Range
            Dim neuesElement As New List(Of Integer)
            'Bisherigen Operanden Clonen
            For Each p As Integer In prefix
                neuesElement.Add(p)
            Next
            'i als Operand einfügne
            neuesElement.Add(i)
            If NumOperanden > 2 Then 'Wert Rechts von = wird später berechnet
                'Rekursion:
                Dim res As AlphametikMitLosung = GeneriereAlphametik(NumOperanden - 1, neuesElement, RangeStart, RangeEnd)
                If res IsNot Nothing Then
                    'Lösung gefunden
                    Return res
                End If
            Else
                'Alle Operanden links von = gesetzt
                'Operatoren ausprobieren:
                Return probiereOperatoren(neuesElement, New List(Of Integer))
            End If
        Next
        'Keine Lösung
        Return Nothing
    End Function

    'Diese Funktion probiert alle Möglichkeiten aus, Operatoren zwischen die Operanden zu setzen,
    'bis eine Lösung gefunden wurde, oder alle Möglichkeiten ausprobiert wurden.
    Function probiereOperatoren(ByVal Operanden As List(Of Integer), ByVal gesetzteOperatoren As List(Of Integer)) As AlphametikMitLosung
        If gesetzteOperatoren.Count < Operanden.Count - 1 Then
            'Es sind noch Operatoren zu setzen
            For i As Integer = 1 To 2 'Nur + und -
                'Bisher gesetzte Operatoren clonen:
                Dim neuesElement As New List(Of Integer)
                For Each O As Integer In gesetzteOperatoren
                    neuesElement.Add(O)
                Next
                'Neuen Operator setzen:
                neuesElement.Add(i)
                'Rekursion:
                Dim res As AlphametikMitLosung = probiereOperatoren(Operanden, neuesElement)
                If res IsNot Nothing Then
                    'Lösung gefunden
                    Return res
                End If
            Next
            'Keine Lösung
            Return Nothing
        Else
            'Kombination ist vollständig, also jetzt testen, ob gültig
            'Zunächst die Rechnung selbst prüfen
            'Wir brauchen zunächst Kopien:
            Dim KopieOperanden As New List(Of Integer)
            Dim KopieOperanden2 As New List(Of String)
            Dim KopieOperatoren As New List(Of Integer)
            For i As Integer = 0 To Operanden.Count - 1
                KopieOperanden.Add(Operanden(i)) 'Für die Rechnung 
                KopieOperanden2.Add(IntegerToWord(Operanden(i))) 'Für die Alphametik
            Next
            For Each O As Integer In gesetzteOperatoren
                KopieOperatoren.Add(O)
            Next
            Dim value As Integer = 0
            Try
                'Wert des Terms links des Gleichhaltezeichens berechenen
                value = Alphametik.ausrechnen(KopieOperanden, KopieOperatoren)
                If value < 10 And value >= 0 Then
                    'Der Wert ist eine einstellige Zahl
                    'Für die Alphametik den Wert des Terms als Wort einfügen
                    KopieOperanden2.Add(IntegerToWord(value))
                    'Alphametik generieren
                    Dim Raetzel As New Alphametik
                    Raetzel.add(KopieOperanden2, gesetzteOperatoren)
                    'Und versuche, die Alphametik zu lösen:
                    Dim Loesung As List(Of Alphametik.LoesungsZuordnung) = Raetzel.Solve()
                    If Loesung Is Nothing Then
                        'Nicht lösbar: Rücksprung
                        Return Nothing
                    Else
                        'Lösung gefunden
                        'Ausgabebereit machen:
                        Dim fund As String = "" 'Die Alphametik selbst
                        For i As Integer = 0 To KopieOperanden2.Count - 1
                            fund &= KopieOperanden2(i)
                            If i = KopieOperanden2.Count - 2 Then
                                'Vorletzter Operand
                                fund &= " = "
                            ElseIf i < KopieOperanden2.Count - 2 Then
                                fund &= " " & Alphametik.OperatorIntToStr(gesetzteOperatoren(i)) & " "
                            End If
                        Next
                        Dim AmL As New AlphametikMitLosung
                        AmL.Symbole = fund
                        'Noch in Zahlen geschrieben erstellen:
                        For Each Zuordnung As Alphametik.LoesungsZuordnung In Loesung
                            fund = fund.Replace(Zuordnung.Symbol, Zuordnung.Wert.ToString())
                        Next
                        AmL.Zahlen = fund
                        'Rückgabe
                        Return AmL
                    End If
                End If

            Catch Ex As OverflowException 'z.B. Division durch 0
            Catch Ex As Exception When Ex.Message.StartsWith("Alphametik: ")
                'Ebenfalls erwartete Fehler
            End Try
            'Keine Lösung
            Return Nothing
        End If
    End Function

    'Konvertierungsfunktionen:
    Function IntegerToWord(ByVal i As Integer) As String
        If i = 0 Then
            Return "NULL"
        ElseIf i = 1 Then
            Return "EINS"
        ElseIf i = 2 Then
            Return "ZWEI"
        ElseIf i = 3 Then
            Return "DREI"
        ElseIf i = 4 Then
            Return "VIER"
        ElseIf i = 5 Then
            Return "FUENF"
        ElseIf i = 6 Then
            Return "SECHS"
        ElseIf i = 7 Then
            Return "SIEBEN"
        ElseIf i = 8 Then
            Return "ACHT"
        ElseIf i = 9 Then
            Return "NEUN"
        Else : Return ""
        End If
    End Function
    Function WordToInteger(ByVal W As String) As Integer
        Select Case W
            Case "NULL"
                Return 0
            Case "EINS"
                Return 1
            Case "ZWEI"
                Return 2
            Case "DREI"
                Return 3
            Case "VIER"
                Return 4
            Case "FUENF"
                Return 5
            Case "SECHS"
                Return 6
            Case "SIEBEN"
                Return 7
            Case "ACHT"
                Return 8
            Case "NEUN"
                Return 9
            Case Else
                Return 0
        End Select
    End Function

End Module
