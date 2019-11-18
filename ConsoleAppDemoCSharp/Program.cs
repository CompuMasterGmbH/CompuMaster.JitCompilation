using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppDemoCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            CompileWithSimple(args);
        }

        static void CompileWithSimple(string[] args)
        {
            CompuMaster.JitCompilation.BaseInMemoryCompiler cscInMemory = 
                new CompuMaster.JitCompilation.CSharpInMemoryCompiler();
            string src = "public class TestClass {public static string Answer() {return \"Hello World!\";}}";
            CompuMaster.JitCompilation.CompileResults cResult = cscInMemory.Compile(src, false);
            System.Console.WriteLine((string)cResult.Invoke("TestClass", "Answer", null)); // will result: "Hello World!"
        }

        static void CompileWithSuccess(string[] args)
        {
            CompuMaster.JitCompilation.BaseInMemoryCompiler cscInMemory = new CompuMaster.JitCompilation.CSharpInMemoryCompiler();
            string src = "public class CmTestCompiler {public static string Answer() {return \"Hello World!\";}}";
            CompuMaster.JitCompilation.CompileResults cResult = cscInMemory.Compile(src, false);
            if ((cResult.CompilerErrors != null) && (cResult.CompilerErrors.HasErrors == false))
            {
                System.Console.WriteLine("Compiled successfully, this is the result:");
                System.Console.WriteLine((string)cResult.Invoke("CmTestCompiler", "Answer", null)); // will result: "Hello World!"
            }
            else
            {
                System.Console.WriteLine("Compilation failed:");
                foreach (System.CodeDom.Compiler.CompilerError cError in cResult.CompilerErrors)
                {
                    System.Console.WriteLine(cError.ToString());
                }
            }
        }

        static void CompileWithError(string[] args)
        {
            CompuMaster.JitCompilation.BaseInMemoryCompiler cscInMemory = new CompuMaster.JitCompilation.CSharpInMemoryCompiler();
            string src = "public class CmTestCompiler {public static string Answer() {return \"\"Hello World!\";}}";
            CompuMaster.JitCompilation.CompileResults cResult = cscInMemory.Compile(src, false);
            if ((cResult.CompilerErrors != null) && (cResult.CompilerErrors.HasErrors == false))
            {
                System.Console.WriteLine("Compiled successfully, this is the result:");
                System.Console.WriteLine((string)cResult.Invoke("CmTestCompiler", "Answer", null)); // will result: "Hello World!"
            }
            else
            {
                System.Console.WriteLine("Compilation failed:");
                foreach (System.CodeDom.Compiler.CompilerError cError in cResult.CompilerErrors)
                {
                    System.Console.WriteLine(cError.ToString());
                }
            }
        }
    }
}
