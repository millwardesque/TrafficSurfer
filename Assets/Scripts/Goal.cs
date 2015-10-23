using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {
	public int scoreValue = 10;
	AudioSource collectionNoise;
	bool hasBeenCollected = false;

	void Awake() {
		collectionNoise = GetComponent<AudioSource>();
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (hasBeenCollected) {
			return;
		}

		PlayerController player = col.GetComponent<PlayerController>();
		if (player != null) {
			OnCollected();
		}
	}

	public void OnCollected() {
		hasBeenCollected = true;
		ScoreManager.Instance.Score += scoreValue;

		collectionNoise.Play();
		GetComponent<SpriteRenderer>().enabled = false;
		StartCoroutine("DestroyOnFinished", collectionNoise.clip.length);
	}

	IEnumerator DestroyOnFinished(float duration) {
		yield return new WaitForSeconds(duration);

		Destroy (gameObject);
		GoalManager.Instance.GenerateGoal();
	}
}
