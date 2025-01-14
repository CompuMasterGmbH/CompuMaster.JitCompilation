Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler
Imports System.Reflection

Namespace CompuMaster.JitCompilation

    ''' <summary>
    ''' A compilation result for access to the compiled assembly or compilation errors
    ''' </summary>
    Public Class CompileResults

        Friend Sub New(ByVal [assembly] As System.Reflection.Assembly, ByVal compilerResults As CompilerResults)
            _CompilerResults = compilerResults
            _Assembly = [assembly]
        End Sub

        Private _CompilerResults As System.CodeDom.Compiler.CompilerResults
        ''' <summary>
        ''' The results of the compilation process
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CompilerResults() As System.CodeDom.Compiler.CompilerResults
            Get
                Return _CompilerResults
            End Get
        End Property

        ''' <summary>
        ''' Errors during compilation (or at least an empty collection if no errors occured)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CompilerErrors() As System.CodeDom.Compiler.CompilerErrorCollection
            Get
                If _CompilerResults Is Nothing Then
                    'Return empty collection (to prevent ObjectReference-Null-Exceptions
                    Return New System.CodeDom.Compiler.CompilerErrorCollection
                Else
                    Return _CompilerResults.Errors
                End If
            End Get
        End Property

        Private _Assembly As System.Reflection.Assembly
        ''' <summary>
        ''' The compiled assembly
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property [Assembly]() As System.Reflection.Assembly
            Get
                Return _Assembly
            End Get
        End Property

        ''' <summary>
        ''' Invoke the Main method (function or void/sub) of the compiled assembly
        ''' </summary>
        ''' <param name="className">Class name (optionally inclusive namespace)</param>
        ''' <returns>The result of the function (if the method is a function)</returns>
        Public Overridable Function InvokeMainMethod(ByVal className As String) As Object
            Return _Invoke(className, "Main", Nothing)
        End Function

        ''' <summary>
        ''' Invoke the Main method (function or void/sub) of the compiled assembly
        ''' </summary>
        ''' <param name="className">Class name (optionally inclusive namespace)</param>
        ''' <param name="parameters">0 to n arguments</param>
        ''' <returns>The result of the function (if the method is a function)</returns>
        Public Overridable Function InvokeMainMethod(ByVal className As String, ByVal ParamArray parameters As Object()) As Object
            Return _Invoke(className, "Main", parameters)
        End Function

        ''' <summary>
        ''' Invoke a method (function or void/sub) of the compiled assembly
        ''' </summary>
        ''' <param name="className">Class name (optionally inclusive namespace)</param>
        ''' <param name="methodName">A method name, method must not be overloaded, but can use parameters</param>
        ''' <param name="parameters">0 to n arguments</param>
        ''' <returns>The result of the function (if the method is a function)</returns>
        Public Overridable Function Invoke(ByVal className As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return _Invoke(className, methodName, parameters)
        End Function

        ''' <summary>
        ''' Invoke a method (function or void/sub) of the compiled assembly
        ''' </summary>
        ''' <param name="className">Class name (optionally inclusive namespace)</param>
        ''' <param name="methodName">A method name, method must not be overloaded, but can use parameters</param>
        ''' <param name="parameters">0 to n arguments</param>
        ''' <returns>The result of the function (if the method is a function)</returns>
        Private Function _Invoke(ByVal className As String, ByVal methodName As String, ByVal parameters As Object()) As Object
            ' Try to find the type
            Dim TargetType As Type = Me.Assembly.GetType(className, throwOnError:=False)
            If TargetType Is Nothing Then
                Throw New System.TypeLoadException($"Class or module not found: {className}")
            End If

            ' Try to get the method
            Dim MyMethodInfo As System.Reflection.MethodInfo = TargetType.GetMethod(methodName, BindingFlags.Public Or BindingFlags.Static Or BindingFlags.Instance)
            If MyMethodInfo Is Nothing Then
                Throw New System.MissingMethodException($"Method not found: {methodName}")
            End If

            ' Determine if the method is static
            If MyMethodInfo.IsStatic Then
                ' Invoke static method (modules are effectively static classes in VB.NET)
                Return MyMethodInfo.Invoke(Nothing, parameters)
            Else
                ' Create an instance and invoke instance method
                Dim MyExecutedInstance As Object = Activator.CreateInstance(TargetType)
                If MyExecutedInstance Is Nothing Then
                    Throw New System.EntryPointNotFoundException($"Instance creation failed for {className}")
                End If
                Return MyMethodInfo.Invoke(MyExecutedInstance, parameters)
            End If
        End Function

    End Class

End Namespace