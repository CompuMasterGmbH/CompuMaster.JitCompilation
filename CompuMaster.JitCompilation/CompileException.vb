Imports System
Imports System.Web
Imports System.Security
Imports System.Security.Permissions
Imports System.Globalization
Imports System.CodeDom.Compiler

<Serializable(), AspNetHostingPermission(SecurityAction.LinkDemand, Level:=AspNetHostingPermissionLevel.Minimal)> _
Public NotInheritable Class CompileException
    Inherits Exception

    Private _results As CompilerResults
    Private _sourceCode As String
    Private _hideSourceCodeFilePathsInExceptionMessages As Boolean

    Public Sub New(ByVal results As CompilerResults, ByVal sourceCode As String, ByVal hideSourceCodeFilePathsInExceptionMessages As Boolean)
        Me._results = results
        Me._sourceCode = sourceCode
        Me._hideSourceCodeFilePathsInExceptionMessages = hideSourceCodeFilePathsInExceptionMessages
    End Sub

    Friend ReadOnly Property FirstCompileError() As CompilerError
        Get
            If ((Me._results Is Nothing) OrElse Not Me._results.Errors.HasErrors) Then
                Return Nothing
            End If
            For Each err As CompilerError In Me._results.Errors
                If Not err.IsWarning Then
                    Return err
                End If
            Next
        End Get
    End Property

    Public Overrides ReadOnly Property Message() As String
        Get
            Dim firstCompileError As CompilerError = Me.FirstCompileError
            If (firstCompileError Is Nothing) Then
                Return MyBase.Message
            End If
            If _hideSourceCodeFilePathsInExceptionMessages Then
                Return String.Format(CultureInfo.CurrentCulture, "Line {0}: Error {1}: {2}", New Object() {firstCompileError.Line, firstCompileError.ErrorNumber, firstCompileError.ErrorText})
            Else
                Return String.Format(CultureInfo.CurrentCulture, "{0}({1}): Error {2}: {3}", New Object() {firstCompileError.FileName, firstCompileError.Line, firstCompileError.ErrorNumber, firstCompileError.ErrorText})
            End If
        End Get
    End Property

    Public ReadOnly Property ErrorsAndWarnings() As String()
        Get
            If Me._results Is Nothing OrElse Me._results.Errors Is Nothing Then Return New String() {}
            Dim Result As New ArrayList
            For Each err As CompilerError In Me._results.Errors
                If err.IsWarning = False Then
                    Result.Add("ERROR: " & FormattedError(err))
                Else
                    Result.Add("WARNING: " & FormattedError(err))
                End If
            Next
            Return CType(Result.ToArray(GetType(String)), String())
        End Get
    End Property

    Public ReadOnly Property Errors() As String()
        Get
            If Me._results Is Nothing OrElse Me._results.Errors Is Nothing Then Return New String() {}
            Dim Result As New ArrayList
            For Each err As CompilerError In Me._results.Errors
                If err.IsWarning = False Then
                    Result.Add(FormattedError(err))
                End If
            Next
            Return CType(Result.ToArray(GetType(String)), String())
        End Get
    End Property

    Public ReadOnly Property Warnings() As String()
        Get
            If Me._results Is Nothing OrElse Me._results.Errors Is Nothing Then Return New String() {}
            Dim Result As New ArrayList
            For Each err As CompilerError In Me._results.Errors
                If err.IsWarning Then
                    Result.Add(FormattedError(err))
                End If
            Next
            Return CType(Result.ToArray(GetType(String)), String())
        End Get
    End Property

    Private Function FormattedError(ByVal err As CompilerError) As String
        If _hideSourceCodeFilePathsInExceptionMessages Then
            Return String.Format(CultureInfo.CurrentCulture, "Line {0}: Error {1}: {2}", New Object() {err.Line, err.ErrorNumber, err.ErrorText})
        Else
            Return String.Format(CultureInfo.CurrentCulture, "{0}({1}): Error {2}: {3}", New Object() {err.FileName, err.Line, err.ErrorNumber, err.ErrorText})
        End If
    End Function

    Public ReadOnly Property Results() As CompilerResults
        Get
            Return Me._results
        End Get
    End Property

    Public ReadOnly Property SourceCode() As String
        Get
            Return Me._sourceCode
        End Get
    End Property

End Class
