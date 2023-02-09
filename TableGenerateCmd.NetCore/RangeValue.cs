namespace TableGenerate;

public class RangeValue
{
    public double Min;
    public double Max;
    public static readonly RangeValue s_default = new RangeValue();

    private RangeValue()
    {
        Min = 0;
        Max = 0;
    }
}
