using Fractions;
namespace Melodroid_2.MusicUtils;

public static class MusicTheory
{
    // Conventional Intervals
    public static readonly Fraction Unison = new Fraction(1);
    public static readonly Fraction MinorSecond = new Fraction(16, 15);
    public static readonly Fraction MajorSecond = new Fraction(9, 8);
    public static readonly Fraction MinorThird = new Fraction(6, 5);
    public static readonly Fraction MajorThird = new Fraction(5, 4);
    public static readonly Fraction PerfectFourth = new Fraction(4, 3);
    public static readonly Fraction PerfectFifth = new Fraction(3, 2);
    public static readonly Fraction MinorSixth = new Fraction(8, 5);
    public static readonly Fraction MajorSixth = new Fraction(5, 3);
    public static readonly Fraction MinorSeventh = new Fraction(9, 5);
    public static readonly Fraction MajorSeventh = new Fraction(15, 8);

    // Common fraction sets
    public static readonly List<Fraction> TET12Fractions = [
        Unison,
        MinorSecond,
        MajorSecond,
        MinorThird,
        MajorThird,
        PerfectFourth,
        PerfectFifth,
        MinorSixth,
        MajorSixth,
        MinorSeventh,
        MajorSeventh
    ];
    public static readonly List<Fraction> MajorChordFractions = [
        Unison,
        PerfectFifth,
        MajorThird
        ];

    // Just-Noticeable-Difference: https://en.wikipedia.org/wiki/Just-noticeable_difference
    public const double JNDApproximateRelativeFrequency = 0.0045; // 0.45%, midway between 500 hz and 1k hz range - JND is frequency dependent
}