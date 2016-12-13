using UnityEngine;
using System.Collections;

public class CarEngine {
	Transform m_car;

	float m_maxSpeed;
	public float MaxSpeed {
		get { return m_maxSpeed; }
		set {
			m_maxSpeed = value;
			if (m_currentSpeed > m_maxSpeed) {
				m_currentSpeed = m_maxSpeed;
			}
		}
	}

	float m_maxAcceleration;
	public float MaxAcceleration {
		get { return m_maxAcceleration; }
		set {
			m_maxAcceleration = value;
		}
	}

	float m_currentSpeed;
	public float CurrentSpeed {
		get { return m_currentSpeed; }
		set {
			m_currentSpeed = Mathf.Clamp(value, 0f, MaxSpeed);
		}
	}

	float m_degreesPerSecond;
	public float DegreesPerSecond {
		get { return m_degreesPerSecond; }
		set {
			m_degreesPerSecond = value;
		}
	}

	public CarEngine(Transform car) {
		m_car = car;
	}

	public void UpdateCarPhysics() {
		RotateBy(DegreesPerSecond * Time.deltaTime);

		CurrentSpeed += MaxAcceleration * Time.deltaTime;

		Vector3 distance = m_car.right * CurrentSpeed * Time.deltaTime;
		m_car.position += distance;
	}

	public void RotateTo(Vector2 direction) {
		float angle = Vector2.Angle(m_car.right, direction);
		Vector3 cross = Vector3.Cross((Vector3)m_car.right, (Vector3)direction);
		if (cross.z < 0f) {
			angle *= -1f;
		}

		RotateBy(angle);
	}

	public void RotateBy(float angle) {
		Quaternion newRotation = m_car.rotation;
		newRotation.eulerAngles = new Vector3(0f, 0f, m_car.rotation.eulerAngles.z + angle);
		m_car.rotation = newRotation;
	}
}