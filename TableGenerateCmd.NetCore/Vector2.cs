
using System;

public struct Vector2
{
    public double X;
    public double Y;

    public static Vector2 Parse(string str)
    {
        str = str.Replace("(", string.Empty).Replace(")", string.Empty);
        var pair = str.Split(',');
        return new Vector2()
        {
            X = double.Parse( pair[0].Split('=')[1] ),
            Y = double.Parse( pair[1].Split('=')[1] )
        };
    }
}

