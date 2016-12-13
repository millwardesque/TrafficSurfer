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

		m_states.Push (new DrivingStateDriving());
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
		car.Engine.DegreesPerSecond = 0f;
	}

	public override void Update(CarController car) {
		car.CheckForOtherCars();
		car.Engine.UpdateCarPhysics ();

		Debug.DrawLine(car.transform.position, car.transform.position + car.transform.right * car.stopDistance, Color.red);
	}
}

/**
 * State for when the car is stopped.
 */
public class DrivingStateStopped : DrivingState {
	public override void Enter(CarController car) {
		car.Engine.CurrentSpeed = 0f;
		car.Engine.DegreesPerSecond = 0f;
	}

	public override void Update(CarController car) {
		car.CheckForOtherCars();
		car.Engine.UpdateCarPhysics ();

		Debug.DrawLine(car.transform.position, car.transform.position + car.transform.right * car.stopDistance, Color.red);
	}
}

/**
 * State for when the car is turning.
 */
public class DrivingStateTurning : DrivingState {
	float m_turnLength = 2f;

	public override void Enter(CarController car) {
		Vector3 turnDirection = ((Vector3)car.TurnDestination - car.transform.position).normalized;
		float totalTurnAngle = Vector3.Angle(car.transform.right, turnDirection);
		Vector3 cross = Vector3.Cross(car.transform.right, turnDirection);
		if (cross.z < 0f) {
			totalTurnAngle *= -1f;
		}

		// Locks all turns at 0, 90, or -90
		// @TODO Replace this with some proper trig.
		if (totalTurnAngle < -1f) {
			totalTurnAngle = -90f;
		} else if (totalTurnAngle > 1f) {
			totalTurnAngle = 90f;
		} else {
			totalTurnAngle = 0f;
		}

		car.Engine.DegreesPerSecond = totalTurnAngle / m_turnLength;

		float turnRadius = 2f;	// Approximate radius of the turn.  Taken from the top-corner of the intersection tile (which is 2x2 units)
		float arcLength = turnRadius * 2f * Mathf.PI * totalTurnAngle / 360f; 
		car.Engine.CurrentSpeed = arcLength / m_turnLength;
	}

	public override void Update(CarController car) {
		Vector2 towardNextTrigger = (car.TurnDestination - (Vector2)car.transform.position);
		float targetRot = Vector2.Angle (Vector2.right, towardNextTrigger);
		if (towardNextTrigger.y < 0.0f) {
			targetRot *= -1f;
		}

		float rot = Mathf.MoveTowardsAngle (car.transform.localEulerAngles.z, targetRot, 100f);
		car.Engine.DegreesPerSecond = rot;

		car.CheckForOtherCars();
		car.Engine.UpdateCarPhysics ();

		Debug.DrawLine(car.transform.position, car.transform.position + car.transform.right * car.stopDistance, Color.red);
	}

	public override void Exit(CarController car) {
		car.turnIndicator.DoneTurn();
	}
}