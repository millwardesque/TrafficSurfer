using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DrivingState {
	Driving,
	Turning,
	Stopped
};

public class CarController : MonoBehaviour {
	public static float TurnAngleVariance = 0.0001f;
	public float maxSpeed = 1f;
	float currentSpeed = 0f;
	Vector3 startPosition;
	Quaternion startRotation;
	Vector2 turnDestination;

	DrivingState m_state;
	public DrivingState State {
		get { return m_state; }
		set {
			m_state = value;

			if (m_state == DrivingState.Driving) {
				currentSpeed = maxSpeed;
			}
			else if (m_state == DrivingState.Turning) {
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
		if (State == DrivingState.Driving) {
			Vector2 distance = transform.up * currentSpeed * Time.deltaTime;
			transform.position += (Vector3)distance;
		}
		else if (State == DrivingState.Turning) {
			Vector2 distance = transform.up * currentSpeed * Time.deltaTime;
			transform.position += (Vector3)distance;
		}
	}

	public void StopCar() {
		State = DrivingState.Stopped;
	}

	public void ChooseDirection(StopLine stopLine) {
		// Note: The turn-duration values are hand-tweaked. Change with caution.
		List<Vector2> possibleDestinations = new List<Vector2>();
		if (stopLine.CanTurnLeft()) {
			possibleDestinations.Add(stopLine.GetLeftTurnDestination());
		}

		if (stopLine.CanGoStraight()) {
			possibleDestinations.Add(stopLine.GetStraightDestination());
		}

		if (stopLine.CanTurnRight()) {
			possibleDestinations.Add(stopLine.GetRightTurnDestination());
		}

		int i = Random.Range(0, possibleDestinations.Count);
		turnDestination = possibleDestinations[i];
		Vector2 turnDirection = (turnDestination - (Vector2)transform.position).normalized;
		RotateTo(turnDirection);
		State = DrivingState.Turning;
	}

	public void RotateTo(Vector2 direction) {
		float angle = Vector2.Angle(transform.up, direction);
		Vector3 cross = Vector3.Cross((Vector3)transform.up, (Vector3)direction);
		if (cross.z < 0f) {
			angle *= -1f;
		}

		// Debug.Log (string.Format("Rotating from {0} to {1} ({2} degrees)", transform.up, direction, angle));

		Quaternion newRotation = transform.rotation;
		newRotation.eulerAngles = new Vector3(0f, 0f, transform.rotation.eulerAngles.z + angle);
		transform.rotation = newRotation;
	}
	
	public void ResetCar() {
		transform.position = startPosition;
		transform.rotation = startRotation;
		State = DrivingState.Driving;
	}
}
