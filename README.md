# CompuMaster.JitCompilation

Compile and execute C# or VB.NET code easily on runtime

## C# Sample
```csharp
CompuMaster.JitCompilation.BaseInMemoryCompiler cscInMemory = 
    new CompuMaster.JitCompilation.CSharpInMemoryCompiler();
string src = "public class TestClass {public static string Answer() {return \"Hello World!\";}}";
CompuMaster.JitCompilation.CompileResults cResult = cscInMemory.Compile(src, false);

// will result: "Hello World!"
System.Console.WriteLine((string)cResult.Invoke("TestClass", "Answer", null)); 
```
