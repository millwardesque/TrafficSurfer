using UnityEngine;
using System.Collections;

public class RoadLane : MonoBehaviour {
	RoadLaneTrigger m_entryTrigger;
	RoadLaneTrigger m_exitTrigger;

	RoadSegment m_road;
	public RoadSegment Road {
		get { return m_road; }
		set { m_road = value; }
	}

	public Vector2 Heading {
		get { return (Vector2)(m_exitTrigger.transform.position - m_entryTrigger.transform.position); }
	}

	public Vector2 EndPoint {
		get { return (Vector2)m_exitTrigger.transform.position; }
	}

	void Start() {
		RoadLaneTrigger[] triggers = GetComponentsInChildren<RoadLaneTrigger> ();
		foreach (RoadLaneTrigger trigger in triggers) {
			trigger.Lane = this;

			if (trigger.type == RoadLaneTriggerType.Entry) {
				m_entryTrigger = trigger;
			} else if (trigger.type == RoadLaneTriggerType.Exit) {
				m_exitTrigger = trigger;
			}
		}
	}

	public void OnLaneEntry(RoadLaneTrigger entryPoint, CarController car) {

	}

	public void OnLaneExit(RoadLaneTrigger exitPoint, CarController car) {

	}
}