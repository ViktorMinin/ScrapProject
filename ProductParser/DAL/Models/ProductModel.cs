using SimMetrics.Net.Metric;
using SimMetricsApi;
using Levenstein = SimMetricsMetricUtilities.Levenstein;

namespace ProductParser.DAL.Models;

public class ProductModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string? ImageUrl { get; set; }
    public string MarketPlace { get; set; }
    public string? Category { get; set; }
}

public class StringDistanceComparer : IEqualityComparer<string>
{
    private double maxDistance;

    public StringDistanceComparer(double max)
    {
        maxDistance = max;
    }

    public bool Equals(string x, string y)
    {
        double distance = new Levenstein().GetSimilarity(x, y);
        return distance <= maxDistance;
    }

    public int GetHashCode(string obj)
    {
        return obj.GetHashCode();
    }
}