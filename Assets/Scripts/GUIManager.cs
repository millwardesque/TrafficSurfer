using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public GameObject gameOverPanel;
	public GameObject pausePanel;
	public UIHighScore highScoreUI;
	public Text scoreLabel;
	public Text timeRemainingLabel;
	
	public static GUIManager Instance = null;

	void Awake () {
		if (Instance == null) {
			Instance = this;
			if (gameOverPanel == null) {
				Debug.LogError("GUI Manager: No Game-Over panel is set.");
			}


			if (pausePanel == null) {
				Debug.LogError("GUI Manager: No Pause panel is set.");
			}

			if (highScoreUI == null) {
				Debug.LogError("GUI Manager: No high-score UI is set.");
			}

			if (scoreLabel == null) {
				Debug.LogError("GUI Manager: No Score label is set.");
			}

			if (timeRemainingLabel == null) {
				Debug.LogError("GUI Manager: No Time-Remaining label is set.");
			}
		}
		else {
			Destroy(gameObject);
		}
	}

	public void OpenPausePanel() {
		pausePanel.SetActive(true);
	}

	public void ClosePausePanel() {
		pausePanel.SetActive(false);
	}

	public void OpenGameOverPanel() {
		gameOverPanel.SetActive(true);
		highScoreUI.RefreshHighScores();
	}
	
	public void CloseGameOverPanel() {
		gameOverPanel.SetActive(false);
	}

	public void UpdateScoreLabel(int newScore) {
		scoreLabel.text = "Score: " + newScore;
	}

	public void UpdateTimeRemaining(int secondsRemaining) {
		timeRemainingLabel.text = "Time Remaining: " + secondsRemaining;
	}
}
