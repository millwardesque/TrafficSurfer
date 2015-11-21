using UnityEngine;
using System.Collections;

public class Objective : MonoBehaviour {
	protected bool m_isComplete = false;
	public bool IsComplete {
		get { return m_isComplete; }
	}


	void Awake() {
		Debug.Log ("Awake");
	}

	public virtual string GetObjectiveDescription() {
		return name;
	}

	protected virtual void OnObjectiveComplete() {
		m_isComplete = true;
		GUIManager.Instance.ShowObjectivePanel(GetObjectiveDescription());
		ScoreManager.Instance.Score += GetCompletionScore();
		MessageManager.Instance.SendMessage(new Message(this, "ObjectiveComplete", null));
	}

	public virtual void IncreaseDifficulty() { }
	public virtual void ResetDifficulty() {	}
	protected virtual int GetCompletionScore() { return 0; }
}