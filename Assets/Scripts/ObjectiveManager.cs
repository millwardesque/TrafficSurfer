using UnityEngine;
using System.Collections;

public class ObjectiveManager : MonoBehaviour {
	Objective[] objectives;

	public static ObjectiveManager Instance = null;

	void Awake() {
		if (Instance == null) {
			Instance = this;
			objectives = GetComponentsInChildren<Objective>(true);
		}
		else {
			Destroy(gameObject);
		}
	}

	void Start() {
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

	public string GetLevelObjectiveString() {
		string objectiveText = "";
		for (int i = 0; i < objectives.Length; ++i) {
			objectiveText = objectives[i].GetObjectiveDescription();
			if (i + 1 < objectives.Length) {
				objectiveText += "\n";
			}
		}

		return objectiveText;
	}

	public Sprite GetLevelObjectiveSprite() {
		Sprite objectiveSprite = null;

		if (objectives.Length > 0) {
			objectiveSprite = objectives[0].GetObjectiveSprite();
		}
		return objectiveSprite;
	}

	public void ShowLevelObjectives() {
		GUIManager.Instance.OpenLevelObjectivesPanel(GetLevelObjectiveString(), GetLevelObjectiveSprite());
	}

	public void IncreaseDifficulty() {
		for (int i = 0; i < objectives.Length; ++i) {
			objectives[i].IncreaseDifficulty();
		}
	}

	public void ResetDifficulty() {
		for (int i = 0; i < objectives.Length; ++i) {
			objectives[i].ResetDifficulty();
		}
	}
}
