using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILevelObjectivesPanel : MonoBehaviour {
	public Text objectiveName;

	public void SetObjective(string objectiveString) {
		objectiveName.text = objectiveString;
	}
}
