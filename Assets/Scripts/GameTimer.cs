using UnityEngine;
using System.Collections;

public class GameTimer : MonoBehaviour {
	public float gameLength;
	float gameTimeRemaining;

	// Use this for initialization
	void Start () {
		gameTimeRemaining = gameLength;	
	}
	
	// Update is called once per frame
	void Update () {
		gameTimeRemaining -= Time.deltaTime;
		GUIManager.Instance.UpdateTimeRemaining(Mathf.FloorToInt(gameTimeRemaining));
	}
}
