Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler

Namespace CompuMaster.JitCompilation

    Public Class VBNetOnDiskCompiler
        Inherits BaseOnDiskCompiler

        ''' <summary>
        ''' Default set of references for VB.NET
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides ReadOnly Property ReferenceDefaultSet() As Common.ReferenceSets
            Get
                Return Common.ReferenceSets.Minimal
            End Get
        End Property

        ''' <summary>
        ''' Default set of namespace imports for VB.NET
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides ReadOnly Property ImportDefaultSet() As Common.ImportSet
            Get
                Return CompuMaster.JitCompilation.Common.ImportSet.Minimal
            End Get
        End Property

        ''' <summary>
        ''' Embed method code into the code of a class CompuMasterJitCompileTempClass
        ''' </summary>
        ''' <param name="[imports]">Namespaces which shall be imported</param>
        ''' <param name="methodCode">Method code like <code>
        ''' Public Function Main(param1 As Object) As Object
        '''    'some code here...
        '''    Return param1
        ''' End Function</code></param>
        ''' <returns></returns>
        Protected Overrides Function EmbedCodeIntoClass(ByVal [imports]() As String, ByVal methodCode As String) As String
            Return VBNetInMemoryCompiler.EmbedVbNetMethodIntoClass([imports], methodCode)
        End Function

        ''' <summary>
        ''' Create a new instance of the VB.NET code provider
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider
            Return New Microsoft.VisualBasic.VBCodeProvider
        End Function

    End Class

End Namespace