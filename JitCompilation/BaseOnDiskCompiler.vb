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

        Public Function ExecuteMainMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal methodCode As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteMainMethod(pathToAssemblyWithMainMethod, methodCode, New String() {}, New String() {}, False, parameters)
        End Function

        Public Function ExecuteMainMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal methodCode As String, ByVal importNamespaces As String(), ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteClassWithMainMethod(pathToAssemblyWithMainMethod, EmbedCodeIntoClass(importNamespaces, methodCode), additionalAssembliesToReference, New String() {}, debugMode, "CompuMasterJitCompileTempClass", parameters)
        End Function

        Public Function ExecuteClassWithMainMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal classCode As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal instanceName As String, ByVal ParamArray parameters As Object()) As Object
            Return Compile(classCode, additionalAssembliesToReference, [imports], debugMode, pathToAssemblyWithMainMethod, Common.TargetType.Library).InvokeMainMethod(instanceName, parameters)
        End Function

        Public Function ExecuteMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal methodCode As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteMethod(pathToAssemblyWithMainMethod, methodCode, New String() {}, New String() {}, False, methodName, parameters)
        End Function

        Public Function ExecuteMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal methodCode As String, ByVal importNamespaces As String(), ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteClassWithMethod(pathToAssemblyWithMainMethod, EmbedCodeIntoClass(importNamespaces, methodCode), additionalAssembliesToReference, New String() {}, debugMode, "CompuMasterJitCompileTempClass", methodName, parameters)
        End Function

        Public Function ExecuteClassWithMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal classCode As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal instanceName As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return Compile(classCode, additionalAssembliesToReference, [imports], debugMode, pathToAssemblyWithMainMethod, Common.TargetType.Library).Invoke(instanceName, methodName, parameters)
        End Function

        ''' <summary>
        ''' Embeds method/property/enum/field code into a new class CompuMasterJitCompileTempClass to make it compilable
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Protected MustOverride Function EmbedCodeIntoClass(ByVal [imports] As String(), ByVal methodCode As String) As String

        Protected MustOverride ReadOnly Property ReferenceDefaultSet() As CompuMaster.JitCompilation.Common.ReferenceSets

        Protected MustOverride ReadOnly Property ImportDefaultSet() As CompuMaster.JitCompilation.Common.ImportSet

        Protected MustOverride Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider

    End Class

End Namespace