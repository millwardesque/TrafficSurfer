using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScoreInputFieldValidator : MonoBehaviour {
	public Button submitButton;

	public void OnValueChangeCheck(string content) {
		// Disable the submit button if the input field is empty.
		if (content.Trim() == "") {
			submitButton.interactable = false;
		}
		else {
			submitButton.interactable = true;
		}
	}

	public void OnUserSubmit() {
		GUIManager.Instance.SubmitHighScoreName();
	}
}
