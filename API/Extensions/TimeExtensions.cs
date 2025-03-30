using System;

namespace API.Extensions;

public static class TimeExtensions
{
    public static DateTime ToDateTimeFromUnix(this long unixTime)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Console.WriteLine(unixTime);
        return epoch.AddSeconds(unixTime);
    }
}
