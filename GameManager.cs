using System.Collections;
using System.Collections.Generic;
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
    public Transform player;
    public GameObject menuGameOver;

    [Header("Menu Config")]
    public GameObject buttonRestart;
    public GameObject buttonResume;
    public Text textMenu;

    [Header("Inventory")]
    public Text txtDiamonds;
    private int diamonds;

    [Header("Enemy IA")]
    public float slimeIdleWaitTime;
    public float slimeDistanceAttack = 2.3f;
    public float slimeAlertTime = 3f;
    public float slimeAttackDelay = 1f;
    public float slimeLookAtSpeed = 1f;

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
    private bool audioHasPlayed;


    void Start()
    {
        rainModule = rainParticle.emission;
        audioBackground.Play();
    }  

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(gameState == GameState.PLAY)
            {
            ChangeGameState(GameState.PAUSE);
            }           
        }
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
                textMenu.text = "Jogo Pausado!";
                menuGameOver.SetActive(true);
                Time.timeScale = 0;
                break;
            case GameState.PLAY:
                menuGameOver.SetActive(false);
                Time.timeScale = 1;
                break;
        }
       
    }

    public void GameOver()
    {
        if (!audioHasPlayed) { 
            audioGameOver.Play();
            audioHasPlayed = true;
        }
        buttonResume.SetActive(false);
        buttonRestart.SetActive(true);
        textMenu.text = "Você morreu!";
        menuGameOver.SetActive(true);
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
    
}
