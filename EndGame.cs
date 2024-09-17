using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private bool endHistoryActive;
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
            if (!endHistoryActive)
            {
                InteractiveButton.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                endHistoryActive = true;
                InteractiveButton.SetActive(false);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                if (uiManager != null)
                {
                    uiManager.OpenEndHistory();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && _gameManager.gameState == GameState.PLAY)
        {
            InteractiveButton.SetActive(false);
            ExitHistory();
        }
    }

    public void ExitHistory()
    {
        uiManager.CloseEndHistory();
        _gameManager.ChangeGameState(GameState.PLAY);
        endHistoryActive = false;
    }
}
