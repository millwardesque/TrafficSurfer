using UnityEngine;
using System.Collections;

public enum GameState {
	IsRunning,
	IsPaused,
	GameOver
};

public class GameManager : MonoBehaviour {
	public Transform playerStart;

	PlayerController m_player;
	public PlayerController Player {
		get { return m_player; }
	}

	GameState m_state = GameState.IsRunning;
	public GameState State {
		get { return m_state; }
		set {
			GameState oldState = m_state;
			m_state = value;

			if (m_state == GameState.IsPaused) {
				Time.timeScale = 0f;
				GUIManager.Instance.OpenPausePanel();
			}
			else if (m_state == GameState.IsRunning) {
				Time.timeScale = 1f;
				if (oldState == GameState.IsPaused) {
					GUIManager.Instance.ClosePausePanel();
				}
				else if (oldState == GameState.GameOver) {
					GUIManager.Instance.CloseGameOverPanel();
				}
			}
			else if (m_state == GameState.GameOver) {
				ScoreManager.Instance.AddScore(new HighScore("TST", ScoreManager.Instance.Score));
				Time.timeScale = 0f;
				GUIManager.Instance.OpenGameOverPanel();
			}
		}
	}

	public static GameManager Instance = null;
	
	void Awake () {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	void Start() {
		GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
		m_player = playerGO.GetComponent<PlayerController>();

		RestartGame();
	}

	void Update() {
		if (State == GameState.IsRunning) {
			if (Input.GetKeyDown(KeyCode.P)) {
				Pause();
			}
			else if (Input.GetKeyDown(KeyCode.G)) {
				GameOver ();
			}
		}
		else if (State == GameState.IsPaused) {
			if (Input.GetKeyDown(KeyCode.P)) {
				Unpause();
			}
		}
	}

	public void Pause() {
		State = GameState.IsPaused;
	}

	public void Unpause() {
		State = GameState.IsRunning;
	}

	public void GameOver() {
		State = GameState.GameOver;
	}

	public void RestartGame() {
		GameTimer.Instance.RestartGame();
		ScoreManager.Instance.RestartGame();
		m_player.ResetPlayer(playerStart);

		CarController[] cars = FindObjectsOfType<CarController>();
		for (int i = 0; i < cars.Length; ++i) {
			cars[i].ResetCar();
		}

		GoalManager.Instance.RestartGame();

		State = GameState.IsRunning;
	}
}
