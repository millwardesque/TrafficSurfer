using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public GameObject gameOverPanel;
	public GameObject youWinPanel;
	public GameObject pausePanel;

	public GameObject howToPlayPanel;
	public UILevelObjectivesPanel levelObjectivesPanel;
	public UIObjectivePanel objectivePanel;
	public UIHighScore highScoreUI;
	public Text scoreLabel;
	public Text timeRemainingLabel;


	public GameObject highScoreNamePrefab;
	GameObject highScoreNamePanel;

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

			if (highScoreNamePrefab == null) {
				Debug.LogError("GUI Manager: No high-score name prefab is set.");
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
		GameManager.Instance.playerName = highScoreNamePanel.GetComponentInChildren<InputField>().text;

		CloseHighScoreNamePanel();
	
		GameManager.Instance.OnHighScoreNameSet();
	}

	public void OpenHighScoreNamePanel() {
		if (null != highScoreNamePanel) {
			CloseHighScoreNamePanel();
		}

		Canvas mainCanvas = FindObjectOfType<Canvas>();
		highScoreNamePanel = Instantiate<GameObject>(highScoreNamePrefab);
		highScoreNamePanel.transform.SetParent(mainCanvas.transform, false);
	}

	public void CloseHighScoreNamePanel() {
		if (null != highScoreNamePanel) {
			Destroy (highScoreNamePanel);
			highScoreNamePanel = null;
		}
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
