Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler

'TODO: XML documentation
'TODO: create NUnit Test class for all methods with 0, 1 and 2 parameters for the called method
'TODO: validate proper operation in all .NET framework environments (.NET 1.x up to the highest version)

Namespace CompuMaster.JitCompilation

    Public MustInherit Class BaseOnDiskCompiler

        Public Function Compile(ByVal sourceCode As String, ByVal debugMode As Boolean, ByVal outputAssemblyPath As String) As CompileOnDiskResults
            Return Compile(sourceCode, New String() {}, debugMode, outputAssemblyPath)
        End Function

        Public Function Compile(ByVal sourceCode As String, ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal outputAssemblyPath As String) As CompileOnDiskResults
            Return Compile(sourceCode, additionalAssembliesToReference, New String() {}, debugMode, outputAssemblyPath, CompuMaster.JitCompilation.Common.TargetType.Library)
        End Function

        Public Function Compile(ByVal sourceCode As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal outputAssemblyPath As String, ByVal targetType As CompuMaster.JitCompilation.Common.TargetType) As CompileOnDiskResults
            If outputAssemblyPath = Nothing Then Throw New ArgumentNullException(NameOf(outputAssemblyPath))
            Return CType(Common.Compile(sourceCode, False, CreateCodeProvider, targetType, Common.AddMinimalSetOfReferences(Me.ReferenceDefaultSet, additionalAssembliesToReference), Common.AddMinimalSetOfImports(Me.ImportDefaultSet, [imports]), debugMode, outputAssemblyPath), CompileOnDiskResults)
        End Function

        Protected MustOverride ReadOnly Property ReferenceDefaultSet() As CompuMaster.JitCompilation.Common.ReferenceSets

        Protected MustOverride ReadOnly Property ImportDefaultSet() As CompuMaster.JitCompilation.Common.ImportSet

        Protected MustOverride Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider

    End Class

End Namespace