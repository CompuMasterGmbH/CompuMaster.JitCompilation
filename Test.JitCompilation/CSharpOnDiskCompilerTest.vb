Imports NUnit.Framework
Imports CompuMaster.JitCompilation
Imports CompuMaster.VisualBasicCompatibility
Imports System.CodeDom

Namespace CompuMaster.Tests.JitCompilation
    <TestFixture()> Public Class CSharpOnDiskCompilerTest

        Private myTestCompiler As CompuMaster.JitCompilation.BaseOnDiskCompiler = New CompuMaster.JitCompilation.CSharpOnDiskCompiler

        <OneTimeTearDown> Public Sub FullTempFilesCleanup()
            CompuMaster.IO.TemporaryFile.CleanupOnApplicationExit()
        End Sub

        'Sub test()
        '
        'End Sub
        '
        <Test()> Public Sub compileCode_with_debug()
            'Some test code
            Dim src As String =
                "public class CmTestCompiler {" & ControlChars.CrLf &
                "public static string Answer() {" & ControlChars.CrLf &
                "return ""Hello World!"";" & ControlChars.CrLf &
                "}" & ControlChars.CrLf &
                "}"

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
                "public class CmTestCompiler {" & ControlChars.CrLf &
                "public static string Answer() {" & ControlChars.CrLf &
                "return ""Hello World!"";" & ControlChars.CrLf &
                "}" & ControlChars.CrLf &
                "}"

            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")
            Dim cResult As CompileOnDiskResults = myTestCompiler.Compile(src, False, OutputAssemblyPath.FilePath)

            Assert.IsEmpty(cResult.CompilerErrors)
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello World!", CType(cResult.Invoke("CmTestCompiler", "Answer", Nothing), String))

            Dim CompiledToDiskAssembly As CompileOnDiskResults = CompileOnDiskResults.LoadFrom(OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello World!", CType(CompiledToDiskAssembly.Invoke("CmTestCompiler", "Answer", Nothing), String))
        End Sub

        <Test()> Public Sub compileCode_with_debug_and_parameters_mainmethod()
            'Some test code
            Dim src As String =
                "public class CmTestCompiler {" & ControlChars.CrLf &
                "public string Main(string country) {" & ControlChars.CrLf &
                "return ""Hello "" + country + ""!"";" & ControlChars.CrLf &
                "}" & ControlChars.CrLf &
                "}"

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
                "public class CmTestCompiler {" & ControlChars.CrLf &
                "public string Answer(string country) {" & ControlChars.CrLf &
                "return ""Hello "" + country + ""!"";" & ControlChars.CrLf &
                "}" & ControlChars.CrLf &
                "}"

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
                "public class CmTestCompiler {" & ControlChars.CrLf &
                "public static string Answer(string country) {" & ControlChars.CrLf &
                "return ""Hello "" + country + ""!"";" & ControlChars.CrLf &
                "}" & ControlChars.CrLf &
                "}"

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
                "namespace My.Test.App {" & ControlChars.CrLf &
                "public class CmTestCompiler {" & ControlChars.CrLf &
                "public string Answer(string country) {" & ControlChars.CrLf &
                "return ""Hello "" + country + ""!"";" & ControlChars.CrLf &
                "}" & ControlChars.CrLf &
                "}" & ControlChars.CrLf &
                "}"

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
                "namespace My.Test.App {" & ControlChars.CrLf &
                "public class CmTestCompiler {" & ControlChars.CrLf &
                "public static string Answer(string country) {" & ControlChars.CrLf &
                "return ""Hello "" + country + ""!"";" & ControlChars.CrLf &
                "}" & ControlChars.CrLf &
                "}" & ControlChars.CrLf &
                "}"

            Dim OutputAssemblyPath As New CompuMaster.IO.TemporaryFile(".dll")
            Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMethod(OutputAssemblyPath.FilePath, src, New String() {}, New String() {}, True, "My.Test.App.CmTestCompiler", "Answer", "Germany")
            Assert.That(System.IO.File.Exists(OutputAssemblyPath.FilePath), "Output assembly file not found: " & OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))

            Dim CompiledToDiskAssembly As CompileOnDiskResults = CompileOnDiskResults.LoadFrom(OutputAssemblyPath.FilePath)
            Assert.AreEqual("Hello Germany!", CType(CompiledToDiskAssembly.Invoke("My.Test.App.CmTestCompiler", "Answer", New Object() {"Germany"}), String))
        End Sub

    End Class

End Namespace