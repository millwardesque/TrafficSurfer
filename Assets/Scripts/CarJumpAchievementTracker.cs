using UnityEngine;
using System.Collections;

public class CarJumpAchievementTracker : MonoBehaviour {
	public int requiredJumps = 1;
	
	private int m_jumps = 0;
	public int Jumps { 
		get { return m_jumps; }
	}

	void Start() {
		MessageManager.Instance.AddListener("OnCarJump", OnCarJump);
		MessageManager.Instance.AddListener("RestartGame", OnRestartGame);
	}

	void OnCarJump(Message message) {
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
		string plural = (requiredJumps > 1 ? "s" : "");
		GUIManager.Instance.ShowAchievementPanel(string.Format ("Jump on {0} car{1}.", requiredJumps, plural));
	}

	void OnRestartGame(Message message) {
		ResetAchievement();
	}

	void ResetAchievement() {
		m_jumps = 0;
	}
}
