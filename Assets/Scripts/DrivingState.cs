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

	public override void Update(CarController car) {
		car.CheckForOtherCars();

		Vector2 distance = car.transform.right * car.CurrentSpeed * Time.deltaTime;
		car.transform.position += (Vector3)distance;

		Debug.DrawLine(car.transform.position, car.transform.position + car.transform.right * car.stopDistance, Color.red);
	}
}

/**
 * State for when the car is stopped.
 */
public class DrivingStateStopped : DrivingState {
	public override void Enter(CarController car) {
		car.CurrentSpeed = 0f;
	}
}

/**
 * State for when the car is turning.
 */
public class DrivingStateTurning : DrivingState {
	public override void Update(CarController car) {
		car.CheckForOtherCars();

		Vector2 turnDirection = (car.TurnDestination - (Vector2)car.transform.position).normalized;

		float angle = Vector2.Angle(car.transform.right, turnDirection);
		angle = Mathf.Clamp(angle, 0f, 1f);
		Vector3 cross = Vector3.Cross((Vector3)car.transform.right, (Vector3)turnDirection);
		if (cross.z < 0f)
		{
			angle *= -1f;
		}

		car.RotateBy(angle);

		Vector2 distance = car.transform.right * car.CurrentSpeed * Time.deltaTime;
		car.transform.position += (Vector3)distance;

		Debug.DrawLine(car.transform.position, car.transform.position + car.transform.right * car.stopDistance, Color.red);
	}
	
	public override void Exit(CarController car) {
		car.turnIndicator.DoneTurn();
	}
}