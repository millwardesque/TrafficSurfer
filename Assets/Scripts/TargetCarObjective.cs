﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetCarObjective : Objective {
	public int requiredJumps = 1;
	public int scorePerCompletedJump = 20;

	int originalRequiredJumps;
	
	private int m_jumps = 0;
	public int Jumps { 
		get { return m_jumps; }
	}

	void Awake() {
		originalRequiredJumps = requiredJumps;
	}
	
	void Start() {
		MessageManager.Instance.AddListener("OnCarJump", OnCarJump);
		MessageManager.Instance.AddListener("RestartGame", OnRestartGame);
	}
	
	void OnCarJump(Message message) {
		Dictionary<string, object> data = (Dictionary<string, object>)message.data;
		if (data.ContainsKey("IsTargetCar") && (bool)data["IsTargetCar"]) {
			LogCarJump();
		}
	}
	
	void LogCarJump() {
		if (m_jumps >= requiredJumps) {
			return;
		}
		
		m_jumps++;
		if (m_jumps >= requiredJumps) {
			OnObjectiveComplete();
		}
	}

	void OnRestartGame(Message message) {
		ResetObjective();
	}
	
	void ResetObjective() {
		m_jumps = 0;
	}
	
	public override string GetObjectiveDescription() {
		string plural = (requiredJumps > 1 ? "s" : "");
		return string.Format ("Jump on {0} target car{1}.", requiredJumps, plural);
	}

	public override void IncreaseDifficulty() {
		requiredJumps ++;
	}

	public override void ResetDifficulty() {
		requiredJumps = originalRequiredJumps;
	}

	protected override int GetCompletionScore() {
		return requiredJumps * scorePerCompletedJump;
	}
}
