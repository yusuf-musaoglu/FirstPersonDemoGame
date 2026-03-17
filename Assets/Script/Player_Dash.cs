using UnityEngine;
using UnityEngine.InputSystem;
public class Player_Dash : MonoBehaviour
{
    private InputAction dashAction;
    public Player player {get; private set;}



    [Header("Time Ditails")]
    [SerializeField] private float dashTime = .5f;
    [SerializeField] private float dashSpeed = 20;
    //private bool isDashing = false;


    void Start()
    {
        dashAction = InputSystem.actions.FindAction("Ability");
        
    }

    void Update()
    {
        if (dashAction.WasPressedThisFrame())
            HandleDash();
        
    }

    private void HandleDash()
    {
        dashTime = .3f;
        while (dashTime < 0)
            Dash();
        
    }

    private void Dash()
    {
        if (dashTime < 0)
        {
            player.CanStandUp();
            dashTime = -1f;
        }
        dashTime -= Time.deltaTime;
        Debug.Log(dashTime);
           
        player.PerformCrouch(true);

        player.move *= dashSpeed;
        player.controller.Move(player.move * Time.deltaTime);
    }
}
