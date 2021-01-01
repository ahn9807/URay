using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URay;

[ExecuteInEditMode]
public class URay_TestAcceleration : MonoBehaviour
{
    public Vector3 origin;
    public Vector3 direction;
    public URay_Acceleration accerlerationStructure;
    public Bounds bounds;
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

    // Start is called before the first frame update
    void Start()
    {
        accerlerationStructure = new URay_Acceleration();
        accerlerationStructure.InitAccelerationStructure(Callback, bounds);
    }

    void Callback()
    {
        sw.Stop();
        Debug.Log("Initializing accerleratinoStrcture Elasped Time: " + sw.ElapsedMilliseconds / 1000f + " seconds");
    }

    public void BuildRaycastStructure()
    {
        sw.Start();
        accerlerationStructure = new URay_Acceleration();
        accerlerationStructure.InitAccelerationStructure(Callback, bounds);
    }

    public void Raycast()
    {
        sw.Restart();
        URay_Intersection phit, hit;
        phit = new URay_Intersection();
        hit = new URay_Intersection();
        for(int i=0;i<1000;i++)
        {
            URay_Raycast.PhysicsRaycast(origin, direction, out phit);
        }
        sw.Stop();
        Debug.Log("Unity Engine Result: " + phit.point + "[" + (sw.ElapsedMilliseconds / 1000f).ToString("F14") + "] seconds");
        sw.Restart();
        for(int i=0;i<1000;i++)
        {
            URay_Raycast.Raycast(origin, direction, out hit);
        }
        sw.Stop();
        Debug.Log("Octree search Result: " + hit.point + "[" + (sw.ElapsedMilliseconds / 1000f).ToString("F14") + "] seconds");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Ray(origin, direction));
    }
}
