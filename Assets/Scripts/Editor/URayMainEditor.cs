using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using URay;

[CustomEditor(typeof(URay_Main))]
public class URayMainEditor : Editor
{ 
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        URay_Main uRayMain = (URay_Main)target;

        if (GUILayout.Button("Render"))
        {
            uRayMain.Render();
        }
    }
}
