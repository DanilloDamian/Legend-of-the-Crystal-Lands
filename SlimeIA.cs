using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeIA : MonoBehaviour
{
    private GameManager _gameManager;
    private Animator animator;
    public int HP = 2;
    private bool isDie;

    public enemyState enemyState;


    private NavMeshAgent agent;
    private int idWayPoint;
    private Vector3 destination;
    [HideInInspector]
    public bool isWalk;
    private bool isAlert;
    private bool isPlayerVisible;
    private bool isAttack;
    void Start()
    {
        _gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ChangeState(enemyState);
    }

    void Update()
    {
        StateManager();

        if (agent.desiredVelocity.magnitude >= 0.1f)
        {
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }
        animator.SetBool("isWalk", isWalk);
        animator.SetBool("isAlert", isAlert);
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if(_gameManager.gameState != GameState.PLAY) { return; }
        if (other.gameObject.tag == "Player")
        {
            isPlayerVisible = true;
            if (enemyState == enemyState.IDLE || enemyState == enemyState.PATROL)
            {
                ChangeState(enemyState.ALERT);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerVisible = false;
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


    public void StateManager()
    {
        if(_gameManager.gameState == GameState.GAMEOVER && (enemyState == enemyState.ALERT || enemyState == enemyState.FURY || enemyState == enemyState.FOLLOW))
        {
            ChangeState(enemyState.IDLE);
        }


        switch (enemyState)
        {
            case enemyState.IDLE:

                break;
            case enemyState.ALERT:
                LookAt();
                break;
            case enemyState.PATROL:

                break;
            case enemyState.FOLLOW:
                LookAt();
                destination = _gameManager.playerInstance.transform.position;
                agent.destination = destination;

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    Attack();
                }

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
        enemyState = newState;
        isAlert = false;

        switch (enemyState)
        {
            case enemyState.IDLE:
                agent.stoppingDistance = 0;
                destination = transform.position;
                agent.destination = destination;

                StartCoroutine(IDLE());
                break;
            case enemyState.ALERT:
                agent.stoppingDistance = 0;
                destination = transform.position;
                agent.destination = destination;
                isAlert = true;
                StartCoroutine(ALERT());
                break;
            case enemyState.FOLLOW:
                agent.stoppingDistance = _gameManager.slimeDistanceAttack;
                StartCoroutine(FOLLOW());
                break;
            case enemyState.PATROL:
                agent.stoppingDistance = 0;
                ChangeWaitPoint();

                StartCoroutine(PATROL());

                break;
            case enemyState.FURY:
                destination = transform.position;
                agent.stoppingDistance = _gameManager.slimeDistanceAttack;
                agent.destination = destination;
                break;
            case enemyState.DIE:
                destination = transform.position;                
                agent.destination = destination;
                break;
        }
    }

    public void ChangeWaitPoint()
    {
        idWayPoint = Random.Range(0, _gameManager.slimeWaitPoints.Length);
        destination = _gameManager.slimeWaitPoints[idWayPoint].position;
        agent.destination = destination;
    }

    IEnumerator Died()
    {
        isDie = true;
        yield return new WaitForSeconds(1.5f);

        if (_gameManager.DropDiamond(_gameManager.chanceDropDiamond))
        {
            Instantiate(_gameManager.diamondPrefab, transform.position, _gameManager.diamondPrefab.transform.rotation);
        }

        Destroy(this.gameObject);
    }

    IEnumerator IDLE()
    {
        yield return new WaitForSeconds(_gameManager.slimeIdleWaitTime);
        StayStill(50);
    }

    IEnumerator PATROL()
    {
        yield return new WaitUntil(() => agent.remainingDistance <= 0);
        StayStill(30);
    }

    IEnumerator ALERT()
    {
        yield return new WaitForSeconds(_gameManager.slimeAlertTime);

        if (isPlayerVisible)
        {
            ChangeState(enemyState.FOLLOW);
        }
        else
        {
            StayStill(10);
        }
    }

    IEnumerator ATTACK()
    {
        yield return new WaitForSeconds(_gameManager.slimeAttackDelay);
        isAttack = false;
    }

    IEnumerator FOLLOW()
    {
        yield return new WaitUntil(() => !isPlayerVisible);
        yield return new WaitForSeconds(_gameManager.slimeAlertTime);
        StayStill(50);
    }
    void StayStill(int percentage)
    {
        if (Rand() < percentage)
        {
            ChangeState(enemyState.IDLE);
        }
        else
        {
            ChangeState(enemyState.PATROL);
        }
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

    void LookAt()
    {
        Vector3 lookDirection = (_gameManager.playerInstance.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, _gameManager.slimeLookAtSpeed * Time.deltaTime);
    }

    int Rand()
    {
        int rand = Random.Range(0, 100);
        return rand;
    }

}
