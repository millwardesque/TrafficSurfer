using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurningNode : MonoBehaviour {
	public static float AngleVariance = 1f; // Variance of angles permitted in valid nodes to allow for float / math rounding.

	public Vector2[] newDirections; // Set by Editor.

	void Awake() {
		if (newDirections.Length == 0) {
			Debug.LogError(string.Format ("Turning Node {0} doesn't have any directions set. Deleting.", this));
			Destroy (gameObject);
		}
	}

	public Vector2 GetRandomDirection(Vector2 currentDirection) {
		List<Vector2> validDirections = new List<Vector2>();
		for (int i = 0; i < newDirections.Length; ++i) {
			float angle = Vector2.Angle(currentDirection, newDirections[i]);
			if (angle <= 90f + TurningNode.AngleVariance && angle >= 0f - TurningNode.AngleVariance) {
				validDirections.Add(newDirections[i]);
			}
		}

		if (validDirections.Count == 0) {
			Debug.LogError(string.Format ("Unable to find valid direction from {0} at Turning Node {1}: Using current direction.", currentDirection, this));
			// Log the various directions / angles for debugging purposes.
			for (int i = 0; i < newDirections.Length; ++i) {
				float angle = Vector2.Angle(currentDirection, newDirections[i]);
				Debug.Log (string.Format("{0} to {1} : {2}", currentDirection, newDirections[i], angle));
			}

			return currentDirection;
		}
		else {
			return validDirections[Random.Range(0, validDirections.Count)];
		}
	}
}
