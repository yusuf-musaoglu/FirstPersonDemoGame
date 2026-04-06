using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public InputAction moveAction ;
    public InputAction lookAction;
    public InputAction jumpAction;
    public InputAction slideAction;
    public InputAction dashAction;
    public InputAction runAction;
    public InputAction crouchAction;



    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");
        runAction = InputSystem.actions.FindAction("Run");
        crouchAction = InputSystem.actions.FindAction("Crouch");
        slideAction = InputSystem.actions.FindAction("Slide");
        dashAction = InputSystem.actions.FindAction("Dash");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
