Imports NUnit.Framework
Imports CompuMaster.JitCompilation
Namespace CompuMaster.Tests.JitCompilation
    Public Module ConsoleTestApp
        Public Sub Main()
            System.Console.WriteLine("Test Init: Main C#")
            Dim csc As New CompuMaster.Tests.JitCompilation.CSharpInMemoryCompiler
            csc.compileCode_without_debug()

            System.Console.WriteLine()

            System.Console.WriteLine("Test Init: Main VB.NET")
            Dim vbc As New CompuMaster.Tests.JitCompilation.VBInMemoryCompiler
            vbc.compileCode_without_debug()
        End Sub
    End Module

End Namespace
