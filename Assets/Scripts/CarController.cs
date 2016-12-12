using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TurnIndex
{
    LeftTurn = 0,
    Straight = 1,
    RightTurn = 2
}

public class CarController : MonoBehaviour {
	public static float TurnAngleVariance = 0.0001f;
	public CarData carData;

	public float maxSpeed = 1f;
	public float maxAcceleration = 0.8f;
	public float stopDistance = 1f;
    public List<Color> carColours = new List<Color>();

    public List<TurnIndex> turnInstructions;

	RaycastHit2D[] hits = new RaycastHit2D[100];
    public TurnIndicator turnIndicator = null;

	float m_currentSpeed = 0f;
	public float CurrentSpeed {
		get { return m_currentSpeed; }
		set { m_currentSpeed = value; }
	}
	
	Vector2 turnDestination;
	public Vector2 TurnDestination {
		get { return turnDestination; }
	}

	DrivingStateMachine m_drivingState;
	public DrivingState State {
		get { return m_drivingState != null ? m_drivingState.State : null; }
	}

	void Awake() {
		m_drivingState = new DrivingStateMachine (this);
	}

	void Start() {
		if (carData != null) {
			carData.Initialize (this);
		}

		turnIndicator = GetComponentInChildren<TurnIndicator>();
		ResetCar ();
	}

	// Update is called once per frame
	void Update () {
		m_drivingState.State.Update (this);
	}

    public void CheckForOtherCars()
    {
        int results = Physics2D.RaycastNonAlloc(transform.position, transform.up, hits, stopDistance);
		bool willHitCar = false;
        for (int i = 0; i < results; ++i)
        {
            CarController car = hits[i].collider.GetComponent<CarController>();
            if (car != null && car != this)
            {
                float angle = Vector2.Angle(transform.up, car.transform.up);
                if (angle <= 90f)
                {
                    m_currentSpeed = 0f;
                    willHitCar = true;
                    break;
                }
            }
        }
		if (!willHitCar && m_currentSpeed < maxSpeed)
		{
			m_currentSpeed += maxAcceleration * Time.deltaTime;
			m_currentSpeed = Mathf.Clamp(m_currentSpeed, 0f, maxSpeed);
		}
    }

	public void StopCar() {
		m_drivingState.ReplaceState (new DrivingStateStopped());
	}

	public void ChooseDirection(StopLine stopLine) {
        int myTurnIndex = 0;
        if (turnInstructions != null && turnInstructions.Count > 0) {
            myTurnIndex = (int)turnInstructions[0];
            turnInstructions.RemoveAt(0);

            switch (myTurnIndex) {
                case (int)TurnIndex.LeftTurn:
                    turnDestination = stopLine.GetLeftTurnDestination();
                    break;
                case (int)TurnIndex.Straight:
                    turnDestination = stopLine.GetStraightDestination();
                    break;
                case (int)TurnIndex.RightTurn:
                    turnDestination = stopLine.GetRightTurnDestination();
                    break;
                default:
                    break;
            }
        }
		else {
            List<Vector2> possibleDestinations = new List<Vector2>(3);
            if (stopLine.CanTurnLeft()) {
                possibleDestinations.Add(stopLine.GetLeftTurnDestination());
            }
            else {
                possibleDestinations.Add(Vector2.zero);
            }

            if (stopLine.CanGoStraight()) {
                possibleDestinations.Add(stopLine.GetStraightDestination());
            }
            else {
                possibleDestinations.Add(Vector2.zero);
            }

            if (stopLine.CanTurnRight()) {
                possibleDestinations.Add(stopLine.GetRightTurnDestination());
            }
            else {
                possibleDestinations.Add(Vector2.zero);
            }

            myTurnIndex = Random.Range(0, possibleDestinations.Count);
            while (possibleDestinations[myTurnIndex] == Vector2.zero) {
                myTurnIndex = Random.Range(0, possibleDestinations.Count);
            }
            turnDestination = possibleDestinations[myTurnIndex];
        }

        switch (myTurnIndex)
        {
           case (int)TurnIndex.LeftTurn:
                turnIndicator.TurnLeft();
                break;
            case (int)TurnIndex.Straight:
                turnIndicator.GoStraight();
                break;
            case (int)TurnIndex.RightTurn:
                turnIndicator.TurnRight();
                break;
            default:
                break;
        }
	}
	
	public void RotateTo(Vector2 direction) {
		float angle = Vector2.Angle(transform.right, direction);
		Vector3 cross = Vector3.Cross((Vector3)transform.right, (Vector3)direction);
		if (cross.z < 0f) {
			angle *= -1f;
		}

        RotateBy(angle);
	}

    public void RotateBy(float angle)
    {
        Quaternion newRotation = transform.rotation;
        newRotation.eulerAngles = new Vector3(0f, 0f, transform.rotation.eulerAngles.z + angle);
        transform.rotation = newRotation;
    }
	
	public void ResetCar() {
		maxSpeed = Random.Range (maxSpeed - 1f, maxSpeed + 1f);
		maxAcceleration = Random.Range (maxAcceleration - 1f, maxAcceleration + 1f);
		Colourize ();

		m_drivingState.ClearStates ();
		m_drivingState.PushState (new DrivingStateDriving());
	}

	void Colourize() {
		if (carColours.Count > 0) {
			GetComponent<SpriteRenderer>().color = carColours[Random.Range(0, carColours.Count)];
		}
		else
		{
			GetComponent<SpriteRenderer>().color = Color.white;
		}

		GetComponent<SpriteRenderer>().material.SetInt("_IsColourized", 1);
	}

	public void StartTurn() {
		m_drivingState.ReplaceState (new DrivingStateTurning());
	}

	public void FinishTurn(Transform finishLine) {
		RotateTo (finishLine.transform.up);

		// Correct the lateral position of the car to line up with the center of the finish line.
		Vector2 distance = finishLine.transform.position - transform.position;
		Vector3 lateralDistance = new Vector3(Mathf.Abs (finishLine.transform.up.y), Mathf.Abs (finishLine.transform.up.x), 0f);
		Vector3 lateralOffset = new Vector3(lateralDistance.x * distance.x, lateralDistance.y * distance.y, 0f);
		transform.position += lateralOffset;

		m_drivingState.ReplaceState (new DrivingStateDriving());
	}
}
