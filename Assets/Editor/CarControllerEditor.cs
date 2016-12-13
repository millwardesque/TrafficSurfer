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
		EditorGUILayout.LabelField("State", myTarget.State == null ? "<unknown>" : myTarget.State.ToString());

		if (myTarget.Engine != null) {
			EditorGUILayout.LabelField ("Speed", myTarget.Engine.CurrentSpeed.ToString());
			EditorGUILayout.LabelField ("Throttle", myTarget.Engine.CurrentThrottle.ToString());
			EditorGUILayout.LabelField ("Brake", myTarget.Engine.CurrentBrake.ToString());
			EditorGUILayout.LabelField ("Tire Rotation", myTarget.Engine.CurrentTireRotation.ToString());
		}
    }
}