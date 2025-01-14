Option Explicit On
Option Strict On

Imports System.CodeDom.Compiler

Namespace CompuMaster.JitCompilation

    ''' <summary>
    ''' Compile a piece of source code to disk
    ''' </summary>
    Public Class CSharpOnDiskCompiler
        Inherits BaseOnDiskCompiler

        ''' <summary>
        ''' Default set of references for C#
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides ReadOnly Property ReferenceDefaultSet() As Common.ReferenceSets
            Get
                Return Common.ReferenceSets.Minimal
            End Get
        End Property

        ''' <summary>
        ''' Default set of namespace imports for C#
        ''' </summary>
        ''' <returns></returns>
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
        ''' public object Main(object param1)
        ''' {
        '''    //some code here...
        '''    return param1;
        ''' }
        ''' </code></param>
        ''' <returns></returns>
        Protected Overrides Function EmbedCodeIntoClass(ByVal [imports]() As String, ByVal methodCode As String) As String
            Return CSharpInMemoryCompiler.EmbedCSharpMethodIntoClass([imports], methodCode)
        End Function

        ''' <summary>
        ''' Create a new instance of the C# code provider
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider
            Return New Microsoft.CSharp.CSharpCodeProvider
        End Function

    End Class

End Namespace