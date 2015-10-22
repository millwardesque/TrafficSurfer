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
	Vector3 startPosition;
	Quaternion startRotation;

	DrivingState m_state;
	public DrivingState State {
		get { return m_state; }
		set {
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
		startPosition = transform.position;
		startRotation = transform.rotation;
		ResetCar();
	}

	// Update is called once per frame
	void Update () {
		if (m_state == DrivingState.Driving) {
			Vector2 distance = transform.up * currentSpeed * Time.deltaTime;
			transform.position += (Vector3)distance;
		}
	}

	public void StopCar() {
		State = DrivingState.Stopped;
	}

	public void ChooseDirection(bool canTurnLeft, bool canGoStraight, bool canTurnRight) {
		// Note: The turn-duration values are hand-tweaked. Change with caution.
		List<PossibleDirection> possibleDirections = new List<PossibleDirection>();
		if (canTurnLeft) {
			possibleDirections.Add(new PossibleDirection(-transform.right, 0.77f));
		}

		if (canGoStraight) {
			possibleDirections.Add(new PossibleDirection(transform.up, 0f));
		}

		if (canTurnRight) {
			possibleDirections.Add(new PossibleDirection(transform.right, 0.47f));
		}

		int randDirection = Random.Range(0, possibleDirections.Count);
		Vector2 turnDirection = possibleDirections[randDirection].direction;
		float turnDuration = possibleDirections[randDirection].duration;

		float angle = Vector2.Angle(transform.up, turnDirection);
		Vector3 cross = Vector3.Cross(transform.up, turnDirection);
		if (cross.z < 0f) {
			angle = -angle;
		}

		if (Mathf.Abs (angle) > CarController.TurnAngleVariance) {
			iTween.RotateBy(gameObject, iTween.Hash("z", angle / 360f, "time", turnDuration, "easeType", "linear"));
		}

		State = DrivingState.Driving;
	}

	public void ResetCar() {
		iTween.Stop(gameObject);
		transform.position = startPosition;
		transform.rotation = startRotation;
		State = DrivingState.Driving;
	}
}
