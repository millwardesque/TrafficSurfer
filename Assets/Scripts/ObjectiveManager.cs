﻿using UnityEngine;
using System.Collections;

public class ObjectiveManager : MonoBehaviour {
	Objective[] objectives;

	public static ObjectiveManager Instance = null;

	void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

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

	public void ShowLevelObjectives() {
		string objectiveText = "";
		for (int i = 0; i < objectives.Length; ++i) {
			objectiveText = objectives[i].GetObjectiveDescription() + "\n";
		}
		GUIManager.Instance.OpenLevelObjectivesPanel(objectiveText);
	}
}
