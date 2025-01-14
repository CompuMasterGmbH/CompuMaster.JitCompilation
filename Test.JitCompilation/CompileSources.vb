Option Explicit On
Option Strict On

Namespace CompuMaster.Tests.JitCompilation

    ''' <summary>
    ''' A test case for the JIT compilation
    ''' </summary>
    Public Class CompileSources
        ''' <summary>
        ''' The source code to compile
        ''' </summary>
        Public SourceCode As String
        ''' <summary>
        ''' Namespaces which shall be imported
        ''' </summary>
        Public ImportNamespaces As New System.Collections.Generic.List(Of String)
        ''' <summary>
        ''' Reference paths to external assemblies which shall be used for compilation
        ''' </summary>
        ''' <remarks>Full paths are recommended. Please also consider to add entries to RequiredFilesForDeployment list.</remarks>
        Public AdditionalReferencePaths As New System.Collections.Generic.List(Of String)
        ''' <summary>
        ''' The list of parameters which shall be used when invoking the method after compilation
        ''' </summary>
        Public InvokeParameters As New System.Collections.Generic.List(Of Object)
        ''' <summary>
        ''' The list of parameters which shall be used when starting the process (=running the .exe file) after compilation
        ''' </summary>
        Public CommandLineParameters As New System.Collections.Generic.List(Of String)
        ''' <summary>
        ''' The On-Disk compiler service
        ''' </summary>
        Public OnDiskCompiler As Global.CompuMaster.JitCompilation.BaseOnDiskCompiler
        ''' <summary>
        ''' The In-Memory compiler service
        ''' </summary>
        Public InMemoryCompiler As Global.CompuMaster.JitCompilation.BaseInMemoryCompiler
        ''' <summary>
        ''' Dependency files which are required to copy additionally to the regular compilation process for OnDisc compilations
        ''' </summary>
        ''' <remarks>Full paths are recommended. Deployment will only copy files with newer modification date or different file size.</remarks>
        Public RequiredFilesForDeployment As New System.Collections.Generic.List(Of String)
    End Class

End Namespace