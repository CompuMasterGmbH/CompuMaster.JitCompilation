Imports NUnit.Framework
Imports CompuMaster.JitCompilation
Imports CompuMaster.VisualBasicCompatibility

Namespace CompuMaster.Tests.JitCompilation
    <TestFixture()> Public Class VBOnDiskCompilerTest

        Private myTestCompiler As CompuMaster.JitCompilation.BaseOnDiskCompiler = New CompuMaster.JitCompilation.VBNetOnDiskCompiler

        <OneTimeTearDown> Public Sub FullTempFilesCleanup()
            CompuMaster.IO.TemporaryFile.CleanupOnApplicationExit()
        End Sub


        <Test>
        Public Sub BasicCompilation()
            'TODOs:
            'Select output assembly
            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")

            'Support parameter /simulate if desired

            'Prepare compilation of required dependencies sources
            Dim TestAppSources As CompileSources = AppHelloWorld()
            Dim Parameters As Object() = TestAppSources.Parameters.ToArray

            'Compile dependencies
            TestAppSources.OnDiskCompiler.Compile(TestAppSources.MethodCode, TestAppSources.AdditionalReferencePaths.ToArray, TestAppSources.ImportNamespaces.ToArray, True, OutputAssemblyPath.FilePath, Common.TargetType.Library)
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)

            'Compile output assembly
            ''Add importData as the first calling parameters to the parameters list of the store command
            'CompilerSourcesStoreData.Parameters.Insert(0, importData)

            'Execute compiled assembly if desired
            'Open (explore) folder with output assemblies
        End Sub

        Private Function AppHelloWorld() As CompileSources
            Dim Result As New CompileSources
            Result.MethodCode =
                "Public Class MainApp" & ControlChars.CrLf &
                "   Public Shared Sub Main(args As String()) " & ControlChars.CrLf &
                "      System.Console.WriteLine (""Hello World!"") " & ControlChars.CrLf &
                "      System.Console.WriteLine (""There are "" & args.Length.ToString() & "" arguments given."") " & ControlChars.CrLf &
                "   End Sub " & ControlChars.CrLf &
                "End Class"
            Result.OnDiskCompiler = myTestCompiler
            Result.ImportNamespaces.Add("System")
            Result.Parameters.Add("Hello World!")
            Return Result
        End Function

        Private Function AppHelloWorldWithDependency() As CompileSources
            Dim Result As New CompileSources
            Result.MethodCode =
                "Public Class MainApp" & ControlChars.CrLf &
                "   Public Shared Sub Main(args As String()) " & ControlChars.CrLf &
                "      System.Console.WriteLine (""Hello World!"") " & ControlChars.CrLf &
                "      System.Console.WriteLine (CompuMaster.VisualBasicCompatibility.Strings.StrDup(12, ""=""c)) " & ControlChars.CrLf &
                "   End Sub " & ControlChars.CrLf &
                "End Class"
            Result.OnDiskCompiler = myTestCompiler
            Result.ImportNamespaces.Add("System")
            Result.AdditionalReferencePaths.Add(System.IO.Path.Combine("CompuMaster.VisualBasicCompatibility.dll"))
            Result.RequiredFilesForDeployment.Add(System.IO.Path.Combine("CompuMaster.VisualBasicCompatibility.dll"))
            Result.Parameters.Add("Hello World!")
            Return Result
        End Function

        Private Function AppForCustomMethodCall()
            Dim Result As New CompileSources
            Result.OnDiskCompiler = myTestCompiler
            Result.MethodCode =
                "Public Class MainApp" & ControlChars.CrLf &
                "   Public Function CustomMethod (ByVal name As String, number As Integer) As String" & ControlChars.CrLf &
                "       Return (""Hello "" & name & ""! It would be nice to see for again "" & (number + 1).ToString() times."") " & ControlChars.CrLf &
                "   End Function" & ControlChars.CrLf &
                "End Class"
            Result.Parameters.AddRange(New Object() {"world", 2})
            Return Result
        End Function

        '<Test()> Public Sub compileCode_with_debug()
        '    'Some test code
        '    Dim src As String = "Public Class CmTestCompiler" & ControlChars.CrLf &
        '    "Public Function Answer() As String " & ControlChars.CrLf &
        '    "Return ""Hello World!"" " & ControlChars.CrLf &
        '    "End Function " & ControlChars.CrLf &
        '    "End Class"
        '
        '    Dim cResult As CompileResults = myTestCompiler.Compile(src, True)
        '
        '    Assert.IsEmpty(cResult.CompilerErrors)
        '    Assert.AreEqual("Hello World!", CType(cResult.Invoke("CmTestCompiler", "Answer", Nothing), String))
        '
        'End Sub
        '
        '<Test()> Public Sub compileCode_without_debug()
        '    'Some test code
        '    Dim src As String = "Public Class CmTestCompiler" & ControlChars.CrLf &
        '    "Public Function Answer() As String " & ControlChars.CrLf &
        '    "Return ""Hello World!"" " & ControlChars.CrLf &
        '    "End Function " & ControlChars.CrLf &
        '    "End Class"
        '
        '    Dim cResult As CompileResults = myTestCompiler.Compile(src, False)
        '
        '    Assert.IsEmpty(cResult.CompilerErrors)
        '    System.Console.WriteLine("OUTPUT of JIT-compiled method: ")
        '    System.Console.WriteLine(CType(cResult.Invoke("CmTestCompiler", "Answer", Nothing), String))
        '    Assert.AreEqual("Hello World!", CType(cResult.Invoke("CmTestCompiler", "Answer", Nothing), String))
        '
        'End Sub
        '
        '<Test()> Public Sub compileCode_with_debug_and_parameters_mainmethod()
        '    'Some test code
        '    Dim src As String = "Public Class CmTestCompiler" & ControlChars.CrLf &
        '    "Public Function Main(country as string) As String " & ControlChars.CrLf &
        '    "Return ""Hello "" & country & ""!"" " & ControlChars.CrLf &
        '    "End Function " & ControlChars.CrLf &
        '    "End Class"
        '
        '    Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMainMethod(src, New String() {}, New String() {}, True, "CmTestCompiler", "Germany")
        '    Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))
        '
        'End Sub
        '
        '<Test()> Public Sub compileCode_with_debug_and_parameters()
        '    'Some test code
        '    Dim src As String = "Public Class CmTestCompiler" & ControlChars.CrLf &
        '    "Public Function Answer(country as string) As String " & ControlChars.CrLf &
        '    "Return ""Hello "" & country & ""!"" " & ControlChars.CrLf &
        '    "End Function " & ControlChars.CrLf &
        '    "End Class"
        '
        '    Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMethod(src, New String() {}, New String() {}, True, "CmTestCompiler", "Answer", "Germany")
        '    Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))
        '
        'End Sub
        '
        '<Test()> Public Sub compileCode_with_debug_and_parameters_static()
        '    'Some test code
        '    Dim src As String = "Public  Class CmTestCompiler" & ControlChars.CrLf &
        '    "Public Shared Function Answer(country as string) As String " & ControlChars.CrLf &
        '    "Return ""Hello "" & country & ""!"" " & ControlChars.CrLf &
        '    "End Function " & ControlChars.CrLf &
        '    "End Class"
        '
        '    Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMethod(src, New String() {}, New String() {}, True, "CmTestCompiler", "Answer", "Germany")
        '    Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))
        '
        'End Sub

    End Class

End Namespace
