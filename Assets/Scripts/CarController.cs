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

	public float stopDistance = 1f;
    public List<Color> carColours = new List<Color>();

    public List<TurnIndex> turnInstructions;

	RaycastHit2D[] hits = new RaycastHit2D[100];
    public TurnIndicator turnIndicator = null;

	CarEngine m_carEngine;
	public CarEngine Engine {
		get { return m_carEngine ?? null; }
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
		m_carEngine = new CarEngine (transform);
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
        for (int i = 0; i < results; ++i)
        {
            CarController car = hits[i].collider.GetComponent<CarController>();
            if (car != null && car != this)
            {
                float angle = Vector2.Angle(transform.up, car.transform.up);
                if (angle <= 90f)
                {
                    m_carEngine.CurrentSpeed = 0f;
                    break;
                }
            }
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

	public void ResetCar() {
		m_carEngine.MaxSpeed = Random.Range (m_carEngine.MaxSpeed - 1f, m_carEngine.MaxSpeed + 1f);
		m_carEngine.MaxAcceleration = Random.Range (m_carEngine.MaxAcceleration - 1f, m_carEngine.MaxAcceleration + 1f);
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
		// Correct the remaining rotation to line up with the road.
		m_carEngine.RotateTo(finishLine.transform.up);

		// Correct the lateral position of the car to line up with the center of the finish line.
		Vector2 distance = finishLine.transform.position - transform.position;
		Vector3 lateralDistance = new Vector3(Mathf.Abs (finishLine.transform.up.y), Mathf.Abs (finishLine.transform.up.x), 0f);
		Vector3 lateralOffset = new Vector3(lateralDistance.x * distance.x, lateralDistance.y * distance.y, 0f);
		transform.position += lateralOffset;

		m_drivingState.ReplaceState (new DrivingStateDriving());
	}
}
