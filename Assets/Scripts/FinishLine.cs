using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {
	IntersectionManager intersection;
	
	void Start() {
		intersection = transform.parent.GetComponent<IntersectionManager>();
		if (intersection == null) {
			Debug.LogError(string.Format ("Finish line {0} doesn't have a parent intersection", this));
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		CarController car = col.GetComponent<CarController>();
		if (car != null && car.State.GetType () == typeof(DrivingStateTurning)) {
			float angle = Vector2.Angle(car.transform.right, transform.up);
			if (Mathf.Abs(angle) < 90f + 5f) {
				car.FinishTurn (transform);
				intersection.SignalCarFinished();
			}
		}
	}
}
