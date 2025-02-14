Imports NUnit.Framework
Imports CompuMaster.JitCompilation
Imports CompuMaster.VisualBasicCompatibility

Namespace CompuMaster.Tests.JitCompilation
    <TestFixture()> Public Class CSharpInMemoryCompiler

        Private myTestCompiler As CompuMaster.JitCompilation.BaseInMemoryCompiler = New CompuMaster.JitCompilation.CSharpInMemoryCompiler

        <Test()> Public Sub compileCode_with_debug()
            'Some test code
            Dim src As String = "public class CmTestCompiler {" & ControlChars.CrLf &
                "public static string Answer() {" & ControlChars.CrLf &
                "return ""Hello World!"";" & ControlChars.CrLf &
                "}" & ControlChars.CrLf &
                "}"

            Dim cResult As CompileResults = myTestCompiler.Compile(src, True)

            Assert.IsEmpty(cResult.CompilerErrors)
            Assert.AreEqual("Hello World!", CType(cResult.Invoke("CmTestCompiler", "Answer", Nothing), String))

        End Sub

        <Test()> Public Sub compileCode_without_debug()
            'Some test code
            Dim src As String = "public class CmTestCompiler {" & ControlChars.CrLf &
                "public static string Answer() {" & ControlChars.CrLf &
                "return ""Hello World!"";" & ControlChars.CrLf &
                "}" & ControlChars.CrLf &
                "}"

            Dim cResult As CompileResults = myTestCompiler.Compile(src, False)

            Assert.IsEmpty(cResult.CompilerErrors)
            Assert.AreEqual("Hello World!", CType(cResult.Invoke("CmTestCompiler", "Answer", Nothing), String))

        End Sub

        <Test()> Public Sub compileCode_with_debug_and_parameters_mainmethod()
            'Some test code
            Dim src As String = "public class CmTestCompiler {" & ControlChars.CrLf &
                    "public string Main(string country) {" & ControlChars.CrLf &
                    "return ""Hello "" + country + ""!"";" & ControlChars.CrLf &
                    "}" & ControlChars.CrLf &
                    "}"

            Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMainMethod(src, New String() {}, New String() {}, True, "CmTestCompiler", "Germany")
            Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))

        End Sub

        <Test()> Public Sub compileCode_with_debug_and_parameters()
            'Some test code
            Dim src As String = "public class CmTestCompiler {" & ControlChars.CrLf &
                    "public string Answer(string country) {" & ControlChars.CrLf &
                    "return ""Hello "" + country + ""!"";" & ControlChars.CrLf &
                    "}" & ControlChars.CrLf &
                    "}"

            Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMethod(src, New String() {}, New String() {}, True, "CmTestCompiler", "Answer", "Germany")
            Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))

        End Sub

        <Test()> Public Sub compileCode_with_debug_and_parameters_static()
            'Some test code
            Dim src As String = "public class CmTestCompiler {" & ControlChars.CrLf &
                    "public static string Answer(string country) {" & ControlChars.CrLf &
                    "return ""Hello "" + country + ""!"";" & ControlChars.CrLf &
                    "}" & ControlChars.CrLf &
                    "}"

            Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMethod(src, New String() {}, New String() {}, True, "CmTestCompiler", "Answer", "Germany")
            Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))

        End Sub

    End Class

End Namespace