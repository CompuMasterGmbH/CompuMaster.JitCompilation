Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler

''' <summary>
''' Provides methods to quickly and easily compile and execute source code
''' </summary>
Namespace CompuMaster.JitCompilation

    ''' <summary>
    ''' Common types and enumerations for CompuMaster.JitCompilation
    ''' </summary>
    Public Class Common

        ''' <summary>
        ''' Default references for different scenarios
        ''' </summary>
        Public Enum ReferenceSets As Byte
            ''' <summary>
            ''' No automatic references at all
            ''' </summary>
            None = 0
            ''' <summary>
            ''' Minimal set of references for basic .NET programming, for .NET Framework e.g. System.dll, System.Xml.dll, System.Data.dll
            ''' </summary>
            Minimal = 1
            ''' <summary>
            ''' Typical set of references for web programming, for .NET Framework e.g. including System.Web.dll
            ''' </summary>
            Web = 2
        End Enum

        ''' <summary>
        ''' Default references for different scenarios
        ''' </summary>
        ''' <param name="referenceSet"></param>
        ''' <returns></returns>
        Public Shared ReadOnly Property ReferenceDefaults(ByVal referenceSet As ReferenceSets) As String()
            Get
#If NETFRAMEWORK Then
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
#Else
                'TODO: check if this is correct for .NET Core
                Select Case referenceSet
                    Case ReferenceSets.None
                        Return New String() {}
                    Case ReferenceSets.Minimal
                        Return New String() {}
                        'Return New String() {"system.dll", "system.xml.dll", "system.data.dll"}
                    Case ReferenceSets.Web
                        Return New String() {}
                        'Return New String() {"system.dll", "system.xml.dll", "system.data.dll", "system.web.dll"}
                    Case Else
                        Throw New ArgumentException("Invalid value", "referenceSet")
                End Select
#End If
            End Get
        End Property

        ''' <summary>
        ''' Default namespace imports for different scenarios
        ''' </summary>
        Public Enum ImportSet As Byte
            ''' <summary>
            ''' No automatic namespace imports at all
            ''' </summary>
            None = 0
            ''' <summary>
            ''' Minimal set of namespace imports for basic .NET programming, for .NET Framework e.g. System, System.Collections, System.Data
            ''' </summary>
            Minimal = 1
            ''' <summary>
            ''' Typical set of namespace imports for web programming, for .NET Framework e.g. including System.Web
            ''' </summary>
            Web = 2
            ''' <summary>
            ''' Minimal set of namespace imports for basic VisualBasic programming, for .NET Framework e.g. Microsoft.VisualBasic, System, System.Collections, System.Data
            ''' </summary>
            VisualBasic = 3
            ''' <summary>
            ''' Typical set of namespace imports for web programming in VisualBasic, for .NET Framework e.g. including Microsoft.VisualBasic, System, System.Collections, System.Data, System.Web
            ''' </summary>
            VisualBasicWeb = 4
        End Enum

        ''' <summary>
        ''' Default namespace imports for different scenarios
        ''' </summary>
        ''' <param name="importsSet"></param>
        ''' <returns></returns>
        Public Shared ReadOnly Property ImportDefaults(ByVal importsSet As ImportSet) As String()
            Get
#If NETFRAMEWORK Then
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
#Else
                'Auto-Imports in .NET Core are not in the same way as in .NET Framework
                'TODO: check if this is correct for .NET Core
                Select Case importsSet
                    Case ImportSet.None
                        Return New String() {}
                    Case ImportSet.Minimal
                        Return New String() {"System", "System.Collections"}
                    Case ImportSet.Web
                        Return New String() {"System", "System.Collections", "System.Web"}
                    Case ImportSet.VisualBasic
                        Return New String() {"Microsoft.VisualBasic", "System", "System.Collections"}
                    Case ImportSet.VisualBasicWeb
                        Return New String() {"Microsoft.VisualBasic", "System", "System.Collections", "System.Web"}
                    Case Else
                        Throw New ArgumentException("Invalid value", "importsSet")
                End Select
#End If
            End Get
        End Property

        ''' <summary>
        ''' Compilation target types
        ''' </summary>
        ''' <remarks>See https://learn.microsoft.com/en-us/dotnet/visual-basic/reference/command-line-compiler/target for more information</remarks>
        Public Enum TargetType As Byte
            ''' <summary>
            ''' Causes the compiler to create a dynamic-link library (.dll)
            ''' </summary>
            Library = 0
            ''' <summary>
            ''' Causes the compiler to create an executable console application (.exe)
            ''' </summary>
            ConsoleApplication = 1
            ''' <summary>
            ''' Causes the compiler to create an executable Windows-based application
            ''' </summary>
            ''' <remarks>
            ''' The executable file is created with an .exe extension. A Windows-based application is one that provides a user interface from either the .NET Framework class library or with the Windows APIs.
            ''' </remarks>
            WindowsApplication = 2
            ''' <summary>
            ''' Causes the compiler to generate a module that can be added to an assembly (usually a .netmodule file)
            ''' </summary>
            ''' <remarks>
            ''' <para>The output file is created with an extension of .netmodule.</para>
            ''' <para>The .NET common language runtime cannot load a file that does Not have an assembly. However, you can incorporate such a file into the assembly manifest Of an assembly by Using -reference.</para>
            ''' <para>When code in one module references internal types in another module, both modules must be incorporated into an assembly manifest by using -reference.</para>
            ''' <para>The -addmodule option imports metadata from a module.</para>
            ''' </remarks>
            NetModule = 3
            ''' <summary>
            ''' Causes the compiler to create an executable Windows-based application that must be run in an app container. This setting is designed to be used for Windows 8.x Store applications.
            ''' </summary>
            ''' <remarks>
            ''' The appcontainerexe setting sets a bit in the Characteristics field of the Portable Executable file. This bit indicates that the app must be run in an app container. When this bit is set, an error occurs if the CreateProcess method tries to launch the application outside of an app container. Aside from this bit setting, -target:appcontainerexe is equivalent to -target:winexe.
            ''' </remarks>
            AppContainerExe = 4
            ''' <summary>
            ''' Causes the compiler to create an intermediate file that you can convert to a Windows Runtime binary (.winmd) file. The .winmd file can be consumed by JavaScript and C++ programs, in addition to managed language programs.            
            ''' </summary>
            ''' <remarks>
            ''' <para>The intermediate file Is created With a .winmdobj extension.</para>
            ''' <para>The .winmdobj file is designed to be used as input for the WinMDExp export tool to produce a Windows metadata (WinMD) file. The WinMD file has a .winmd extension and contains both the code from the original library and the WinMD definitions that JavaScript, C++, and the Windows Runtime use.</para>
            ''' </remarks>
            WinMDObj = 5
        End Enum

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
#Disable Warning BC40000 ' Typ oder Element ist veraltet
            Dim oICCompiler As ICodeCompiler = codeProvider.CreateCompiler
#Enable Warning BC40000 ' Typ oder Element ist veraltet

            Dim oCParams As CompilerParameters = New CompilerParameters
            Dim oCResults As CompilerResults

            ' Setup the Compiler Parameters  
            ' Add any referenced assemblies
            For MyCounter As Integer = 0 To references.Length - 1
                If references(MyCounter) <> Nothing Then oCParams.ReferencedAssemblies.Add(references(MyCounter))
            Next
            Select Case targetType
                Case TargetType.NetModule
                    oCParams.CompilerOptions = "/t:module"
                Case TargetType.ConsoleApplication
                    oCParams.CompilerOptions = "/t:exe"
                Case TargetType.WindowsApplication
                    oCParams.CompilerOptions = "/t:winexe"
                Case TargetType.AppContainerExe
                    oCParams.CompilerOptions = "/t:appcontainerexe"
                Case TargetType.WinMDObj
                    oCParams.CompilerOptions = "/t:winmdobj"
                Case TargetType.Library
                    oCParams.CompilerOptions = "/t:library"
                Case Else
                    Throw New NotImplementedException("Target type not implemented: " & targetType.ToString)
            End Select
            If debugMode Then
                oCParams.CompilerOptions &= " /debug"
            End If
            Dim importsParameter As String = String.Join(",", [imports])
            If importsParameter <> Nothing AndAlso importsParameter.Trim <> Nothing Then
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
            oCResults = oICCompiler.CompileAssemblyFromSource(oCParams, sourceCode)

            ' Check for compile time errors 
            If oCResults.Errors.Count <> 0 Then
                Throw New CompileException(oCResults, sourceCode, hideSourceCodeFilePathsInExceptionMessages)
            Else
                'Workaround for oCResults.CompiledAssembly: assembly sometimes with unexpected auto-extension
                If System.IO.Path.GetFileName(oCResults.PathToAssembly) <> System.IO.Path.GetFileName(outputAssemblyPath) Then
                    'Compiler build successfully, but into another output file (or it thinks that)
                    'e.g. ConsoleExe: compiler result references "k5sbejen.hdb.exe.dll", but in fact, compiler created "k5sbejen.hdb.exe" (as requested)
                    If System.IO.File.Exists(oCResults.PathToAssembly) Then
                        'Compiler result is correct
                        oCResults.Output.Add("POST-COMPILATION WARNING: Output assembly requested and expected by compiler at """ & outputAssemblyPath & """, but found at """ & oCResults.PathToAssembly & """")
                    ElseIf System.IO.File.Exists(outputAssemblyPath) Then
                        'Requested file name is correct
                        oCResults.Output.Add("POST-COMPILATION WARNING: Output assembly requested and found at """ & outputAssemblyPath & """, but expected by compiler at """ & oCResults.PathToAssembly & """")
                        oCResults.PathToAssembly = outputAssemblyPath
                    Else
                        'None of the expected files can be found!
                        Throw New System.IO.FileNotFoundException("Output assembly missing: " & outputAssemblyPath, outputAssemblyPath)
                    End If
                End If

                ' No errors on compile, at maximum a few warnings, so continue to process...
                Select Case targetType
                    Case TargetType.NetModule
                        If outputAssemblyPath = Nothing Then
                            Return New CompileResults(oCResults.CompiledAssembly, oCResults)
                        Else
                            Return New CompileOnDiskResults(outputAssemblyPath, oCResults)
                        End If
                    Case TargetType.Library, TargetType.ConsoleApplication, TargetType.WindowsApplication, TargetType.AppContainerExe, TargetType.WinMDObj
                        If outputAssemblyPath = Nothing Then
                            Return New CompileResults(oCResults.CompiledAssembly, oCResults)
                        Else
                            Return New CompileOnDiskResults(oCResults.CompiledAssembly, outputAssemblyPath, oCResults)
                        End If
                    Case Else
                        Throw New NotImplementedException("Target type not implemented: " & targetType.ToString)
                End Select
            End If

        End Function


        Public Shared Function AddMinimalSetOfReferences(ByVal baseSetOfReferences As ReferenceSets, ByVal additionalAssembliesToReference() As String) As String()
            Dim refs As New List(Of String)
            For Each ref As String In CompuMaster.JitCompilation.Common.ReferenceDefaults(baseSetOfReferences)
                refs.Add(ref)
            Next
            If Not additionalAssembliesToReference Is Nothing Then
                For Each assemblyPath As String In additionalAssembliesToReference
                    refs.Add(assemblyPath)
                Next
            End If
            Return refs.ToArray
        End Function

        Public Shared Function AddMinimalSetOfImports(ByVal baseSetOfImports As ImportSet, ByVal additionalImports() As String) As String()
            Dim [imports] As New List(Of String)
            For Each import As String In CompuMaster.JitCompilation.Common.ImportDefaults(baseSetOfImports)
                [imports].Add(import)
            Next
            If Not additionalImports Is Nothing Then
                For Each import As String In additionalImports
                    [imports].Add(import)
                Next
            End If
            Return [imports].ToArray
        End Function

    End Class

End Namespace