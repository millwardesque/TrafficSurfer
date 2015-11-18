using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CarJumpAchievementTracker))]
public class CarJumpAchievementTrackerEditor : Editor {
	public override void OnInspectorGUI()
	{
		CarJumpAchievementTracker myTarget = (CarJumpAchievementTracker)target;
		
		DrawDefaultInspector();
		EditorGUILayout.LabelField("Current Jumps", myTarget.Jumps.ToString());
	}
}
