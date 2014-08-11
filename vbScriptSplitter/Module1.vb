Module Module1

    Sub Main()


        Dim scs As New Subtext.Scripting.ScriptSplitter(vbCr & vbLf & "SELECT * FROM T1" & vbCr & vbLf & "GO" & vbCr & vbLf & "SELECT * FROM T2" & vbCr & vbLf & "GO" & vbCr & vbLf & vbCr & vbLf & "GO" & vbCr & vbLf & vbCr & vbLf & "SELECT * FROM T3" & vbCr & vbLf & "GO" & vbCr & vbLf)
        For Each str As String In scs
            Console.WriteLine(str)
        Next

    End Sub

End Module
