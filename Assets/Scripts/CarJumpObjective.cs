using UnityEngine;
using System.Collections;

public class CarJumpObjective : MonoBehaviour {
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
			OnObjectiveCompleted();
		}
	}

	void OnObjectiveCompleted() {
		string plural = (requiredJumps > 1 ? "s" : "");
		GUIManager.Instance.ShowObjectivePanel(string.Format ("Jump on {0} car{1}.", requiredJumps, plural));
	}

	void OnRestartGame(Message message) {
		ResetObjective();
	}

	void ResetObjective() {
		m_jumps = 0;
	}
}
