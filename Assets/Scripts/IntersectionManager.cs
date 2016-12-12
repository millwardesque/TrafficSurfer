using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum IntersectionState {
	Empty,
	ReleaseNextCar,
	WaitingForCar
};

public struct WaitingCar
{
    public CarController car;
    public StopLine stopLine;

    public WaitingCar(CarController car, StopLine stopLine)
    {
        this.car = car;
        this.stopLine = stopLine;
    }

    public override string ToString()
    {
        if (car != null && stopLine != null)
        {
            return string.Format("{0} at stop {1}", this.car.name, this.stopLine.name);
        }
        else if (car != null)
        {
            return string.Format("{0} at null stop", this.car.name);
        }
        else if (stopLine != null)
        {
            return string.Format("null car at stop {0}", this.stopLine.name);
        }
        else
        {
            return "Null car & stopLine";
        }
    }
}

public class IntersectionManager : MonoBehaviour {
    WaitingCar m_waitingOnCar;
    public WaitingCar WaitingOnCar
    {
        get { return m_waitingOnCar; }
    }

	Queue<WaitingCar> cars = new Queue<WaitingCar>();
    public Queue<WaitingCar> CarQueue
    {
        get { return cars; }
    }

    IntersectionState m_state = IntersectionState.Empty;
	public IntersectionState State {
		get { return m_state; }
		set {
			m_state = value;
            if (m_state == IntersectionState.Empty)
            {
                m_waitingOnCar = new WaitingCar();
            }
		}
	}

	void Update() {
		if (State == IntersectionState.ReleaseNextCar) {
			if (cars.Count == 0) {
				State = IntersectionState.Empty;
			}
			else {
				WaitingCar waitingCar = cars.Dequeue();
                m_waitingOnCar = waitingCar;
                CarController car = waitingCar.car;

				car.StartTurn ();
                
                State = IntersectionState.WaitingForCar;
			}
		}
	}

	void OnDrawGizmos () {
		Gizmos.DrawIcon (transform.position, "Intersection.png", true);
	}

	public void Enqueue(CarController car, StopLine stopLine) {
		cars.Enqueue(new WaitingCar(car, stopLine));

        if (State == IntersectionState.Empty)
        {
            State = IntersectionState.ReleaseNextCar;
        }
	}

	public void SignalCarFinished() {
		if (State == IntersectionState.WaitingForCar) {
			State = IntersectionState.ReleaseNextCar;
		}
	}

    public void ResetIntersection()
    {
        cars.Clear();
        State = IntersectionState.Empty;
    }
}
