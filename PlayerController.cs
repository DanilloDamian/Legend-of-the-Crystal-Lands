using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    [Header("Config Player")]
    public float movementSpeed = 3f;

    [Header("Camera")]
    public GameObject cambB;

    private Vector3 direction;
    private bool isWalk;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Attack");
        }

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
        animator.SetBool("isWalk", isWalk);

    }


    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "CamTrigger":
                cambB.SetActive(true);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "CamTrigger":
                cambB.SetActive(false);
                break;
        }
    }


}
