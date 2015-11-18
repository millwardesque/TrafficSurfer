using UnityEngine;
using UnityEngine.UI;
using System.Collections;


enum UIAchievementPanelState {
	Hidden,
	Visible
};

public class UIAchievementPanel : MonoBehaviour {
	public Text achievementName;
	public float showAchievementDuration = 2f;
	float countdown = 0f;

	UIAchievementPanelState m_state;
	UIAchievementPanelState State {
		get { return m_state; }
		set {
			m_state = value;
			if (m_state == UIAchievementPanelState.Hidden) {
				gameObject.SetActive(false);
			}
			else if (m_state == UIAchievementPanelState.Visible) {
				countdown = showAchievementDuration;
			}
		}
	}

	void Update() {
		if (m_state == UIAchievementPanelState.Visible) {
			countdown -= Time.deltaTime;
			if (countdown <= 0f) {
				State = UIAchievementPanelState.Hidden;
			}
		}
	}

	public void ShowAchievement(string achievementString) {
		achievementName.text = achievementString;
		State = UIAchievementPanelState.Visible;
	}

	public void HideAchievement() {
		State = UIAchievementPanelState.Hidden;
	}
}
