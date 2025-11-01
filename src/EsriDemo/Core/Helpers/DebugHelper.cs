using System.Diagnostics.CodeAnalysis;

namespace EsriDemo.Core.Helpers;

[ExcludeFromCodeCoverage]
public static class DebugHelper
{
    public static bool IsDebug()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}