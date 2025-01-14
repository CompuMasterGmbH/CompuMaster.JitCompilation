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
    Public Class CompileResults

        Friend Sub New(ByVal [assembly] As System.Reflection.Assembly, ByVal compilerResults As CompilerResults)
            _CompilerResults = compilerResults
            _Assembly = [assembly]
        End Sub

        Private _CompilerResults As CompilerResults
        Public ReadOnly Property CompilerResults() As CompilerResults
            Get
                Return _CompilerResults
            End Get
        End Property

        Public ReadOnly Property CompilerErrors() As CompilerErrorCollection
            Get
                If _CompilerResults Is Nothing Then
                    'Return empty collection (to prevent ObjectReference-Null-Exceptions
                    Return New CompilerErrorCollection
                Else
                    Return _CompilerResults.Errors
                End If
            End Get
        End Property

        Private _Assembly As System.Reflection.Assembly
        Public ReadOnly Property [Assembly]() As System.Reflection.Assembly
            Get
                Return _Assembly
            End Get
        End Property

        Public Function InvokeMainMethod(ByVal instanceName As String) As Object
            Return _Invoke(instanceName, "Main", Nothing)
        End Function

        Public Function InvokeMainMethod(ByVal instanceName As String, ByVal ParamArray parameters As Object()) As Object
            Return _Invoke(instanceName, "Main", parameters)
        End Function

        Public Function Invoke(ByVal instanceName As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return _Invoke(instanceName, methodName, parameters)
        End Function

        Private Function _Invoke(ByVal instanceName As String, ByVal methodName As String, ByVal parameters As Object()) As Object

            Dim MyExecutedInstance As Object = Me.Assembly.CreateInstance(instanceName)
            Dim MyMethodInfo As System.Reflection.MethodInfo = MyExecutedInstance.GetType.GetMethod(methodName)

            Return MyMethodInfo.Invoke(MyExecutedInstance, parameters)

        End Function

    End Class

End Namespace