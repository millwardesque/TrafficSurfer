﻿using UnityEngine;
using System.Collections;

public enum GameState {
	IsRunning,
	IsPaused,
	GameOver
};

public class GameManager : MonoBehaviour {
	public Transform playerStart;
	public string playerName = "";
    AudioSource backgroundMusic;

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
                backgroundMusic.Pause();
			}
			else if (m_state == GameState.IsRunning) {
                backgroundMusic.Play();
                Time.timeScale = 1f;
				if (oldState == GameState.IsPaused) {
					GUIManager.Instance.ClosePausePanel();
				}
				else if (oldState == GameState.GameOver) {
					GUIManager.Instance.CloseGameOverPanel();
				}
			}
			else if (m_state == GameState.GameOver) {
				Time.timeScale = 0f;
                backgroundMusic.Stop();

				if (ScoreManager.Instance.IsHighScore(ScoreManager.Instance.Score)) {
					GUIManager.Instance.OpenHighScoreNamePanel();
				}
				else {
					GUIManager.Instance.OpenGameOverPanel();
				}
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
		GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
		m_player = playerGO.GetComponent<PlayerController>();

		RestartGame();
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

	public void GameOver() {
		State = GameState.GameOver;
	}

	public void RestartGame() {
		GameTimer.Instance.RestartGame();
		ScoreManager.Instance.RestartGame();
		m_player.ResetPlayer(playerStart);
		ChangeMusicPitch(1f);

		CarController[] cars = FindObjectsOfType<CarController>();
		for (int i = 0; i < cars.Length; ++i) {
			cars[i].ResetCar();
		}

        IntersectionManager[] intersections = FindObjectsOfType<IntersectionManager>();
        for (int i = 0; i < intersections.Length; ++i)
        {
            intersections[i].ResetIntersection();
        }


        CameraTools cameraTools = Camera.main.GetComponent<CameraTools>();
        cameraTools.followTarget = Player.transform;
        cameraTools.FollowState = CameraFollowState.Follow;

		/**
		 * Disabled until I can make the car-target indicator work with ProCamera2D.
		 * When re-enabling, add the following line to the top of the file:

		using Com.LuisPedroFonseca.ProCamera2D;
		
		Camera.main.GetComponent<ProCamera2D>().CameraTargets.Clear();
		Camera.main.GetComponent<ProCamera2D>().AddCameraTarget(Player.transform);
		*/

        ChooseTargetCar();

		State = GameState.IsRunning;
	}

	public void OnReachedTargetCar() {
		ScoreManager.Instance.Score += 20;
		ChooseTargetCar();
	}

	void ChooseTargetCar() {
		CarController oldCar = Player.TargetCar;
		CarController[] cars = FindObjectsOfType<CarController>();
		int index = Random.Range(0, cars.Length);
		while (cars[index] == oldCar || cars[index] == null) {
			index = Random.Range(0, cars.Length);
		}

		if (oldCar != null) {
			oldCar.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
		}

        TargetCarIndicator.Instance.targetCar = cars[index];
        Player.TargetCar = cars[index];
        cars[index].Colourize(new Color(1f, 0f, 0f));
	}

    public void GoToMainMenu()
    {
        Application.LoadLevel("Main Menu");
    }

	public void ChangeMusicPitch(float pitch) {
		backgroundMusic.pitch = pitch;
	}

	public void OnHighScoreNameSet() {
		ScoreManager.Instance.AddHighScore(new HighScore(playerName, ScoreManager.Instance.Score));
		GUIManager.Instance.OpenGameOverPanel();
	}
}
