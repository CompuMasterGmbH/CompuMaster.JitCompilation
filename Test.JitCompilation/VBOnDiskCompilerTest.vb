Imports NUnit.Framework
Imports CompuMaster.JitCompilation
Imports CompuMaster.VisualBasicCompatibility

Namespace CompuMaster.Tests.JitCompilation
    <TestFixture()> Public Class VBOnDiskCompilerTest

        Private myTestCompiler As CompuMaster.JitCompilation.BaseOnDiskCompiler = New CompuMaster.JitCompilation.VBNetOnDiskCompiler

        <OneTimeTearDown> Public Sub FullTempFilesCleanup()
            CompuMaster.IO.TemporaryFile.CleanupOnApplicationExit()
        End Sub

        <Test()> Public Sub compileCode_with_debug()
            'Some test code
            Dim src As String =
                "Public Class CmTestCompiler" & ControlChars.CrLf &
                "Public Function Answer() As String " & ControlChars.CrLf &
                "Return ""Hello World!"" " & ControlChars.CrLf &
                "End Function " & ControlChars.CrLf &
                "End Class"

            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")
            Dim cResult As CompileOnDiskResults = myTestCompiler.Compile(src, True, OutputAssemblyPath.FilePath)

            Assert.IsEmpty(cResult.CompilerErrors)
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello World!", CType(cResult.Invoke("CmTestCompiler", "Answer", Nothing), String))

            Dim CompiledToDiskAssembly As CompileOnDiskResults = CompileOnDiskResults.LoadFrom(OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello World!", CType(CompiledToDiskAssembly.Invoke("CmTestCompiler", "Answer", Nothing), String))
        End Sub

        <Test()> Public Sub compileCode_without_debug()
            'Some test code
            Dim src As String =
                "Public Class CmTestCompiler" & ControlChars.CrLf &
                "Public Function Answer() As String " & ControlChars.CrLf &
                "Return ""Hello World!"" " & ControlChars.CrLf &
                "End Function " & ControlChars.CrLf &
                "End Class"

            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")
            Dim cResult As CompileOnDiskResults = myTestCompiler.Compile(src, False, OutputAssemblyPath.FilePath)

            Assert.IsEmpty(cResult.CompilerErrors)
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)
            System.Console.WriteLine("OUTPUT of JIT-compiled method: ")
            System.Console.WriteLine(CType(cResult.Invoke("CmTestCompiler", "Answer", Nothing), String))
            Assert.AreEqual("Hello World!", CType(cResult.Invoke("CmTestCompiler", "Answer", Nothing), String))

            Dim CompiledToDiskAssembly As CompileOnDiskResults = CompileOnDiskResults.LoadFrom(OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello World!", CType(CompiledToDiskAssembly.Invoke("CmTestCompiler", "Answer", Nothing), String))
        End Sub

        <Test()> Public Sub compileCode_with_debug_and_parameters_mainmethod()
            'Some test code
            Dim src As String =
                "Public Class CmTestCompiler" & ControlChars.CrLf &
                "Public Function Main(country as string) As String " & ControlChars.CrLf &
                "Return ""Hello "" & country & ""!"" " & ControlChars.CrLf &
                "End Function " & ControlChars.CrLf &
                "End Class"

            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")
            Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMainMethod(OutputAssemblyPath.FilePath, src, New String() {}, New String() {}, True, "CmTestCompiler", "Germany")
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))

            Dim CompiledToDiskAssembly As CompileOnDiskResults = CompileOnDiskResults.LoadFrom(OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello Germany!", CType(CompiledToDiskAssembly.InvokeMainMethod("CmTestCompiler", New Object() {"Germany"}), String))
        End Sub

        <Test()> Public Sub compileCode_with_debug_and_parameters()
            'Some test code
            Dim src As String =
                "Public Class CmTestCompiler" & ControlChars.CrLf &
                "Public Function Answer(country as string) As String " & ControlChars.CrLf &
                "Return ""Hello "" & country & ""!"" " & ControlChars.CrLf &
                "End Function " & ControlChars.CrLf &
                "End Class"

            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")
            Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMethod(OutputAssemblyPath.FilePath, src, New String() {}, New String() {}, True, "CmTestCompiler", "Answer", "Germany")
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))

            Dim CompiledToDiskAssembly As CompileOnDiskResults = CompileOnDiskResults.LoadFrom(OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello Germany!", CType(CompiledToDiskAssembly.Invoke("CmTestCompiler", "Answer", New Object() {"Germany"}), String))
        End Sub

        <Test()> Public Sub compileCode_with_debug_and_parameters_static()
            'Some test code
            Dim src As String =
                "Public  Class CmTestCompiler" & ControlChars.CrLf &
                "Public Shared Function Answer(country as string) As String " & ControlChars.CrLf &
                "Return ""Hello "" & country & ""!"" " & ControlChars.CrLf &
                "End Function " & ControlChars.CrLf &
                "End Class"

            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")
            Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMethod(OutputAssemblyPath.FilePath, src, New String() {}, New String() {}, True, "CmTestCompiler", "Answer", "Germany")
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))

            Dim CompiledToDiskAssembly As CompileOnDiskResults = CompileOnDiskResults.LoadFrom(OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello Germany!", CType(CompiledToDiskAssembly.Invoke("CmTestCompiler", "Answer", New Object() {"Germany"}), String))
        End Sub

        <Test()> Public Sub compileCode_with_namespace_and_debug_and_parameters()
            'Some test code
            Dim src As String =
                "Namespace My.Test.App" & ControlChars.CrLf &
                "Public Class CmTestCompiler" & ControlChars.CrLf &
                "Public Function Answer(country as string) As String " & ControlChars.CrLf &
                "Return ""Hello "" & country & ""!"" " & ControlChars.CrLf &
                "End Function " & ControlChars.CrLf &
                "End Class" & ControlChars.CrLf &
                "End Namespace"

            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")
            Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMethod(OutputAssemblyPath.FilePath, src, New String() {}, New String() {}, True, "My.Test.App.CmTestCompiler", "Answer", "Germany")
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))

            Dim CompiledToDiskAssembly As CompileOnDiskResults = CompileOnDiskResults.LoadFrom(OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello Germany!", CType(CompiledToDiskAssembly.Invoke("My.Test.App.CmTestCompiler", "Answer", New Object() {"Germany"}), String))
        End Sub

        <Test()> Public Sub compileCode_with_namespace_and_debug_and_parameters_static()
            'Some test code
            Dim src As String =
                "Namespace My.Test.App" & ControlChars.CrLf &
                "Public Class CmTestCompiler" & ControlChars.CrLf &
                "Public Shared Function Answer(country as string) As String " & ControlChars.CrLf &
                "Return ""Hello "" & country & ""!"" " & ControlChars.CrLf &
                "End Function " & ControlChars.CrLf &
                "End Class" & ControlChars.CrLf &
                "End Namespace"

            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")
            Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMethod(OutputAssemblyPath.FilePath, src, New String() {}, New String() {}, True, "My.Test.App.CmTestCompiler", "Answer", "Germany")
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))

            Dim CompiledToDiskAssembly As CompileOnDiskResults = CompileOnDiskResults.LoadFrom(OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello Germany!", CType(CompiledToDiskAssembly.Invoke("My.Test.App.CmTestCompiler", "Answer", New Object() {"Germany"}), String))
        End Sub

    End Class

End Namespace
