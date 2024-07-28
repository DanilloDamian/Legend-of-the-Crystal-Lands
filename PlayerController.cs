using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    [Header("Config Player")]
    public float movementSpeed = 3f;    

    [Header("Attack Config")]
    public ParticleSystem fxAttack;

    private Vector3 direction;
    private bool isWalk;
    private float horizontal;
    private float vertical;
    private bool isAttack;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Inputs();

        MoveCharacter();

        UpdateAnimator();
    }

    private void Inputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Fire1") && !isAttack)
        {
            isAttack = true;
            animator.SetTrigger("Attack");
            fxAttack.Emit(1);
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

}
