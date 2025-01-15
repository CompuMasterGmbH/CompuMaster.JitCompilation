Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler

Namespace CompuMaster.JitCompilation

    ''' <summary>
    ''' Base class for in-memory compilers
    ''' </summary>
    Public MustInherit Class BaseInMemoryCompiler

        ''' <summary>
        ''' Compile a piece of source code for execution in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="debugMode"></param>
        ''' <returns></returns>
        Public Function Compile(ByVal sourceCode As String, ByVal debugMode As Boolean) As CompileResults
            Return Compile(sourceCode, New String() {}, debugMode)
        End Function

        ''' <summary>
        ''' Compile a piece of source code for execution in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="debugMode"></param>
        ''' <returns></returns>
        Public Function Compile(ByVal sourceCode As String, ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean) As CompileResults
            Return Compile(sourceCode, additionalAssembliesToReference, New String() {}, debugMode, CompuMaster.JitCompilation.Common.TargetType.Library)
        End Function

        ''' <summary>
        ''' Compile a piece of source code for execution in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="[imports]"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="targetType"></param>
        ''' <returns></returns>
        Public Function Compile(ByVal sourceCode As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal targetType As CompuMaster.JitCompilation.Common.TargetType) As CompileResults
            Return Common.Compile(sourceCode, True, CreateCodeProvider, targetType, Common.AddMinimalSetOfReferences(Me.ReferenceDefaultSet, additionalAssembliesToReference), Common.AddMinimalSetOfImports(Me.ImportDefaultSet, [imports]), debugMode, Nothing)
        End Function

        ''' <summary>
        ''' Compile code of a class to an assembly and invoke method Main of it 
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteMainMethod(ByVal methodCode As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteMainMethod(methodCode, New String() {}, New String() {}, False, parameters)
        End Function

        ''' <summary>
        ''' Compile code of a class to an assembly and invoke method Main of it 
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <param name="importNamespaces"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteMainMethod(ByVal methodCode As String, ByVal importNamespaces As String(), ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteClassWithMainMethod(EmbedCodeIntoClass(importNamespaces, methodCode), additionalAssembliesToReference, New String() {}, debugMode, "CompuMasterJitCompileTempClass", parameters)
        End Function

        ''' <summary>
        ''' Compile code of a class to an assembly and invoke method Main of it 
        ''' </summary>
        ''' <param name="classCode"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="importNamespaces"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="instanceName"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteClassWithMainMethod(ByVal classCode As String, ByVal additionalAssembliesToReference As String(), ByVal importNamespaces As String(), ByVal debugMode As Boolean, ByVal instanceName As String, ByVal ParamArray parameters As Object()) As Object
            Return Compile(classCode, additionalAssembliesToReference, importNamespaces, debugMode, Common.TargetType.Library).InvokeMainMethod(instanceName, parameters)
        End Function

        ''' <summary>
        ''' Compile code of a class to an assembly and invoke a method of it 
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <param name="methodName"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteMethod(ByVal methodCode As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteMethod(methodCode, New String() {}, New String() {}, False, methodName, parameters)
        End Function

        ''' <summary>
        ''' Compile code of a class to an assembly and invoke a method of it 
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <param name="importNamespaces"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="methodName"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteMethod(ByVal methodCode As String, ByVal importNamespaces As String(), ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteClassWithMethod(EmbedCodeIntoClass(importNamespaces, methodCode), additionalAssembliesToReference, New String() {}, debugMode, "CompuMasterJitCompileTempClass", methodName, parameters)
        End Function

        ''' <summary>
        ''' Compile code of a class to an assembly and invoke a method of it 
        ''' </summary>
        ''' <param name="classCode"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="importNamespaces"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="instanceName"></param>
        ''' <param name="methodName"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteClassWithMethod(ByVal classCode As String, ByVal additionalAssembliesToReference As String(), ByVal importNamespaces As String(), ByVal debugMode As Boolean, ByVal instanceName As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return Compile(classCode, additionalAssembliesToReference, importNamespaces, debugMode, Common.TargetType.Library).Invoke(instanceName, methodName, parameters)
        End Function

        ''' <summary>
        ''' Embeds method/property/enum/field code into a new class CompuMasterJitCompileTempClass to make it compilable
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Protected MustOverride Function EmbedCodeIntoClass(ByVal [imports] As String(), ByVal methodCode As String) As String

        ''' <summary>
        ''' Default set of references
        ''' </summary>
        ''' <returns></returns>
        Protected MustOverride ReadOnly Property ReferenceDefaultSet() As CompuMaster.JitCompilation.Common.ReferenceSets

        ''' <summary>
        ''' Default set of namespace importNamespaces
        ''' </summary>
        ''' <returns></returns>
        Protected MustOverride ReadOnly Property ImportDefaultSet() As CompuMaster.JitCompilation.Common.ImportSet

        ''' <summary>
        ''' Create a new instance of the code provider
        ''' </summary>
        ''' <returns></returns>
        Protected MustOverride Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider

    End Class

End Namespace