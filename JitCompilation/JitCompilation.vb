Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler

Namespace CompuMaster.JitCompilation

    ''' <summary>
    ''' Compile a piece of source code for execution in memory
    ''' </summary>
    <Obsolete("Use VBNetInMemoryCompiler or CSharpInMemoryCompiler instead", True), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Class InMemoryCompiler

#Region "C#"
        ''' <summary>
        ''' Compile some C# source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <returns></returns>
        Public Function CompileCSharp(ByVal sourceCode As String) As CompileResults
            Return CompileCSharp(sourceCode, False)
        End Function

        ''' <summary>
        ''' Compile some C# source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="debugMode"></param>
        ''' <returns></returns>
        Public Function CompileCSharp(ByVal sourceCode As String, ByVal debugMode As Boolean) As CompileResults
            Return Compile(sourceCode, New Microsoft.CSharp.CSharpCodeProvider, CompuMaster.JitCompilation.Common.TargetType.Library, CompuMaster.JitCompilation.Common.ReferenceDefaults(CompuMaster.JitCompilation.Common.ReferenceSets.Minimal), CompuMaster.JitCompilation.Common.ImportDefaults(CompuMaster.JitCompilation.Common.ImportSet.None), False)
        End Function

        ''' <summary>
        ''' Execute a C# function in memory
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteCSharpFunctionMain(ByVal methodCode As String, ByVal ParamArray parameters As Object()) As Object
            Return CompileCSharpFunction(methodCode).InvokeMainMethod("CompuMasterJitCompileTempClass", parameters)
        End Function

        ''' <summary>
        ''' Compile a C# function in memory
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <returns></returns>
        Public Function CompileCSharpFunction(ByVal methodCode As String) As CompileResults
            'Given is a function code like the following one
            '---------------------------------------
            'object Main(object param1)
            '{
            '   'some code here...
            '   return param1;
            '}
            '---------------------------------------
            Dim MyTempClassCode As String =
                "public class CompuMasterJitCompileTempClass" & System.Environment.NewLine &
                "{" & System.Environment.NewLine &
                methodCode & System.Environment.NewLine &
                "}"

            Return CompileCSharp(MyTempClassCode, False)
        End Function

#End Region

#Region "VB.NET"
        ''' <summary>
        ''' Compile some VB.NET source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <returns></returns>
        Public Function CompileVbNet(ByVal sourceCode As String) As CompileResults
            Return CompileVbNet(sourceCode, False)
        End Function

        ''' <summary>
        ''' Compile some VB.NET source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="debugMode"></param>
        ''' <returns></returns>
        Public Function CompileVbNet(ByVal sourceCode As String, ByVal debugMode As Boolean) As CompileResults
            Return Compile(sourceCode, New Microsoft.VisualBasic.VBCodeProvider, CompuMaster.JitCompilation.Common.TargetType.Library, CompuMaster.JitCompilation.Common.ReferenceDefaults(CompuMaster.JitCompilation.Common.ReferenceSets.Minimal), CompuMaster.JitCompilation.Common.ImportDefaults(CompuMaster.JitCompilation.Common.ImportSet.Minimal), False)
        End Function

        ''' <summary>
        ''' Execute a VB.NET function in memory
        ''' </summary>
        ''' <param name="methodCode">A class implementing a method Main</param>
        ''' <param name="parameters">Optional parameters required for the method Main</param>
        ''' <returns>The result value as is has been created by the method Main</returns>
        Public Function ExecuteVbNetFunctionMain(ByVal methodCode As String, ByVal ParamArray parameters As Object()) As Object
            Return CompileVbNetFunction(methodCode).InvokeMainMethod("CompuMasterJitCompileTempClass", parameters)
        End Function

        ''' <summary>
        ''' Compile a VB.NET function in memory
        ''' </summary>
        ''' <param name="methodCode">Method code like <code>
        ''' Public Function Main(param1 As Object) As Object
        '''    //some code here...
        '''    Return param1
        ''' End Function</code></param>
        ''' <returns></returns>
        Public Function CompileVbNetFunction(ByVal methodCode As String) As CompileResults
            Dim MyTempClassCode As String =
                "Public Class CompuMasterJitCompileTempClass" & System.Environment.NewLine &
                System.Environment.NewLine &
                methodCode & System.Environment.NewLine &
                System.Environment.NewLine &
                "End Class"
            Return CompileVbNet(MyTempClassCode, False)
        End Function

#End Region

        ''' <summary>
        ''' Compile some source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="codeProvider"></param>
        ''' <param name="targetType"></param>
        ''' <param name="references"></param>
        ''' <param name="imports"></param>
        ''' <param name="debugMode"></param>
        ''' <returns></returns>
        Public Function Compile(ByVal sourceCode As String, ByVal codeProvider As System.CodeDom.Compiler.CodeDomProvider, ByVal targetType As CompuMaster.JitCompilation.Common.TargetType, ByVal references As String(), ByVal [imports] As String(), ByVal debugMode As Boolean) As CompileResults
            Return Common.Compile(sourceCode, True, codeProvider, targetType, references, [imports], debugMode, Nothing)
        End Function

    End Class

    <Obsolete("Use VBNetOnDiscCompiler or CSharpOnDiscCompiler instead", True), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Class OnDiscCompiler

#Region "C#"
        ''' <summary>
        ''' Compile some C# source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <returns></returns>
        Public Function CompileCSharp(ByVal sourceCode As String, ByVal outputAssemblyPath As String) As CompileResults
            Return CompileCSharp(sourceCode, False, outputAssemblyPath)
        End Function

        ''' <summary>
        ''' Compile some C# source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="outputAssemblyPath"></param>
        ''' <returns></returns>
        Public Function CompileCSharp(ByVal sourceCode As String, ByVal debugMode As Boolean, ByVal outputAssemblyPath As String) As CompileResults
            Return CompileCSharp(sourceCode, New String() {}, debugMode, outputAssemblyPath)
        End Function

        ''' <summary>
        ''' Compile some C# source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="outputAssemblyPath"></param>
        ''' <returns></returns>
        Public Function CompileCSharp(ByVal sourceCode As String, ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal outputAssemblyPath As String) As CompileResults
            Dim refs As New List(Of String)
            For Each ref As String In CompuMaster.JitCompilation.Common.ReferenceDefaults(CompuMaster.JitCompilation.Common.ReferenceSets.Minimal)
                refs.Add(ref)
            Next
            If Not additionalAssembliesToReference Is Nothing Then
                For Each assemblyPath As String In additionalAssembliesToReference
                    refs.Add(assemblyPath)
                Next
            End If
            Return Compile(sourceCode, New Microsoft.CSharp.CSharpCodeProvider, CompuMaster.JitCompilation.Common.TargetType.Library, refs.ToArray, CompuMaster.JitCompilation.Common.ImportDefaults(CompuMaster.JitCompilation.Common.ImportSet.None), False, outputAssemblyPath)
        End Function

        ''' <summary>
        ''' Execute a C# function in memory
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <param name="outputAssemblyPath"></param>
        ''' <param name="parameters">Optional parameters required for the method Main</param>
        ''' <returns>The result value as is has been created by the method Main</returns>
        Public Function ExecuteCSharpFunctionMain(ByVal methodCode As String, ByVal outputAssemblyPath As String, ByVal ParamArray parameters As Object()) As Object
            Return CompileCSharpFunction(methodCode, outputAssemblyPath).InvokeMainMethod("CompuMasterJitCompileTempClass", parameters)
        End Function

        ''' <summary>
        ''' Compile a C# function in memory
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <param name="outputAssemblyPath"></param>
        ''' <returns></returns>
        Public Function CompileCSharpFunction(ByVal methodCode As String, ByVal outputAssemblyPath As String) As CompileResults
            Dim MyTempClassCode As String = CSharpInMemoryCompiler.EmbedCSharpMethodIntoClass(New String() {}, methodCode)
            Return CompileCSharp(MyTempClassCode, outputAssemblyPath)
        End Function

#End Region

#Region "VB.NET"
        ''' <summary>
        ''' Compile some VB.NET source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <returns></returns>
        Public Function CompileVbNet(ByVal sourceCode As String, ByVal outputAssemblyPath As String) As CompileResults
            Return CompileVbNet(sourceCode, False, outputAssemblyPath)
        End Function

        ''' <summary>
        ''' Compile some VB.NET source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="outputAssemblyPath"></param>
        ''' <returns></returns>
        Public Function CompileVbNet(ByVal sourceCode As String, ByVal debugMode As Boolean, ByVal outputAssemblyPath As String) As CompileResults
            Return CompileVbNet(sourceCode, New String() {}, debugMode, outputAssemblyPath)
        End Function

        ''' <summary>
        ''' Compile some VB.NET source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="outputAssemblyPath"></param>
        ''' <returns></returns>
        Public Function CompileVbNet(ByVal sourceCode As String, ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal outputAssemblyPath As String) As CompileResults
            Dim refs As New List(Of String)
            For Each ref As String In CompuMaster.JitCompilation.Common.ReferenceDefaults(CompuMaster.JitCompilation.Common.ReferenceSets.Minimal)
                refs.Add(ref)
            Next
            If Not additionalAssembliesToReference Is Nothing Then
                For Each assemblyPath As String In additionalAssembliesToReference
                    refs.Add(assemblyPath)
                Next
            End If
            Return Compile(sourceCode, New Microsoft.VisualBasic.VBCodeProvider, CompuMaster.JitCompilation.Common.TargetType.Library, refs.ToArray, CompuMaster.JitCompilation.Common.ImportDefaults(CompuMaster.JitCompilation.Common.ImportSet.Minimal), False, outputAssemblyPath)
        End Function

        ''' <summary>
        ''' Execute a VB.NET function in memory
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <param name="outputAssemblyPath"></param>
        ''' <param name="parameters">Optional parameters required for the method Main</param>
        ''' <returns>The result value as is has been created by the method Main</returns>
        Public Function ExecuteVbNetFunctionMain(ByVal methodCode As String, ByVal outputAssemblyPath As String, ByVal ParamArray parameters As Object()) As Object
            Return CompileVbNetFunction(methodCode, outputAssemblyPath).InvokeMainMethod("CompuMasterJitCompileTempClass", parameters)
        End Function

        Public Function ExecuteVbNetClassWithFunctionMain(ByVal classCode As String, ByVal additionalAssembliesToReference As String(), ByVal outputAssemblyPath As String, ByVal ParamArray parameters As Object()) As Object
            Return CompileVbNetFunction(classCode, outputAssemblyPath).InvokeMainMethod("CompuMasterJitCompileTempClass", parameters)
        End Function

        ''' <summary>
        ''' Compile a VB.NET function in memory
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <param name="outputAssemblyPath"></param>
        ''' <returns></returns>
        Public Function CompileVbNetFunction(ByVal methodCode As String, ByVal outputAssemblyPath As String) As CompileResults
            Dim MyTempClassCode As String = VBNetInMemoryCompiler.EmbedVbNetMethodIntoClass(New String() {}, methodCode)
            Return CompileVbNet(MyTempClassCode, outputAssemblyPath)
        End Function

#End Region

        ''' <summary>
        ''' Compile some source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="codeProvider"></param>
        ''' <param name="targetType"></param>
        ''' <param name="references"></param>
        ''' <param name="imports"></param>
        ''' <param name="debugMode"></param>
        ''' <returns></returns>
        Public Function Compile(ByVal sourceCode As String, ByVal codeProvider As System.CodeDom.Compiler.CodeDomProvider, ByVal targetType As CompuMaster.JitCompilation.Common.TargetType, ByVal references As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal outputAssemblyPath As String) As CompileOnDiskResults
            If outputAssemblyPath = Nothing Then Throw New ArgumentNullException(NameOf(outputAssemblyPath))
            Return CType(Common.Compile(sourceCode, False, codeProvider, targetType, references, [imports], debugMode, outputAssemblyPath), CompileOnDiskResults)
        End Function

    End Class

End Namespace