using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TargetCarIndicator))]
public class TargetCarIndicatorEditor : Editor {
	public override void OnInspectorGUI()
	{
		TargetCarIndicator myTarget = (TargetCarIndicator)target;
		
		DrawDefaultInspector();
		EditorGUILayout.LabelField("Target Car", myTarget.TargetCar == null ? "<null>" : myTarget.TargetCar.name);
	}
}
