Option Explicit On 
Option Strict On

Imports System.Text

Namespace CompuMaster.JitCompilation

    Public Class ExecuteConsoleAppResult
        ''' <summary>
        ''' Standard output of the executed console application
        ''' </summary>
        ''' <returns></returns>
        Public Property StandardOutput As New StringBuilder()
        ''' <summary>
        ''' Standard error output of the executed console application
        ''' </summary>
        ''' <returns></returns>
        Public Property StandardErrorOutput As New StringBuilder()
        ''' <summary>
        ''' Exit code of the executed console application
        ''' </summary>
        ''' <returns></returns>
        Public Property ExitCode As Integer?
    End Class

End Namespace