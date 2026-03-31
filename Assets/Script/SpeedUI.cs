using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Text speed;

    private void Awake()
    {
        speed.text = "Speed:";

    }
    // Update is called once per frame
    void Update()
    {
        
        speed.text = "Speed: " + player.move.z.ToString();
    }
}