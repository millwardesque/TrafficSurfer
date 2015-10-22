using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {
	public static ScoreManager Instance = null;

	int m_score = 0;
	public int Score {
		get { return m_score; }
		set {
			m_score = value;
			GUIManager.Instance.UpdateScoreLabel(m_score);
		}
	}
	
	void Awake () {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	void Start() {
		Score = 0;
	}
}
