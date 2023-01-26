using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

[CustomEditor(typeof(WorldButton))]
public class UISegmentedControlButtonEditor : UnityEditor.UI.ButtonEditor
{
  public override void OnInspectorGUI()
    {
        WorldButton component = (WorldButton)target;

        base.OnInspectorGUI();

        component.world = (World)EditorGUILayout.EnumPopup("World", component.world);

        component.lockImage = (Image)EditorGUILayout.ObjectField("lockImage",component.lockImage, typeof(Image), allowSceneObjects:true);
    }
}