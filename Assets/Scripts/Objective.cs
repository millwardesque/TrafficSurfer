using UnityEngine;
using System.Collections;

public class Objective : MonoBehaviour {
	protected bool m_isComplete = false;
	public bool IsComplete {
		get { return m_isComplete; }
	}

	public virtual string GetObjectiveDescription() {
		return name;
	}

	protected virtual void OnObjectiveComplete() {
		m_isComplete = true;
		GUIManager.Instance.ShowObjectivePanel(GetObjectiveDescription());
		MessageManager.Instance.SendMessage(new Message(this, "ObjectiveComplete", null));
	}

	public virtual void IncreaseDifficulty() { }
}
