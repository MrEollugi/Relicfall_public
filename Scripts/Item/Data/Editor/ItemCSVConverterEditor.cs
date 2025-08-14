#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemDataConverter))]
public class ItemCSVconverterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var converter = target as ItemDataConverter;

        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(converter), typeof(MonoScript), false);

        if (GUILayout.Button("CSV를 JSON으로 변환"))
        {
            converter.ConvertCSVToJSON();
        }
    }
}
#endif