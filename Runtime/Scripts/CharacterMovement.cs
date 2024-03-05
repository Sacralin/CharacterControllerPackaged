using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    private FirstAndThirdPersonCharacterInputs inputActions;
    private CharacterController characterController;
    private Animator animator;
    private TwoDimentionalAnimationStateController animationStateController;

    public Camera clickAndPointCamera; // this will need to be the active cinemachine camera
    private NavMeshAgent agent;

    public bool firstPersonEnabled;
    public bool thirdPersonEnabled;
    public bool pointAndClickEnabled;
    public bool pointAndClickCameraOnly;
    public bool crouchEnabled;
    public bool jumpEnabled;

    //movement settings
    private Vector3 moveDirection;
    private Vector2 currentInput;
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    private float currentSpeed;
    public float gravity = -9.81f;
    public float jumpForce = 4f;
    
    //collision mesh settings 
    private bool isCrouched;
    [SerializeField]
    private bool isGrounded;
    private float standingHeight = 1.8f;
    private float crouchedHeight = 1.3f;
    private float standingCenterY = 0.98f;
    private float crouchedCenterY = 0.7f;
    private float crouchedCenterZOffset = 0.2f;
    private float standingRadius = 0.25f;
    private float crouchedRadius = 0.47f;
    
    // Start is called before the first frame update
    void Start()
    {
        animationStateController = GetComponent<TwoDimentionalAnimationStateController>();
        clickAndPointCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        inputActions = new FirstAndThirdPersonCharacterInputs();
        inputActions.CharacterControls.Enable();
        if (pointAndClickCameraOnly) 
        {
            animationStateController.enabled = false;
        }
        else
        {

        }

    }

    // Update is called once per frame
    void Update()
    {
        //isGrounded = characterController.isGrounded;

        //DISABLE THE NAVMESH IF THE PLAYER INPUTS A COMMAND
        if (inputActions.CharacterControls.Walk.triggered) // if player moves the character through the keyboard
        {
            if (characterController.enabled == false) // if the character controller is not enabled
            {
                agent.enabled = false; // disable the navmesh agent
                characterController.enabled = true; // enable the character controller
            }
        }


        if (characterController.enabled)
        {
            if (pointAndClickCameraOnly) { HandleCameraRelativeMovementInput(); }
            else { HandleMovementInput(); }
        }
        if (jumpEnabled) { HandleJump(); }
        if (crouchEnabled) { Crouch(); }
        if (pointAndClickEnabled) { HandleClickAndPoint(); }
    }

    private void HandleJump()
    {
        if (inputActions.CharacterControls.Jump.triggered)
        {
            if (isGrounded)
            {
                moveDirection.y = jumpForce;
                animator.SetBool("isJumping", true);
            }
        }
        
    }

    //this method is called from the jump animation once the animation is complete.
    public void OnJumpAnimationCompleted()
    {
        animator.SetBool("isJumping", false);
    }

    private void HandleMovementInput()
    {
        bool runPressed = inputActions.CharacterControls.Run.ReadValue<float>() > 0; //is run pressed
        if (isCrouched) { currentSpeed = walkSpeed; } //if crouched limit current speed to walkspeed
        else { currentSpeed = runPressed ? runSpeed : walkSpeed; } // else if run pressed apply run speed, if not then apply walk speed
        
        currentInput = inputActions.CharacterControls.Walk.ReadValue<Vector2>(); // read input values from composite vector 2 inputs 
        
        float moveDirectionY = moveDirection.y; //stores Y
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.y * currentSpeed) +  //applies input from player
            (transform.TransformDirection(Vector3.right) * currentInput.x * currentSpeed);
        moveDirection.y = moveDirectionY; //reapplys Y 
        
        if (!characterController.isGrounded) { moveDirection.y += gravity * Time.deltaTime; } //apply gravity
        characterController.Move(moveDirection * Time.deltaTime); //apply movement to player character
    }

    private void HandleCameraRelativeMovementInput()
    {
        bool runPressed = inputActions.CharacterControls.Run.ReadValue<float>() > 0; // is run pressed
        if (isCrouched) { currentSpeed = walkSpeed; } // if crouched limit current speed to walkspeed
        else { currentSpeed = runPressed ? runSpeed : walkSpeed; } // else if run pressed apply run speed, if not then apply walk speed

        currentInput = inputActions.CharacterControls.Walk.ReadValue<Vector2>(); // read input values from composite vector 2 inputs

        // Get the camera's forward and right vectors
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Flatten the vectors to ignore vertical component
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction relative to camera
        Vector3 moveDirection = (cameraForward * currentInput.y * walkSpeed) +
                                (cameraRight * currentInput.x * walkSpeed);

        // Apply gravity if not grounded
        if (!characterController.isGrounded)
        {
            moveDirection.y += gravity * Time.deltaTime;
        }

        // Move the character
        characterController.Move(moveDirection * Time.deltaTime); // apply movement to player character

        // Rotate the character to face the movement direction
        if (moveDirection.magnitude > 0.1f) // Only rotate if there's actual movement
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
        
        if (currentInput != new Vector2(0, 0))
        {
            animator.SetFloat("Velocity Z", 0.5f);
        }
        else
        {
            animator.SetFloat("Velocity Z", 0f);
        }


    }


    private void Crouch()
    {
        
        if (inputActions.CharacterControls.Crouch.triggered) // if crouch is triggered switch settings 
        {
            animator.SetBool("isCrouched", !isCrouched);
            isCrouched = animator.GetBool("isCrouched"); //check if currently crouched
            characterController.height = isCrouched ? crouchedHeight : standingHeight;
            characterController.center = isCrouched ? new Vector3(0, crouchedCenterY, crouchedCenterZOffset) : new Vector3(0, standingCenterY, 0);
            characterController.radius = isCrouched ? crouchedRadius : standingRadius;
        }
    }
    
    private void HandleClickAndPoint()
    {
        if (agent.enabled)
        {
            if (agent.remainingDistance < 0.3f)
            {
                characterController.enabled = true;
                agent.enabled = false;
                animator.SetFloat("Velocity Z", 0f);
            }
            else
            {
                animator.SetFloat("Velocity Z", 0.5f);
            }
        }

        if (inputActions.CharacterControls.LeftMouseClick.ReadValue<float>() == 0) return;

        RaycastHit hit;
        var ray = clickAndPointCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            agent.enabled = true;
            characterController.enabled = false;
            agent.destination = hit.point;
            
        }

        
        
    }
}
