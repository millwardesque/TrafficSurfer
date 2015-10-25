using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(IntersectionManager))]
public class IntersectionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        IntersectionManager myTarget = (IntersectionManager)target;

        DrawDefaultInspector();
        EditorGUILayout.LabelField("State", myTarget.State.ToString());
        EditorGUILayout.LabelField("Queue size", myTarget.CarQueue.Count.ToString());
        EditorGUILayout.LabelField("Waiting for", myTarget.WaitingOnCar.ToString());
    }
}