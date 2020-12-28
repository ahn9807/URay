using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class URay_Sampler
{
    public static System.Random rand = new System.Random();
    public static float UniformNumber()
    {
        return (float)rand.NextDouble();
    }

    public static Vector3 UniformSphere()
    {
        double theta = 2 * Math.PI * UniformNumber();
        double phi = Math.Acos(1 - 2f * UniformNumber());

        return new Vector3((float)(Math.Sin(phi) * Math.Cos(theta)), (float)(Math.Sin(phi) * Math.Sin(theta)), (float)(Math.Cos(phi)));
    }

    public static Vector3 UniformVector3()
    {
        return new Vector3(UniformNumber(), UniformNumber(), UniformNumber());
    }

    public static Vector3 UniformHemisphere()
    {
        double theta = 2 * Math.PI * UniformNumber();
        double phi = Math.Acos(1 - UniformNumber());

        return new Vector3((float)(Math.Sin(phi) * Math.Cos(theta)), (float)(Math.Cos(phi)), (float)(Math.Sin(phi) * Math.Sin(theta))); 
    }

    public static Vector2 UniformDisk()
    {
        float sample1 = UniformNumber();
        float sample2 = UniformNumber();
        return new Vector2((float)Math.Sqrt(sample1) * (float)Math.Cos(2 * (float)Math.PI * sample2), (float)Math.Sqrt(sample1) * (float)Math.Sin(2 * (float)Math.PI * sample2));
    }

    public static Vector3 CosineHemisphere()
    {
        Vector2 d = UniformDisk();
        float z = (float)Math.Sqrt((float)Math.Max(0, 1 - d.x * d.x - d.y * d.y));
        return new Vector3(d.x, z, d.y);
    }
}
