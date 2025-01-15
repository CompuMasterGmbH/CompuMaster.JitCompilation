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
            System.Console.WriteLine("# CompuMaster.JitCompilation samples");

            System.Console.WriteLine();
            System.Console.WriteLine("## Sample: " + nameof(CompileSampleSimpleInMemory));
            CompileSampleSimpleInMemory(args);

            System.Console.WriteLine();
            System.Console.WriteLine("## Sample: " + nameof(CompileSampleVbNetInMemory));
            CompileSampleVbNetInMemory(args);

            System.Console.WriteLine();
            System.Console.WriteLine("## Sample: " + nameof(CompileSampleConsoleApp));
            CompileSampleConsoleApp(args);

            System.Console.WriteLine();
            System.Console.WriteLine("## Sample: " + nameof(CompileWithSuccess));
            CompileWithSuccess(args);

            System.Console.WriteLine();
            System.Console.WriteLine("## Sample: " + nameof(CompileWithError));
            CompileWithError(args);
        }

        /// <summary>
        /// Simple demonstration of how to compile C# code and running a method from the compiled assembly
        /// </summary>
        /// <param name="args"></param>
        static void CompileSampleSimpleInMemory(string[] args)
        {
            string src = "public class TestClass {public static string Answer() {return \"Hello World from inside of a dynamically created assembly based on C# code!\";}}";
            var cscInMemory = new CompuMaster.JitCompilation.CSharpInMemoryCompiler();
            var cResult = cscInMemory.Compile(src, false);
            System.Console.WriteLine((string)cResult.Invoke("TestClass", "Answer", null)); // result: "Hello World from inside of a dynamically created assembly based on C# code!"
        }

        /// <summary>
        /// Simple demonstration of how to compile C# code and running a method from the compiled assembly
        /// </summary>
        /// <param name="args"></param>
        static void CompileSampleVbNetInMemory(string[] args)
        {
            string src = 
                "Public Class TestClass\r\n" +
                "   Public Shared Function Answer() As String\r\n" +
                "       Return \"Hello World from inside of a dynamically created assembly based on VB.NET code!\"\r\n" +
                "   End Function\r\n" +
                "End Class";
            var cscInMemory = new CompuMaster.JitCompilation.VBNetInMemoryCompiler();
            var cResult = cscInMemory.Compile(src, false);
            System.Console.WriteLine((string)cResult.Invoke("TestClass", "Answer", null)); // result: "Hello World from inside of a dynamically created assembly based on VB.NET code!"
        }

        /// <summary>
        /// Demonstration of how to compile C# code into an .exe console application assembly and running a method from this assembly by various ways
        /// </summary>
        /// <param name="args"></param>
        static void CompileSampleConsoleApp(string[] args)
        {
            string src = 
                "public class TestClass {" +
                "   public static void Main(string[] args) {" +
                "       System.Console.WriteLine(\"Running and printing from inside of console application\"); " +
                "       foreach (string greeting in args)" +
                "           System.Console.WriteLine(\"Hello \" + greeting + \"!\"); " +
                "       System.Console.WriteLine(Answer()); " +
                "   } " +
                "   public static string Answer() {" +
                "       return \"Hello World from inside of a dynamically created assembly!\";" +
                "   }" +
                "}";
            string tempFilePath = System.IO.Path.GetTempFileName() + ".exe";
            var cscOnDisk = new CompuMaster.JitCompilation.CSharpOnDiskCompiler();
            var cResult = cscOnDisk.Compile(src, Array.Empty<string>(), Array.Empty<string>(), false, tempFilePath, CompuMaster.JitCompilation.Common.TargetType.ConsoleApplication);
            System.Console.WriteLine("Compiled into assembly at location: " + cResult.Assembly.Location + " with name " + cResult.Assembly.FullName);
            System.Console.WriteLine();
            System.Console.WriteLine("### Invoking method from compiled assembly:");
            System.Console.WriteLine((string)cResult.Invoke("TestClass", "Answer", null)); // result: "Hello World from inside of a dynamically created assembly!"
            System.Console.WriteLine();
            System.Console.WriteLine("### Running compiled console application with redirection of standard output:");
            var consoleArgs = new string[] { "Community", "Earth" };
            System.Console.WriteLine(cResult.ExecuteConsoleApp(consoleArgs).StandardOutput);
            System.Console.WriteLine();
            System.Console.WriteLine("### Running compiled console application via System.Diagnostics.Process.Start:");
            var appProc = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFilePath, string.Join(" ", consoleArgs)) { UseShellExecute = false }); // run compiled console application
            appProc.WaitForExit();
        }

        /// <summary>
        /// Demonstrates how to compile C# code with a check for successful compilation and running a method from the compiled assembly
        /// </summary>
        /// <param name="args"></param>
        static void CompileWithSuccess(string[] args)
        {
            // compilable C# code class with a method returning a string
            string src = "public class CmTestCompiler {public static string Answer() {return \"Hello World from inside of a dynamically created assembly!\";}}";
            
            CompuMaster.JitCompilation.BaseInMemoryCompiler cscInMemory = new CompuMaster.JitCompilation.CSharpInMemoryCompiler();
            CompuMaster.JitCompilation.CompileResults cResult = cscInMemory.Compile(src, false);
            if ((cResult.CompilerErrors != null) && (cResult.CompilerErrors.HasErrors == false))
            {
                System.Console.WriteLine("Compiled successfully, this is the result:");
                System.Console.WriteLine((string)cResult.Invoke("CmTestCompiler", "Answer", null)); // result: "Hello World from inside of a dynamically created assembly!"
            }
            else
            {
                //Output of the compiler
                foreach (string info in cResult.CompilerResults.Output)
                {
                    System.Console.WriteLine("INFO: " + info.ToString());
                }
                // Output of the compiler errors
                System.Console.WriteLine("Compilation failed:");
                foreach (System.CodeDom.Compiler.CompilerError cError in cResult.CompilerErrors)
                {
                    System.Console.WriteLine(cError.ToString());
                }
            }
        }

        /// <summary>
        /// This method demonstrates compilation errors and shows output and errors from compiler
        /// </summary>
        /// <param name="args"></param>
        static void CompileWithError(string[] args)
        {
            // syntax error in following code: accidentially 2 double quotes for the beginning of a string
            string src = "public class CmTestCompiler {public static string Answer() {return \"\"Hello World from inside of a dynamically created assembly!\";}}"; 

            CompuMaster.JitCompilation.BaseInMemoryCompiler cscInMemory = new CompuMaster.JitCompilation.CSharpInMemoryCompiler();
            CompuMaster.JitCompilation.CompileResults cResult = cscInMemory.Compile(src, false);
            if ((cResult.CompilerErrors != null) && (cResult.CompilerErrors.HasErrors == false))
            {
                System.Console.WriteLine("Compiled successfully, this is the result:");
                System.Console.WriteLine((string)cResult.Invoke("CmTestCompiler", "Answer", null)); // result: "Hello World from inside of a dynamically created assembly!"
            }
            else
            {
                //Output of the compiler
                foreach (string info in cResult.CompilerResults.Output)
                {
                    System.Console.WriteLine("INFO: " + info.ToString());
                }
                // Output of the compiler errors
                System.Console.WriteLine("Compilation failed:");
                foreach (System.CodeDom.Compiler.CompilerError cError in cResult.CompilerErrors)
                {
                    System.Console.WriteLine(cError.ToString());
                }
            }
        }
    }
}
