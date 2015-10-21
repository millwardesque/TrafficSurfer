﻿using UnityEngine;
using System.Collections;

public enum TurnDirections {
	LeftTurn = 0,
	Straight,
	RightTurn
};

public class StopLine : MonoBehaviour {
	IntersectionManager intersection;
	public FinishLine[] turnTargets = new FinishLine[3];

	void Start() {
		intersection = transform.parent.GetComponent<IntersectionManager>();
		if (intersection == null) {
			Debug.LogError(string.Format ("Stop line {0} doesn't have a parent intersection", this));
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		CarController car = col.GetComponent<CarController>();
		if (car != null) {
			intersection.Enqueue(car, this);
		}
	}

	public bool CanTurnLeft() {
		return (turnTargets[(int)TurnDirections.LeftTurn] != null);
	}

	public bool CanGoStraight() {
		return (turnTargets[(int)TurnDirections.Straight] != null);
	}

	public bool CanTurnRight() {
		return (turnTargets[(int)TurnDirections.RightTurn] != null);
	}
}
