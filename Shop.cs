﻿using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private bool shopActive;
    public UIManager uiManager;
    public GameObject InteractiveButton;
    private GameManager _gameManager;

    void Start()
    {
        _gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
    }


    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && _gameManager.gameState == GameState.PLAY)
        {
            if (!shopActive)
            {
                InteractiveButton.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                shopActive = true;
                InteractiveButton.SetActive(false);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                if (uiManager != null)
                {
                    uiManager.OpenShop();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && _gameManager.gameState == GameState.PLAY)
        {
            InteractiveButton.SetActive(false);
            ExitShop();
        }
    }

    public void ExitShop()
    {
        uiManager.CloseShop();
        _gameManager.ChangeGameState(GameState.PLAY);
        shopActive = false;
    }
}


