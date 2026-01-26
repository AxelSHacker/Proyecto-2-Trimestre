using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerControler))]
public class CustomMonobehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerControler pc = target as PlayerControler;
        if (GUILayout.Button("Editor Init"))
        {
            pc.EditorInit();
        }
        base.OnInspectorGUI();
    }
}