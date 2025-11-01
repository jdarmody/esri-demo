namespace EsriDemo.Core.Api;

public class RequestMetrics
{
    public string Uri { get; set; }
    public List<TimeSpan> TimeSpans { get; set; }
        
    public RequestMetrics(string uri)
    {
        Uri = uri;
        TimeSpans = new List<TimeSpan>();
    }
        
    public void Add(TimeSpan timeSpan)
    {
        TimeSpans.Add(timeSpan);
    }

    public TimeSpan Average => TimeSpan.FromMilliseconds(TimeSpans.Average(t => t.TotalMilliseconds));

    public override string ToString()
    {
        return $"{Average} [Count: {TimeSpans.Count}]";
    }
}