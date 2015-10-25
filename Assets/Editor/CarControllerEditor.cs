using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CarController))]
public class CarControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CarController myTarget = (CarController)target;

        DrawDefaultInspector();
        EditorGUILayout.LabelField("State", myTarget.State.ToString());
    }
}