using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSegmentEntry : MonoBehaviour {
	float m_roadLength = 0f;

	public Vector2 Heading {
		get { return transform.right; }
	}

	public Vector2 EndPoint {
		get { return (Vector2)transform.position + Heading * m_roadLength; }
	}

	void Awake() {
		m_roadLength = GetComponentInParent<Sprite> ().bounds.extents.x * 2f;
	}
}
