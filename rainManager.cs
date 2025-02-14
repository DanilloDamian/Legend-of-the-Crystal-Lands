﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rainManager : MonoBehaviour
{
    private GameManager _gameManager;
    public bool isRain;

    void Start()
    {
        _gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            _gameManager.OnOffRaind(isRain);            
        }
    }
}
