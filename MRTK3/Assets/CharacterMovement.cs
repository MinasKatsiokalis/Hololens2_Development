using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;

    private void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerInput.CharacterControl.Move.started += onMovementInput;
        playerInput.CharacterControl.Move.canceled += onMovementInput;
    }

    void onMovementInput (InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    private void OnEnable()
    {
        playerInput.CharacterControl.Enable();    
    }

    private void OnDisable()
    {
        playerInput.CharacterControl.Disable();
    }

    void Update()
    {
        HandleAnimation();
        HandleRotation();
        characterController.Move(currentMovement * Time.deltaTime);
    }

    void HandleAnimation()
    {
        bool isWalking = animator.GetBool("Walking");

        if (isMovementPressed && !isWalking)
            animator.SetBool("Walking", true);
        else if(!isMovementPressed && isWalking)
            animator.SetBool("Walking", false);
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, 15.0f * Time.deltaTime);
        }
    }
}
