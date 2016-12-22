using UnityEngine;
using System.Collections;

public enum RoadLaneTriggerType {
	Entry,
	Exit
};

public class RoadLaneTrigger : MonoBehaviour
{
	public RoadLaneTriggerType type;

	RoadLane m_lane;
	public RoadLane Lane {
		get { return m_lane; }
		set { m_lane = value; }
	}

	void OnTriggerEnter2D(Collider2D col) {
		CarController car = col.GetComponent<CarController> ();
		if (car != null && m_lane != null) {

			if (type == RoadLaneTriggerType.Entry) {
				m_lane.OnLaneEntry (this, car);
			} else {
				m_lane.OnLaneExit (this, car);
			}
		}
	}
}

