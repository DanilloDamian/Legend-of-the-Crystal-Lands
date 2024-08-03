using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public GameObject vCam2;
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
                vCam2.SetActive(true);
                break;
            case "Diamond":
                _gameManager.UpdateDiamonds(1);
                Destroy(other.gameObject);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "CamTrigger":
                vCam2.SetActive(false);
                break;
        }
    }
}
