Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler

'TODO: XML documentation
'TODO: create NUnit Test class for all methods with 0, 1 and 2 parameters for the called method
'TODO: validate proper operation in all .NET framework environments (.NET 1.x up to the highest version)

Namespace CompuMaster.JitCompilation

    ''' <summary>
    ''' Compile a piece of source code for execution in memory
    ''' </summary>
    Public Class CSharpInMemoryCompiler
        Inherits BaseInMemoryCompiler

        Protected Overrides ReadOnly Property ReferenceDefaultSet() As Common.ReferenceSets
            Get
                Return Common.ReferenceSets.Minimal
            End Get
        End Property

        Protected Overrides ReadOnly Property ImportDefaultSet() As Common.ImportSet
            Get
                Return CompuMaster.JitCompilation.Common.ImportSet.None
            End Get
        End Property

        ''' <summary>
        ''' Embed method code into the code of a class CompuMasterJitCompileTempClass
        ''' </summary>
        ''' <param name="[imports]">Namespaces which shall be imported</param>
        ''' <param name="methodCode">Method code like <code>
        ''' object Main(object param1)
        ''' {
        '''    //some code here...
        '''    return param1;
        ''' }
        ''' </code></param>
        ''' <returns></returns>
        Protected Overloads Overrides Function EmbedCodeIntoClass(ByVal [imports]() As String, ByVal methodCode As String) As String
            Return EmbedCSharpMethodIntoClass([imports], methodCode)
        End Function

        ''' <summary>
        ''' Embed method code into the code of a class CompuMasterJitCompileTempClass
        ''' </summary>
        ''' <param name="[imports]">Namespaces which shall be imported</param>
        ''' <param name="methodCode">Method code like <code>
        ''' object Main(object param1)
        ''' {
        '''    //some code here...
        '''    return param1;
        ''' }
        ''' </code></param>
        ''' <returns></returns>
        Friend Shared Function EmbedCSharpMethodIntoClass(ByVal [imports]() As String, ByVal methodCode As String) As String
            Dim ImportCommands As String = ""
            For Each importNamespace As String In [imports]
                ImportCommands &= "using " & importNamespace & System.Environment.NewLine
            Next
            Return ImportCommands & System.Environment.NewLine &
                 "public class CompuMasterJitCompileTempClass" & System.Environment.NewLine &
                 "{" & System.Environment.NewLine &
                 methodCode & System.Environment.NewLine &
                 "}"
        End Function

        Protected Overrides Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider
            Return New Microsoft.CSharp.CSharpCodeProvider
        End Function

    End Class

End Namespace