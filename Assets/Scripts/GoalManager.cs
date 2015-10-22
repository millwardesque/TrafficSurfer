using UnityEngine;
using System.Collections;

public class GoalManager : MonoBehaviour {
	public Goal goalPrefab;
	public float minDistanceFromPlayer = 1f;

	// Spawn bounding box.
	Vector2 bottomLeft;
	Vector2 topRight;

	public static GoalManager Instance = null;

	void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy (gameObject);
		}
	}

	void Start() {
		bottomLeft = new Vector2(-4.6f, -3.4f);
		topRight = new Vector2(4.6f, 3.4f);
	}

	public Goal GenerateGoal() {
		Vector2 playerPosition = GameManager.Instance.Player.transform.position;
		Goal newGoal = Instantiate<Goal>(goalPrefab);
		newGoal.transform.SetParent(transform);

		Vector2 newPosition = new Vector2(Random.Range(bottomLeft.x, topRight.x), Random.Range (bottomLeft.y, topRight.y));
		float distanceFromPlayer = (newPosition - playerPosition).magnitude;
		Vector2 directionFromPlayer = (newPosition - playerPosition).normalized;
		if (distanceFromPlayer < minDistanceFromPlayer) {
			newPosition += directionFromPlayer * (minDistanceFromPlayer - distanceFromPlayer);

			// If the position is out of bounds, move the goal to the *other* side of the player
			if (!(newPosition.x >= bottomLeft.x && newPosition.x <= topRight.x &&
			    newPosition.y >= bottomLeft.y && newPosition.y <= topRight.y)) {
				newPosition -= 2 * directionFromPlayer * (minDistanceFromPlayer - distanceFromPlayer);
			}
		}
		newGoal.transform.position = newPosition;

		return newGoal;
	}

	public void RestartGame() {
		Goal[] goals = FindObjectsOfType<Goal>();
		for (int i = 0; i < goals.Length; ++i) {
			Destroy(goals[i].gameObject);
		}

		GenerateGoal();
	}
}
