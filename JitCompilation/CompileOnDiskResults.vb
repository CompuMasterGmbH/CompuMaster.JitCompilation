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

    End Class

End Namespace