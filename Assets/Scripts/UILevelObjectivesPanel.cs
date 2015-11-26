using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILevelObjectivesPanel : MonoBehaviour {
	public Text objectiveName;
	public Image objectiveImage;

	public void SetObjective(string objectiveString, Sprite objectiveSprite) {
		objectiveName.text = objectiveString;
		objectiveImage.sprite = objectiveSprite;
	}
}
