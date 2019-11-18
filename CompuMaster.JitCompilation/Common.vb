Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler

'TODO: XML documentation
'TODO: create NUnit Test class for all methods with 0, 1 and 2 parameters for the called method
'TODO: validate proper operation in all .NET framework environments (.NET 1.x up to the highest version)

Namespace CompuMaster.JitCompilation

    ''' -----------------------------------------------------------------------------
    ''' Project	 : CompuMaster.JitCompilation
    ''' Class	 : JitCompilation.Common
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Common types and enumerations for CompuMaster.JitCompilation
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[wezel]	23.01.2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Common

        Public Enum ReferenceSets As Byte
            None = 0
            Minimal = 1
            Web = 2
        End Enum
        Public Shared ReadOnly Property ReferenceDefaults(ByVal referenceSet As ReferenceSets) As String()
            Get
                Select Case referenceSet
                    Case ReferenceSets.None
                        Return New String() {}
                    Case ReferenceSets.Minimal
                        Return New String() {"system.dll", "system.xml.dll", "system.data.dll"}
                    Case ReferenceSets.Web
                        Return New String() {"system.dll", "system.xml.dll", "system.data.dll", "system.web.dll"}
                    Case Else
                        Throw New ArgumentException("Invalid value", "referenceSet")
                End Select
            End Get
        End Property

        Public Enum ImportSet As Byte
            None = 0
            Minimal = 1
            Web = 2
            VisualBasic = 3
            VisualBasicWeb = 4
        End Enum
        Public Shared ReadOnly Property ImportDefaults(ByVal importsSet As ImportSet) As String()
            Get
                Select Case importsSet
                    Case ImportSet.None
                        Return New String() {}
                    Case ImportSet.Minimal
                        Return New String() {"System", "System.Collections", "System.Data"}
                    Case ImportSet.Web
                        Return New String() {"System", "System.Collections", "System.Data", "System.Web"}
                    Case ImportSet.VisualBasic
                        Return New String() {"Microsoft.VisualBasic", "System", "System.Collections", "System.Data"}
                    Case ImportSet.VisualBasicWeb
                        Return New String() {"Microsoft.VisualBasic", "System", "System.Collections", "System.Data", "System.Web"}
                    Case Else
                        Throw New ArgumentException("Invalid value", "importsSet")
                End Select
            End Get
        End Property

        Public Enum TargetType As Byte
            Library = 0
            ConsoleApplication = 1
            WindowsApplication = 2
            [Module] = 3
        End Enum

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Compile some source code in memory
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="codeProvider"></param>
        ''' <param name="targetType"></param>
        ''' <param name="references"></param>
        ''' <param name="imports"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="outputAssemblyPath">An output path for the created assembly or null (Nothing in VisualBasic) to compile in-memory</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	    23.01.2008	Created
        '''     [zeutzheim] 08.07.2009  Modified
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function Compile(ByVal sourceCode As String, ByVal hideSourceCodeFilePathsInExceptionMessages As Boolean, ByVal codeProvider As System.CodeDom.Compiler.CodeDomProvider, ByVal targetType As CompuMaster.JitCompilation.Common.TargetType, ByVal references As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal outputAssemblyPath As String) As CompileResults

            'Parameter validation
            If sourceCode Is Nothing Then
                Throw New ArgumentNullException("sourceCode")
            End If
            If codeProvider Is Nothing Then
                Throw New ArgumentNullException("codeProvider")
            End If
            If references Is Nothing Then
                Throw New ArgumentNullException("references")
            End If
            If [imports] Is Nothing Then
                Throw New ArgumentNullException("imports")
            End If

            ' Obsolete in 2.0 framework
            Dim oICCompiler As ICodeCompiler = codeProvider.CreateCompiler

            Dim oCParams As CompilerParameters = New CompilerParameters
            Dim oCResults As CompilerResults

            ' Setup the Compiler Parameters  
            ' Add any referenced assemblies
            For MyCounter As Integer = 0 To references.Length - 1
                If references(MyCounter) <> Nothing Then oCParams.ReferencedAssemblies.Add(references(MyCounter))
            Next
            Select Case targetType
                Case targetType.Module
                    oCParams.CompilerOptions = "/t:module"
                Case targetType.ConsoleApplication
                    oCParams.CompilerOptions = "/t:exe"
                Case targetType.WindowsApplication
                    oCParams.CompilerOptions = "/t:winexe"
                Case Else
                    oCParams.CompilerOptions = "/t:library"
            End Select
            If debugMode Then
                oCParams.CompilerOptions &= " /debug"
            End If
            Dim importsParameter As String = Strings.Join([imports], ",")
            If Trim(importsParameter) <> Nothing Then
                oCParams.CompilerOptions &= " /imports:" & importsParameter
            End If
            oCParams.IncludeDebugInformation = debugMode

            If outputAssemblyPath = Nothing Then
                oCParams.GenerateInMemory = True
            Else
                oCParams.GenerateInMemory = False
                oCParams.OutputAssembly = outputAssemblyPath
            End If

            ' Compile and get results 
            ' 2.0 Framework - Method called from Code Provider
            'oCResults = oCodeProvider.CompileAssemblyFromSource(oCParams, vbCode)
            ' 1.1 Framework - Method called from CodeCompiler Interface
            oCResults = oICCompiler.CompileAssemblyFromSource(oCParams, sourceCode)

            ' Check for compile time errors 
            Dim Result As CompileResults
            If oCResults.Errors.Count <> 0 Then
                Throw New CompileException(oCResults, sourceCode, hideSourceCodeFilePathsInExceptionMessages)
            Else
                ' No Errors On Compile, at maximum a few Warnings, so continue to process...
                Result = New CompileResults(oCResults.CompiledAssembly, Nothing)
            End If

            Return Result

        End Function

        Public Shared Function AddMinimalSetOfReferences(ByVal baseSetOfReferences As ReferenceSets, ByVal additionalAssembliesToReference() As String) As String()
            Dim refs As New ArrayList
            For Each ref As String In CompuMaster.JitCompilation.Common.ReferenceDefaults(baseSetOfReferences)
                refs.Add(ref)
            Next
            If Not additionalAssembliesToReference Is Nothing Then
                For Each assemblyPath As String In additionalAssembliesToReference
                    refs.Add(assemblyPath)
                Next
            End If
            Return CType(refs.ToArray(GetType(String)), String())
        End Function

        Public Shared Function AddMinimalSetOfImports(ByVal baseSetOfImports As ImportSet, ByVal additionalImports() As String) As String()
            Dim [imports] As New ArrayList
            For Each import As String In CompuMaster.JitCompilation.Common.ImportDefaults(baseSetOfImports)
                [imports].Add(import)
            Next
            If Not additionalImports Is Nothing Then
                For Each import As String In additionalImports
                    [imports].Add(import)
                Next
            End If
            Return CType([imports].ToArray(GetType(String)), String())
        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Provides methods to quickly and easily compile and execute source code
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	04/10/2004	Created
    ''' 	[adminwezel]	2006-02-10  Latest patch
    ''' </history>
    ''' <copyright>CompuMaster GmbH</copyright>
    ''' -----------------------------------------------------------------------------
    Friend Class NamespaceDoc
    End Class

End Namespace