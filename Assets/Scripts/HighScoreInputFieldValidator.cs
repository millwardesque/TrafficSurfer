using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScoreInputFieldValidator : MonoBehaviour {
	public Button submitButton;
	
	void Update() {
		if (Input.GetButtonDown("Submit") && submitButton.interactable) {
			GUIManager.Instance.SubmitHighScoreName();
		}
	}

	public void OnValueChangeCheck(string content) {
		// Disable the submit button if the input field is empty.
		if (content == "") {
			submitButton.interactable = false;
		}
		else {
			submitButton.interactable = true;
		}
	}
}
