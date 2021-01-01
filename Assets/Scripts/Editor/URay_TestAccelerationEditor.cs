using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using URay;

[CustomEditor(typeof(URay_TestAcceleration))]
public class URay_TestAccelerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        URay_TestAcceleration uRayMain = (URay_TestAcceleration)target;

        if (GUILayout.Button("Build Structure"))
        {
            uRayMain.BuildRaycastStructure();
        }

        if (GUILayout.Button("Ray cast"))
        {
            uRayMain.Raycast();
        }
    }
}
