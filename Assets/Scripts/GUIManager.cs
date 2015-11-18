using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public GameObject gameOverPanel;
	public GameObject pausePanel;
	public GameObject highScoreNamePanel;
	public UIAchievementPanel achievementPanel;
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

			if (highScoreNamePanel == null) {
				Debug.LogError("GUI Manager: No high-score name panel is set.");
			}

			if (achievementPanel == null) {
				Debug.LogError("GUI Manager: No achievement panel is set.");
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

			MessageManager.Instance.AddListener("RestartGame", OnRestartGame);
		}
		else {
			Destroy(gameObject);
		}
	}

	public void OnRestartGame(Message message) {
		ClosePausePanel();
		CloseGameOverPanel();
		CloseHighScoreNamePanel();
		HideAchievementPanel();
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

	public void SubmitHighScoreName() {
		GameManager.Instance.playerName = highScoreNamePanel.GetComponentInChildren<InputField>().text;
		highScoreNamePanel.SetActive(false);

		GameManager.Instance.OnHighScoreNameSet();
	}

	public void OpenHighScoreNamePanel() {
		highScoreNamePanel.SetActive(true);
	}

	public void CloseHighScoreNamePanel() {
		highScoreNamePanel.SetActive(false);
	}

	public void ShowAchievementPanel(string achievementName) {
		achievementPanel.gameObject.SetActive(true);
		achievementPanel.ShowAchievement(achievementName);
	}

	public void HideAchievementPanel() {
		achievementPanel.HideAchievement();
	}
}
