Option Explicit On
Option Strict On

Imports System.CodeDom.Compiler

Namespace CompuMaster.JitCompilation

    <Obsolete("Use VBNetOnDiskCompiler instead", False)>
    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Public Class VBNetOnDiscCompiler
        Inherits BaseOnDiscCompiler

        Protected Overrides ReadOnly Property ReferenceDefaultSet() As Common.ReferenceSets
            Get
                Return Common.ReferenceSets.Minimal
            End Get
        End Property

        Protected Overrides ReadOnly Property ImportDefaultSet() As Common.ImportSet
            Get
                Return CompuMaster.JitCompilation.Common.ImportSet.Minimal
            End Get
        End Property

        Protected Overrides Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider
            Return New Microsoft.VisualBasic.VBCodeProvider
        End Function

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

    End Class

End Namespace