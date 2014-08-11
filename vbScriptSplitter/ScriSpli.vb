

#Region "Disclaimer/Info"

'''////////////////////////////////////////////////////////////////////////////////////////////////
' Subtext WebLog
' 
' Subtext is an open source weblog system that is a fork of the .TEXT
' weblog system.
'
' For updated news and information please visit http://subtextproject.com/
' Subtext is hosted at Google Code at http://code.google.com/p/subtext/
' The development mailing list is at subtext@googlegroups.com 
'
' This project is licensed under the BSD license.  See the License.txt file for more information.
'''////////////////////////////////////////////////////////////////////////////////////////////////

#End Region

Imports System.Collections
Imports System.Collections.Generic
Imports vbScriptSplitter.Subtext.Scripting.Exceptions


Namespace Subtext.Scripting


    Public Class ScriptSplitter
        Implements IEnumerable(Of String)


        Private ReadOnly _reader As System.IO.TextReader
        Private _builder As New System.Text.StringBuilder()
        Private _current As Char
        Private _lastChar As Char
        Private _scriptReader As ScriptReader

        Public Sub New(script As String)
            _reader = New System.IO.StringReader(script)
            _scriptReader = New SeparatorLineReader(Me)
        End Sub

        Friend ReadOnly Property HasNext() As Boolean
            Get
                Return _reader.Peek() <> -1
            End Get
        End Property

        Friend ReadOnly Property Current() As Char
            Get
                Return _current
            End Get
        End Property

        Friend ReadOnly Property LastChar() As Char
            Get
                Return _lastChar
            End Get
        End Property

#Region "IEnumerable<string> Members"

        Public Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            Dim lsScripts As New List(Of String)


            While [Next]()
                If Split() Then
                    Dim script As String = _builder.ToString().Trim()
                    If script.Length > 0 Then
                        'yield Return (script)
                        lsScripts.Add(script)
                    End If
                    Reset()
                End If
            End While

            If _builder.Length > 0 Then
                Dim scriptRemains As String = _builder.ToString().Trim()
                If scriptRemains.Length > 0 Then
                    ' yield Return (scriptRemains)
                    lsScripts.Add(scriptRemains)
                End If
            End If

            ' http://stackoverflow.com/questions/875587/can-i-implement-yield-return-for-ienumerable-functions-in-vb-net
            ' http://www.dotnetperls.com/getenumerator
            Return lsScripts.GetEnumerator()
        End Function


        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

#End Region

        Friend Function [Next]() As Boolean
            If Not HasNext Then
                Return False
            End If

            _lastChar = _current

            _current = ChrW(_reader.Read())
            Return True
        End Function


        Friend Function Peek() As Integer
            Return _reader.Peek()
        End Function


        Private Function Split() As Boolean
            Return _scriptReader.ReadNextSection()
        End Function


        Friend Sub SetParser(newReader As ScriptReader)
            _scriptReader = newReader
        End Sub


        Friend Sub Append(text As String)
            _builder.Append(text)
        End Sub


        Friend Sub Append(c As Char)
            _builder.Append(c)
        End Sub


        Private Sub Reset()
            _current = InlineAssignHelper(_lastChar, Char.MinValue)
            _builder = New System.Text.StringBuilder()
        End Sub


        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function


    End Class



    MustInherit Class ScriptReader
        Protected ReadOnly Splitter As ScriptSplitter

        Protected Sub New(splitter__1 As ScriptSplitter)
            Splitter = splitter__1
        End Sub


        ''' <summary>
        ''' This acts as a template method. Specific Reader instances 
        ''' override the component methods.
        ''' </summary>
        Public Function ReadNextSection() As Boolean
            If IsQuote Then
                ReadQuotedString()
                Return False
            End If

            If BeginDashDashComment Then
                Return ReadDashDashComment()
            End If

            If BeginSlashStarComment Then
                ReadSlashStarComment()
                Return False
            End If

            Return ReadNext()
        End Function


        Protected Overridable Function ReadDashDashComment() As Boolean
            Splitter.Append(Current)
            While Splitter.[Next]()
                Splitter.Append(Current)
                If EndOfLine Then
                    Exit While
                End If
            End While
            'We should be EndOfLine or EndOfScript here.
            Splitter.SetParser(New SeparatorLineReader(Splitter))
            Return False
        End Function


        Protected Overridable Sub ReadSlashStarComment()
            If ReadSlashStarCommentWithResult() Then
                Splitter.SetParser(New SeparatorLineReader(Splitter))
                Return
            End If
        End Sub


        Private Function ReadSlashStarCommentWithResult() As Boolean
            Splitter.Append(Current)
            While Splitter.[Next]()
                If BeginSlashStarComment Then
                    ReadSlashStarCommentWithResult()
                    Continue While
                End If
                Splitter.Append(Current)

                If EndSlashStarComment Then
                    Return True
                End If
            End While
            Return False
        End Function


        Protected Overridable Sub ReadQuotedString()
            Splitter.Append(Current)
            While Splitter.[Next]()
                Splitter.Append(Current)
                If IsQuote Then
                    Return
                End If
            End While
        End Sub

        Protected MustOverride Function ReadNext() As Boolean

#Region "Helper methods and properties"

        Protected ReadOnly Property HasNext() As Boolean
            Get
                Return Splitter.HasNext
            End Get
        End Property

        Protected ReadOnly Property WhiteSpace() As Boolean
            Get
                Return Char.IsWhiteSpace(Splitter.Current)
            End Get
        End Property

        Protected ReadOnly Property EndOfLine() As Boolean
            Get
                Return ControlChars.Lf = Splitter.Current
            End Get
        End Property

        Protected ReadOnly Property IsQuote() As Boolean
            Get
                Return "'"c = Splitter.Current
            End Get
        End Property

        Protected ReadOnly Property Current() As Char
            Get
                Return Splitter.Current
            End Get
        End Property

        Protected ReadOnly Property LastChar() As Char
            Get
                Return Splitter.LastChar
            End Get
        End Property

        Private ReadOnly Property BeginDashDashComment() As Boolean
            Get
                Return Current = "-"c AndAlso Peek() = "-"c
            End Get
        End Property

        Private ReadOnly Property BeginSlashStarComment() As Boolean
            Get
                Return Current = "/"c AndAlso Peek() = "*"c
            End Get
        End Property


        Private ReadOnly Property EndSlashStarComment() As Boolean
            Get
                Return LastChar = "*"c AndAlso Current = "/"c
            End Get
        End Property


        Protected Shared Function CharEquals(expected As Char, actual As Char) As Boolean
            Return [Char].ToLowerInvariant(expected) = [Char].ToLowerInvariant(actual)
        End Function


        Protected Function CharEquals(compare As Char) As Boolean
            Return CharEquals(Current, compare)
        End Function


        Protected Function Peek() As Char
            If Not HasNext Then
                Return Char.MinValue
            End If

            Return ChrW(Splitter.Peek())
        End Function

#End Region
    End Class



    Class SeparatorLineReader
        Inherits ScriptReader
        Private _builder As New System.Text.StringBuilder()
        Private _foundGo As Boolean
        Private _gFound As Boolean

        Public Sub New(splitter As ScriptSplitter)
            MyBase.New(splitter)
        End Sub


        Private Sub Reset()
            _foundGo = False
            _gFound = False
            _builder = New System.Text.StringBuilder()
        End Sub


        Protected Overrides Function ReadDashDashComment() As Boolean
            If Not _foundGo Then
                MyBase.ReadDashDashComment()
                Return False
            End If
            MyBase.ReadDashDashComment()
            Return True
        End Function


        Protected Overrides Sub ReadSlashStarComment()
            If _foundGo Then
                Throw New SqlParseException("SqlParseException: Incorrect syntax near GO")
            End If
            MyBase.ReadSlashStarComment()
        End Sub


        Protected Overrides Function ReadNext() As Boolean


            If EndOfLine Then
                'End of line or script
                If Not _foundGo Then
                    _builder.Append(Current)
                    Splitter.Append(_builder.ToString())
                    Splitter.SetParser(New SeparatorLineReader(Splitter))
                    Return False
                End If
                Reset()
                Return True
            End If

            If WhiteSpace Then
                _builder.Append(Current)
                Return False
            End If

            If Not CharEquals("g"c) AndAlso Not CharEquals("o"c) Then
                FoundNonEmptyCharacter(Current)
                Return False
            End If

            If CharEquals("o"c) Then
                If CharEquals("g"c, LastChar) AndAlso Not _foundGo Then
                    _foundGo = True
                Else
                    FoundNonEmptyCharacter(Current)
                End If
            End If

            If CharEquals("g"c, Current) Then
                If _gFound OrElse (Not [Char].IsWhiteSpace(LastChar) AndAlso LastChar <> Char.MinValue) Then
                    FoundNonEmptyCharacter(Current)
                    Return False
                End If

                _gFound = True
            End If

            If Not HasNext AndAlso _foundGo Then
                Reset()
                Return True
            End If

            _builder.Append(Current)
            Return False
        End Function


        Private Sub FoundNonEmptyCharacter(c As Char)
            _builder.Append(c)
            Splitter.Append(_builder.ToString())
            Splitter.SetParser(New SqlScriptReader(Splitter))
        End Sub


    End Class



    Class SqlScriptReader
        Inherits ScriptReader


        Public Sub New(splitter As ScriptSplitter)
            MyBase.New(splitter)
        End Sub


        Protected Overrides Function ReadNext() As Boolean
            If EndOfLine Then
                'end of line
                Splitter.Append(Current)
                Splitter.SetParser(New SeparatorLineReader(Splitter))
                Return False
            End If

            Splitter.Append(Current)
            Return False
        End Function


    End Class


End Namespace
