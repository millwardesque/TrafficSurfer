using UnityEngine;
using System.Collections;

public class GameTimer : MonoBehaviour {
	public float gameLength;
	float gameTimeRemaining;

	public static GameTimer Instance = null;
	
	void Awake () {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		RestartGame ();
	}
	
	// Update is called once per frame
	void Update () {
		gameTimeRemaining -= Time.deltaTime;
		GUIManager.Instance.UpdateTimeRemaining(Mathf.FloorToInt(gameTimeRemaining));
	}

	public void RestartGame() {
		gameTimeRemaining = gameLength;
		GUIManager.Instance.UpdateTimeRemaining(Mathf.FloorToInt(gameTimeRemaining));
	}
}
