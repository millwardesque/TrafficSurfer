using UnityEngine;
using System.Collections;

public class CarEngine {
	CarController m_car;
	Rigidbody2D m_carRB;

	float m_enginePower = 40f;
	public float EnginePower {
		get { return m_enginePower; }
		set { m_enginePower = Mathf.Clamp(value, 0f, value); }
	}

	float m_dragCoefficient = 0.426f;
	public float DragCoefficient {
		get { return m_dragCoefficient; }
		set { m_dragCoefficient = Mathf.Clamp (value, 0f, value); }
	}

	public float RollResistanceCoefficient {
		get { return 30f * m_dragCoefficient; }
	}

	float m_brakingCoefficient = 0.3f;
	public float BrakingCoefficient {
		get { return m_brakingCoefficient; }
		set { m_brakingCoefficient = Mathf.Clamp (value, 0f, value); }
	}

	float m_currentThrottle = 0f;
	public float CurrentThrottle {
		get { return m_currentThrottle; }
		set { m_currentThrottle = Mathf.Clamp01 (value); }
	}

	float m_currentBrake = 0f;
	public float CurrentBrake {
		get { return m_currentBrake; }
		set { m_currentBrake = Mathf.Clamp01 (value); }
	}

	float m_maxTireRotation = 45f;
	public float MaxTireRotation {
		get { return m_maxTireRotation; }
		set { m_maxTireRotation = Mathf.Clamp (value, 0, 90f); }	// Limit tire rotation to 90 degrees
	}

	float m_currentTireRotation = 0f;
	public float CurrentTireRotation {
		get { return m_currentTireRotation; }
		set { m_currentTireRotation = Mathf.Clamp (value, -MaxTireRotation, MaxTireRotation); }
	}

	float m_wheelbaseLength = 1f;
	public float WheelbaseLength {
		get { return m_wheelbaseLength; }
		set { m_wheelbaseLength = Mathf.Clamp (value, 0f, value); }
	}

	public float CurrentSpeed {
		get { return m_carRB.velocity.magnitude; }
	}

	public Vector2 CurrentVelocity {
		get { return m_carRB.velocity; }
	}

	public CarEngine(CarController car) {
		m_car = car;
		m_carRB = car.GetComponentInChildren<Rigidbody2D> ();
	}

	public void UpdateCarPhysics() {
		bool isAtRest = Vector2.Dot (m_carRB.velocity, m_car.Heading) < 0.1f;
		Vector2 dragForce = -1f * DragCoefficient * m_carRB.velocity * m_carRB.velocity.magnitude;
		Vector2 rollResistanceForce = -1f * RollResistanceCoefficient * m_carRB.velocity;

		if (CurrentThrottle > Mathf.Epsilon) {
			Vector2 tractionForce = CurrentThrottle * EnginePower * m_car.Heading;
			m_carRB.AddForce (tractionForce);
			m_carRB.AddForce (dragForce);
			m_carRB.AddForce (rollResistanceForce);
		}
		else if (CurrentBrake > Mathf.Epsilon) {
			if (!isAtRest) {
				Vector2 brakingForce = CurrentBrake * -1f * BrakingCoefficient * m_car.Heading;
				m_carRB.AddForce (brakingForce);
				m_carRB.AddForce (dragForce);
				m_carRB.AddForce (rollResistanceForce);
			} else {
				m_carRB.velocity = Vector2.zero;
			}
		}

		if (Mathf.Abs (CurrentTireRotation) < float.Epsilon) {
			m_carRB.angularVelocity = 0f;
		} else {
			float radius = WheelbaseLength / Mathf.Sin (CurrentTireRotation * Mathf.Deg2Rad);
			float angularVelocityInRadians = m_carRB.velocity.magnitude / radius;
			m_carRB.angularVelocity = angularVelocityInRadians * Mathf.Rad2Deg;
		}
	}
}