#nullable enable
namespace EsriDemo.Core.Navigation;

public class NavigationResult
{
    public bool Success { get; set; }
    public Exception? Exception { get; set; }
    public IDictionary<string, object>? ReturnedParameters { get; set; }
}