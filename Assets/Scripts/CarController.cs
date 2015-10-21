using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DrivingState {
	Driving,
	Stopped
};

struct PossibleDirection {
	public Vector2 direction;
	public float duration;

	public PossibleDirection(Vector2 direction, float duration) {
		this.direction = direction;
		this.duration = duration;
	}
}

public class CarController : MonoBehaviour {
	public static float TurnAngleVariance = 0.0001f;
	public float maxSpeed = 1f;
	float currentSpeed = 0f;

	DrivingState m_state;
	public DrivingState State {
		get { return m_state; }
		set {
			DrivingState oldState = m_state;
			m_state = value;

			if (m_state == DrivingState.Driving) {
				currentSpeed = maxSpeed;
			}
			else if (m_state == DrivingState.Stopped) {
				currentSpeed = 0f;
			}
		}
	}

	void Start () {
		State = DrivingState.Driving;
	}

	// Update is called once per frame
	void Update () {
		if (m_state == DrivingState.Driving) {
			Vector2 distance = transform.up * currentSpeed * Time.deltaTime;
			transform.position += (Vector3)distance;
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		StopLine stopLine = col.GetComponent<StopLine>();
		if (stopLine != null) {
			State = DrivingState.Stopped;
		}
	}

	public void ChooseDirection(bool canTurnLeft, bool canGoStraight, bool canTurnRight) {
		// Note: The turn-duration values are hand-tweaked. Change with caution.
		List<PossibleDirection> possibleDirections = new List<PossibleDirection>();
		if (canTurnLeft) {
			possibleDirections.Add(new PossibleDirection(-transform.right, 1.8f));
		}

		if (canGoStraight) {
			possibleDirections.Add(new PossibleDirection(transform.up, 0f));
		}

		if (canTurnRight) {
			possibleDirections.Add(new PossibleDirection(transform.right, 0.9f));
		}

		int randDirection = Random.Range(0, possibleDirections.Count);
		Vector2 turnDirection = possibleDirections[randDirection].direction;
		float turnDuration = possibleDirections[randDirection].duration;

		for (int i = 0; i < possibleDirections.Count; ++i) {
			Debug.Log (string.Format("Possible direction {0}", possibleDirections[i].direction));
		}
		Debug.Log (string.Format("Chose direction {0}", turnDirection));

		float angle = Vector2.Angle(transform.up, turnDirection);
		Vector3 cross = Vector3.Cross(transform.up, turnDirection);
		if (cross.z < 0f) {
			angle = -angle;
		}

		if (Mathf.Abs (angle) > CarController.TurnAngleVariance) {
			iTween.RotateBy(gameObject, iTween.Hash("z", angle / 360f, "time", turnDuration));
		}

		State = DrivingState.Driving;
	}
}
