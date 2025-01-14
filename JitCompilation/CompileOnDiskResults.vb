Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler

'TODO: XML documentation
'TODO: create NUnit Test class for all methods with 0, 1 and 2 parameters for the called method
'TODO: validate proper operation in all .NET framework environments (.NET 1.x up to the highest version)

Namespace CompuMaster.JitCompilation

    ''' <summary>
    ''' A compilation result for access to the compiled assembly or compilation errors
    ''' </summary>
    Public Class CompileOnDiskResults
        Inherits CompileResults

        Friend Sub New(ByVal [assembly] As System.Reflection.Assembly, outputAssemblyPath As String, ByVal compilerResults As System.CodeDom.Compiler.CompilerResults)
            MyBase.New([assembly], compilerResults)
            Me.OutputAssemblyPath = outputAssemblyPath
        End Sub

        ''' <summary>
        ''' Load a compiled assembly from disk and make it accessible for further invocations
        ''' </summary>
        ''' <param name="outputAssemblyPath"></param>
        Friend Sub New(outputAssemblyPath As String)
            MyBase.New(System.Reflection.Assembly.LoadFrom(outputAssemblyPath), Nothing)
            Me.OutputAssemblyPath = outputAssemblyPath
        End Sub

        ''' <summary>
        ''' Load a compiled assembly from disk and make it accessible for further invocations
        ''' </summary>
        ''' <param name="outputAssemblyPath"></param>
        Friend Sub New(ByVal [assembly] As System.Reflection.Assembly)
            MyBase.New([assembly], Nothing)
            Me.OutputAssemblyPath = [assembly].Location
        End Sub

        ''' <summary>
        ''' Load a compiled assembly from disk and make it accessible for further invocations
        ''' </summary>
        ''' <param name="outputAssemblyPath"></param>
        ''' <returns></returns>
        Public Shared Function LoadFrom(outputAssemblyPath As String) As CompileOnDiskResults
            Return New CompileOnDiskResults(outputAssemblyPath)
        End Function

        ''' <summary>
        ''' Load a compiled assembly from disk and make it accessible for further invocations
        ''' </summary>
        ''' <param name="[assembly]"></param>
        ''' <returns></returns>
        Public Shared Function LoadFrom([assembly] As System.Reflection.Assembly) As CompileOnDiskResults
            Return New CompileOnDiskResults([assembly])
        End Function

        ''' <summary>
        ''' The path to the compiled assembly on disk
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property OutputAssemblyPath As String

        Public Function ExecuteMainMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteMainMethod(pathToAssemblyWithMainMethod, New String() {}, New String() {}, False, parameters)
        End Function

        Public Function ExecuteMainMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal importNamespaces As String(), ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteClassWithMainMethod(pathToAssemblyWithMainMethod, additionalAssembliesToReference, New String() {}, debugMode, "CompuMasterJitCompileTempClass", parameters)
        End Function

        Public Function ExecuteClassWithMainMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal instanceName As String, ByVal ParamArray parameters As Object()) As Object
            Return Me.InvokeMainMethod(instanceName, parameters)
        End Function

        Public Function ExecuteMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteMethod(pathToAssemblyWithMainMethod, New String() {}, New String() {}, False, methodName, parameters)
        End Function

        Public Function ExecuteMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal importNamespaces As String(), ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteClassWithMethod(pathToAssemblyWithMainMethod, additionalAssembliesToReference, New String() {}, debugMode, "CompuMasterJitCompileTempClass", methodName, parameters)
        End Function

        Public Function ExecuteClassWithMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal instanceName As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return Me.Invoke(instanceName, methodName, parameters)
        End Function


    End Class

End Namespace