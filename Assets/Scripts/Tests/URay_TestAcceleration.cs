using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URay;

[ExecuteInEditMode]
public class URay_TestAcceleration : MonoBehaviour
{
    public Vector3 origin;
    public Vector3 direction;
    public URay_OctreeAcceleration accerlerationStructure;
    public URay_LinearAcceleration linearAcc;
    public Bounds bounds;
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

    // Start is called before the first frame update
    void Start()
    {
        accerlerationStructure = new URay_OctreeAcceleration();
        accerlerationStructure.InitAccelerationStructure(Callback, bounds);
        linearAcc = new URay_LinearAcceleration();
    }

    void Callback()
    {
        sw.Stop();
        Debug.Log("Initializing accerleratinoStrcture Elasped Time: " + sw.ElapsedMilliseconds / 1000f + " seconds");
    }

    public void BuildRaycastStructure()
    {
        sw.Start();
        accerlerationStructure = new URay_OctreeAcceleration();
        accerlerationStructure.InitAccelerationStructure(Callback, bounds);
        linearAcc = new URay_LinearAcceleration();
        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if(go != null && go.GetComponent<MeshFilter>() != null)
            {
                linearAcc.AddMesh(go.GetComponent<MeshFilter>().sharedMesh);
            }
        }
    }

    public void Raycast()
    {
        sw.Restart();
        URay_Intersection phit, hit;
        phit = new URay_Intersection();
        hit = new URay_Intersection();
        for(int i=0;i<1000;i++)
        {
            URay_OctreeRaycast.PhysicsRaycast(origin, direction, out phit);
        }
        sw.Stop();
        Debug.Log("Unity Engine Result: " + phit.point + " Normal: " + phit.normal + "[" + (sw.ElapsedMilliseconds / 1000f).ToString("F4") + "] seconds");
        sw.Restart();
        for(int i=0;i<1000;i++)
        {
            URay_OctreeRaycast.Raycast(origin, direction, out hit);
        }
        sw.Stop();
        Debug.Log("Octree search Result: Point:  " + hit.point + " Normal: " + hit.normal + "[" + (sw.ElapsedMilliseconds / 1000f).ToString("F4") + "] seconds");
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            linearAcc.RayIntersect(new URay_Ray(origin, direction), out hit, false);
        }
        sw.Stop();
        Debug.Log("Octree search Result: Point:  " + hit.point + " Normal: " + hit.normal + "[" + (sw.ElapsedMilliseconds / 1000f).ToString("F4") + "] seconds");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Ray(origin, direction));
    }
}
