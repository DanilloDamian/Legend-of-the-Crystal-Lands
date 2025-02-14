﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject[] hearts;
    [SerializeField]
    private GameObject uiShop;
    public GameObject[] initialHistoryTexts;
    public GameObject arrowNextImage;
    public GameObject historyPanel;
    public GameObject endHistoryPanel;
    public bool arrowActive;    
    private int textIndex = 0;
    private GameManager _gameManager;

    void Start()
    {
        _gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
        Time.timeScale = 0.05f;
    }

    void FixedUpdate()
    {
        if (_gameManager.historyActive)
        {
            StopAllCoroutines();
            StartCoroutine(HistoryInitial());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            initialHistoryTexts[textIndex].gameObject.SetActive(false);
            textIndex++;
            if (textIndex < initialHistoryTexts.Length)
            {
                initialHistoryTexts[textIndex].gameObject.SetActive(true);
            }
            else
            {
                _gameManager.historyActive = false;
                CloseHistory();
                Time.timeScale = 1f;
            }
        }
    }

    public void UpdateLifes(int life)
    {
        switch (life)
        {
            case 0:
                hearts[2].SetActive(false);
                hearts[1].SetActive(false);
                hearts[0].SetActive(false);
                break;
            case 1:
                hearts[2].SetActive(false);
                hearts[1].SetActive(false);
                hearts[0].SetActive(true);
                break;
            case 2:
                hearts[2].SetActive(false);
                hearts[1].SetActive(true);
                hearts[0].SetActive(true);
                break;
            case 3:
                hearts[2].SetActive(true);
                hearts[1].SetActive(true);
                hearts[0].SetActive(true);
                break;
        }
    }

    public void OpenShop()
    {
        uiShop.SetActive(true);
    }

    public void CloseShop()
    {
        uiShop.SetActive(false);
    }

    public void CloseHistory()
    {
        historyPanel.SetActive(false);
    }

    public void OpenHistory()
    {
        historyPanel.SetActive(true);
    }

    public void CloseEndHistory()
    {
        endHistoryPanel.SetActive(false);
    }

    public void OpenEndHistory()
    {
        endHistoryPanel.SetActive(true);
    }

    IEnumerator HistoryInitial()
    {
        arrowActive = !arrowActive;
        arrowNextImage.SetActive(arrowActive);
        yield return new WaitForSeconds(1f);
    }
}
