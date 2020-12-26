using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using URay;

[CustomEditor(typeof(URayMain))]
public class URayMainEditor : Editor
{ 
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        URayMain uRayMain = (URayMain)target;

        if (GUILayout.Button("Render"))
        {
            uRayMain.Render();
        }
    }
}
