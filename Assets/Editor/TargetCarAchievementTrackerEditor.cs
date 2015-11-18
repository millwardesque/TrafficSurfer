using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TargetCarAchievementTracker))]
public class TargetCarAchievementTrackerEditor : Editor {
	public override void OnInspectorGUI()
	{
		TargetCarAchievementTracker myTarget = (TargetCarAchievementTracker)target;
		
		DrawDefaultInspector();
		EditorGUILayout.LabelField("Current Jumps", myTarget.Jumps.ToString());
	}
}
