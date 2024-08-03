using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    private GameManager _gameManager;

    [Header("Config Player")]
    public float movementSpeed = 3f;    

    [Header("Attack Config")]
    public ParticleSystem fxAttack;

    private Vector3 direction;
    private bool isWalk;
    private float horizontal;
    private float vertical;
    [SerializeField]
    private bool isAttack;

    public int HP = 3;
    public Transform hitBox;
    [Range(0, 1)]
    public float hitrange;
    public LayerMask hitMask;
    public int hitDamage = 1;
    public AudioSource audioSword;

    void Start()
    {
        _gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(_gameManager.gameState == GameState.PLAY)
        {
            Inputs();

            MoveCharacter();

            UpdateAnimator();
        }
       
    }

   void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "TakeDamage")
        {
            GetHit(1);
        }
    }

    private void Inputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Fire1") && !isAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        isAttack = true;
        animator.SetTrigger("Attack");
        audioSword.Play();
        fxAttack.Emit(1);
        Collider[] hitInfo = Physics.OverlapSphere(hitBox.position, hitrange, hitMask);
        foreach(Collider c in hitInfo)
        {           
            c.gameObject.SendMessage("GetHit", hitDamage, SendMessageOptions.DontRequireReceiver);
        }

    }

    private void MoveCharacter()
    {
        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }

        characterController.Move(direction * movementSpeed * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isWalk", isWalk);
    }

    public void AttackDone()
    {
        isAttack = false;
    }

    private void OnDrawGizmosSelected()
    {
        
        if (hitBox)
        {
            Gizmos.DrawWireSphere(hitBox.position, hitrange);
        }
    }

    void GetHit(int amount)
    {
        HP -= amount;
        if(HP > 0)
        {
            animator.SetTrigger("Hit");
        }
        else
        {
            _gameManager.ChangeGameState(GameState.GAMEOVER);
            animator.SetTrigger("Die");
        }
    }
}
