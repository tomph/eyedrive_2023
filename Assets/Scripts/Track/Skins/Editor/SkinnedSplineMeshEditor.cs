using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SkinnedSplineMesh))]
public class SkinnedSplineMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SkinnedSplineMesh myTarget = (SkinnedSplineMesh)target;

        myTarget.skin = (World)EditorGUILayout.EnumPopup("Skin", myTarget.skin);

        if (GUILayout.Button("Apply Skin"))
        {
            myTarget.ApplySkin();
        }
    }
}