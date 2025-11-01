namespace EsriDemo.Features.Base;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class AnalyticsPageAttribute : Attribute
{
    public string PageName { get; }

    public AnalyticsPageAttribute(string pageName)
    {
        PageName = pageName;
    }
}