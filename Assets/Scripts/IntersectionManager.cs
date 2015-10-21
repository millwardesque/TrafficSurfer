using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum IntersectionState {
	Empty,
	ReleaseNextCar,
	WaitingForCar
};

struct WaitingCar {
	public CarController car;
	public StopLine stopLine;

	public WaitingCar(CarController car, StopLine stopLine) {
		this.car = car;
		this.stopLine = stopLine;
	}
};

public class IntersectionManager : MonoBehaviour {
	Queue<WaitingCar> cars = new Queue<WaitingCar>();

	IntersectionState m_state = IntersectionState.Empty;
	public IntersectionState State {
		get { return m_state; }
		set {
			IntersectionState oldState = m_state;
			m_state = value;
		}
	}

	void Update() {
		if (State == IntersectionState.Empty) {
			if (cars.Count > 0) {
				State = IntersectionState.ReleaseNextCar;
			}
		}
		else if (State == IntersectionState.ReleaseNextCar) {
			if (cars.Count == 0) {
				State = IntersectionState.Empty;
			}
			else {
				WaitingCar waitingCar = cars.Dequeue();
				CarController car = waitingCar.car;
				StopLine stopLine = waitingCar.stopLine;

				Debug.Log (string.Format("Choosing direction from L:{0} S:{1} R:{2} ({3} {4})", stopLine.CanTurnLeft(), stopLine.CanGoStraight(), stopLine.CanTurnRight(), this, stopLine));
				car.ChooseDirection(stopLine.CanTurnLeft(), stopLine.CanGoStraight(), stopLine.CanTurnRight());

				State = IntersectionState.WaitingForCar;
			}
		}
		else if (State == IntersectionState.WaitingForCar) {
			// Do nothing since we're waiting.
		}
	}

	void OnDrawGizmos () {
		Gizmos.DrawIcon (transform.position, "Intersection.png", true);
	}

	public void Enqueue(CarController car, StopLine stopLine) {
		cars.Enqueue(new WaitingCar(car, stopLine));
	}

	public void SignalCarFinished() {
		if (State == IntersectionState.WaitingForCar) {
			State = IntersectionState.ReleaseNextCar;
		}
	}
}
