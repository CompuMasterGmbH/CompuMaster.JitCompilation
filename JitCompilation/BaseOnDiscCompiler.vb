Option Explicit On 
Option Strict On

Namespace CompuMaster.JitCompilation

    <Obsolete("Use BaseOnDiskCompiler instead", False)>
    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Public MustInherit Class BaseOnDiscCompiler
        Inherits BaseOnDiskCompiler

    End Class

End Namespace