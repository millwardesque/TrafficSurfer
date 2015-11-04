using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameManager myTarget = (GameManager)target;

        DrawDefaultInspector();
        EditorGUILayout.LabelField("State", myTarget.State.ToString());
    }
}