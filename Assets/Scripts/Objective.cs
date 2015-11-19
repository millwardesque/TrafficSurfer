using UnityEngine;
using System.Collections;

public class Objective : MonoBehaviour {
	protected bool m_isComplete = false;
	public bool IsComplete {
		get { return m_isComplete; }
	}

	protected virtual void OnObjectiveComplete() { }
}
