using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

[CustomEditor(typeof(ModeButton))]
public class ModeButtonEditor : UnityEditor.UI.ButtonEditor
{
    public override void OnInspectorGUI()
    {

        ModeButton component = (ModeButton)target;

        base.OnInspectorGUI();

        component.mode = (GameType)EditorGUILayout.EnumPopup("Mode", component.mode);
    }
}