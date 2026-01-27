using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomMonoBehaviour), true)]
public class CustomMonobehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CustomMonoBehaviour pc = target as CustomMonoBehaviour;
        
        if (GUILayout.Button("Editor Init"))
        {
            pc.EditorInit();
        }
        base.OnInspectorGUI();
    }
}