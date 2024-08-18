using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;

public class BossManager : MonoBehaviour
{

    private GameManager _gameManager;
    private Animator animator;
    public int maxHP = 2;
    public int HP;
    private bool isDie;
    private bool isPlayerVisible;
    private bool isAttack;
    private NavMeshAgent agent;
    private Vector3 destination;
    public enemyState bossState;
    private Vector3 initialPosition;
    public GameObject cubeTriggerRain;


    void Start()
    {
        _gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        HP = maxHP;
        ChangeState(bossState);
        initialPosition = this.transform.position;
    }


    void Update()
    {
        StateManager();
        if (agent.remainingDistance < 4f)
        {
            agent.destination = initialPosition;
        }
    }

    public void GetHit(int amountDmg)
    {
        if (isDie) { return; }
        HP -= amountDmg;
        if (HP > 0)
        {
            ChangeState(enemyState.FURY);
            animator.SetTrigger("GetHit");
        }
        else
        {
            ChangeState(enemyState.DIE);
            animator.SetTrigger("Die");
            StartCoroutine(Died());
        }
    }

    IEnumerator Died()
    {
        isDie = true;
        yield return new WaitForSeconds(3f);
        _gameManager.OnOffRaind(false);
        cubeTriggerRain.GetComponent<rainManager>().isRain = false;
        Destroy(this.gameObject);
    }

    public void StateManager()
    {
        if (_gameManager.gameState == GameState.GAMEOVER && bossState == enemyState.FURY)
        {
            ChangeState(enemyState.IDLE);
        }

        switch (bossState)
        {
            case enemyState.IDLE:
                LookAt();
                break;
            case enemyState.FURY:
                LookAt();
                destination = _gameManager.playerInstance.transform.position;
                agent.destination = destination;
                isAttack = false;
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    Attack();
                }
                break;
        }
    }

    public void ChangeState(enemyState newState)
    {
        StopAllCoroutines();
        bossState = newState;
        switch (bossState)
        {
            case enemyState.IDLE:
                agent.stoppingDistance = 0;
                destination = transform.position;
                agent.destination = destination;
                break;
            case enemyState.FURY:
                destination = transform.position;
                agent.stoppingDistance = _gameManager.bossDistanceAttack;
                agent.destination = destination;
                break;
            case enemyState.DIE:
                destination = transform.position;
                agent.destination = destination;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_gameManager.gameState != GameState.PLAY) { return; }
        if (other.gameObject.tag == "Player")
        {
            isPlayerVisible = true;
            LookAt();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerVisible = false;
        }
    }

    void LookAt()
    {
        Vector3 lookDirection = (_gameManager.playerInstance.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, _gameManager.slimeLookAtSpeed * Time.deltaTime);
    }

    IEnumerator ATTACK()
    {
        yield return new WaitForSeconds(_gameManager.bossAttackDelay);
        isAttack = false;
    }

    void Attack()
    {
        if (!isAttack && isPlayerVisible)
        {
            isAttack = true;
            animator.SetTrigger("Attack");
            _gameManager.PlayAudioAttackSlime();
        }
    }

    public void AttackDone()
    {
        StartCoroutine(ATTACK());
    }

    public void Restart()
    {
        ChangeState(enemyState.IDLE);
        HP = maxHP;
    }

}
