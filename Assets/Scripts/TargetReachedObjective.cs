using UnityEngine;
using System.Collections;

public class TargetReachedObjective : Objective {
    public int completionScore = 50;
    public string objectiveDescription;

    public override string GetObjectiveDescription() {
        if (objectiveDescription.Length > 0) {
            return objectiveDescription;
        }
        else {
            return "Reach the exit on a car";
        }        
    }

    protected override int GetCompletionScore() {
        return completionScore;
    }

    void OnTriggerEnter2D(Collider2D col) {
        PlayerController possiblePlayer = col.GetComponent<PlayerController>();
        if (possiblePlayer == GameManager.Instance.Player) {
            OnObjectiveComplete();
        }
    }
}
