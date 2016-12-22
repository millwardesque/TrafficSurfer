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
			car.Engine.UpdateCarPhysics ();
		} else {
			car.StartDriving ();
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

		// Calculate the angular velocity needed to complete the turn.
		// FPos(x,y) = (AngVel(x,y)/2)*T^2 + LinVel(x,y)*T + IPos(x,y)
		// car.TurnDestination = (AngVel(x,y)/2) * (m_turnLength^2) + car.Engine.CurrentVelocity * m_turnLength + car.transform.position;
		// (AngVel(x,y)/2) * (m_turnLength^2) = -car.TurnDestination + car.Engine.CurrentVelocity * m_turnLength + car.transform.position;
		// AngVel(x, y) = 2f * (-car.TurnDestination + car.Engine.CurrentVelocity * m_turnLength + car.transform.position) / (m_turnLength^2)
		float angularVelocityInRadians = 2f * (-car.TurnDestination + car.Engine.CurrentVelocity * m_turnLength + (Vector2)car.transform.position).magnitude / (m_turnLength * m_turnLength);

		// Calculate the wheel angle needed to reach the required. angular velocity.
		// car.Engine.CurrentSpeed / radius = angularVelocityInRadians
		// radius = car.Engine.CurrentSpeed / angularVelocityInRadians
		// Mathf.Sin (wheelAngle * Mathf.Deg2Rad) = WheelbaseLength / (car.Engine.CurrentSpeed / angularVelocityInRadians)
		// wheelAngle = Arcsin(WheelbaseLength / (car.Engine.CurrentSpeed / angularVelocityInRadians)) * Mathf.Rad2Deg
		float wheelAngle = Mathf.Asin(car.Engine.WheelbaseLength / (car.Engine.CurrentSpeed / angularVelocityInRadians)) * Mathf.Rad2Deg;

		Vector3 turnDirection = ((Vector3)car.TurnDestination - car.transform.position).normalized;
		Vector3 cross = Vector3.Cross(car.Heading, turnDirection);
		if (cross.z >= 0f) {
			car.Engine.CurrentTireRotation = wheelAngle;
		} else {
			car.Engine.CurrentTireRotation = -wheelAngle;
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

/**
 * State for when the car is steering towards a heading.
 */
public class DrivingStateDrivingToHeading : DrivingState {
	Vector2 m_targetHeading;
	float m_turnDuration = 0.5f;

	public override void Enter(CarController car) {
		car.Engine.CurrentThrottle = 1f;
		car.Engine.CurrentBrake = 0f;

		m_targetHeading = car.TargetHeading;
		CalculateUpdatedHeading (car);
	}

	public override void Update(CarController car) {
		car.CheckForOtherCars();
		if (car.CollisionIsPossible) {
			car.StopCar ();
		} else {
			// Adjust tire angle to turn if necessary.
			if (m_targetHeading != car.TargetHeading) {
				m_targetHeading = car.TargetHeading;
			}

			CalculateUpdatedHeading (car);
			car.Engine.UpdateCarPhysics ();
		}
		Debug.DrawLine(car.transform.position, car.transform.position + car.Heading * car.stopDistance, Color.red);
	}

	void CalculateUpdatedHeading(CarController car) {
		// Don't attempt to turn if speed is zero.
		if (car.Engine.CurrentSpeed < float.Epsilon) {
			car.Engine.CurrentTireRotation = 0f;
			return;
		}

		// Calculate the angular velocity needed to reach the target heading
		// FPos(x,y) = (AngVel(x,y)/2)*T^2 + LinVel(x,y)*T + IPos(x,y)
		// car.TurnDestination = (AngVel(x,y)/2) * (m_turnLength^2) + car.Engine.CurrentVelocity * m_turnLength + car.transform.position;
		// (AngVel(x,y)/2) * (m_turnLength^2) = -car.TurnDestination + car.Engine.CurrentVelocity * m_turnLength + car.transform.position;
		// AngVel(x, y) = 2f * (-car.TurnDestination + car.Engine.CurrentVelocity * m_turnLength + car.transform.position) / (m_turnLength^2)
		float angularVelocityInRadians = 2f * (-m_targetHeading + car.Engine.CurrentVelocity * m_turnDuration + (Vector2)car.transform.position).magnitude / (m_turnDuration * m_turnDuration);

		// Calculate the wheel angle needed to reach the required. angular velocity.
		// car.Engine.CurrentSpeed / radius = angularVelocityInRadians
		// radius = car.Engine.CurrentSpeed / angularVelocityInRadians
		// Mathf.Sin (wheelAngle * Mathf.Deg2Rad) = WheelbaseLength / (car.Engine.CurrentSpeed / angularVelocityInRadians)
		// wheelAngle = Arcsin(WheelbaseLength / (car.Engine.CurrentSpeed / angularVelocityInRadians)) * Mathf.Rad2Deg
		float wheelAngle = Mathf.Asin(car.Engine.WheelbaseLength / (car.Engine.CurrentSpeed / angularVelocityInRadians)) * Mathf.Rad2Deg;
		Vector3 headingDirection = ((Vector3)m_targetHeading - car.transform.position).normalized;
		Vector3 cross = Vector3.Cross(car.Heading, headingDirection);
		if (cross.z >= 0f) {
			car.Engine.CurrentTireRotation = wheelAngle;
		} else {
			car.Engine.CurrentTireRotation = -wheelAngle;
		}

		Debug.Log (string.Format ("Wbl: {0}, Cs: {1}, Avr: {2}, Asin({3})", car.Engine.WheelbaseLength, car.Engine.CurrentSpeed, angularVelocityInRadians, car.Engine.WheelbaseLength / (car.Engine.CurrentSpeed / angularVelocityInRadians)));
	}
}