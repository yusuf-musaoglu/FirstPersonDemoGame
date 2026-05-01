using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private InputAction moveAction ;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction slideAction;
    private InputAction dashAction;
    private InputAction runAction;
    private InputAction crouchAction;

    private CharacterController controller;

    private Vector2 moveInput;
    private Vector3 move;
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

    [Header("Crouch Ditails")]
    private bool isCrouching;
    private bool isStanding;
    private float crouchTime = 0;
    private float standTime = 0;
    float standPos = 2;
    float currentPos = 2;
    float crouchPos = .4f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    
    //[SerializeField] private float groundDistance = 0.6f;
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
    [SerializeField] private float slopeSpeed = 6;
    private bool isSliding = false;
    private bool isSloping = false;
    private float slideAtTheMoment = 5f;
    private float angle;
    private Vector3 lastPosition;


    [Header("Dash Ditails")]
    [SerializeField] private float dashTime;
    [SerializeField] private float dashSpeed = 20;
    private bool isDashing = false;
    private float dashAtTheMoment = 5f;
    private bool dashComplite = false;

    [Header("Roll Ditails")]
    [SerializeField] private float rollSpeed = 1;
    [SerializeField] private float duration;
    [SerializeField] private Vector3 triggerOfRolling;
    



    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");
        runAction = InputSystem.actions.FindAction("Run");
        crouchAction = InputSystem.actions.FindAction("Crouch");
        slideAction = InputSystem.actions.FindAction("Slide");
        dashAction = InputSystem.actions.FindAction("Dash");

        controller = GetComponentInChildren<CharacterController>();
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
        HandleCrouch();
        Gravity();
        Looking();
        Jump();
        Movement();
        GravityFallRoll();

        // Dash Section
        if (dashAction.WasPerformedThisFrame() && (moveY != 0 || moveX != 0) && !air_forgeTrigger)
            dashAtTheMoment = 0f;

        Dash();

        // Slide Section
        if (slideAction.WasPressedThisFrame())
            slideAtTheMoment = 0f;

        Slide();
    }

    private void GravityFallRoll()
    {
        if (groundTimer > 1 && isGroundDetect)
        {
            isStanding = false;
            //transform.localScale = new Vector3(transform.localScale.x, crouchPos, transform.localScale.z);
            controller.height = crouchPos;

            if (controller.isGrounded)
                StartCoroutine(Rolling());
        }
    }

    private void Dash()
    {
        if (dashAtTheMoment < dashTime)
        {
            OnDisableAbility();
            isDashing = true;
            isStanding = false;

            PerformCrouch(true);

            move = new Vector3();
            controller.Move(move * Time.deltaTime);
            dashAtTheMoment += Time.deltaTime;

            if (dashAtTheMoment >= dashTime)
            {
                isDashing = false;
                dashComplite = true;
            }
        }
        if (dashComplite && controller.isGrounded)
        {
            StartCoroutine(Rolling());
            dashComplite = false;
        }
    }
    private void Slide()
    {
        if (slideAtTheMoment < slideTime)
        {
            isSliding = true;
            isStanding = false;

            PerformCrouch(true);

            move.y = verticalVelocity;
            controller.Move(move * Time.deltaTime);
            slideAtTheMoment += Time.deltaTime;

            if (slideAtTheMoment > slideTime)
            {
                isSliding = false;
                isStanding = true;
            }
        }

        if (slideAction.WasReleasedThisFrame() && slideAtTheMoment > slideTime)
        {
            slopeSpeed = 6;
            isStanding = true;
        }
    }

    private void Slope()
    {
        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hitInfo, 1))
        {
            angle = Vector3.Angle(hitInfo.normal, Vector3.up);
            if (angle > IsItSloping() && controller.isGrounded)
            {
                isStanding = false;
                slopeSpeed = Math.Clamp(slopeSpeed += Time.deltaTime * 20, 0, 30);
                move = Vector3.ProjectOnPlane(new Vector3(0, -slopeSpeed, 0), hitInfo.normal);
                controller.Move(move * Time.deltaTime);
                
                lastPosition = transform.position;

                return;
            }
        }
        else if (slideAction.IsPressed())
            controller.Move(MovementDirection() * slopeSpeed * Time.deltaTime);

    }
    private Vector3 MovementDirection()
    {
        Vector3 direction = transform.position - lastPosition;

        if (direction.magnitude < 0.01f)
            return Vector3.zero;
            
        return direction.normalized;
    }

    private IEnumerator Rolling()
    {
        groundTimer = 0;
        duration = .5f;
        float rollAtTheMoment = 0;
        Vector3 axis = RollAxis(); // rotasyonda input hatasi olursa burayi tekrar dene!
        while (rollAtTheMoment < duration)
        {
            lookAction.Disable();

            rollAtTheMoment += rollSpeed * Time.deltaTime;
            float angle = rollAtTheMoment / duration * 360f;
            
            cameraRoot.localRotation = Quaternion.AngleAxis(angle, axis);
            
            yield return null;
        }
        isStanding = true;
        isCrouching = false;

        lookAction.Enable();
        OnEnableAbility();       
    }
    public void CheckForEdge() 
    {  
        Vector3 origin = transform.position + (transform.forward * .6f) + (Vector3.up * 2f);
        wallClimb = Physics.Raycast(origin, Vector3.down * 1.5f, out RaycastHit hitPos, maxDistange);
    
        if (wallClimb && !air_forgeTrigger && CanStandUp() && jumpAction.WasPressedThisFrame() && moveY > 0)
        {
            Vector3 finalPos = hitPos.point + new Vector3(0, controller.height / 2, 0);
            StartCoroutine(ClimbEdge(finalPos));
        }
    }
    private IEnumerator ClimbEdge(Vector3 targetPos) // hala hatali
    {
        Vector3 startPos = transform.position;
        
        float duration = .5f;
        float climbAtTheMoment = 0;
        
        while (climbAtTheMoment < duration)
        {
            //transform.localScale = new Vector3(transform.localScale.x, crouchPos, transform.localScale.z);
            controller.height = crouchPos;

            float percent = climbAtTheMoment / duration;
            float curve = percent * percent * (2f * percent);
                        
            transform.position = Vector3.Slerp(startPos, targetPos, curve);
            
            climbAtTheMoment += Time.deltaTime;
            yield return null;
        }

        isStanding = true;
        transform.position = targetPos;
    }
    private void HandleCrouch()
    {
        if (crouchAction.IsPressed())
        {
            jumpAction.Disable();
            crouchTime = 0;
            PerformCrouch(true);
        }
        else if (isStanding)
        {
            jumpAction.Disable();
            if (CanStandUp())
            {
                standTime = 0;
                PerformCrouch(false);
                jumpAction.Enable();
            }
        }
        if (crouchAction.WasReleasedThisFrame())
            isStanding = true;
    }
    public void PerformCrouch(bool crouch)
    {
        isCrouching = crouch;

        if (isCrouching)
        {
            crouchTime += Time.deltaTime * 5;
            
            float crouching = Mathf.Lerp(currentPos, crouchPos, crouchTime);
            currentPos = crouching;
            
            //transform.localScale = new Vector3(transform.localScale.x, crouching, transform.localScale.z);
            controller.height = crouching;
        }
        if (!isCrouching)
        {
            standTime += Time.deltaTime * 10;
            
            if (currentPos > 1.99f) currentPos = 2;

            float standing = Mathf.Lerp(currentPos, standPos, standTime);
            currentPos = standing;

            //transform.localScale = new Vector3(transform.localScale.x, standing, transform.localScale.z);
            controller.height = standing;
            
            if (standing == 2)
                isStanding = false;
        }
            
    }
    public bool CanStandUp()
    {
        Vector3 startPoint = transform.position + Vector3.up * 0.1f; 
        float checkDistance = 1.05f; 

        return !Physics.SphereCast(startPoint, checkRadius, Vector3.up, out RaycastHit hit, checkDistance, groundMask) && !isDashing;
    }
    private void Jump()
    {
        if (jumpAction.WasPressedThisFrame())
        {
            if (controller.isGrounded)
                verticalVelocity = MathF.Sqrt(jumpForge * gravity);
        }
        
        if (jumpAction.IsPressed())
        {
            if (air_forgeTrigger)
                verticalVelocity = MathF.Sqrt(air_forge * gravity);
            else if (groundTimer > .8f)
                verticalVelocity -= Mathf.Lerp(verticalVelocity, 5, 0.1f);

        }
    }
    private void Movement()
    {
        if (!toClimb)
        {
            move = transform.TransformDirection(moveX, 0, moveY);

            move.y = verticalVelocity;

            isSloping = slideAction.IsPressed() ? true : false;
            
            Slope();
            
            move *= HorizontalVelocity();
            
            controller.Move(move * Time.deltaTime);
        }
    }
    private float HorizontalVelocity()
    {
        if (isSliding)
            return slideSpeed;
        else if (isDashing)
            return dashSpeed;
        else if (runAction.IsPressed())
            return runSpeed;
        else if (isCrouching && !isDashing) 
            return crouchSpeed;
        else 
            return walkSpeed;
    }
    private void Gravity()
    {
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isGroundDetect = Physics.CheckBox(groundCheck.position, triggerOfRolling, Quaternion.Euler(0,0,0), groundMask);
        
        if (isDashing || toClimb)
            verticalVelocity = 0;
        else if (!toClimb && !controller.isGrounded)
            verticalVelocity -= gravity * Time.deltaTime;
        else if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -.1f;
        else
            verticalVelocity = -.1f;

        verticalVelocity = Math.Clamp(verticalVelocity,-15 ,15);

        if (!controller.isGrounded && !toClimb && !air_forgeTrigger && !isSloping)
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
    public void OnEnableAbility()
    {
        jumpAction.Enable();
        dashAction.Enable();
        slideAction.Enable();
    }
    public void OnDisableAbility()
    {
        jumpAction.Disable();
        dashAction.Disable();
        slideAction.Disable();
    }
    private float IsItSloping()
    {
        return controller.slopeLimit = isSloping ? 5 : 45; 
    }
    private void ClimbToLatter()
    {
        if (toClimb)
        {
            move = transform.TransformDirection(moveX, moveY, 0);
            move *= HorizontalVelocity();
            if (yRotation < 0)
                controller.Move(move * Time.deltaTime);
            if (yRotation > 0)
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
        Gizmos.DrawWireCube(groundCheck.position, triggerOfRolling);

        Ray r = new Ray(transform.position + (transform.forward * .6f) + (Vector3.up * 2f), Vector3.down);
        Gizmos.DrawRay(r);
        
        Ray r2 = new Ray(groundCheck.position, Vector3.down);
        Gizmos.DrawRay(r2);
    }
}
