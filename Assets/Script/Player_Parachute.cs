
using UnityEngine;

public class Player_Parachute : Player_Movement
{
    public Player_Movement player;
    private bool isJumped;

    private void Awake()
    {
        player = new Player_Movement();

    }

    void Update()
    {

    }

    
}
