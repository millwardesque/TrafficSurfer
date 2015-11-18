using UnityEngine;
using System.Collections;

public class CarJumpAchievementTracker : MonoBehaviour {
	public int requiredJumps = 1;
	
	private int m_jumps = 0;
	public int Jumps { 
		get { return m_jumps; }
	}

	void Start() {
		MessageManager.Instance.AddListener("OnCarJump", OnLogCarJump);
		MessageManager.Instance.AddListener("RestartGame", OnRestartGame);
	}

	void OnLogCarJump(Message message) {
		LogCarJump();
	}

	void LogCarJump() {
		if (m_jumps >= requiredJumps) {
			return;
		}

		m_jumps++;
		if (m_jumps >= requiredJumps) {
			OnAchievementCompleted();
		}
	}

	void OnAchievementCompleted() {
		GUIManager.Instance.ShowAchievementPanel(gameObject.name);
	}

	void OnRestartGame(Message message) {
		ResetAchievement();
	}

	void ResetAchievement() {
		m_jumps = 0;
	}
}
