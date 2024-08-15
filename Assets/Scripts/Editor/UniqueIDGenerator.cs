using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RuneDataNew))]
public class UniqueIDGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ScriptableObject item = (ScriptableObject)target;

        if (GUILayout.Button("Generate Unique ID"))
        {
            string uniqueID = GenerateCustomID();
            SerializedProperty idProperty = serializedObject.FindProperty("id");
            idProperty.stringValue = uniqueID;
            serializedObject.ApplyModifiedProperties();
        }
    }

    private string GenerateCustomID()
    {
        string prefix = "Rune";
        string part1 = Random.Range(10000, 99999).ToString(); // Generate a random 5-digit number
        string part2 = Random.Range(10000, 99999).ToString(); // Generate another random 5-digit number

        return $"{prefix}-{part1}-{part2}";
    }
}
