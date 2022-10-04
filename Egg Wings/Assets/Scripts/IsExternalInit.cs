using System.ComponentModel;
namespace System.Runtime.CompilerServices
{
    // This is needed to get init for records to work correctly
    // https://stackoverflow.com/questions/73100829/compile-error-when-using-record-types-with-unity3d
    // https://stackoverflow.com/questions/62648189/testing-c-sharp-9-0-in-vs2019-cs0518-isexternalinit-is-not-defined-or-imported/62656145#62656145
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}