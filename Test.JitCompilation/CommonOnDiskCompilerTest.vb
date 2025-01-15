Imports NUnit.Framework
Imports CompuMaster.JitCompilation
Imports CompuMaster.VisualBasicCompatibility

Namespace CompuMaster.Tests.JitCompilation
    <TestFixture()> Public Class CommonOnDiskCompilerTest

        Private myTestCompiler As CompuMaster.JitCompilation.BaseOnDiskCompiler = New CompuMaster.JitCompilation.VBNetOnDiskCompiler

        <OneTimeTearDown> Public Sub FullTempFilesCleanup()
            CompuMaster.IO.TemporaryFile.CleanupOnApplicationExit()
        End Sub


        <Test>
        Public Sub BasicCompilationDllLibrary()
            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")
            Dim TestAppSources As CompileSources = AppHelloWorld()
            Dim Parameters As Object() = TestAppSources.InvokeParameters.ToArray

            'Compile dependencies
            Dim CompilationResult As CompileOnDiskResults = TestAppSources.OnDiskCompiler.Compile(TestAppSources.SourceCode, TestAppSources.AdditionalReferencePaths.ToArray, TestAppSources.ImportNamespaces.ToArray, True, OutputAssemblyPath.FilePath, Common.TargetType.Library)
            Assert.That(CompilationResult.CompilerErrors.Count, [Is].Zero())
            Assert.That(CompilationResult.CompilerResults.Errors.Count, [Is].Zero())
            For Each OutputInfo As String In CompilationResult.CompilerResults.Output
                System.Console.WriteLine(OutputInfo)
            Next
            For Each ErrorInfo As String In CompilationResult.CompilerResults.Errors
                System.Console.WriteLine("STD-ERR: " & ErrorInfo.Replace(ControlChars.CrLf, ControlChars.Cr).Replace(ControlChars.Lf, ControlChars.Cr).Replace(ControlChars.Cr, "STD-ERR: " & System.Environment.NewLine))
            Next
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)

            Dim InvokeResult As Object = CompilationResult.InvokeMainMethod("MainApp", Parameters)
            Assert.That(InvokeResult, [Is].Null, "Sub Main must/does not return a value")
        End Sub

        <Test>
        Public Sub BasicCompilationNetModule()
            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".netmodule")
            Dim TestAppSources As CompileSources = AppHelloWorld()
            Dim Parameters As Object() = TestAppSources.InvokeParameters.ToArray

            'Compile dependencies
            Dim CompilationResult As CompileOnDiskResults = TestAppSources.OnDiskCompiler.Compile(TestAppSources.SourceCode, TestAppSources.AdditionalReferencePaths.ToArray, TestAppSources.ImportNamespaces.ToArray, True, OutputAssemblyPath.FilePath, Common.TargetType.NetModule)
            Assert.That(CompilationResult.CompilerErrors.Count, [Is].Zero())
            Assert.That(CompilationResult.CompilerResults.Errors.Count, [Is].Zero())
            For Each OutputInfo As String In CompilationResult.CompilerResults.Output
                System.Console.WriteLine(OutputInfo)
            Next
            For Each ErrorInfo As String In CompilationResult.CompilerResults.Errors
                System.Console.WriteLine("STD-ERR: " & ErrorInfo.Replace(ControlChars.CrLf, ControlChars.Cr).Replace(ControlChars.Lf, ControlChars.Cr).Replace(ControlChars.Cr, "STD-ERR: " & System.Environment.NewLine))
            Next
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)

            'Compile output assembly
            ''Add importData as the first calling parameters to the parameters list of the store command
            'CompilerSourcesStoreData.InvokeParameters.Insert(0, importData)

            'Execute compiled assembly if desired
            'Open (explore) folder with output assemblies
        End Sub

        <Test>
        Public Sub BasicCompilationExeConsole()
            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".exe")
            Dim TestAppSources As CompileSources = AppHelloWorld()
            Dim Parameters As Object() = TestAppSources.InvokeParameters.ToArray

            'Compile dependencies
            Dim CompilationResult As CompileOnDiskResults = TestAppSources.OnDiskCompiler.Compile(TestAppSources.SourceCode, TestAppSources.AdditionalReferencePaths.ToArray, TestAppSources.ImportNamespaces.ToArray, True, OutputAssemblyPath.FilePath, Common.TargetType.ConsoleApplication)
            Assert.That(CompilationResult.CompilerErrors.Count, [Is].Zero())
            Assert.That(CompilationResult.CompilerResults.Errors.Count, [Is].Zero())
            For Each OutputInfo As String In CompilationResult.CompilerResults.Output
                System.Console.WriteLine(OutputInfo)
            Next
            For Each ErrorInfo As String In CompilationResult.CompilerResults.Errors
                System.Console.WriteLine("STD-ERR: " & ErrorInfo.Replace(ControlChars.CrLf, ControlChars.Cr).Replace(ControlChars.Lf, ControlChars.Cr).Replace(ControlChars.Cr, "STD-ERR: " & System.Environment.NewLine))
            Next
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)

            Dim InvokeResult As Object = CompilationResult.InvokeMainMethod("MainApp", Parameters)
            Assert.That(InvokeResult, [Is].Null, "Sub Main must/does not return a value")

            Dim RunResult As ExecuteConsoleAppResult = CompilationResult.ExecuteConsoleApp(TestAppSources.CommandLineParameters.ToArray)
            Assert.That(RunResult.ExitCode, [Is].Zero())
            Assert.That(RunResult.StandardOutput.ToString, [Does].Contain("Hello World!"))
            Assert.That(RunResult.StandardOutput.ToString, [Does].Contain("There are 2 arguments given."), "Intended commandline arguments: " & String.Join(" ", TestAppSources.CommandLineParameters))
            Assert.That(RunResult.StandardErrorOutput.Length, [Is].Zero)
        End Sub

        <Test>
        Public Sub BasicCompilationExeConsoleModule()
            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".exe")
            Dim TestAppSources As CompileSources = AppHelloWorldModule()
            Dim Parameters As Object() = TestAppSources.InvokeParameters.ToArray

            'Compile dependencies
            Dim CompilationResult As CompileOnDiskResults = TestAppSources.OnDiskCompiler.Compile(TestAppSources.SourceCode, TestAppSources.AdditionalReferencePaths.ToArray, TestAppSources.ImportNamespaces.ToArray, True, OutputAssemblyPath.FilePath, Common.TargetType.ConsoleApplication)
            Assert.That(CompilationResult.CompilerErrors.Count, [Is].Zero())
            Assert.That(CompilationResult.CompilerResults.Errors.Count, [Is].Zero())
            For Each OutputInfo As String In CompilationResult.CompilerResults.Output
                System.Console.WriteLine(OutputInfo)
            Next
            For Each ErrorInfo As String In CompilationResult.CompilerResults.Errors
                System.Console.WriteLine("STD-ERR: " & ErrorInfo.Replace(ControlChars.CrLf, ControlChars.Cr).Replace(ControlChars.Lf, ControlChars.Cr).Replace(ControlChars.Cr, "STD-ERR: " & System.Environment.NewLine))
            Next
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)

            Dim InvokeResult As Object = CompilationResult.InvokeMainMethod("MainApp", Parameters)
            Assert.That(InvokeResult, [Is].Null, "Sub Main must/does not return a value")

            Dim RunResult As ExecuteConsoleAppResult = CompilationResult.ExecuteConsoleApp(TestAppSources.CommandLineParameters.ToArray)
            Assert.That(RunResult.ExitCode, [Is].Zero())
            Assert.That(RunResult.StandardOutput.ToString, [Does].Contain("Hello World!"))
            Assert.That(RunResult.StandardOutput.ToString, [Does].Contain("There are 2 arguments given."), "Intended commandline arguments: " & String.Join(" ", TestAppSources.CommandLineParameters))
            Assert.That(RunResult.StandardErrorOutput.Length, [Is].Zero)

            Dim RunSeparateExeResult As ExecuteConsoleAppResult = CompileOnDiskResults.ExecuteConsoleAppFromDisk(OutputAssemblyPath.FilePath, TestAppSources.CommandLineParameters.ToArray)
            Assert.That(RunResult.ExitCode, [Is].Zero())
            Assert.That(RunResult.StandardOutput.ToString, [Does].Contain("Hello World!"))
            Assert.That(RunResult.StandardOutput.ToString, [Does].Contain("There are 2 arguments given."), "Intended commandline arguments: " & String.Join(" ", TestAppSources.CommandLineParameters))
            Assert.That(RunResult.StandardErrorOutput.Length, [Is].Zero)
        End Sub

        <Test>
        Public Sub BasicCompilationExeWindowsApp()
            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".exe")
            Dim TestAppSources As CompileSources = AppHelloWorld()
            Dim Parameters As Object() = TestAppSources.InvokeParameters.ToArray

            'Compile dependencies
            Dim CompilationResult As CompileOnDiskResults = TestAppSources.OnDiskCompiler.Compile(TestAppSources.SourceCode, TestAppSources.AdditionalReferencePaths.ToArray, TestAppSources.ImportNamespaces.ToArray, True, OutputAssemblyPath.FilePath, Common.TargetType.WindowsApplication)
            Assert.That(CompilationResult.CompilerErrors.Count, [Is].Zero())
            Assert.That(CompilationResult.CompilerResults.Errors.Count, [Is].Zero())
            For Each OutputInfo As String In CompilationResult.CompilerResults.Output
                System.Console.WriteLine(OutputInfo)
            Next
            For Each ErrorInfo As String In CompilationResult.CompilerResults.Errors
                System.Console.WriteLine("STD-ERR: " & ErrorInfo.Replace(ControlChars.CrLf, ControlChars.Cr).Replace(ControlChars.Lf, ControlChars.Cr).Replace(ControlChars.Cr, "STD-ERR: " & System.Environment.NewLine))
            Next
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)

            Dim InvokeResult As Object = CompilationResult.InvokeMainMethod("MainApp", Parameters)
            Assert.That(InvokeResult, [Is].Null, "Sub Main must/does not return a value")

            Dim RunResult As ExecuteConsoleAppResult = CompilationResult.ExecuteConsoleApp(TestAppSources.CommandLineParameters.ToArray)
            Assert.That(RunResult.ExitCode, [Is].Zero())
            Assert.That(RunResult.StandardOutput.ToString, [Does].Contain("Hello World!"))
            Assert.That(RunResult.StandardOutput.ToString, [Does].Contain("There are 2 arguments given."))
            Assert.That(RunResult.StandardErrorOutput.Length, [Is].Zero)   'Compile output assembly
        End Sub

        <Test>
        Public Sub BasicCompilationAppContainerExe()
            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".exe")
            Dim TestAppSources As CompileSources = AppHelloWorld()
            Dim Parameters As Object() = TestAppSources.InvokeParameters.ToArray

            'Compile dependencies
            Dim CompilationResult As CompileOnDiskResults = TestAppSources.OnDiskCompiler.Compile(TestAppSources.SourceCode, TestAppSources.AdditionalReferencePaths.ToArray, TestAppSources.ImportNamespaces.ToArray, True, OutputAssemblyPath.FilePath, Common.TargetType.AppContainerExe)
            Assert.That(CompilationResult.CompilerErrors.Count, [Is].Zero())
            Assert.That(CompilationResult.CompilerResults.Errors.Count, [Is].Zero())
            For Each OutputInfo As String In CompilationResult.CompilerResults.Output
                System.Console.WriteLine(OutputInfo)
            Next
            For Each ErrorInfo As String In CompilationResult.CompilerResults.Errors
                System.Console.WriteLine("STD-ERR: " & ErrorInfo.Replace(ControlChars.CrLf, ControlChars.Cr).Replace(ControlChars.Lf, ControlChars.Cr).Replace(ControlChars.Cr, "STD-ERR: " & System.Environment.NewLine))
            Next
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)

            'Compile output assembly
            ''Add importData as the first calling parameters to the parameters list of the store command
            'CompilerSourcesStoreData.InvokeParameters.Insert(0, importData)

            'Execute compiled assembly if desired
            'Open (explore) folder with output assemblies
        End Sub

        <Test>
        Public Sub BasicCompilationWinMDObj()
            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".winmdobj")
            Dim TestAppSources As CompileSources = AppHelloWorld()
            Dim Parameters As Object() = TestAppSources.InvokeParameters.ToArray

            'Compile dependencies
            Dim CompilationResult As CompileOnDiskResults = TestAppSources.OnDiskCompiler.Compile(TestAppSources.SourceCode, TestAppSources.AdditionalReferencePaths.ToArray, TestAppSources.ImportNamespaces.ToArray, True, OutputAssemblyPath.FilePath, Common.TargetType.WinMDObj)
            Assert.That(CompilationResult.CompilerErrors.Count, [Is].Zero())
            Assert.That(CompilationResult.CompilerResults.Errors.Count, [Is].Zero())
            For Each OutputInfo As String In CompilationResult.CompilerResults.Output
                System.Console.WriteLine(OutputInfo)
            Next
            For Each ErrorInfo As String In CompilationResult.CompilerResults.Errors
                System.Console.WriteLine("STD-ERR: " & ErrorInfo.Replace(ControlChars.CrLf, ControlChars.Cr).Replace(ControlChars.Lf, ControlChars.Cr).Replace(ControlChars.Cr, "STD-ERR: " & System.Environment.NewLine))
            Next
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)

            'Compile output assembly
            ''Add importData as the first calling parameters to the parameters list of the store command
            'CompilerSourcesStoreData.InvokeParameters.Insert(0, importData)

            'Execute compiled assembly if desired
            'Open (explore) folder with output assemblies
        End Sub

        <Test>
        Public Sub BasicCompilationExeConsoleWithDependency()
            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".exe")
            Dim TestAppSources As CompileSources = AppHelloWorldWithDependency()
            Dim Parameters As Object() = TestAppSources.InvokeParameters.ToArray

            'Compile dependencies
            Dim CompilationResult As CompileOnDiskResults = TestAppSources.OnDiskCompiler.Compile(TestAppSources.SourceCode, TestAppSources.AdditionalReferencePaths.ToArray, TestAppSources.ImportNamespaces.ToArray, True, OutputAssemblyPath.FilePath, Common.TargetType.ConsoleApplication)
            Assert.That(CompilationResult.CompilerErrors.Count, [Is].Zero())
            Assert.That(CompilationResult.CompilerResults.Errors.Count, [Is].Zero())
            For Each OutputInfo As String In CompilationResult.CompilerResults.Output
                System.Console.WriteLine(OutputInfo)
            Next
            For Each ErrorInfo As String In CompilationResult.CompilerResults.Errors
                System.Console.WriteLine("STD-ERR: " & ErrorInfo.Replace(ControlChars.CrLf, ControlChars.Cr).Replace(ControlChars.Lf, ControlChars.Cr).Replace(ControlChars.Cr, "STD-ERR: " & System.Environment.NewLine))
            Next
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)
            For Each ExpectedReferenceAssembly As String In TestAppSources.AdditionalReferencePaths
                Dim ExpectedReferenceAssemblyFullPath As String = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(OutputAssemblyPath.FilePath), ExpectedReferenceAssembly)
                Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Referenced assembly file not found at output location: " & ExpectedReferenceAssemblyFullPath)
            Next

            Console.Write("## InvokeMainMethod")
            Dim InvokeResult As Object = CompilationResult.InvokeMainMethod("MainApp", Parameters)
            Assert.That(InvokeResult, [Is].Null, "Sub Main must/does not return a value")

            Console.Write("## ExecuteConsoleApp")
            Dim RunResult As ExecuteConsoleAppResult = CompilationResult.ExecuteConsoleApp(TestAppSources.CommandLineParameters.ToArray)
            If RunResult.ExitCode <> 0 Then
                System.Console.WriteLine("STD-ERR: " & RunResult.StandardErrorOutput.ToString)
            End If
            Assert.That(RunResult.StandardOutput.ToString, [Does].Contain("Hello World!"))
            Assert.That(RunResult.StandardErrorOutput.Length, [Is].Zero)
            Assert.That(RunResult.ExitCode, [Is].Zero())
        End Sub

        <Test>
        Public Sub BasicCompilationDllLibraryCustomInstanceMethodCall()
            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")
            Dim TestAppSources As CompileSources = AppForCustomInstanceMethodCall()
            Dim Parameters As Object() = TestAppSources.InvokeParameters.ToArray

            'Compile dependencies
            Dim CompilationResult As CompileOnDiskResults = TestAppSources.OnDiskCompiler.Compile(TestAppSources.SourceCode, TestAppSources.AdditionalReferencePaths.ToArray, TestAppSources.ImportNamespaces.ToArray, True, OutputAssemblyPath.FilePath, Common.TargetType.Library)
            Assert.That(CompilationResult.CompilerErrors.Count, [Is].Zero())
            Assert.That(CompilationResult.CompilerResults.Errors.Count, [Is].Zero())
            For Each OutputInfo As String In CompilationResult.CompilerResults.Output
                System.Console.WriteLine(OutputInfo)
            Next
            For Each ErrorInfo As String In CompilationResult.CompilerResults.Errors
                System.Console.WriteLine("STD-ERR: " & ErrorInfo.Replace(ControlChars.CrLf, ControlChars.Cr).Replace(ControlChars.Lf, ControlChars.Cr).Replace(ControlChars.Cr, "STD-ERR: " & System.Environment.NewLine))
            Next
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)

            Dim InvokeResult As Object = CompilationResult.Invoke("MainApp", "CustomMethod", New Object() {"USA", 3})
            Assert.AreEqual("Hello USA! It would be nice to see for again 4 times.", CType(InvokeResult, String))
        End Sub

        <Test>
        Public Sub BasicCompilationDllLibraryCustomStaticMethodCall()
            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")
            Dim TestAppSources As CompileSources = AppForCustomStaticMethodCall()
            Dim Parameters As Object() = TestAppSources.InvokeParameters.ToArray

            'Compile dependencies
            Dim CompilationResult As CompileOnDiskResults = TestAppSources.OnDiskCompiler.Compile(TestAppSources.SourceCode, TestAppSources.AdditionalReferencePaths.ToArray, TestAppSources.ImportNamespaces.ToArray, True, OutputAssemblyPath.FilePath, Common.TargetType.Library)
            Assert.That(CompilationResult.CompilerErrors.Count, [Is].Zero())
            Assert.That(CompilationResult.CompilerResults.Errors.Count, [Is].Zero())
            For Each OutputInfo As String In CompilationResult.CompilerResults.Output
                System.Console.WriteLine(OutputInfo)
            Next
            For Each ErrorInfo As String In CompilationResult.CompilerResults.Errors
                System.Console.WriteLine("STD-ERR: " & ErrorInfo.Replace(ControlChars.CrLf, ControlChars.Cr).Replace(ControlChars.Lf, ControlChars.Cr).Replace(ControlChars.Cr, "STD-ERR: " & System.Environment.NewLine))
            Next
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)

            Dim InvokeResult As Object = CompilationResult.Invoke("MainApp", "CustomMethod", New Object() {"USA", 3})
            Assert.AreEqual("Hello USA! It would be nice to see for again 4 times.", CType(InvokeResult, String))
        End Sub

        ''' <summary>
        ''' Hello world app code with class and static method
        ''' </summary>
        ''' <returns></returns>
        Private Function AppHelloWorld() As CompileSources
            Dim Result As New CompileSources
            Result.SourceCode =
                "Public Class MainApp" & ControlChars.CrLf &
                "   Public Shared Sub Main(args As String()) " & ControlChars.CrLf &
                "      Console.WriteLine (""Hello World!"") " & ControlChars.CrLf &
                "      Console.WriteLine (""There are "" & args.Length.ToString() & "" arguments given."") " & ControlChars.CrLf &
                "      For Each Arg As String in args" & ControlChars.CrLf &
                "         Console.WriteLine (""* "" & Arg) " & ControlChars.CrLf &
                "      Next" & ControlChars.CrLf &
                "   End Sub " & ControlChars.CrLf &
                "End Class"
            Result.OnDiskCompiler = myTestCompiler
            Result.ImportNamespaces.Add("System")
            Result.CommandLineParameters.AddRange(New String() {"Hello World!", "Hello USA!"})
            Result.InvokeParameters.Add(New String() {"Hello World!", "Hello USA!"})
            Return Result
        End Function

        ''' <summary>
        ''' Hello world app code with module and method
        ''' </summary>
        ''' <returns></returns>
        Private Function AppHelloWorldModule() As CompileSources
            Dim Result As New CompileSources
            Result.SourceCode =
                "Public Module MainApp" & ControlChars.CrLf &
                "   Public Sub Main(args As String()) " & ControlChars.CrLf &
                "      System.Console.WriteLine (""Hello World!"") " & ControlChars.CrLf &
                "      System.Console.WriteLine (""There are "" & args.Length.ToString() & "" arguments given."") " & ControlChars.CrLf &
                "      For Each Arg As String in args" & ControlChars.CrLf &
                "         Console.WriteLine (""* "" & Arg) " & ControlChars.CrLf &
                "      Next" & ControlChars.CrLf &
                "   End Sub " & ControlChars.CrLf &
                "End Module"
            Result.OnDiskCompiler = myTestCompiler
            Result.ImportNamespaces.Add("System")
            Result.CommandLineParameters.AddRange(New String() {"Hello World!", "Hello USA!"})
            Result.InvokeParameters.Add(New String() {"Hello World!", "Hello USA!"})
            Return Result
        End Function

        ''' <summary>
        ''' Hello world app code with class and static method, but with external library dependency
        ''' </summary>
        ''' <returns></returns>
        Private Function AppHelloWorldWithDependency() As CompileSources
            Dim Result As New CompileSources
            Result.SourceCode =
                "Public Class MainApp" & ControlChars.CrLf &
                "   Public Shared Sub Main(args As String()) " & ControlChars.CrLf &
                "      System.Console.WriteLine (""Hello World!"") " & ControlChars.CrLf &
                "      System.Console.WriteLine (CompuMaster.VisualBasicCompatibility.Strings.StrDup(12, ""=""c)) " & ControlChars.CrLf &
                "   End Sub " & ControlChars.CrLf &
                "End Class"
            Result.OnDiskCompiler = myTestCompiler
            Result.ImportNamespaces.Add("System")
            Dim ExecutingAssembly As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
            Dim ExecutingAssemblyDirectory As String = System.IO.Path.GetDirectoryName(ExecutingAssembly.Location)
            Result.AdditionalReferencePaths.Add(System.IO.Path.Combine(ExecutingAssemblyDirectory, "CompuMaster.VisualBasicCompatibility.dll"))
            Result.RequiredFilesForDeployment.Add(System.IO.Path.Combine(ExecutingAssemblyDirectory, "CompuMaster.VisualBasicCompatibility.dll"))
            Result.CommandLineParameters.AddRange(New String() {"Hello World!", "Hello USA!"})
            Result.InvokeParameters.Add(New String() {"Hello World!", "Hello USA!"})
            Return Result
        End Function

        ''' <summary>
        ''' Hello world app code with class and static method
        ''' </summary>
        ''' <returns></returns>
        Private Function AppForCustomInstanceMethodCall()
            Dim Result As New CompileSources
            Result.OnDiskCompiler = myTestCompiler
            Result.SourceCode =
                "Public Class MainApp" & ControlChars.CrLf &
                "   Public Function CustomMethod (ByVal name As String, number As Integer) As String" & ControlChars.CrLf &
                "       Return (""Hello "" & name & ""! It would be nice to see for again "" & (number + 1).ToString() & "" times."") " & ControlChars.CrLf &
                "   End Function" & ControlChars.CrLf &
                "End Class"
            Result.InvokeParameters.AddRange(New Object() {"world", 2})
            Return Result
        End Function

        ''' <summary>
        ''' Hello world app code with class and static method
        ''' </summary>
        ''' <returns></returns>
        Private Function AppForCustomStaticMethodCall()
            Dim Result As New CompileSources
            Result.OnDiskCompiler = myTestCompiler
            Result.SourceCode =
                "Public Class MainApp" & ControlChars.CrLf &
                "   Public Shared Function CustomMethod (ByVal name As String, number As Integer) As String" & ControlChars.CrLf &
                "       Return (""Hello "" & name & ""! It would be nice to see for again "" & (number + 1).ToString() & "" times."") " & ControlChars.CrLf &
                "   End Function" & ControlChars.CrLf &
                "End Class"
            Result.InvokeParameters.AddRange(New Object() {"world", 2})
            Return Result
        End Function

    End Class

End Namespace
