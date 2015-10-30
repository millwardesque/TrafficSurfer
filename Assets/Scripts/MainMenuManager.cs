﻿using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
    static public MainMenuManager Instance = null;

    public UIHighScore highScoreUI;
    public GameObject highScorePanel;
    public GameObject optionsPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void OnNewGame()
    {
        Application.LoadLevel("Prototype");
    }

    public void OnOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void OnHighScores()
    {
        highScorePanel.SetActive(true);
        highScoreUI.RefreshHighScores();
    }

    public void OnBackToMainMenu()
    {
        highScorePanel.SetActive(false);
        optionsPanel.SetActive(false);
    }
}