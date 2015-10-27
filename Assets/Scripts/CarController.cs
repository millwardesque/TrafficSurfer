﻿using UnityEngine;
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

	float m_currentSpeed = 0f;
	public float CurrentSpeed {
		get { return m_currentSpeed; }
	}

	Vector3 startPosition;
	Quaternion startRotation;
	Vector2 turnDestination;

	DrivingState m_state;
	public DrivingState State {
		get { return m_state; }
		set {
			m_state = value;

			if (m_state == DrivingState.Driving) {
				m_currentSpeed = maxSpeed;
			}
			else if (m_state == DrivingState.Turning) {
				m_currentSpeed = maxSpeed;
			}
			else if (m_state == DrivingState.Stopped) {
				m_currentSpeed = 0f;
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
            CheckForOtherCars();

            Vector2 distance = transform.up * m_currentSpeed * Time.deltaTime;
			transform.position += (Vector3)distance;

            Debug.DrawLine(transform.position, transform.position + transform.up / 2f, Color.red);
        }
		else if (State == DrivingState.Turning) {
            CheckForOtherCars();

            Vector2 distance = transform.up * m_currentSpeed * Time.deltaTime;
			transform.position += (Vector3)distance;
		}
	}

    void CheckForOtherCars()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up, 0.5f);
        for (int i = 0; i < hits.Length; ++i)
        {
            CarController car = hits[i].collider.GetComponent<CarController>();
            bool willHitCar = false;
            if (car != null && car != this)
            {
                float angle = Vector2.Angle(transform.up, car.transform.up);
                if (angle <= 90f)
                {
                    m_currentSpeed = 0f;
                    willHitCar = true;
                    break;
                }
            }
            if (!willHitCar)
            {
                m_currentSpeed = maxSpeed;
            }
        }
    }

	public void StopCar() {
		State = DrivingState.Stopped;
	}

	public void ChooseDirection(StopLine stopLine) {
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

		Quaternion newRotation = transform.rotation;
		newRotation.eulerAngles = new Vector3(0f, 0f, transform.rotation.eulerAngles.z + angle);
		transform.rotation = newRotation;
	}
	
	public void ResetCar() {
		transform.position = startPosition;
		transform.rotation = startRotation;
		State = DrivingState.Driving;
	}

	public void Colourize() {
		GetComponent<SpriteRenderer>().material.SetInt("_IsColourized", 1);
	}
}
