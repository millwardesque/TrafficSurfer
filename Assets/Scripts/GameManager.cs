using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine.SceneManagement;

public enum GameState {
	LevelIntro,
	IsRunning,
	IsPaused,
	GameOver,
	YouWin
};

public class GameManager : MonoBehaviour {
	public Transform playerStart;
	public string playerName = "";
	public float minTargetSpawnDistance = 5f;
    AudioSource backgroundMusic;

	PlayerController m_player;
	public PlayerController Player {
		get { return m_player; }
	}

	GameState m_state = GameState.LevelIntro;
	public GameState State {
		get { return m_state; }
		set {
			GameState oldState = m_state;
			m_state = value;

			if (m_state == GameState.IsPaused) {
				Time.timeScale = 0f;
				GUIManager.Instance.OpenPausePanel();
                backgroundMusic.Pause();
			}
			else if (m_state == GameState.LevelIntro) {
				Time.timeScale = 0f;
				ObjectiveManager.Instance.ShowLevelObjectives();
				backgroundMusic.Pause();
			}
			else if (m_state == GameState.IsRunning) {
                backgroundMusic.Play();
                Time.timeScale = 1f;
				if (oldState == GameState.LevelIntro) {
					GUIManager.Instance.CloseLevelObjectivesPanel();
				}
				else if (oldState == GameState.IsPaused) {
					GUIManager.Instance.ClosePausePanel();
				}
				else if (oldState == GameState.GameOver) {
					GUIManager.Instance.CloseGameOverPanel();
				}
			}
			else if (m_state == GameState.GameOver) {
				Time.timeScale = 0f;
                backgroundMusic.Stop();

				GUIManager.Instance.HideObjectivePanel();
				if (ScoreManager.Instance.IsHighScore(ScoreManager.Instance.Score)) {
					GUIManager.Instance.OpenHighScoreNamePanel();
				}
				else {
					GUIManager.Instance.OpenGameOverPanel();
				}
			}
			else if (m_state == GameState.YouWin) {
				Time.timeScale = 0f;
				SoundFXManager.Instance.PlayYouWinSFX();
				backgroundMusic.Stop();
				ObjectiveManager.Instance.IncreaseDifficulty();

				GUIManager.Instance.HideObjectivePanel();
				GUIManager.Instance.OpenYouWinPanel();
			}
		}
	}

	public static GameManager Instance = null;
	
	void Awake () {
		if (Instance == null) {
			Instance = this;
            backgroundMusic = GetComponent<AudioSource>();
		}
		else {
			Destroy(gameObject);
		}
	}

	void Start() {
		MessageManager.Instance.AddListener("AllObjectivesComplete", OnAllObjectivesComplete);

		GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
		m_player = playerGO.GetComponent<PlayerController>();

		RestartGame();
	}

	void Update() {
		if (State == GameState.IsRunning) {
			if (Input.GetButtonDown ("Pause")) {
				Pause();
			}
		}
		else if (State == GameState.LevelIntro) {
			if (Input.GetButtonDown("Submit")) {
				CloseObjectiveAndStart();
			}
		}
		else if (State == GameState.YouWin) {
			if (Input.GetButtonDown("Submit")) {
				RestartGame();
			}
		}
		else if (State == GameState.IsPaused) {
			if (Input.GetButtonDown ("Pause")) {
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

	public void ResetScoreAndRestartGame() {
		ScoreManager.Instance.ResetScore();
		ObjectiveManager.Instance.ResetDifficulty();
		RestartGame ();
	}

	public void RestartGame() {
		GameTimer.Instance.RestartGame();
		m_player.ResetPlayer(playerStart);
		ChangeMusicPitch(1f);

		CarManager.Instance.GenerateCars(true);

        IntersectionManager[] intersections = FindObjectsOfType<IntersectionManager>();
        for (int i = 0; i < intersections.Length; ++i)
        {
            intersections[i].ResetIntersection();
        }

		ProCamera2D proCamera = Camera.main.GetComponent<ProCamera2D>();
		if (proCamera) {
			proCamera.CameraTargets.Clear();
			proCamera.AddCameraTarget(Player.transform);
		}

		ChooseTargetCar();

		MessageManager.Instance.SendMessage(new Message(this, "RestartGame", null));

		State = GameState.LevelIntro;
	}

	public void OnReachedTargetCar() {
		ScoreManager.Instance.Score += 20;
		ChooseTargetCar();
	}

	void ChooseTargetCar() {
		CarController oldCar = Player.TargetCar;
		List<CarController> cars = CarManager.Instance.Cars;

		int index = Random.Range(0, cars.Count);
		int maxLoops = 1000;
		int loopCount = 0;
		while (cars[index] == oldCar || cars[index] == null || (cars[index].transform.position - Player.transform.position).magnitude < minTargetSpawnDistance) {
			index = Random.Range(0, cars.Count);
			loopCount++;
			if (loopCount >= maxLoops) {
				break;
			}
		}

        if (TargetCarIndicator.Instance != null) {
            TargetCarIndicator.Instance.TargetCar = cars[index];
        }
        
        Player.TargetCar = cars[index];
	}

	public void OnAllObjectivesComplete(Message message) {
		State = GameState.YouWin;
	}

	public void CloseObjectiveAndStart() {
		State = GameState.IsRunning;
	}

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

	public void ChangeMusicPitch(float pitch) {
		backgroundMusic.pitch = pitch;
	}

	public void OnHighScoreNameSet() {
		ScoreManager.Instance.AddHighScore(new HighScore(playerName, ScoreManager.Instance.Score));
		GUIManager.Instance.OpenGameOverPanel();
	}
}
