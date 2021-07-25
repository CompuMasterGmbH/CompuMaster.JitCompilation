Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler

'TODO: XML documentation
'TODO: create NUnit Test class for all methods with 0, 1 and 2 parameters for the called method
'TODO: validate proper operation in all .NET framework environments (.NET 1.x up to the highest version)

Namespace CompuMaster.JitCompilation

    Public MustInherit Class BaseInMemoryCompiler

        Public Function Compile(ByVal sourceCode As String, ByVal debugMode As Boolean) As CompileResults
            Return Compile(sourceCode, New String() {}, debugMode)
        End Function

        Public Function Compile(ByVal sourceCode As String, ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean) As CompileResults
            Return Compile(sourceCode, additionalAssembliesToReference, New String() {}, debugMode, CompuMaster.JitCompilation.Common.TargetType.Library)
        End Function

        Public Function Compile(ByVal sourceCode As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal targetType As CompuMaster.JitCompilation.Common.TargetType) As CompileResults
            Return Common.Compile(sourceCode, True, CreateCodeProvider, targetType, Common.AddMinimalSetOfReferences(Me.ReferenceDefaultSet, additionalAssembliesToReference), Common.AddMinimalSetOfImports(Me.ImportDefaultSet, [imports]), debugMode, Nothing)
        End Function

        Public Function ExecuteMainMethod(ByVal methodCode As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteMainMethod(methodCode, New String() {}, New String() {}, False, parameters)
        End Function

        Public Function ExecuteMainMethod(ByVal methodCode As String, ByVal importNamespaces As String(), ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteClassWithMainMethod(EmbedFunctionIntoClass(importNamespaces, methodCode), additionalAssembliesToReference, New String() {}, debugMode, "CompuMasterJitCompileTempClass", parameters)
        End Function

        Public Function ExecuteClassWithMainMethod(ByVal classCode As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal instanceName As String, ByVal ParamArray parameters As Object()) As Object
            Return Compile(classCode, additionalAssembliesToReference, [imports], debugMode, Common.TargetType.Library).InvokeMainMethod(instanceName, parameters)
        End Function

        Public Function ExecuteMethod(ByVal methodCode As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteMethod(methodCode, New String() {}, New String() {}, False, methodName, parameters)
        End Function

        Public Function ExecuteMethod(ByVal methodCode As String, ByVal importNamespaces As String(), ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteClassWithMethod(EmbedFunctionIntoClass(importNamespaces, methodCode), additionalAssembliesToReference, New String() {}, debugMode, "CompuMasterJitCompileTempClass", methodName, parameters)
        End Function

        Public Function ExecuteClassWithMethod(ByVal classCode As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal instanceName As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return Compile(classCode, additionalAssembliesToReference, [imports], debugMode, Common.TargetType.Library).Invoke(instanceName, methodName, parameters)
        End Function

        ''' <summary>
        ''' Embeds a function code into a new class CompuMasterJitCompileTempClass to make it compilable
        ''' </summary>
        ''' <param name="functionCode"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Protected MustOverride Function EmbedFunctionIntoClass(ByVal [imports] As String(), ByVal functionCode As String) As String

        Protected MustOverride ReadOnly Property ReferenceDefaultSet() As CompuMaster.JitCompilation.Common.ReferenceSets

        Protected MustOverride ReadOnly Property ImportDefaultSet() As CompuMaster.JitCompilation.Common.ImportSet

        Protected MustOverride Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : CompuMaster.JitCompilation
    ''' Class	 : JitCompilation.InMemoryCompiler
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Compile a piece of source code for execution in memory
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[wezel]	23.01.2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class CSharpInMemoryCompiler
        Inherits BaseInMemoryCompiler

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

        Protected Overloads Overrides Function EmbedFunctionIntoClass(ByVal [imports]() As String, ByVal functionCode As String) As String
            'Given is a function code like the following one
            '---------------------------------------
            'object Main(object param1)
            '{
            '   'some code here...
            '   return param1;
            '}
            '---------------------------------------
            Dim ImportCommands As String = ""
            For Each importNamespace As String In [imports]
                ImportCommands &= "using " & importNamespace & System.Environment.NewLine
            Next
            Return ImportCommands & System.Environment.NewLine & _
                 "public class CompuMasterJitCompileTempClass" & System.Environment.NewLine & _
                 "{" & System.Environment.NewLine & _
                 functionCode & System.Environment.NewLine & _
                 "}"
        End Function

        Protected Overrides Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider
            Return New Microsoft.CSharp.CSharpCodeProvider
        End Function

    End Class

    Public Class VBNetInMemoryCompiler
        Inherits BaseInMemoryCompiler

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

        Protected Overloads Overrides Function EmbedFunctionIntoClass(ByVal [imports]() As String, ByVal functionCode As String) As String
            'Given is a function code like the following one
            '---------------------------------------
            'Public Function Main(param1 as object)
            '   'some code here...
            '   Return param1
            'End Function
            '---------------------------------------
            Dim ImportCommands As String = Nothing
            For Each importNamespace As String In [imports]
                ImportCommands &= "Imports " & importNamespace & System.Environment.NewLine
            Next
            Return "Option Strict On" & System.Environment.NewLine & System.Environment.NewLine & _
                ImportCommands & System.Environment.NewLine & _
                "Public Class CompuMasterJitCompileTempClass" & System.Environment.NewLine & _
                System.Environment.NewLine & _
                functionCode & System.Environment.NewLine & _
                System.Environment.NewLine & _
                "End Class"
        End Function

        Protected Overrides Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider
            Return New Microsoft.VisualBasic.VBCodeProvider
        End Function

    End Class

End Namespace