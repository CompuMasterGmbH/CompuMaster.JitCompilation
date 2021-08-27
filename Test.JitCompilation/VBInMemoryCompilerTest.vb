Imports NUnit.Framework
Imports CompuMaster.JitCompilation
Imports CompuMaster.VisualBasicCompatibility

Namespace CompuMaster.Tests.JitCompilation
    <TestFixture()> Public Class VBInMemoryCompiler

        Private myTestCompiler As CompuMaster.JitCompilation.BaseInMemoryCompiler = New CompuMaster.JitCompilation.VBNetInMemoryCompiler

        <Test()> Public Sub compileCode_with_debug()
            'Some test code
            Dim src As String = "Public Class CmTestCompiler" & ControlChars.CrLf &
            "Public Function Answer() As String " & ControlChars.CrLf &
            "Return ""Hello World!"" " & ControlChars.CrLf &
            "End Function " & ControlChars.CrLf &
            "End Class"

            Dim cResult As CompileResults = myTestCompiler.Compile(src, True)

            Assert.IsEmpty(cResult.CompilerErrors)
            Assert.AreEqual("Hello World!", CType(cResult.Invoke("CmTestCompiler", "Answer", Nothing), String))

        End Sub

        <Test()> Public Sub compileCode_without_debug()
            'Some test code
            Dim src As String = "Public Class CmTestCompiler" & ControlChars.CrLf &
            "Public Function Answer() As String " & ControlChars.CrLf &
            "Return ""Hello World!"" " & ControlChars.CrLf &
            "End Function " & ControlChars.CrLf &
            "End Class"

            Dim cResult As CompileResults = myTestCompiler.Compile(src, False)

            Assert.IsEmpty(cResult.CompilerErrors)
            System.Console.WriteLine("OUTPUT of JIT-compiled method: ")
            System.Console.WriteLine(CType(cResult.Invoke("CmTestCompiler", "Answer", Nothing), String))
            Assert.AreEqual("Hello World!", CType(cResult.Invoke("CmTestCompiler", "Answer", Nothing), String))

        End Sub

        <Test()> Public Sub compileCode_with_debug_and_parameters_mainmethod()
            'Some test code
            Dim src As String = "Public Class CmTestCompiler" & ControlChars.CrLf &
            "Public Function Main(country as string) As String " & ControlChars.CrLf &
            "Return ""Hello "" & country & ""!"" " & ControlChars.CrLf &
            "End Function " & ControlChars.CrLf &
            "End Class"

            Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMainMethod(src, New String() {}, New String() {}, True, "CmTestCompiler", "Germany")
            Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))

        End Sub

        <Test()> Public Sub compileCode_with_debug_and_parameters()
            'Some test code
            Dim src As String = "Public Class CmTestCompiler" & ControlChars.CrLf &
            "Public Function Answer(country as string) As String " & ControlChars.CrLf &
            "Return ""Hello "" & country & ""!"" " & ControlChars.CrLf &
            "End Function " & ControlChars.CrLf &
            "End Class"

            Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMethod(src, New String() {}, New String() {}, True, "CmTestCompiler", "Answer", "Germany")
            Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))

        End Sub

        <Test()> Public Sub compileCode_with_debug_and_parameters_static()
            'Some test code
            Dim src As String = "Public  Class CmTestCompiler" & ControlChars.CrLf &
            "Public Shared Function Answer(country as string) As String " & ControlChars.CrLf &
            "Return ""Hello "" & country & ""!"" " & ControlChars.CrLf &
            "End Function " & ControlChars.CrLf &
            "End Class"

            Dim ExecutionResult As Object = myTestCompiler.ExecuteClassWithMethod(src, New String() {}, New String() {}, True, "CmTestCompiler", "Answer", "Germany")
            Assert.AreEqual("Hello Germany!", CType(ExecutionResult, String))

        End Sub

    End Class

End Namespace
