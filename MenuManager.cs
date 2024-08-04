﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private GameManager _gameManager;


    void Start()
    {
        _gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
    }

    public void ResumeGame()
    {
        _gameManager.ChangeGameState(GameState.PLAY);
    }

    public void RestarGame()
    {
        _gameManager.RestartGame();
    }

    public void ReturnMenu()
    {
        _gameManager.ReturnMenu();
    }
}
