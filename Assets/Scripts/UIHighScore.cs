using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIHighScore : MonoBehaviour {
	Text[] highScoreLabels;
	
	public void RefreshHighScores() {
		highScoreLabels = GetComponentsInChildren<Text>();
		List<HighScore> scores = ScoreManager.Instance.GetHighScores();

		for (int i = 0; i < scores.Count; ++i) {
			if (i < highScoreLabels.Length) {
                highScoreLabels[i].text = string.Format("{0}: {1}", i + 1, scores[i].ToString());
			}
		}

        // Clear any labels that don't have corresponding high scores.
        for (int i = scores.Count; i < highScoreLabels.Length; ++i)
        {
            highScoreLabels[i].text = string.Format("{0}: {1}", i + 1, "<none>");
        }
	}
}
