using System;
using System.Collections;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction slideAction;
    private InputAction dashAction;
    private InputAction runAction;
    private InputAction crouchAction;
    public CharacterController controller;

    private Vector2 moveInput;
    public Vector3 move;
    private float moveX;
    private float moveY;
    private Vector2 turnInput;
    private float turnX;
    private float turnY;
    private float yRotation;

    private float verticalVelocity;

    [Header("Movement Details")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float mouseSensitivity =.1f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpForge = 4;
    [SerializeField] private float air_forge = .1f;

    [Header("Layers")]
    private bool air_forgeTrigger;
    private bool toClimb;
        
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float checkRadius = 0.5f;

    private bool isCrouching = false;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.6f;
    private float groundTimer = 0;
    private bool isGrounded;
    private bool isGroundDetect;

    [Header("Wall Check")]
    [SerializeField] private float climbSpeed;
    [SerializeField] private float maxDistange;
    private bool wallClimb;
    private bool wallCheck;

    [Header("Slide Ditails")]
    [SerializeField] private float slideTime;
    [SerializeField] private float slideSpeed = 20;
    private bool isSliding = false;
    private float slideAtTheMoment = 5f;

    [Header("Dash Ditails")]
    [SerializeField] private float dashTime;
    [SerializeField] private float dashSpeed = 20;
    private bool isDashing = false;
    private float dashAtTheMoment = 5f;
    private bool dashComplite = false;

    [Header("Roll Ditails")]
    [SerializeField] private float rollSpeed = 1;
    [SerializeField] private float duration;
    [SerializeField] private Vector3 size;
    



    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");
        runAction = InputSystem.actions.FindAction("Run");
        crouchAction = InputSystem.actions.FindAction("Crouch");
        slideAction = InputSystem.actions.FindAction("Slide");
        dashAction = InputSystem.actions.FindAction("Dash");

        controller = GetComponent<CharacterController>();
    }
    private void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputManagement();

        Motion();

        Climb();
    }

    private void Climb()
    {
        CheckForEdge();
        ClimbToLatter();  
    }

    private void Motion()
    {
        if (slideAction.WasPressedThisFrame())
            slideAtTheMoment = 0f;

        if (dashAction.WasPerformedThisFrame())
            dashAtTheMoment = 0f;

        HandleCrouch();
        Gravity();
        Looking();
        Jump();
        Movement();
        Dash();

        GravityFallRoll();

        if (slideAtTheMoment < slideTime)
            Slide();
        
    }

    private void GravityFallRoll()
    {
        if (groundTimer > 1 && isGroundDetect)
        {
            StartCoroutine(Rolling());
        }
    }

    private void Dash()
    {
        if (dashAtTheMoment < dashTime)
        {
            isDashing = true;

            PerformCrouch(true);

            move = new Vector3();
            controller.Move(move * Time.deltaTime);
            dashAtTheMoment += Time.deltaTime;

            if (dashAtTheMoment > dashTime)
            {
                isDashing = false;
                dashComplite = true;
                isCrouching = false;
            }
            
        }
        if (dashComplite && isGroundDetect)
        {
            dashComplite = false;
            StartCoroutine(Rolling());
            isCrouching = true;
        }
        
    }

    private void Slide()
    {
        isSliding = true;
        PerformCrouch(true);

        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
        slideAtTheMoment += Time.deltaTime;

        if (slideAtTheMoment > slideTime)
        {
            isSliding = false;
            CanStandUp();
        }
    }

    IEnumerator Rolling()
    {
        duration = .5f;
        float rollAtTheMoment = 0;
        Vector3 axis = RollAxis(); // rotasyonda input hatasi olursa burayi tekrar dene!
        while (rollAtTheMoment < duration)
        {
            rollAtTheMoment += rollSpeed * Time.deltaTime;
            float angle = rollAtTheMoment / duration * 360f;
            
            if (moveX == 0 && moveY == 0)
                controller.Move(transform.TransformDirection(0,0,1) * Time.deltaTime);
            cameraRoot.localRotation = Quaternion.AngleAxis(angle, axis);
            lookAction.Disable();
            
            yield return null;
        }
        lookAction.Enable();
    }
    
    public void CheckForEdge() 
    {  
        Vector3 origin = transform.position + (transform.forward * .6f) + (Vector3.up * 2f);
        wallClimb = Physics.Raycast(origin, Vector3.down * 1.5f, out RaycastHit shelfHit, maxDistange);
    
        if (wallClimb && !air_forgeTrigger && CanStandUp() && jumpAction.WasPressedThisFrame() && moveY > 0)
        {
            Vector3 finalPos = shelfHit.point + new Vector3(0, (controller.height / 2) + .1f, 0);
            StartCoroutine(ClimbEdge(finalPos));
        }
    }
    IEnumerator ClimbEdge(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        
        float duration = 1f;
        float climbAtTheMoment = 0;
        
        while (climbAtTheMoment < duration)
        {
            float percent = climbAtTheMoment / duration;
            float curve = percent * percent * (2f * percent);
            
            PerformCrouch(true);
            
            transform.position = Vector3.Lerp(startPos, targetPos, curve);
            climbAtTheMoment += Time.deltaTime;
            yield return null;
        }

        CanStandUp();
        transform.position = targetPos;
    }
    private void HandleCrouch()
    {
        if (crouchAction.IsPressed())
        {
            jumpAction.Disable();
            PerformCrouch(true);
        }
        else if (isCrouching)
        {
            jumpAction.Disable();
            
            if (CanStandUp())
            {
                PerformCrouch(false);
                jumpAction.Enable();
            }
        }
    }
    private void Jump()
    {
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            verticalVelocity = MathF.Sqrt(jumpForge * gravity);
        }
        if (jumpAction.IsPressed() && air_forgeTrigger)
        {
            verticalVelocity = MathF.Sqrt(air_forge * gravity);
        }
        if (jumpAction.IsPressed() && !isGrounded && verticalVelocity < 0)
        {
            verticalVelocity -= Mathf.Lerp(verticalVelocity, 5, 0.1f);
        }
    }
    private void Movement()
    {
        move = transform.TransformDirection(moveX, 0, moveY);

        move.y = verticalVelocity;
        
        move *= isItRun();
        
        controller.Move(move * Time.deltaTime);
    }
    private void Gravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isGroundDetect = Physics.CheckBox(groundCheck.position, size, Quaternion.Euler(0,0,0), groundMask);
        
        if (isDashing)
            verticalVelocity = 0;
        else if (!toClimb && !isGrounded)
            verticalVelocity -= gravity * Time.deltaTime;
        else if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -1f;
        else
            verticalVelocity = 0;

        verticalVelocity = Math.Clamp(verticalVelocity,-15 ,15);

        if (!isGrounded && !toClimb)
            groundTimer += Time.deltaTime;
        else
            groundTimer = 0;

    }
    private void Looking()
    {
        float mouseX = turnX * mouseSensitivity;
        float mouseY = turnY * mouseSensitivity;

        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -70f, 70f);
        cameraRoot.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
        
        transform.Rotate(Vector3.up * mouseX);
    }
    private void InputManagement()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        moveX = moveInput.x;
        moveY = moveInput.y;

        turnInput = lookAction.ReadValue<Vector2>();

        turnX = turnInput.x;
        turnY = turnInput.y;
        
    }
    private Vector3 RollAxis()
    {
      return (moveX == 0 && moveY == 0) ? Vector3.right : new Vector3(moveY, 0, -moveX);  
    }  
    public void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
    }
    public void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
    }
    private float isItRun()
    {
        if (isSliding)
            return slideSpeed;
        else if (isCrouching && !isDashing) 
            return crouchSpeed;
        else if (isDashing)
            return dashSpeed;
        else if (runAction.IsPressed())
            return runSpeed;
        else 
            return walkSpeed;
    }
    public void PerformCrouch(bool crouch)
    {
        isCrouching = crouch;
        transform.localScale = new Vector3(transform.localScale.x, isCrouching ? .4f : 1f, transform.localScale.z);
    }
    public bool CanStandUp()
    {
        Vector3 startPoint = transform.position + Vector3.up * 0.1f; 
        float checkDistance = 1.05f; 

        return !Physics.SphereCast(startPoint, checkRadius, Vector3.up, out RaycastHit hit, checkDistance, groundMask) && !isDashing;
    }
    private void ClimbToLatter()
    {
        move = transform.TransformDirection(0, moveY, 0);
        move *= isItRun();
        if (yRotation < 0 && toClimb)
        {    
            controller.Move(move * Time.deltaTime);
        }
        if (yRotation > 0 && toClimb)
        {
            controller.Move(-move * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        air_forgeTrigger = other.gameObject.layer == 7;
        toClimb = other.gameObject.layer == 8;
        wallClimb = other.gameObject.layer == 9;
    }
    private void OnTriggerExit(Collider other)
    {
        air_forgeTrigger = other.gameObject.layer == 0;
        toClimb = other.gameObject.layer == 0;
        wallClimb = other.gameObject.layer == 0;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        Ray r = new Ray(transform.position + (transform.forward * .6f) + (Vector3.up * 2f), Vector3.down * 155f);
        Gizmos.DrawRay(r);
        Gizmos.DrawWireCube(groundCheck.position, size);
    }

}
