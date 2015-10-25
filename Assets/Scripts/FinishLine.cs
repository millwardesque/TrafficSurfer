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
		if (car != null && car.State == DrivingState.Turning) {
			float angle = Vector2.Angle(car.transform.up, transform.up);
			if (Mathf.Abs(angle) < 90f + 5f) {
				car.RotateTo (transform.up);
	
				// Correct the lateral position of the car to line up with the center of the finish line.
				Vector2 distance = transform.position - car.transform.position;
				Vector3 lateralOffset = new Vector3(Mathf.Abs (transform.right.x) * distance.x, Mathf.Abs (transform.right.y) * distance.y, 0f);
				car.transform.position += lateralOffset;

                intersection.SignalCarFinished();
                car.State = DrivingState.Driving;
			}
		}
	}
}
