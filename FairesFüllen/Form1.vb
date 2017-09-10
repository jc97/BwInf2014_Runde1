Public Class Form1
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Ini(True)
    End Sub

    'Die Variablen:
    Private BehaelterPersonA As New List(Of Behaelter)
    Private BehaelterPersonB As New List(Of Behaelter)
    Private GesammtVolumen As Integer = 0
    Private GesammtVolumenA As Integer = 0
    Private GesammtVolumenB As Integer = 0
    Private GesammtFluessigkeit As Integer = 0
    Private GesammtFluessigkeitA As Integer = 0
    Private GesammtFluessigkeitB As Integer = 0

    'Eine Liste aller bereits durchgerechneten Knoten, damit keine Knoten mehrfach betrachtet werden
    'Für jeden Knoten wird dabei ein eindeutiger String (ähnlich der Dateien zu den Rätseln) erzeugt.
    Private KnotenListe As List(Of String)

    Private Sub Solve()
        KnotenListe = New List(Of String) 'Eine neue Liste für die bereits abgearbeiteten Knoten anlegen
        'Die Gesammtflüssigkeit muss gerade sein, da bereits vorher in LoadFile() überprüft
        Dim ZielProPerson As Integer = CInt(Math.Round(GesammtFluessigkeit / 2))
        'Der Ausgangszustand
        Dim AusgangsRaetsel As New Raetsel(KnotenListe, BehaelterPersonA, BehaelterPersonB)
        'Den Ausgangszustand in die Liste der abgearbeiteten Knoten einfügen.
        'Sollte der Ausgangszustand wieder erreicht werden, wäre der Lösungsweg dahin ein Zyklus
        KnotenListe.Add(AusgangsRaetsel.ToString())
        'Die Schlange für die Breitensuche:
        Dim SolveQueue As New Queue(Of Raetsel)
        'Eine Variable für die Lösung:
        Dim Loesung As Raetsel = Nothing
        'Den Ausgangszustand in die Schlange aufnehmen:
        SolveQueue.Enqueue(AusgangsRaetsel)
        Dim node As Raetsel 'Variable für den aktuellen Knoten / Zustand
        'Breitensuche: Solange in der Schlange noch nicht bearbeitete Knoten sind, die Knoten abarbeiten:
        While SolveQueue.Count > 0 And Loesung Is Nothing
            'Den vodersten Knoten auslesen und aus der Schlange nehmen
            node = SolveQueue.Dequeue()
            'Wenn der vorderste Knoten bereits ein gültiger Endzustand, also eine Löstung ist,
            'diese Speichern und die Suche abbrechen
            If node.endZustand() Then
                Loesung = node
                Exit While
            Else
                'Wenn nicht: Alle möglichen Folgezustände berechnen:
                node.berechneNaechsteZustaende()
                'Diese Folgezustände in die Schlange schreiben:
                For Each child As Raetsel In node.getNodes()
                    SolveQueue.Enqueue(child)
                Next
            End If
        End While
        If Loesung Is Nothing Then 'Wenn keine Lösung gefunden wurde:
            ToolStripTextBox1.Text = "Keine Lösung gefunden"
            AskForRetry("Keine Lösung gefunden", "Das Rätsel ist nicht lösbar!")
        Else
            'Es wurde eine Lösung gefunden.
            'Diese Lösung nun darstellen:

            'ListView leeren und einstellen:
            ListView1.Clear()
            ListView1.FullRowSelect = True

            'Für alle Behälter von Person A eine Spalte anlegen:
            For i As Integer = 0 To BehaelterPersonA.Count - 1
                'ChrW(65 + i) lieferten den i-ten (mit i = 0 angefangen) Buchstaben im Alphabet.
                ListView1.Columns.Add(ChrW(65 + i) & " (V= " & BehaelterPersonA(i).Volumen & " )", 85) 'Breite: 85
            Next
            'Trennspalte:
            ListView1.Columns.Add(" ", 15)
            'Für alle Behälter von Person B eine Spalte anlegen:
            For i As Integer = 0 To BehaelterPersonB.Count - 1
                'ChrW(65 + i) lieferten den i-ten (mit i = 0 angefangen) Buchstaben im Alphabet.
                ListView1.Columns.Add(ChrW(65 + i + BehaelterPersonA.Count) & " (V= " & BehaelterPersonB(i).Volumen & " )", 85) 'Breite 85
            Next

            Dim Schritte As Integer = 0 'Lösungsschritte zählen
            Dim current As Raetsel = Loesung 'Variable für den aktuellen Zustand
            Do 'Alle Zustände vom Endzustand aus rückwärts zum Ausgangszustand durchlaufen.
                If current.Parent IsNot Nothing Then 'Der Ausgangszustand wird gesondert behandelt. Daher nur bis zum vorletzten Zustand von unten
                    Schritte += 1 'Hochzählen
                    Dim Menge As Integer = 0 'Die Flüssigkeitsmenge, die in diesem Schritt umgefüllt wird
                    Dim Quelle As String = "" 'Quell-Behälter
                    Dim Ziel As String = "" 'Ziel-Behälter
                    Dim item As ListViewItem = Nothing 'Item für den ListView
                    For i As Integer = 0 To BehaelterPersonA.Count - 1 'Spalten für die Behälter der Person A
                        If item Is Nothing Then
                            'Erster Behälter, also ListView-Item anlegen (kein SubItem):
                            item = ListView1.Items.Add(current.getFuellstand(i, False).ToString())
                            'Wenn sich der Füllstand im vorherigen Füllstand unterscheidet, ist dieser Behälter entweder Ziel oder Quelle:
                            If current.getFuellstand(i, False) <> current.Parent.getFuellstand(i, False) Then
                                Menge = current.getFuellstand(i, False) - current.Parent.getFuellstand(i, False) 'Differenz zum vorherigen Zustand
                                If Menge < 0 Then 'Negative Differenz, Behälter ist also Quelle:
                                    Quelle = ChrW(65 + i)
                                Else 'Keine negative Differenz, Behälter ist also Ziel:
                                    Ziel = ChrW(65 + i)
                                End If
                                item.Font = New Font(item.Font.FontFamily, 12, FontStyle.Bold) 'Schrift einstellen (fettdruck und Schriftgröße)
                            Else
                                item.Font = New Font(item.Font.FontFamily, 12, FontStyle.Regular) 'Nur die Schriftgröße ändern
                            End If
                            item.UseItemStyleForSubItems = False 'Die Subitems müssten gesondert formatiert werden, da ein Fettdruck des Items nicht für SubItems gelten soll
                        Else
                            Dim SubItem As ListViewItem.ListViewSubItem = item.SubItems.Add(current.getFuellstand(i, False).ToString()) 'SubItem
                            'Wenn sich der Füllstand im vorherigen Füllstand unterscheidet, ist dieser Behälter entweder Ziel oder Quelle:
                            If current.getFuellstand(i, False) <> current.Parent.getFuellstand(i, False) Then
                                Menge = current.getFuellstand(i, False) - current.Parent.getFuellstand(i, False) 'Differenz zum vorherigen Zustand
                                If Menge < 0 Then 'Negative Differenz, Behälter ist also Quelle:
                                    Quelle = ChrW(65 + i)
                                Else 'Keine negative Differenz, Behälter ist also Ziel:
                                    Ziel = ChrW(65 + i)
                                End If
                                SubItem.Font = New Font(SubItem.Font.FontFamily, 12, FontStyle.Bold) 'Schriftgröße ändern und fett drucken
                            Else
                                SubItem.Font = New Font(SubItem.Font.FontFamily, 12, FontStyle.Regular) 'Nur die Schriftgröße ändern
                            End If
                        End If
                    Next
                    item.SubItems.Add("") 'Trennspalte
                    For i As Integer = 0 To BehaelterPersonB.Count - 1 'Spalten für die Behälter der Person B
                        Dim SubItem As ListViewItem.ListViewSubItem = item.SubItems.Add(current.getFuellstand(i, True).ToString()) 'SubItem
                        'Wenn sich der Füllstand im vorherigen Füllstand unterscheidet, ist dieser Behälter entweder Ziel oder Quelle:
                        If current.getFuellstand(i, True) <> current.Parent.getFuellstand(i, True) Then
                            Menge = current.getFuellstand(i, True) - current.Parent.getFuellstand(i, True) 'Differenz zum vorherigen Zustand
                            If Menge < 0 Then 'Negative Differenz, Behälter ist also Quelle:
                                Quelle = ChrW(65 + i + BehaelterPersonA.Count)
                            Else 'Keine negative Differenz, Behälter ist also Ziel:
                                Ziel = ChrW(65 + i + BehaelterPersonA.Count)
                            End If
                            SubItem.Font = New Font(SubItem.Font.FontFamily, 12, FontStyle.Bold) 'Schriftgröße ändern und fett drucken
                        Else
                            SubItem.Font = New Font(SubItem.Font.FontFamily, 12, FontStyle.Regular) 'Nur die Schriftgröße ändern
                        End If
                    Next
                    If Menge < 0 Then
                        Menge = Menge * -1 'In der Anleitung keine Vorzeichen Anzeigen (Es wird immer die Quelle und das Ziel angegeben).
                    End If
                    'Gruppe im LV erzeugen, um die Anleitung anzuzeigen:
                    Dim group As ListViewGroup = New ListViewGroup("Von " & Quelle & " " & Menge & " Maßen in " & Ziel & " umfüllen")
                    ListView1.Groups.Insert(0, group) 'Gruppe am Anfang einfügen.
                    item.Group = group 'Item in die Gruppe einfügen.
                Else 'Den Ausgangszustand:
                    Dim group As ListViewGroup = New ListViewGroup("Ausgangssituation") 'Gruppe erzeugen
                    ListView1.Groups.Insert(0, group) 'Gruppe am Angang einfügen
                    Dim item As ListViewItem = Nothing 'Variable für das LV-Item.
                    For i As Integer = 0 To BehaelterPersonA.Count - 1 'Behälter von Person A zur Anzeige durchlaufen
                        If item Is Nothing Then 'Für die erste Spalte das Item erzeugen
                            item = ListView1.Items.Add(current.getFuellstand(i, False).ToString()) 'Den Füllstand des Behälters in die Spalte schreiben
                        Else 'Für alle weiteren Spalten ein SubItem anlegen:
                            Dim SubItem As ListViewItem.ListViewSubItem = item.SubItems.Add(current.getFuellstand(i, False).ToString()) 'Den Füllstand des Behälters in die Spalte schreiben
                        End If
                    Next
                    item.SubItems.Add("") 'Trennspalte
                    For i As Integer = 0 To BehaelterPersonB.Count - 1 'Behälter von Person B zur Anzeige durchlaufen
                        'SubItem Anlegen:
                        Dim SubItem As ListViewItem.ListViewSubItem = item.SubItems.Add(current.getFuellstand(i, True).ToString())
                    Next
                    item.Group = group 'Item in die Gruppe einfügen
                End If
                current = current.Parent 'Zum oberen (vorherigen) Zustand gehen...
                'Wenn wir bereits beim Ausgangszustand sind, ist der vorherige Zustand Nothing, (existiert natürlich nicht)
            Loop Until current Is Nothing 'Fertig
            ToolStripTextBox1.Text = "Lösung mit " & Schritte & " Schritten gefunden" 'Die Anzahl der Lösungen ausgeben
        End If
    End Sub

    Private Sub LoadFile(ByVal File As String)
        Try 'Wenn irgendetwas schief läuft, muss die Datei ungültig sein
            BehaelterPersonA.Clear()
            BehaelterPersonB.Clear()
            'Alle Zeilen der Datei lesen
            Dim Lines() As String = My.Computer.FileSystem.ReadAllText(File).Split(CChar(vbLf))
            'Nur die Zeilen 0 bis 2 sind relevant, bzw sollten existieren:
            For i As Integer = 0 To 2 Step 1
                'Mögliche Leerstellen am Zeilenanfang und Zeilenende entfernen
                Lines(i) = Lines(i).Trim()
            Next
            Dim AnzahlBehaelter_PersonA As Integer = CInt(Lines(0)) 'DIe erste Zeile enthält die Anzahl der Behälter, die Person A gehören
            If AnzahlBehaelter_PersonA < 1 Then
                AskForRetry("Ungültige Datei", "Die Datei ist ungültig: Person A braucht auch einen Behälter!")
            Else
                'Die Behälter, ihre Volumen und ihre Füllstände einlesen...
                Dim Behaelter() As String = Lines(1).Split(CChar(" ")) 'Die Volumen
                Dim BehaelterFuellungen() As String = Lines(2).Split(CChar(" ")) ' DIe Füllstände
                If Behaelter.Length <> BehaelterFuellungen.Length Then 'Es müssen genauso viele Angaben über die Füllstände, wie Behälter existieren.
                    AskForRetry("Ungültige Datei", "Die Datei ist ungültig: Zu wenige oder zu viele Angaben in Zeile 3")
                End If
                Dim i As Integer = 0 'Zählervariable für die Behälter
                For Each B As String In Behaelter
                    Dim V As Integer = CInt(B)
                    If V < 1 Then 'Ein Volumen 0 oder gar ein negatives Volumen? Nicht sinnvoll.
                        AskForRetry("Ungültige Datei", "Die Datei ist ungültig: Ein Behälter mit einem Volumen <= 0 ???")
                    Else
                        Dim Fuellung As Integer = CInt(BehaelterFuellungen(i))
                        If Fuellung < 0 Then 'Den Füllstand prüfen: Ein negativer Füllstand ist nicht sinnvoll.
                            AskForRetry("Ungültige Datei", "Die Datei ist ungültig: Ein Behälter mit einem negativem Füllstand ???")
                        Else
                            If Fuellung > V Then
                                AskForRetry("Ungültige Datei", "Die Datei ist ungültig: Wenn der Füllstand höher als das Volumen ist, was passiert dann?" & vbNewLine & "Genau, der Behälter läuft über!")
                            Else
                                'Wem gehört der Behälter: A oder B?
                                If i >= AnzahlBehaelter_PersonA Then 'Persson A
                                    BehaelterPersonB.Add(New Behaelter(Fuellung, V))
                                    GesammtFluessigkeitB += Fuellung 'Gesammtflüssigkeit von Person B zählen
                                    GesammtVolumenB += V 'Gesammtvolumen von Person Bzählen
                                Else
                                    BehaelterPersonA.Add(New Behaelter(Fuellung, V))
                                    GesammtFluessigkeitA += Fuellung 'Gesammtflüssigkeit von Person B zählen
                                    GesammtVolumenA += V 'Gesammtvolumen von Person Bzählen
                                End If
                            End If
                        End If
                    End If
                    i += 1 'Nächster Behälter:
                Next
                GesammtFluessigkeit = GesammtFluessigkeitA + GesammtFluessigkeitB
                GesammtVolumen = GesammtVolumenA + GesammtVolumenB
                If GesammtFluessigkeit Mod 2 <> 0 Then 'Die gesammte Menge der Flüssigkeit muss gerade sein, da sie sich sonst nicht ganzzahlig aufteilen lässt.
                    AskForRetry("Ungültige Datei", "Die Datei ist ungültig: Das Programm arbeitet nur mit Ganzzahlen, d.h. die Menge der zu verteilenden Flüssigkeit muss gerade sein!")
                ElseIf GesammtFluessigkeit / 2 > GesammtVolumenA Or GesammtFluessigkeit / 2 > GesammtVolumenB Then 'Jede Person muss in ihren Behältern Platz für ihren Anteil der Flüssigkeit haben.
                    AskForRetry("Ungültige Datei", "Die Datei ist ungültig: Die Behälter einer der beiden Personen sind zu klein, um den zugestandenen Anteil aufzunehmen.")
                Else
                    'Die Datei ist gültig
                    Solve() 'Nach einer Lösung suche.
                End If
            End If
        Catch ex As Exception
            AskForRetry("Fehler: " & ex.GetType().ToString(), "Die Datei ist ungültig:" & vbNewLine & ex.Message)  'Ein nicht erwarteter Fehler in der Datei.
        End Try
    End Sub

    Private Sub Ini(ByVal UseCommandLine As Boolean)
        Dim File As String
        'Ist eine Datei über die Parameter auf der Kommandozeile übergeben worden?
        If Environment.GetCommandLineArgs.Length > 1 And UseCommandLine = True Then
            File = Environment.GetCommandLineArgs(1)
            LoadFile(File)
        Else
            'OpenFileDialog anzeigen...
            Dim OFD As New OpenFileDialog
            OFD.CheckFileExists = True
            OFD.CheckPathExists = True
            OFD.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString()
            Dim OFDResult As DialogResult = OFD.ShowDialog()
            If OFDResult = Windows.Forms.DialogResult.OK And OFD.FileName.Length > 0 Then
                LoadFile(OFD.FileName)
            Else
                AskForRetry("Keine Datei angegeben", "Keine Datei angegeben! Wiederholen?")
            End If
        End If
    End Sub

    'Diese Methode wird verwendet, um den Benutzer zu fragen, ob das Programm beendet werden soll,
    'oder ob es neu durchlaufen soll.
    Private Sub AskForRetry(ByVal Title As String, ByVal Text As String)
        If MsgBox(Text, MsgBoxStyle.RetryCancel Or MsgBoxStyle.Critical, Title) = MsgBoxResult.Retry Then
            Ini(False)
        Else
            Me.Close()
        End If
    End Sub

    Private Sub NeuToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NeuToolStripMenuItem.Click
        Ini(False)
    End Sub

End Class
