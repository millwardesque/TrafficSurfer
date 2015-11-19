using UnityEngine;
using System.Collections;

public class ObjectiveManager : MonoBehaviour {
	Objective[] objectives;

	void Start() {
		objectives = GetComponentsInChildren<Objective>();

		MessageManager.Instance.AddListener("ObjectiveComplete", OnObjectiveComplete);
	}

	void OnObjectiveComplete(Message message) {
		bool areAllComplete = true;
		for (int i = 0; i < objectives.Length; ++i) {
			areAllComplete &= objectives[i].IsComplete;
		}

		if (areAllComplete) {
			MessageManager.Instance.SendMessage(new Message(this, "AllObjectivesComplete", null));
		}
	}
}
