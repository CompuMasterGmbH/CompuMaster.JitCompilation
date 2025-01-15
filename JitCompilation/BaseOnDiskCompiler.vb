Option Explicit On 
Option Strict On

Imports System.CodeDom.Compiler

Namespace CompuMaster.JitCompilation

    ''' <summary>
    ''' Base class for on-disk compilers
    ''' </summary>
    Public MustInherit Class BaseOnDiskCompiler

        ''' <summary>
        ''' Compile a piece of source code to disk
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="outputAssemblyPath"></param>
        ''' <returns></returns>
        Public Function Compile(ByVal sourceCode As String, ByVal debugMode As Boolean, ByVal outputAssemblyPath As String) As CompileOnDiskResults
            Return Compile(sourceCode, New String() {}, debugMode, outputAssemblyPath)
        End Function

        ''' <summary>
        ''' Compile a piece of source code to disk
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="outputAssemblyPath"></param>
        ''' <returns></returns>
        Public Function Compile(ByVal sourceCode As String, ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal outputAssemblyPath As String) As CompileOnDiskResults
            Return Compile(sourceCode, additionalAssembliesToReference, New String() {}, debugMode, outputAssemblyPath, CompuMaster.JitCompilation.Common.TargetType.Library)
        End Function

        ''' <summary>
        ''' Compile a piece of source code to disk
        ''' </summary>
        ''' <param name="sourceCode"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="[imports]"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="outputAssemblyPath"></param>
        ''' <param name="targetType"></param>
        ''' <returns></returns>
        Public Function Compile(ByVal sourceCode As String, ByVal additionalAssembliesToReference As String(), ByVal [imports] As String(), ByVal debugMode As Boolean, ByVal outputAssemblyPath As String, ByVal targetType As CompuMaster.JitCompilation.Common.TargetType) As CompileOnDiskResults
            If outputAssemblyPath = Nothing Then Throw New ArgumentNullException(NameOf(outputAssemblyPath))
            Dim Result = CType(Common.Compile(sourceCode, False, CreateCodeProvider, targetType, Common.AddMinimalSetOfReferences(Me.ReferenceDefaultSet, additionalAssembliesToReference), Common.AddMinimalSetOfImports(Me.ImportDefaultSet, [imports]), debugMode, outputAssemblyPath), CompileOnDiskResults)
            DeployAssemblyReferences(outputAssemblyPath, additionalAssembliesToReference)
            Return Result
        End Function

        ''' <summary>
        ''' Deploy referenced assemblies to the output directory
        ''' </summary>
        ''' <param name="outputAssemblyPath">The path of the output assembly file</param>
        ''' <param name="additionalAssembliesToReference">The referenced assembly files which are required for deployment</param>
        Protected Sub DeployAssemblyReferences(ByVal outputAssemblyPath As String, ByVal additionalAssembliesToReference As String())
            For Each ReferenceFile In additionalAssembliesToReference
                Dim SourceFile As System.IO.FileInfo = New System.IO.FileInfo(ReferenceFile)
                If Not SourceFile.Exists() Then
                    Throw New System.IO.FileNotFoundException("Referenced assembly not found: " & ReferenceFile)
                End If
                Dim TargetFile As New System.IO.FileInfo(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(outputAssemblyPath), System.IO.Path.GetFileName(ReferenceFile)))
                If Not TargetFile.Exists() OrElse SourceFile.LastWriteTimeUtc > TargetFile.LastWriteTimeUtc OrElse SourceFile.Length <> TargetFile.Length Then
                    SourceFile.CopyTo(TargetFile.FullName, True)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Compile code of a class to an assembly and invoke method Main of it 
        ''' </summary>
        ''' <param name="pathToAssemblyWithMainMethod"></param>
        ''' <param name="methodCode"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteMainMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal methodCode As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteMainMethod(pathToAssemblyWithMainMethod, methodCode, New String() {}, New String() {}, False, parameters)
        End Function

        ''' <summary>
        ''' Compile code of a class to an assembly and invoke method Main of it 
        ''' </summary>
        ''' <param name="pathToAssemblyWithMainMethod"></param>
        ''' <param name="methodCode"></param>
        ''' <param name="importNamespaces"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteMainMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal methodCode As String, ByVal importNamespaces As String(), ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteClassWithMainMethod(pathToAssemblyWithMainMethod, EmbedCodeIntoClass(importNamespaces, methodCode), additionalAssembliesToReference, New String() {}, debugMode, "CompuMasterJitCompileTempClass", parameters)
        End Function

        ''' <summary>
        ''' Compile code of a class to an assembly and invoke method Main of it 
        ''' </summary>
        ''' <param name="pathToAssemblyWithMainMethod"></param>
        ''' <param name="classCode"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="importNamespaces"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="instanceName"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteClassWithMainMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal classCode As String, ByVal additionalAssembliesToReference As String(), ByVal importNamespaces As String(), ByVal debugMode As Boolean, ByVal instanceName As String, ByVal ParamArray parameters As Object()) As Object
            Return Compile(classCode, additionalAssembliesToReference, importNamespaces, debugMode, pathToAssemblyWithMainMethod, Common.TargetType.Library).InvokeMainMethod(instanceName, parameters)
        End Function

        ''' <summary>
        ''' Compile code of a class to an assembly and invoke a method of it 
        ''' </summary>
        ''' <param name="pathToAssemblyWithMainMethod"></param>
        ''' <param name="methodCode"></param>
        ''' <param name="methodName"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal methodCode As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteMethod(pathToAssemblyWithMainMethod, methodCode, New String() {}, New String() {}, False, methodName, parameters)
        End Function

        ''' <summary>
        ''' Compile code of a class to an assembly and invoke a method of it 
        ''' </summary>
        ''' <param name="pathToAssemblyWithMainMethod"></param>
        ''' <param name="methodCode"></param>
        ''' <param name="importNamespaces"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="methodName"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal methodCode As String, ByVal importNamespaces As String(), ByVal additionalAssembliesToReference As String(), ByVal debugMode As Boolean, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return ExecuteClassWithMethod(pathToAssemblyWithMainMethod, EmbedCodeIntoClass(importNamespaces, methodCode), additionalAssembliesToReference, New String() {}, debugMode, "CompuMasterJitCompileTempClass", methodName, parameters)
        End Function

        ''' <summary>
        ''' Compile code of a class to an assembly and invoke a method of it 
        ''' </summary>
        ''' <param name="pathToAssemblyWithMainMethod"></param>
        ''' <param name="classCode"></param>
        ''' <param name="additionalAssembliesToReference"></param>
        ''' <param name="importNamespaces"></param>
        ''' <param name="debugMode"></param>
        ''' <param name="instanceName"></param>
        ''' <param name="methodName"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteClassWithMethod(ByVal pathToAssemblyWithMainMethod As String, ByVal classCode As String, ByVal additionalAssembliesToReference As String(), ByVal importNamespaces As String(), ByVal debugMode As Boolean, ByVal instanceName As String, ByVal methodName As String, ByVal ParamArray parameters As Object()) As Object
            Return Compile(classCode, additionalAssembliesToReference, importNamespaces, debugMode, pathToAssemblyWithMainMethod, Common.TargetType.Library).Invoke(instanceName, methodName, parameters)
        End Function

        ''' <summary>
        ''' Embeds method/property/enum/field code into a new class CompuMasterJitCompileTempClass to make it compilable
        ''' </summary>
        ''' <param name="methodCode"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Protected MustOverride Function EmbedCodeIntoClass(ByVal [imports] As String(), ByVal methodCode As String) As String

        ''' <summary>
        ''' Default set of references
        ''' </summary>
        ''' <returns></returns>
        Protected MustOverride ReadOnly Property ReferenceDefaultSet() As CompuMaster.JitCompilation.Common.ReferenceSets

        ''' <summary>
        ''' Default set of namespace importNamespaces
        ''' </summary>
        ''' <returns></returns>
        Protected MustOverride ReadOnly Property ImportDefaultSet() As CompuMaster.JitCompilation.Common.ImportSet

        ''' <summary>
        ''' Create a new instance of the code provider
        ''' </summary>
        ''' <returns></returns>
        Protected MustOverride Function CreateCodeProvider() As System.CodeDom.Compiler.CodeDomProvider

    End Class

End Namespace