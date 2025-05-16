using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melodroid_2.MusicUtils;

public static class Utils
{
    //Thanks stack overflow https://stackoverflow.com/questions/147515/least-common-multiple-for-3-or-more-numbers/29717490#29717490
    public static long LCM(long[] numbers)
    {
        long LcmResult = numbers.Aggregate(lcm);
        if (LcmResult > short.MaxValue)
            throw new ArgumentException($"Lcm result {LcmResult} is larger than short.MaxValue: This exceeds maximum midi time division");
        return LcmResult;
    }
    public static long lcm(long a, long b)
    {
        return Math.Abs(a * b) / GCD(a, b);
    }
    public static long GCD(long a, long b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }
}
