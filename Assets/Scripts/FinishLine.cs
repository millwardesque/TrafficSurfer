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
	
	void OnTriggerExit2D(Collider2D col) {
		CarController car = col.GetComponent<CarController>();
		if (car != null) {
			intersection.SignalCarFinished();
		}
	}
}
