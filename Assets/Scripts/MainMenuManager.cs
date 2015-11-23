using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
    static public MainMenuManager Instance = null;

    public UIHighScore highScoreUI;
    public GameObject highScorePanel;
	public GameObject creditsPanel;
	public GameObject howToPlayPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void OnNewGame()
    {
        Application.LoadLevel("Level-1");
    }
	
    public void OnHighScores()
    {
        highScorePanel.SetActive(true);
        highScoreUI.RefreshHighScores();
    }

    public void OnClearHighScores()
    {
        ScoreManager.Instance.EraseHighScores();
        highScoreUI.RefreshHighScores();
    }

    public void OnBackToMainMenu()
    {
        highScorePanel.SetActive(false);
		creditsPanel.SetActive(false);
		howToPlayPanel.SetActive(false);
    }

	public void OnHowToPlay() {
		howToPlayPanel.SetActive(true);
	}

	public void OnCredits() {
		creditsPanel.SetActive(true);
	}

	public void OnExit() {
		Application.Quit();
	}
}
