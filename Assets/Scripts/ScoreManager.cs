using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighScore {
	public string name;
	public int score;

	public HighScore() {
		name = "";
		score = 0;
	}

	public HighScore(string name, int score) {
		this.name = name;
		this.score = score;
	}

	public override string ToString() {
		return string.Format("{0}: {1}", this.name, this.score);
	}
}

public class ScoreManager : MonoBehaviour {
	public static ScoreManager Instance = null;
	public static string highScorePath = "highscores.txt";
	public int maxHighScores = 5;

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
		RestartGame ();
	}

	public void RestartGame() {
		Score = 0;
	}

	public List<HighScore> GetHighScores() {
		List<HighScore> scores = new List<HighScore>();

		// Get existing high scores
		if (ES2.Exists(ScoreManager.highScorePath)) {
			scores = ES2.LoadList<HighScore>(ScoreManager.highScorePath);
		}

		return scores;
	}

	public void AddScore(HighScore score) {
		if (score.score < 1) {
			return;
		}

		bool hasInserted = false;
		List<HighScore> scores = GetHighScores();
		for (int i = 0; i < scores.Count; ++i) {
			if (score.score >= scores[i].score) {
				scores.Insert(i, score);
				hasInserted = true;
				break;
			}
		}

		if (!hasInserted) {
			scores.Add(score);
		}

		// Trim the list length to the max number of entries in the list.
		while (scores.Count > maxHighScores) {
			scores.RemoveAt(scores.Count - 1);
		}
		ES2.Save(scores, ScoreManager.highScorePath);
	}

	void DumpHighScores(List<HighScore> scores) {
		Debug.Log ("Dumping high-scores");
		for(int i = 0; i < scores.Count; ++i) {
			Debug.Log (scores[i].ToString());
		}
	}
}
