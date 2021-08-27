Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler

'TODO: XML documentation
'TODO: create NUnit Test class for all methods with 0, 1 and 2 parameters for the called method
'TODO: validate proper operation in all .NET framework environments (.NET 1.x up to the highest version)

Namespace CompuMaster.JitCompilation

    Public MustInherit Class BaseOnDiscCompiler

        Public Function Compile(ByVal sourceCode As String, ByVal debugMode As Boolean, ByVal outputAssemblyPath As String) As CompileResults
            Return Compile(sourceCode, New String() {}, debugMode, outputAssemblyPath)
        End Function

        Public Function Compile(ByVal sourceCode As String, ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal outputAssemblyPath As String) As CompileResults
            Return Compile(sourceCode, additionalAssembliesToReference, New String() {}, debugMode, outputAssemblyPath, CompuMaster.JitCompilation.Common.TargetType.Library)
        End Function

        Public Function Compile(ByVal sourceCode As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal outputAssemblyPath As String, ByVal targetType As CompuMaster.JitCompilation.Common.TargetType) As CompileResults
            Return Common.Compile(sourceCode, False, CreateCodeProvider, targetType, Common.AddMinimalSetOfReferences(Me.ReferenceDefaultSet, additionalAssembliesToReference), Common.AddMinimalSetOfImports(Me.ImportDefaultSet, [imports]), debugMode, outputAssemblyPath)
        End Function

        Protected MustOverride ReadOnly Property ReferenceDefaultSet() As CompuMaster.JitCompilation.Common.ReferenceSets

        Protected MustOverride ReadOnly Property ImportDefaultSet() As CompuMaster.JitCompilation.Common.ImportSet

        Protected MustOverride Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider

    End Class

    Public Class CSharpDiscCompiler
        Inherits BaseOnDiscCompiler

        Protected Overrides ReadOnly Property ReferenceDefaultSet() As Common.ReferenceSets
            Get
                Return Common.ReferenceSets.Minimal
            End Get
        End Property

        Protected Overrides ReadOnly Property ImportDefaultSet() As Common.ImportSet
            Get
                Return CompuMaster.JitCompilation.Common.ImportSet.None
            End Get
        End Property

        Protected Overrides Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider
            Return New Microsoft.CSharp.CSharpCodeProvider
        End Function

    End Class

    Public Class VBNetOnDiscCompiler
        Inherits BaseOnDiscCompiler

        Protected Overrides ReadOnly Property ReferenceDefaultSet() As Common.ReferenceSets
            Get
                Return Common.ReferenceSets.Minimal
            End Get
        End Property

        Protected Overrides ReadOnly Property ImportDefaultSet() As Common.ImportSet
            Get
                Return CompuMaster.JitCompilation.Common.ImportSet.Minimal
            End Get
        End Property

        Protected Overrides Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider
            Return New Microsoft.VisualBasic.VBCodeProvider
        End Function

    End Class

End Namespace