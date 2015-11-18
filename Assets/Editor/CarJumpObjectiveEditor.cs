using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CarJumpObjective))]
public class CarJumpObjectiveEditor : Editor {
	public override void OnInspectorGUI()
	{
		CarJumpObjective myTarget = (CarJumpObjective)target;
		
		DrawDefaultInspector();
		EditorGUILayout.LabelField("Current Jumps", myTarget.Jumps.ToString());
	}
}
