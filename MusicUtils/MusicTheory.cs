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
    //public static readonly Fraction Tritone = new Fraction(7, 5);
    public static readonly double Tritone = 1.41421356237; //sqrt(2)
    public static readonly Fraction PerfectFifth = new Fraction(3, 2);
    public static readonly Fraction MinorSixth = new Fraction(8, 5);
    public static readonly Fraction MajorSixth = new Fraction(5, 3);
    public static readonly Fraction MinorSeventh = new Fraction(9, 5);
    public static readonly Fraction MajorSeventh = new Fraction(15, 8);
    public static readonly Fraction Octave = new Fraction(2);

    // Complete Good Fractions
    public static readonly Fraction SubMajorSecond = new Fraction(10, 9);
    public static readonly Fraction SubMinorSeventh = new Fraction(16, 9);

    // Common fraction sets
    public static readonly List<double> TET12Fractions = [
        (double)Unison,
        (double)MinorSecond,
        (double)MajorSecond,
        (double)MinorThird,
        (double)MajorThird,
        (double)PerfectFourth,
        Tritone,
        (double)PerfectFifth,
        (double)MinorSixth,
        (double)MajorSixth,
        (double)MinorSeventh,
        (double)MajorSeventh
    ];

    public static readonly List<Fraction> GoodFractions = [
        Unison,
        MinorSecond,
        SubMajorSecond,
        MajorSecond,
        MinorThird,
        MajorThird,
        PerfectFourth,
        PerfectFifth,
        MinorSixth,
        MajorSixth,
        SubMinorSeventh,
        MinorSeventh,
        MajorSeventh,
        Octave
    ];
    
    public static readonly List<Fraction> GoodFractionsNo9 = [
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
        MajorSeventh,
        Octave
    ];

    // Minor seventh
    public static readonly List<Fraction> MinorSeventhChord = [
        Unison,
        MajorThird,
        PerfectFifth,
        MinorSeventh
       ];

    // A major ninth
    public static readonly List<Fraction> LCM8 = [
        Unison,
        MajorSecond,
        MajorThird,
        PerfectFifth,
        MajorSeventh
       ];

    // Almost a minor eleventh at 12-TET note A#
    public static readonly List<Fraction> LCM15 = [
        Unison,
        MinorSecond,
        MinorThird,
        PerfectFourth,
        MinorSixth,
        MajorSixth,
        MinorSeventh
       ];
    public static readonly List<Fraction> LCM20 = [
        Unison,        
        MinorThird,
        MajorThird,
        PerfectFifth,
        MinorSixth,        
        MinorSeventh
       ];
    
    // A.K.A. C Major Scale A.K.A. G13
    public static readonly List<Fraction> LCM24 = [
        Unison,
        MajorSecond,
        MajorThird,
        PerfectFourth,
        PerfectFifth,
        MajorSixth,
        MajorSeventh
       ];

    // LCM24 but played as perfect fourths - C# Major scale
    public static readonly List<double> LCM24PerfectFourths = new List<double>() {
            (double)Unison,
            (double)PerfectFourth,
            (double)MinorSeventh,
            (double)MinorThird,
            (double)MinorSixth,
            (double)MinorSecond,
            Tritone};
    
    // LCM24 but played as perfect fifths - G Major scale
    public static readonly List<double> LCM24PerfectFifths = new List<double>() {
            (double)Unison,
            (double)PerfectFifth,
            (double)MajorSecond,
            (double)MajorSixth,
            (double)MajorThird,
            (double)MajorSeventh,
            Tritone};

    // Subset of 15@4 minus 5/3 relative @4, Subset of 15@11,
    public static readonly List<double> PentatonicScale = new List<double>() {
            (double)Unison,
            (double)MajorSecond,
            (double)MajorThird,
            (double)PerfectFifth,
            (double)MajorSixth,
            };



    // A C minor eleventh - almost full LCM 15 at 12-TET note D (Missing fraction 5/3 relative note D)
    public static readonly List<Fraction> MinorEleventhChord = [
        Unison,
        MajorSecond,
        MinorThird,
        PerfectFourth,
        PerfectFifth,
        MinorSeventh
       ];
    
    public static readonly List<Fraction> MajorChordFractions = [
        Unison,
        MajorThird,
        PerfectFifth
        ];
    public static readonly List<Fraction> MinorChordFractions = [
        Unison,
        MinorThird,
        PerfectFifth
        ];
    public static readonly List<Fraction> Sus2ChordFractions = [
        Unison,
        MajorSecond,
        PerfectFifth
        ];
    public static readonly List<double> DimChord = [
        (double)Unison,
        (double)MinorThird,
        Tritone
        ];
    public static readonly List<double> DimChord7 = [
        (double)Unison,
        (double)MinorThird,
        Tritone,
        (double)MajorSixth
        ];
    public static readonly List<double> AugmentedChord = [
        (double)Unison,
        (double)MajorThird,
        (double)MinorSixth
        ];

    // Just-Noticeable-Difference: https://en.wikipedia.org/wiki/Just-noticeable_difference
    public const double JNDApproximateRelativeFrequency = 0.0045; // 0.45%, midway between 500 hz and 1k hz range - JND is frequency dependent

    // Maximum Bin Radius - the maximum bin radius which causes overlap between the good fractions - half of the width 81/80 - 1
    public const double MaximumBinRadius = (81.0 / 80.0 - 1) / 2.0;
}