Public Class Raetsel
    'Diese Klasse modelliert ein zu lösendes Rätsel und gleichzeit auch einen Zustand in einem Rätsel:
    'Beispiel: Wird in einem Rätsel ein Behälter umgefüllt, ändert sich der Zustand und es entsteht ein neues Rätsel
    'mit neuen Parametern (also eine Art "Unterrätsel"):

    Private ListeAbgearbeiteterZustaende As List(Of String) 'Die Liste aller bereits abgearbeiteten Zustände
    Private BehaelterA As List(Of Behaelter)
    Private BehaelterB As List(Of Behaelter)
    Private aParent As Raetsel 'Vorheriger Zustand ("Elternrätsel")
    Private nodes As List(Of Raetsel) 'Mögliche Folgezustände ("Kindrätsel")

    Public Function getNodes() As List(Of Raetsel) 'Getter für die Liste der Folgezustände
        Return nodes 'Nothing, wenn diese noch nicht mit berechneNaechsteZustaende() berechnet wurden.
    End Function

    'Konstruktoren
    Public Sub New(ByRef pList As List(Of String), ByVal PersonA As List(Of Behaelter), ByVal PersonB As List(Of Behaelter))
        BehaelterA = PersonA
        BehaelterB = PersonB
        ListeAbgearbeiteterZustaende = pList
        nodes = New List(Of Raetsel)
    End Sub
    Public Sub New(ByRef pList As List(Of String))
        BehaelterA = New List(Of Behaelter)
        BehaelterB = New List(Of Behaelter)
        ListeAbgearbeiteterZustaende = pList
        nodes = New List(Of Raetsel)
    End Sub

    'Behaelter hinzufügen
    Public Sub addBehaelter(ByVal pBehaelter As Behaelter, ByVal PersonB As Boolean)
        If Not PersonB Then
            BehaelterA.Add(pBehaelter)
        Else
            BehaelterB.Add(pBehaelter)
        End If
    End Sub

    'Füllstand eines Behälters ändern
    'Behälter werden dabei immer eindeutig durch ihren Index in Kombination mit der Person identifiziert
    Public Sub setzteFuellstand(ByVal Fuellung As Integer, ByVal Index As Integer, ByVal PersonB As Boolean)
        If Not PersonB Then
            BehaelterA(Index).Fuellstand = Fuellung
        Else
            BehaelterB(Index).Fuellstand = Fuellung
        End If
    End Sub

    'Füllstand eines Behälters abfragen
    'Behälter werden dabei immer eindeutig durch ihren Index in Kombination mit der Person identifiziert
    Public Function getFuellstand(ByVal Index As Integer, ByVal PersonB As Boolean) As Integer
        If Not PersonB Then
            Return BehaelterA(Index).Fuellstand
        Else
            Return BehaelterB(Index).Fuellstand
        End If
    End Function

    'String für die Liste der abgearbeitetn Zustände erzeugen (änlich einer Prüfsumme)
    Public Overrides Function ToString() As String
        'Es werden alle Füllstände durch ein | getrennt in den String geschrieben.
        'Dieser stellt einen Zustand innerhalb eines Rätsels eindeutig da.
        Dim Result As String = ""
        For Each B As Behaelter In BehaelterA
            Result &= B.Fuellstand.ToString() & "|"
        Next
        For Each B As Behaelter In BehaelterB
            Result &= B.Fuellstand.ToString() & "|"
        Next
        Return Result
    End Function

    'Eigenschaft (Getter + Setter) für das Elternrätsel
    'Dies ist für die Verkettung ähnlich einem Baum nötig.
    Public Property Parent As Raetsel
        Get
            Return aParent
        End Get
        Set(ByVal value As Raetsel)
            aParent = value
        End Set
    End Property

    'Überprüfen, ob der Zustand eine gültige Lösung für das Ausgangsrätsel ist,
    'also ob, die Flüssigkeit gleichmäßig aufgeteilt ist.
    Public Function endZustand() As Boolean
        Dim FluessigkeitA As Integer = 0
        Dim FluessigkeitB As Integer = 0
        'Füllstände von Person A zusammen addieren
        For Each B As Behaelter In BehaelterA
            FluessigkeitA += B.Fuellstand
        Next
        'Füllstände von Person B zusammen addieren
        For Each B As Behaelter In BehaelterB
            FluessigkeitB += B.Fuellstand
        Next
        'Zurückgeben, ob die Füllstände gleich sind
        Return FluessigkeitA = FluessigkeitB
    End Function

    'Rätsel clonen (um Kindrätsel erzeugen zu können)
    Public Function copy() As Raetsel
        'Neus Rätsel erzeugen:
        Dim R As New Raetsel(ListeAbgearbeiteterZustaende)
        R.Parent = Me 'Das aktuelle Rätsel als "Elternrätsel" definieren
        For Each B As Behaelter In BehaelterA 'Behälter von Person A kopieren
            R.addBehaelter(New Behaelter(B.Fuellstand, B.Volumen), False)
        Next
        For Each B As Behaelter In BehaelterB 'Behälter von Person B kopieren
            R.addBehaelter(New Behaelter(B.Fuellstand, B.Volumen), True)
        Next
        Return R 'Das neue Rätsel zurückgeben
    End Function

    'Neues Kindrätsel aus einem möglichen Lösungsschritt (Von X nach Y umfüllen) erzeugen:
    'Behälter werden dabei immer eindeutig durch ihren Index in Kombination mit der Person identifiziert
    Public Sub newChild(ByVal QuellePersonB As Boolean, ByVal QuelleIndex As Integer,
                        ByVal ZielPersonB As Boolean, ByVal ZielIndex As Integer)
        'Es kann nicht von einem Behälter in den gleichen Behälter umgefüllt werden:
        If QuellePersonB <> ZielPersonB Or QuelleIndex <> ZielIndex Then
            Dim Quelle As Behaelter
            Dim Ziel As Behaelter
            If Not QuellePersonB Then 'Wem gehört die Quelle?
                Quelle = BehaelterA(QuelleIndex)
            Else
                Quelle = BehaelterB(QuelleIndex)
            End If
            If Not ZielPersonB Then 'Wem gehört das Ziel?
                Ziel = BehaelterA(ZielIndex)
            Else
                Ziel = BehaelterB(ZielIndex)
            End If
            'Wenn der Füllstand der Quelle größer als 0 ist, und im Ziel noch platz ist,
            'dann kann von der Quelle in das Ziel umgefüllt werden
            If Quelle.Fuellstand > 0 And Ziel.Fuellstand < Ziel.Volumen Then
                Dim menge As Integer = 0
                If Quelle.Fuellstand + Ziel.Fuellstand <= Ziel.Volumen Then
                    'Es wird umgefüllt, bis die Quelle leer ist
                    menge = Quelle.Fuellstand
                Else
                    'Der Inalt der Quelle passt nicht ganze in das Ziel, daher wird umgefüllt
                    'bis das Ziel voll ist.
                    menge = Ziel.Volumen - Ziel.Fuellstand
                End If
                '"Kindrätsel" erzeugen
                Dim child As Raetsel = Me.copy()
                'Füllstand der Quelle im "Kindrätsel" ändern
                child.setzteFuellstand(Quelle.Fuellstand - menge, QuelleIndex, QuellePersonB)
                'Füllstand des Ziels im "Kindrätsel" ändern
                child.setzteFuellstand(Ziel.Fuellstand + menge, ZielIndex, ZielPersonB)
                Dim VergleichsString As String = child.ToString 'Der String des "Kindrätsels"
                If Not ListeAbgearbeiteterZustaende.Contains(VergleichsString) Then
                    'Zustand noch nicht erreicht / abgearbeitet
                    'Jetzt in die Liste der erreichten Zustände einfügen
                    ListeAbgearbeiteterZustaende.Add(VergleichsString)
                    'Zustand in die Liste der "Kindrätsel" einfügen.
                    nodes.Add(child)
                End If
            End If
        End If
    End Sub

    'Liste der möglichen Folgezustände berechnen:
    Public Sub berechneNaechsteZustaende()
        'Als Quelle alle Behälter einmal wählen:
        'Erst alle Behälter von Person A:
        For QuellIndex As Integer = 0 To BehaelterA.Count - 1
            'Von einem Behälter von Person A zu einem Behälter von Person A umfüllen,
            'also auch als Ziel alle Behälter von Person A einmal wählen
            For ZielIndex As Integer = 0 To BehaelterA.Count - 1
                If QuellIndex <> ZielIndex Then 'Quelle = Ziel ist natürlich nicht sinnvoll
                    newChild(False, QuellIndex, False, ZielIndex) 'Das entsprechende "Kindrätsel" erzeugen
                End If
            Next
            'Von einem Behälter von Person A zu einem Behälter von Person B umfüllen,
            'also als Ziel alle Behälter von Person B einmal wählen
            For ZielIndex As Integer = 0 To BehaelterB.Count - 1
                newChild(False, QuellIndex, True, ZielIndex)  'Das entsprechende "Kindrätsel" erzeugen
            Next
        Next
        'Dann alle Behälter von Person B als Quelle:
        For QuellIndex As Integer = 0 To BehaelterB.Count - 1
            'Von einem Behälter von Person B zu einem Behälter von Person A umfüllen,
            'also als Ziel alle Behälter von Person A einmal wählen
            For ZielIndex As Integer = 0 To BehaelterA.Count - 1
                newChild(True, QuellIndex, False, ZielIndex)  'Das entsprechende "Kindrätsel" erzeugen
            Next
            'Von einem Behälter von Person B zu einem Behälter von Person B umfüllen,
            'also auch als Ziel alle Behälter von Person B einmal wählen
            For ZielIndex As Integer = 0 To BehaelterB.Count - 1
                If QuellIndex <> ZielIndex Then 'Quelle = Ziel ist natürlich nicht sinnvoll
                    newChild(True, QuellIndex, True, ZielIndex)  'Das entsprechende "Kindrätsel" erzeugen
                End If
            Next
        Next
    End Sub
End Class
