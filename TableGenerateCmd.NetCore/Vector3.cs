
using System;

public struct Vector3
{
    public double X;
    public double Y;
    public double Z;

    public static Vector3 Parse(string str)
    {
        str = str.Replace("(", string.Empty).Replace(")", string.Empty);
        var pair = str.Split(',');
        return new Vector3()
        {
            X = double.Parse( pair[0].Split('=')[1] ),
            Y = double.Parse( pair[1].Split('=')[1] ),
            Z = double.Parse( pair[2].Split('=')[1] )
        };
    }
}

