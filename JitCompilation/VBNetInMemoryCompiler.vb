Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler

'TODO: XML documentation
'TODO: create NUnit Test class for all methods with 0, 1 and 2 parameters for the called method
'TODO: validate proper operation in all .NET framework environments (.NET 1.x up to the highest version)

Namespace CompuMaster.JitCompilation

    Public Class VBNetInMemoryCompiler
        Inherits BaseInMemoryCompiler

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

        Protected Overloads Overrides Function EmbedFunctionIntoClass(ByVal [imports]() As String, ByVal functionCode As String) As String
            'Given is a function code like the following one
            '---------------------------------------
            'Public Function Main(param1 as object)
            '   'some code here...
            '   Return param1
            'End Function
            '---------------------------------------
            Dim ImportCommands As String = Nothing
            For Each importNamespace As String In [imports]
                ImportCommands &= "Imports " & importNamespace & System.Environment.NewLine
            Next
            Return "Option Strict On" & System.Environment.NewLine & System.Environment.NewLine &
                ImportCommands & System.Environment.NewLine &
                "Public Class CompuMasterJitCompileTempClass" & System.Environment.NewLine &
                System.Environment.NewLine &
                functionCode & System.Environment.NewLine &
                System.Environment.NewLine &
                "End Class"
        End Function

        Protected Overrides Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider
            Return New Microsoft.VisualBasic.VBCodeProvider
        End Function

    End Class

End Namespace