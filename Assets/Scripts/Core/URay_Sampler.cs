using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class URay_Sampler
{
    public static System.Random rand = new System.Random();
    public static double UniformNumber()
    {
        return rand.NextDouble();
    }

    public static Vector3d UniformSphere()
    {
        double theta = 2 * Math.PI * UniformNumber();
        double phi = Math.Acos(1 - 2f * UniformNumber());

        return new Vector3d((Math.Sin(phi) * Math.Cos(theta)), (Math.Sin(phi) * Math.Sin(theta)), (Math.Cos(phi)));
    }

    public static Vector3d UniformVector3()
    {
        return new Vector3d(UniformNumber(), UniformNumber(), UniformNumber());
    }

    public static Vector3d UniformHemisphere()
    {
        double theta = 2 * Math.PI * UniformNumber();
        double phi = Math.Acos(1 - UniformNumber());

        return new Vector3d(Math.Sin(phi) * Math.Cos(theta), Math.Sin(phi) * Math.Sin(theta), Math.Cos(phi));
    }
}
