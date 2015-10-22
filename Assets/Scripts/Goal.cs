using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {
	public int scoreValue = 10;

	void OnTriggerEnter2D(Collider2D col) {
		PlayerController player = col.GetComponent<PlayerController>();
		if (player != null) {
			OnCollected();
		}
	}

	public void OnCollected() {
		ScoreManager.Instance.Score += scoreValue;
		GoalManager.Instance.GenerateGoal();
		Destroy(gameObject);
	}
}
