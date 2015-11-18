using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

enum UIAchievementPanelState {
	Hidden,
	Visible
};

public class UIAchievementPanel : MonoBehaviour {
	public Text achievementName;
	public float showAchievementDuration = 2f;
	float countdown = 0f;
	Queue<string> achievementQueue = new Queue<string>();
	
	UIAchievementPanelState m_state;
	UIAchievementPanelState State {
		get { return m_state; }
		set {
			m_state = value;
			if (m_state == UIAchievementPanelState.Hidden) {
				gameObject.SetActive(false);
			}
			else if (m_state == UIAchievementPanelState.Visible) {
				achievementName.text = achievementQueue.Dequeue();
				countdown = showAchievementDuration;
			}
		}
	}

	void Update() {
		if (m_state == UIAchievementPanelState.Visible) {
			countdown -= Time.unscaledDeltaTime;
			if (countdown <= 0f) {
				if (achievementQueue.Count > 0) {
					State = UIAchievementPanelState.Visible;
				}
				else {
					State = UIAchievementPanelState.Hidden;
				}
			}
		}
	}

	public void ShowAchievement(string achievementString) {
		achievementQueue.Enqueue(achievementString);

		if (m_state != UIAchievementPanelState.Visible) {
			State = UIAchievementPanelState.Visible;
		}
	}

	public void HideAchievement() {
		achievementQueue.Clear();
		State = UIAchievementPanelState.Hidden;
	}
}
