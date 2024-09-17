using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
     private GameManager _gameManager;

    void Start()
    {
        _gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "CamTrigger":
                _gameManager.ControlCam2(true);
                break;
            case "Diamond":
                _gameManager.UpdateDiamonds(1);
                Destroy(other.gameObject);
                break;
            case "BossTrigger":
                _gameManager.ControlCamBoss(true);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "CamTrigger":
                _gameManager.ControlCam2(false);
                break;
            case "BossTrigger":
                _gameManager.ControlCamBoss(false);
                break;
        }
    }
}
