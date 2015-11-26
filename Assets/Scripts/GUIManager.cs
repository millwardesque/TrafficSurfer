using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public GameObject gameOverPanel;
	public GameObject youWinPanel;
	public GameObject pausePanel;
	public GameObject highScoreNamePanel;
	public GameObject howToPlayPanel;
	public UILevelObjectivesPanel levelObjectivesPanel;
	public UIObjectivePanel objectivePanel;
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

			if (youWinPanel == null) {
				Debug.LogError("GUI Manager: No You-Win panel is set.");
			}

			if (pausePanel == null) {
				Debug.LogError("GUI Manager: No Pause panel is set.");
			}

			if (levelObjectivesPanel == null) {
				Debug.LogError("GUI Manager: No level-objectives is set.");
			}

			if (howToPlayPanel == null) {
				Debug.LogError("GUI Manager: No how-to-play panel is set.");
			}

			if (highScoreNamePanel == null) {
				Debug.LogError("GUI Manager: No high-score name panel is set.");
			}

			if (objectivePanel == null) {
				Debug.LogError("GUI Manager: No objective panel is set.");
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

	void Start() {
		MessageManager.Instance.AddListener("RestartGame", OnRestartGame);
	}

	public void OnRestartGame(Message message) {
		ClosePausePanel();
		CloseGameOverPanel();
		CloseYouWinPanel();
		CloseHighScoreNamePanel();
		HideObjectivePanel();
		CloseLevelObjectivesPanel();
		CloseHowToPlayPanel();
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

	public void OpenYouWinPanel() {
		youWinPanel.SetActive(true);
	}
	
	public void CloseYouWinPanel() {
		youWinPanel.SetActive(false);
	}

	public void OpenLevelObjectivesPanel(string objectiveName, Sprite objectiveSprite) {
		levelObjectivesPanel.gameObject.SetActive(true);
		levelObjectivesPanel.SetObjective(objectiveName, objectiveSprite);
	}
	
	public void CloseLevelObjectivesPanel() {
		levelObjectivesPanel.gameObject.SetActive(false);
	}

	public void UpdateScoreLabel(int newScore) {
		scoreLabel.text = "Score: " + newScore;
	}

	public void UpdateTimeRemaining(int secondsRemaining) {
		timeRemainingLabel.text = "Time Remaining: " + secondsRemaining;
	}

	public void SubmitHighScoreName() {
		InputField nameField = highScoreNamePanel.GetComponentInChildren<InputField>();

		// This is a hack for hiding the carat. It doesn't really work, but I'll take it for now...
		GameObject carat = GameObject.Find("InputField Input Caret");
		if (carat != null) {
			carat.GetComponent<RectTransform>().localScale = Vector3.zero;
			carat.SetActive(false);
		}

		GameManager.Instance.playerName = nameField.text;
		highScoreNamePanel.SetActive(false);

		GameManager.Instance.OnHighScoreNameSet();
	}

	public void OpenHighScoreNamePanel() {
		highScoreNamePanel.SetActive(true);
		highScoreNamePanel.GetComponentInChildren<InputField>().ActivateInputField();
		highScoreNamePanel.GetComponentInChildren<InputField>().Select();

		// This is a hack for hiding the carat. It doesn't really work, but I'll take it for now...
		GameObject carat = GameObject.Find("InputField Input Caret");
		if (carat != null) {
			carat.SetActive(true);
			carat.GetComponent<RectTransform>().localScale = Vector3.one;
		}
	}

	public void CloseHighScoreNamePanel() {
		highScoreNamePanel.SetActive(false);
	}

	public void ShowObjectivePanel(string objectiveName) {
		objectivePanel.gameObject.SetActive(true);
		objectivePanel.ShowObjective(objectiveName);
	}

	public void HideObjectivePanel() {
		objectivePanel.HideObjective();
	}

	public void OpenHowToPlayPanel() {
		howToPlayPanel.SetActive(true);
	}

	public void CloseHowToPlayPanel() {
		howToPlayPanel.SetActive(false);
	}
}
