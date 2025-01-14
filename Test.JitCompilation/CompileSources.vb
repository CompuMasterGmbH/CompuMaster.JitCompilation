Option Explicit On
Option Strict On

Namespace CompuMaster.Tests.JitCompilation

    Public Class CompileSources
        Public MethodCode As String
        Public ImportNamespaces As New System.Collections.Generic.List(Of String)
        Public AdditionalReferencePaths As New System.Collections.Generic.List(Of String)
        Public Parameters As New System.Collections.Generic.List(Of Object)
        Public OnDiskCompiler As Global.CompuMaster.JitCompilation.BaseOnDiskCompiler
        Public InMemoryCompiler As Global.CompuMaster.JitCompilation.BaseInMemoryCompiler
        ''' <summary>
        ''' Dependency files which are required to copy additionally to the regular compilation process for OnDisc compilations
        ''' </summary>
        ''' <remarks></remarks>
        Public RequiredFilesForDeployment As New System.Collections.Generic.List(Of String)
    End Class

End Namespace