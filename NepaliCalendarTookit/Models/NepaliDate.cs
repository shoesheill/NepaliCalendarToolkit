public class NepaliDate
{
    public int Year { get; set; }

    public int Month { get; set; }
    public int Day { get; set; }

    public override string ToString()
    {
        return $"{Year}-{Month:D2}-{Day:D2}";
    }
}