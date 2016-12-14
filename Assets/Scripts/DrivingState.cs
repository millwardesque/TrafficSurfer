using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * State machine for driving states.
 */
public class DrivingStateMachine {
	Stack<DrivingState> m_states;
	public DrivingState State {
		get { return m_states.Peek (); }
	}

	CarController m_car;

	public DrivingStateMachine(CarController car) {
		m_states = new Stack<DrivingState> ();
		m_car = car;

		m_states.Push (new DrivingStateStopped());
	}

	public void PushState(DrivingState state) {
		if (state == null) {
			Debug.LogError ("Error: Attempting to push null driving state onto car '" + m_car.name + "'");
			return;
		}

		if (m_states.Peek () != null) {
			m_states.Peek ().Exit (m_car);
		}

		m_states.Push (state);
		state.Enter (m_car);
	}

	public void PopState() {
		if (m_states.Peek() == null) {
			Debug.LogError ("Error: Attempting to pop last state from car '" + m_car.name + "'.");
			return;
		}

		m_states.Pop ().Exit (m_car);
	}

	public void ReplaceState(DrivingState state) {
		if (state == null) {
			Debug.LogError ("Error: Attempting to replace driving state for car '" + m_car.name + "' with null state");
			return;
		}

		if (m_states.Peek () != null) {
			m_states.Pop ().Exit (m_car);
		}

		m_states.Push (state);
		state.Enter (m_car);
	}

	public void ClearStates() {
		while (m_states.Count > 1) {
			PopState ();
		}
	}
}

/**
 * Base DrivingState class.
 */
public class DrivingState {
	public virtual void Enter(CarController car) { }
	public virtual void Update(CarController car) {	}
	public virtual void Exit(CarController car) { }
}

/**
 * State for when the car is driving.
 */
public class DrivingStateDriving : DrivingState {
	public override void Enter(CarController car) {
		car.Engine.CurrentTireRotation = 0f;
		car.Engine.CurrentThrottle = 1f;
		car.Engine.CurrentBrake = 0f;
	}

	public override void Update(CarController car) {
		Debug.DrawLine(car.transform.position, car.transform.position + car.Heading * car.stopDistance, Color.red);

		car.CheckForOtherCars();
		if (car.CollisionIsPossible) {
			car.StopCar ();
		} else {
			car.Engine.UpdateCarPhysics ();
		}
	}
}

/**
 * State for when the car is stopped.
 */
public class DrivingStateStopped : DrivingState {
	public override void Enter(CarController car) {
		car.Engine.CurrentThrottle = 0f;
		car.Engine.CurrentBrake = 1f;
	}

	public override void Update(CarController car) {
		Debug.DrawLine(car.transform.position, car.transform.position + car.Heading * car.stopDistance, Color.red);

		car.CheckForOtherCars();
		if (car.CollisionIsPossible) {
			car.StartDriving ();
		} else {
			car.Engine.UpdateCarPhysics ();
		}
	}
}

/**
 * State for when the car is turning.
 */
public class DrivingStateTurning : DrivingState {
	float m_turnLength = 2f;

	public override void Enter(CarController car) {
		car.Engine.CurrentThrottle = 1f;
		car.Engine.CurrentBrake = 0f;

		Vector3 turnDirection = ((Vector3)car.TurnDestination - car.transform.position).normalized;
		float totalTurnAngle = Vector3.Angle(car.Heading, turnDirection);
		Vector3 cross = Vector3.Cross(car.Heading, turnDirection);
		if (cross.z >= 0f) {
			car.Engine.CurrentTireRotation = car.Engine.MaxTireRotation;
		} else {
			car.Engine.CurrentTireRotation = -1f * car.Engine.MaxTireRotation;
		}
	}

	public override void Update(CarController car) {
		car.CheckForOtherCars();
		car.Engine.UpdateCarPhysics ();

		Debug.DrawLine(car.transform.position, car.transform.position + car.Heading * car.stopDistance, Color.red);
	}

	public override void Exit(CarController car) {
		car.turnIndicator.DoneTurn();
	}
}