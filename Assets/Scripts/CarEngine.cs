using UnityEngine;
using System.Collections;

public class CarEngine {
	Rigidbody2D m_car;

	float m_enginePower = 1f;
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

	float m_brakingCoefficient = 0.5f;
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

	float m_maxTireRotation = 20f;
	public float MaxTireRotation {
		get { return m_maxTireRotation; }
		set { m_maxTireRotation = Mathf.Clamp (value, -90f, 90f); }	// Limit tire rotation to 90 degrees in either direction
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
		get { return m_car.velocity.magnitude; }
	}

	public CarEngine(Rigidbody2D car) {
		m_car = car;
	}

	public void UpdateCarPhysics() {
		bool isAtRest = Vector2.Dot (m_car.velocity, m_car.transform.right) < 0.1f;
		Vector2 dragForce = -1f * DragCoefficient * m_car.velocity * m_car.velocity.magnitude;
		Vector2 rollResistanceForce = -1f * RollResistanceCoefficient * m_car.velocity;

		if (CurrentThrottle > Mathf.Epsilon) {
			Vector2 tractionForce = CurrentThrottle * EnginePower * m_car.transform.right;
			m_car.AddForce (tractionForce);
			m_car.AddForce (dragForce);
			m_car.AddForce (rollResistanceForce);
		}
		else if (CurrentBrake > Mathf.Epsilon) {
			if (!isAtRest) {
				Vector2 brakingForce = CurrentBrake * -1f * BrakingCoefficient * m_car.transform.right;
				m_car.AddForce (brakingForce);
				m_car.AddForce (dragForce);
				m_car.AddForce (rollResistanceForce);
			} else {
				m_car.velocity = Vector2.zero;
			}
		}

		if (Mathf.Abs (CurrentTireRotation) < float.Epsilon) {
			m_car.angularVelocity = 0f;
		} else {
			float radius = WheelbaseLength / Mathf.Sin (CurrentTireRotation);
			float angularVelocityInRadians = m_car.velocity.magnitude / radius;
			m_car.angularVelocity = angularVelocityInRadians * 180f / Mathf.PI;
		}
	}
}