using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadSegment : MonoBehaviour
{
	List<RoadLane> m_lanes;

	public float speedLimit = 60f;

	// Use this for initialization
	void Start ()
	{
		m_lanes = new List<RoadLane> (GetComponentsInChildren<RoadLane> ());
		foreach (RoadLane lane in m_lanes) {
			lane.Road = this;
		}
	}
	
	public RoadLane GetRightLane(RoadLane lane) {
		int index = m_lanes.IndexOf (lane);
		if (index + 1 < m_lanes.Count) {
			return m_lanes [index + 1];
		}
		else {
			return null;
		}
	}

	public RoadLane GetLeftLane(RoadLane lane) {
		int index = m_lanes.IndexOf (lane);
		if (index - 1 >= 0) {
			return m_lanes [index - 1];
		}
		else {
			return null;
		}
	}
}

