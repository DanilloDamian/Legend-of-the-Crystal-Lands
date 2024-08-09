using Cinemachine;
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
    public GameObject canvas;
    public UIManager uiManager;

    [Header("Menu Config")]
    public GameObject buttonRestart;
    public GameObject buttonResume;
    public GameObject buttonQuit;
    public Text textMenu;
    public Text textRestart;
    private bool isNewGame = true;
    public int timeRestart = 5;


    [Header("Inventory")]
    public Text txtDiamonds;
    private int diamonds;

    [Header("Enemy IA")]
    public float slimeIdleWaitTime = 4f;
    public float slimeDistanceAttack = 2.3f;
    public float slimeAlertTime = 3f;
    public float slimeAttackDelay = 1f;
    public float slimeLookAtSpeed = 1f;
    public EnemyManager enemyManager;

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
                ChangeGameState(GameState.PAUSE);
            }
        }

        if (vcam1 != null && vcam2 != null)
        {
            UpdateCams(playerInstance.transform);
        }     
        
    }


    public void UpdateCams(Transform focus)
    {
        vcam1.Follow = focus;
        vcam2.Follow = focus;
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
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                break;
            case GameState.PLAY:
                menuGameOver.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
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
        ControlCam2(false);
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
        isNewGame = true;
        textRestart.gameObject.SetActive(false);
        OnOffRaind(false);
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

}
