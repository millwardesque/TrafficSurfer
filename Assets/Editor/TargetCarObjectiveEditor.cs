using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TargetCarObjective))]
public class TargetCarObjectiveEditor : Editor {
	public override void OnInspectorGUI()
	{
		TargetCarObjective myTarget = (TargetCarObjective)target;
		
		DrawDefaultInspector();
		EditorGUILayout.LabelField("Current Jumps", myTarget.Jumps.ToString());
	}
}
