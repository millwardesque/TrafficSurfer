using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

enum UIObjectivePanelState {
	Hidden,
	Visible
};

public class UIObjectivePanel : MonoBehaviour {
	public Text objectiveName;
	public float showObjectiveDuration = 2f;
	float countdown = 0f;
	Queue<string> objectiveQueue = new Queue<string>();
	
	UIObjectivePanelState m_state;
	UIObjectivePanelState State {
		get { return m_state; }
		set {
			m_state = value;
			if (m_state == UIObjectivePanelState.Hidden) {
				gameObject.SetActive(false);
			}
			else if (m_state == UIObjectivePanelState.Visible) {
				objectiveName.text = objectiveQueue.Dequeue();
				countdown = showObjectiveDuration;
			}
		}
	}

	void Update() {
		if (m_state == UIObjectivePanelState.Visible) {
			countdown -= Time.unscaledDeltaTime;
			if (countdown <= 0f) {
				if (objectiveQueue.Count > 0) {
					State = UIObjectivePanelState.Visible;
				}
				else {
					State = UIObjectivePanelState.Hidden;
				}
			}
		}
	}

	public void ShowObjective(string objectiveString) {
		objectiveQueue.Enqueue(objectiveString);

		if (m_state != UIObjectivePanelState.Visible) {
			State = UIObjectivePanelState.Visible;
		}
	}

	public void HideObjective() {
		objectiveQueue.Clear();
		State = UIObjectivePanelState.Hidden;
	}
}
