using CountryRiskAI.API.Models.DTOs;

namespace CountryRiskAI.API.Helpers;

public static class StatsCalculator
{
    public static IndicatorSeriesStats CalculateStats(List<EconomicIndicatorDto> series)
    {
        var validData = series
            .Where(x => x.Value.HasValue)
            .OrderBy(x => x.Year)
            .ToList();

        if (validData.Count < 5)
        {
            return new IndicatorSeriesStats
            {
                Average = 0,
                Last = null,
                Trend = 0,
                StdDev = 0,
                YearsUsed = validData.Count
            };
        }

        var values = validData.Select(x => x.Value!.Value).ToList();
        var average = values.Average();
        var last = values.Last();
        var stdDev = CalculateStandardDeviation(values);

        // Trend: promedio últimos 3 - promedio primeros 3
        var firstThree = values.Take(3).Average();
        var lastThree = values.TakeLast(3).Average();
        var trend = lastThree - firstThree;

        return new IndicatorSeriesStats
        {
            Average = average,
            Last = last,
            Trend = trend,
            StdDev = stdDev,
            YearsUsed = validData.Count
        };
    }

    private static double CalculateStandardDeviation(List<double> values)
    {
        if (values.Count < 2)
        {
            return 0;
        }

        var avg = values.Average();
        var sumSquaredDiffs = values.Sum(x => Math.Pow(x - avg, 2));
        return Math.Sqrt(sumSquaredDiffs / values.Count);
    }
}
