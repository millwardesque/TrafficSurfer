using UnityEngine;
using System.Collections;

public enum GameState {
	IsRunning,
	IsPaused
};

public class GameManager : MonoBehaviour {
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

	void Update() {
		if (State == GameState.IsRunning) {
			if (Input.GetKeyDown(KeyCode.P)) {
				Pause();
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
}
