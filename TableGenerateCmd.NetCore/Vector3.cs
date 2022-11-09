
using System;

public struct Vector3
{
    public double X;
    public double Y;
    public double Z;

    public static Vector3 Parse(string str)
    {
        var array = str.Split(',');
        return new Vector3() { X = double.Parse(array[0]), Y = double.Parse(array[1]), Z = double.Parse(array[2]) };
    }
}

