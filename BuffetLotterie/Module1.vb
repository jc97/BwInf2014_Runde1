Option Strict On
Module Module1
    'Struktur für die Lösung
    Public Structure BueffetLotterieLoesung
        Dim Runden As Integer
        'Die Anzahl der zu nutzenden Chancen des Sonderrechts:
        'Das Sonderrecht wird im Folgenden als Wiederholung bezeichnet.
        Dim Wiederholungen As Integer
    End Structure

    Sub Main()
        Dim Teilnehmerzahl As Integer = 28 'Standardteilnehmerzahl
        If Environment.GetCommandLineArgs().Count > 1 Then
            'Es wurde eine andere Teilnehmerzahl übergeben
            Try
                Teilnehmerzahl = CInt(Environment.GetCommandLineArgs(1))
                If Teilnehmerzahl < 1 Then
                    Console.Error.WriteLine("Ungültige Teilnehmerzahl")
                    Exit Sub
                End If
            Catch ex As Exception
                Console.Error.WriteLine("Ungültige Teilnehmerzahl")
                Exit Sub
            End Try
        End If
        BueffetLotterie(Teilnehmerzahl)

        'Damit die Konsole sich in der IDE nicht schließt:
#If DEBUG Then
        Console.ReadKey()
#End If
    End Sub

    Public Sub BueffetLotterie(ByVal Teilnehmer As Integer)
        'Teilnehmerzahl ausgeben:
        Console.WriteLine("Teilnehmerzahl: " & Teilnehmer.ToString())
        'Die Lösung berechnen:
        Dim Aufgabe As BueffetLotterieLoesung = BueffetLotterie(Teilnehmer, 1, 16, 0, 1)
        'Lösung ausgeben:
        Console.WriteLine("Das Geburtstagskind kann frühestens nach " & Aufgabe.Runden & " Runden an das Bueffet.")
        Console.WriteLine("Dazu muss es " & Aufgabe.Wiederholungen & "-mal zwei Silben sprechen.")
        Console.WriteLine()
    End Sub

    Public Function BueffetLotterie(ByVal n As Integer, ByVal position As Integer,
                                    ByVal Silben As Integer, ByVal Runden As Integer,
                                    ByVal Wiederholungen As Integer) As BueffetLotterieLoesung
        'n: Aktuelle Teilnehmerzahl
        'position: Aktuelle Postion
        'Silben: Anzahl der Silben
        'Anzahl der bisherigen Runden
        'Wiederholungen: Anzahl der bisher möglichen Wiederholungen (Nutzungen des Sonderrechts)

        'Die neue Position ohne Nutzung des Sonderrechts:
        Dim NeuePosition As Integer = ((position + Silben) Mod n) - 1
        If NeuePosition < 0 Then
            NeuePosition += n
        End If

        'Die Möglichen Nutzungen des Sonderrechts berechnen:
        Dim WiederholungenNeu As Integer = Wiederholungen
        If (n - position) + 2 < Silben Then
            'Von der Startposition aus, wird Position 1 min. einmal erreicht und überschritten
            'Deshalb eine Nutzung möglich
            WiederholungenNeu += 1
            'Wie oft kann dann von Position 1 wieder Position 1 erreicht werden?
            'Übrigbleibenen Silben berechnen:
            Dim RestSilben As Integer = Silben - ((n - position) + 3)
            'Anzahl der Umrundungen berechnen und zu den Wiederholungen aufaddieren:
            WiederholungenNeu += CInt(Math.Floor(RestSilben / (n + 1)))
        End If
        'Lösung gefunden?
        If ((NeuePosition - WiederholungenNeu) <= 1) And ((NeuePosition > 0) Or (WiederholungenNeu >= n)) Then
            'Ja:
            'Das Geburtstagskind kann durch Wiederholungen die neue Position auf 1 dekremntieren.
            'Objekt für Lösung erzeugen:
            Dim Loesung As New BueffetLotterieLoesung
            Loesung.Runden = Runden + 1
            If NeuePosition > 0 Then
                Loesung.Wiederholungen = NeuePosition - 1
            Else
                Loesung.Wiederholungen = n - 1
            End If
            'Und zurückgeben:
            Return Loesung
        Else
            'Nein, keine Lösung gefunden:
            'Rekursion: Teilnehmer um 1 dekrementieren und die Runden um 1 inkrementieren.
            Return BueffetLotterie(n - 1, NeuePosition, Silben, Runden + 1, WiederholungenNeu)
        End If
    End Function
End Module
