Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler
Imports System.Text
Imports System.Threading

Namespace CompuMaster.JitCompilation

    ''' <summary>
    ''' A compilation result for access to the compiled assembly or compilation errors
    ''' </summary>
    Public Class CompileOnDiskResults
        Inherits CompileResults

        Friend Sub New(ByVal [assembly] As System.Reflection.Assembly, outputAssemblyPath As String, ByVal compilerResults As System.CodeDom.Compiler.CompilerResults)
            MyBase.New([assembly], compilerResults)
            If outputAssemblyPath = Nothing Then Throw New ArgumentNullException(NameOf(outputAssemblyPath))
            Me.OutputAssemblyPath = outputAssemblyPath
        End Sub

        ''' <summary>
        ''' Load a compiled assembly from disk and make it accessible for further invocations
        ''' </summary>
        ''' <param name="outputAssemblyPath"></param>
        Friend Sub New(outputAssemblyPath As String)
            MyBase.New(System.Reflection.Assembly.LoadFrom(outputAssemblyPath), Nothing)
            If outputAssemblyPath = Nothing Then Throw New ArgumentNullException(NameOf(outputAssemblyPath))
            Me.OutputAssemblyPath = outputAssemblyPath
        End Sub

        ''' <summary>
        ''' ATTENTION: Use this constructor overload for .netmodule only: No loading of compiled assembly from disk, just make it accessible for further execution via Execute methods (involving System.Diagnostics.Process.Start)
        ''' </summary>
        ''' <param name="outputAssemblyPath"></param>
        ''' <remarks>Required usually for .netmodule only, since those output files don't contain a valid assembly manifest</remarks>
        Friend Sub New(outputAssemblyPath As String, ByVal compilerResults As System.CodeDom.Compiler.CompilerResults)
            MyBase.New(Nothing, compilerResults)
            If outputAssemblyPath = Nothing Then Throw New ArgumentNullException(NameOf(outputAssemblyPath))
            Me.OutputAssemblyPath = outputAssemblyPath
        End Sub

        ''' <summary>
        ''' Load a compiled assembly from disk and make it accessible for further invocations
        ''' </summary>
        ''' <param name="[assembly]"></param>
        Friend Sub New(ByVal [assembly] As System.Reflection.Assembly)
            MyBase.New([assembly], Nothing)
            Me.OutputAssemblyPath = [assembly].Location
        End Sub

        ''' <summary>
        ''' Create a CompileOnDiskResults without loading the compiled assembly from disk, but make it accessible for further execution via Execute methods (involving System.Diagnostics.Process.Start)
        ''' </summary>
        ''' <param name="outputAssemblyPath"></param>
        ''' <returns></returns>
        Public Shared Function PrepareExecutionFrom(outputAssemblyPath As String) As CompileOnDiskResults
            Return New CompileOnDiskResults(outputAssemblyPath, Nothing)
        End Function

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

        Public Function ExecuteClassWithMainMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal className As String, ByVal ParamArray parameters As Object()) As Object
            Return Me.InvokeMainMethod(className, parameters)
        End Function

        Public Function ExecuteMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteMethod(pathToAssemblyWithMainMethod, New String() {}, New String() {}, False, methodName, parameters)
        End Function

        Public Function ExecuteMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal importNamespaces As String(), ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteClassWithMethod(pathToAssemblyWithMainMethod, additionalAssembliesToReference, New String() {}, debugMode, "CompuMasterJitCompileTempClass", methodName, parameters)
        End Function

        Public Function ExecuteClassWithMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal className As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return Me.Invoke(className, methodName, parameters)
        End Function

        ''' <summary>
        ''' Execute a console application and return its exit code
        ''' </summary>
        ''' <param name="exePath"></param>
        ''' <returns></returns>
        Public Function ExecuteConsoleApp(ParamArray args As String()) As ExecuteConsoleAppResult
            Return ExecuteConsoleApp(CType(Nothing, System.IO.DirectoryInfo), args)
        End Function

        ''' <summary>
        ''' Execute a console application and return its exit code
        ''' </summary>
        ''' <param name="exePath"></param>
        ''' <returns></returns>
        Public Function ExecuteConsoleApp(workingDirectory As System.IO.DirectoryInfo, ParamArray args As String()) As ExecuteConsoleAppResult
            Dim Result As New ExecuteConsoleAppResult
            Result.ExitCode = ExecuteConsoleAppFromDisk(Me.OutputAssemblyPath, workingDirectory, args, Result.StandardOutput, Result.StandardErrorOutput)
            Return Result
        End Function

        ''' <summary>
        ''' Execute a console application and return its exit code
        ''' </summary>
        ''' <param name="exePath"></param>
        ''' <returns></returns>
        Public Shared Function ExecuteConsoleAppFromDisk(ByVal exePath As String, ParamArray args As String()) As ExecuteConsoleAppResult
            Return ExecuteConsoleAppFromDisk(exePath, CType(Nothing, System.IO.DirectoryInfo), args)
        End Function

        ''' <summary>
        ''' Execute a console application and return its exit code
        ''' </summary>
        ''' <param name="exePath"></param>
        ''' <returns></returns>
        Public Shared Function ExecuteConsoleAppFromDisk(ByVal exePath As String, workingDirectory As System.IO.DirectoryInfo, ParamArray args As String()) As ExecuteConsoleAppResult
            Dim Result As New ExecuteConsoleAppResult
            Result.ExitCode = ExecuteConsoleAppFromDisk(exePath, workingDirectory, args, Result.StandardOutput, Result.StandardErrorOutput)
            Return Result
        End Function

        ''' <summary>
        ''' Execute a console application and return its exit code
        ''' </summary>
        ''' <param name="exePath"></param>
        ''' <param name="stdout"></param>
        ''' <param name="stderr"></param>
        ''' <returns></returns>
        Friend Shared Function ExecuteConsoleAppFromDisk(ByVal exePath As String, workingDirectory As System.IO.DirectoryInfo, args As String(), ByVal stdout As StringBuilder, ByVal stderr As StringBuilder, Optional timeout As TimeSpan? = Nothing) As Integer
            Try
                Dim process As New Process()
                process.StartInfo.FileName = exePath
                If args IsNot Nothing AndAlso args.Length <> 0 Then
                    Dim ArgsList As New List(Of String)
                    For Each arg In args
                        If arg.Contains(" ") = True AndAlso arg.Contains("""") = False Then
                            ArgsList.Add("""" & arg & """")
                        Else
                            ArgsList.Add(arg)
                        End If
                    Next
                    process.StartInfo.Arguments = String.Join(" ", ArgsList)
                Else
                    process.StartInfo.Arguments = ""
                End If
                If workingDirectory IsNot Nothing Then process.StartInfo.WorkingDirectory = workingDirectory.FullName
                process.StartInfo.RedirectStandardOutput = True
                process.StartInfo.RedirectStandardError = True
                process.StartInfo.UseShellExecute = False
                process.StartInfo.CreateNoWindow = True

                AddHandler process.OutputDataReceived, Sub(sender, e)
                                                           If e.Data IsNot Nothing Then
                                                               stdout.AppendLine(e.Data)
                                                           End If
                                                       End Sub
                AddHandler process.ErrorDataReceived, Sub(sender, e)
                                                          If e.Data IsNot Nothing Then
                                                              stderr.AppendLine(e.Data)
                                                          End If
                                                      End Sub

                process.Start()
                process.BeginOutputReadLine()
                process.BeginErrorReadLine()

                If timeout.HasValue Then
                    If Not process.WaitForExit(CType(timeout.Value.TotalMilliseconds, Int32)) Then
                        process.Kill()
                        stderr.AppendLine("Error: Process timed out.")
                        Return -2
                    End If
                Else
                    process.WaitForExit()
                End If


                Return process.ExitCode
            Catch ex As Exception
                stderr.AppendLine("Error: " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>
        ''' Execute a console application asynchronously and return its exit code
        ''' </summary>
        ''' <param name="exePath"></param>
        ''' <param name="workingDirectory"></param>
        ''' <param name="args"></param>
        ''' <param name="stdout"></param>
        ''' <param name="stderr"></param>
        ''' <param name="timeout"></param>
        ''' <returns></returns>
        Friend Shared Async Function ExecuteConsoleAppFromDiskAsync(ByVal exePath As String, workingDirectory As System.IO.DirectoryInfo, args As String(), ByVal stdout As StringBuilder, ByVal stderr As StringBuilder, Optional timeout As TimeSpan? = Nothing) As Task(Of Integer)
            Try
                Dim process As New Process()
                process.StartInfo.FileName = exePath
                If args IsNot Nothing AndAlso args.Length <> 0 Then
                    Dim ArgsList As New List(Of String)
                    For Each arg In args
                        If arg.Contains(" ") = True AndAlso arg.Contains("""") = False Then
                            ArgsList.Add("""" & arg & """")
                        Else
                            ArgsList.Add(arg)
                        End If
                    Next
                    process.StartInfo.Arguments = String.Join(" ", ArgsList)
                Else
                    process.StartInfo.Arguments = ""
                End If
                If workingDirectory IsNot Nothing Then process.StartInfo.WorkingDirectory = workingDirectory.FullName
                process.StartInfo.RedirectStandardOutput = True
                process.StartInfo.RedirectStandardError = True
                process.StartInfo.UseShellExecute = False
                process.StartInfo.CreateNoWindow = True

                Dim tcs As New TaskCompletionSource(Of Integer)()

                AddHandler process.OutputDataReceived, Sub(sender, e)
                                                           If e.Data IsNot Nothing Then
                                                               stdout.AppendLine(e.Data)
                                                           End If
                                                       End Sub
                AddHandler process.ErrorDataReceived, Sub(sender, e)
                                                          If e.Data IsNot Nothing Then
                                                              stderr.AppendLine(e.Data)
                                                          End If
                                                      End Sub
                AddHandler process.Exited, Sub()
                                               tcs.TrySetResult(process.ExitCode)
                                           End Sub

                process.EnableRaisingEvents = True
                process.Start()
                process.BeginOutputReadLine()
                process.BeginErrorReadLine()

                Dim completedTask = Await Task.WhenAny(tcs.Task, Task.Delay(If(timeout.HasValue, CType(timeout.Value.TotalMilliseconds, Int32), Int32.MaxValue)))
                If completedTask IsNot tcs.Task Then
                    process.Kill()
                    stderr.AppendLine("Error: Process timed out.")
                    Return -2
                End If

                Return Await tcs.Task
            Catch ex As Exception
                stderr.AppendLine("Error: " & ex.Message)
                Return -1
            End Try
        End Function

    End Class

End Namespace