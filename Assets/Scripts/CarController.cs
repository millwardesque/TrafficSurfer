using UnityEngine;
using System.Collections;

public enum DrivingState {
	Driving
};

public class CarController : MonoBehaviour {
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
		TurningNode turningNode = col.GetComponent<TurningNode>();
		if (turningNode != null) {
			Vector2 newDirection = turningNode.GetRandomDirection(transform.up);
			float angle = Vector2.Angle(transform.up, newDirection);

			// HACK: If the car is turning, snap it to the position of the turning node to keep it on the road.
			if (angle > TurningNode.AngleVariance) {
				transform.Rotate (new Vector3(0f, 0f, -1f), angle);
				transform.position = col.transform.position;
			}
		}
	}
}
