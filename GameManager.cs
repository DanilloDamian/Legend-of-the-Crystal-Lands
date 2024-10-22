﻿using Cinemachine;
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public enum enemyState
{
    IDLE, ALERT, PATROL, FOLLOW, FURY, DIE
}

public enum GameState
{
    PLAY, GAMEOVER, PAUSE
}

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    public GameObject playerPrefab;
    public GameObject playerInstance;
    public PlayerController playerController;
    public GameObject menuGameOver;
    public CinemachineVirtualCamera vcam1;
    public CinemachineVirtualCamera vcam2;
    public CinemachineVirtualCamera vcam3;
    public GameObject canvas;
    public UIManager uiManager;
    public bool historyActive = true;

    [Header("Menu Config")]
    public GameObject buttonRestart;
    public GameObject buttonResume;
    public GameObject buttonQuit;
    public Text textMenu;
    public Text textRestart;
    public GameObject textShopAccess;
    private bool isNewGame = true;
    public int timeRestart = 5;


    [Header("Inventory")]
    public Text txtDiamonds;
    public int diamonds =0;

    [Header("Enemy IA")]
    public float slimeIdleWaitTime = 4f;
    public float slimeDistanceAttack = 2.3f;
    public float slimeAlertTime = 3f;
    public float slimeAttackDelay = 1f;
    public float slimeLookAtSpeed = 1f;
    public EnemyManager enemyManager;

    [Header("Boss")]
    public float bossAttackDelay = 10f;
    public float bossDistanceAttack = 5f;

    public Transform[] slimeWaitPoints;

    [Header("Rain manager")]
    public PostProcessVolume postB;
    public ParticleSystem rainParticle;
    private ParticleSystem.EmissionModule rainModule;
    public int rainRateOverTime;
    public int rainIncrement;
    public float rainIncrementDelay;

    [Header("Drop")]
    public GameObject diamondPrefab;
    public int chanceDropDiamond = 50;

    [Header("Audio")]
    public AudioSource audioBackground;
    public AudioSource audioRain;
    public AudioSource audioGameOver;
    public AudioSource audioMenuConfirm;
    public AudioSource audioAttackSlime;
    public AudioSource audioSword;
    public AudioSource audioPlayerTakeDamage;
    public AudioSource audioBossFight;
    public AudioSource audioPeace;
    private bool audioHasPlayed;


    void Start()
    {
        rainModule = rainParticle.emission;
        audioBackground.Play();
        playerInstance = Instantiate(playerPrefab);
        uiManager = canvas.GetComponent<UIManager>();
        playerController = playerInstance.GetComponent<PlayerController>();
        enemyManager = FindObjectOfType(typeof(EnemyManager)) as EnemyManager;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {        
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameState == GameState.PLAY)
            {
                isNewGame = false;
                textShopAccess.SetActive(false);
                ChangeGameState(GameState.PAUSE);
            }
        }

        if (vcam1 != null && vcam2 != null && vcam3 != null)
        {
            UpdateCams(playerInstance.transform);
        }     
        
    }

    public void UpdateCams(Transform focus)
    {
        vcam1.Follow = focus;
        vcam2.Follow = focus;
        vcam3.Follow = focus;
    }

    public void OnOffRaind(bool isRain)
    {
        StopCoroutine("RainManager");
        StopCoroutine("PostBManager");
        StartCoroutine("RainManager", isRain);
        StartCoroutine("PostBManager", isRain);
    }

    IEnumerator RainManager(bool isRain)
    {
        switch (isRain)
        {
            case true:
                audioRain.Play();
                for (float r = rainModule.rateOverTime.constant; r < rainRateOverTime; r += rainIncrement)
                {
                    rainModule.rateOverTime = r;
                    yield return new WaitForSeconds(rainIncrementDelay);
                }
                rainModule.rateOverTime = rainRateOverTime;
                break;
            case false:
                audioRain.Stop();
                for (float r = rainModule.rateOverTime.constant; r > 0; r -= rainIncrement)
                {
                    rainModule.rateOverTime = r;
                    yield return new WaitForSeconds(rainIncrementDelay);
                }
                rainModule.rateOverTime = 0;
                break;
        }
    }

    IEnumerator PostBManager(bool isRain)
    {
        switch (isRain)
        {
            case true:
                for (float w = postB.weight; w < 1; w += 1 * Time.deltaTime)
                {
                    postB.weight = w;
                    yield return new WaitForEndOfFrame();
                }
                postB.weight = 1;
                break;
            case false:
                for (float w = postB.weight; w > 0; w -= 1 * Time.deltaTime)
                {
                    postB.weight = w;
                    yield return new WaitForEndOfFrame();
                }
                postB.weight = 0;
                break;
        }
    }

    public void ChangeGameState(GameState newGameState)
    {
        gameState = newGameState;

        switch (gameState)
        {
            case GameState.GAMEOVER:
                GameOver();
                break;
            case GameState.PAUSE:
                buttonResume.SetActive(true);
                buttonRestart.SetActive(false);
                buttonQuit.SetActive(true);
                textMenu.text = "Jogo Pausado!";
                menuGameOver.SetActive(true);
                uiManager.CloseShop();
                uiManager.CloseHistory();
                uiManager.CloseEndHistory();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                break;
            case GameState.PLAY:
                menuGameOver.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
                if (historyActive)
                {
                    uiManager.OpenHistory();
                    Time.timeScale = 0.05f;
                }
                if (isNewGame)
                {
                    Destroy(playerInstance);
                    playerInstance = Instantiate(playerPrefab);
                    isNewGame = false;
                    playerController = playerInstance.GetComponent<PlayerController>();
                    UpdatePlayerHP(playerController.HP);
                }
                break;
        }

    }

    public void UpdatePlayerHP(int HP)
    {
        uiManager.UpdateLifes(HP);
    }

    public void GameOver()
    {
        if (!audioHasPlayed)
        {
            audioGameOver.Play();
            audioHasPlayed = true;
        }
        buttonResume.SetActive(false);
        buttonRestart.SetActive(true);
        buttonQuit.SetActive(true);
        textMenu.text = "Você morreu!";
        menuGameOver.SetActive(true);
        textShopAccess.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UpdateDiamonds(int amount)
    {
        diamonds += amount;
        txtDiamonds.text = diamonds.ToString();
    }

    public bool DropDiamond(int chance)
    {
        int random = Random.Range(0, 100);
        return random <= chance ? true : false;
    }

    public void PlayAudioAttackSlime()
    {
        audioAttackSlime.PlayDelayed(0.07f);
    }

    public void PlayAudioAttackPlayer()
    {
        audioSword.Play();
    }
    public void PlayAudioPlayerTakeDamage()
    {
        audioPlayerTakeDamage.Play();
    }
    public void RestartGame()
    {
        textRestart.text = timeRestart.ToString();
        isNewGame = true;
        textRestart.gameObject.SetActive(true);        
        StartCoroutine(LoopWithDelay());
        enemyManager.ResetAllEnemies();
        StartCoroutine(RestartGameCourotine());
    }

    IEnumerator LoopWithDelay()
    {
        buttonResume.SetActive(false);
        buttonRestart.SetActive(false);
        buttonQuit.SetActive(false);
        for (int i = timeRestart; i >= 0; i--)
        {
            textRestart.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator RestartGameCourotine()
    {
        yield return new WaitForSeconds(timeRestart);
        ControlCam2(false);
        ControlCamBoss(false);
        isNewGame = true;
        textRestart.gameObject.SetActive(false);
        OnOffRaind(false);
        if(!audioPeace.isPlaying)
        {
           if(audioBossFight.isPlaying)
            {
                audioBossFight.Pause();
                audioBackground.Play();
            }
        }
        ChangeGameState(GameState.PLAY);
    }

    public void ReturnMenu()
    {
        Application.Quit();
    }

    public void ControlCam2(bool camBool)
    {
        vcam2.gameObject.SetActive(camBool);
    }

    public void ControlCamBoss(bool camBool)
    {
        vcam3.gameObject.SetActive(camBool);
    }

    public void BuyBonusDamage(int price)
    {
        if (!playerController.haveBonusDamage)
        {
            UpdateDiamonds(-price);
            playerController.BonusDamage();
        }
        
    }
    public void BuyExtraSpeed(int price)
    {
        if (!playerController.haveBonusSpeed)
        {
            UpdateDiamonds(-price);
            playerController.BonusSpeed();
        }
    }    

    public void BuyExtraLife(int price)
    {
        if (playerController.HP > 0 && playerController.HP < 3)
        {
            UpdateDiamonds(-price);
            playerController.UpdateLife(1);
        }
        else
        {
            return;
        }
    }

    public void LeaveGame()
    {
        StartCoroutine(Leave());
    }
    IEnumerator Leave()
    {
        audioMenuConfirm.Play();
        yield return new WaitForSecondsRealtime(0.2f);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
