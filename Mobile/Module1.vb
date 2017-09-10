Module Module1

    Sub Main()
        Dim Aufgabe As New List(Of Integer)
        For i As Integer = 1 To Environment.GetCommandLineArgs().Count - 1
            Try
                Dim Gewicht As Integer = CInt(Environment.GetCommandLineArgs(i))
                If Gewicht > 0 Then
                    Aufgabe.Add(Gewicht)
                Else
                    Console.Error.WriteLine("Ungültiges Gewicht: " & Environment.GetCommandLineArgs(i))
                    Exit Sub
                End If
            Catch ex As InvalidCastException
                Console.Error.WriteLine("Ungültiges Gewicht: " & Environment.GetCommandLineArgs(i))
                Exit Sub
            End Try
        Next
        Dim M As Mobile = Mobile.MobileBerechnen(Aufgabe)
        Console.WriteLine(M.Notation)

        'Damit das Fenster in der IDE nicht sofort geschlossen wird:
#If DEBUG Then
        Console.ReadKey()
#End If
    End Sub

End Module
