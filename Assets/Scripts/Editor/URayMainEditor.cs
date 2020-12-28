using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using URay;
using System.IO;

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

        if(GUILayout.Button("Save to PNG"))
        {
            Texture2D texture = (Texture2D)uRayMain.screenImage.texture;
            byte[] bytes = texture.EncodeToPNG();
            string path = EditorUtility.SaveFilePanel(
                "Save png", "", "result.png", "png"
            );
            if(path.Length == 0)
            {
                return;
            }

            File.WriteAllBytes(path, bytes);
        }
    }
}
