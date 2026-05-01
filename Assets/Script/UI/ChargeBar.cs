using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class ChargeBar : MonoBehaviour
{
    private InputAction releaseTheRod;
    private Slider slider;
    public FishingRod_Script frs;
    private bool increase = true; 
    
    public float value;

    private void Start()
    {
        releaseTheRod = InputSystem.actions.FindAction("LeftClick");
        slider = FindFirstObjectByType<Slider>();
    }

    private void Update()
    {
        SliderMovement();
    }

    private void SliderMovement()
    {
        if (releaseTheRod.IsPressed())
        {
            if (increase)
            {
                slider.value += Time.deltaTime;
                if (slider.value >= 1)
                    increase = false;
            }
            else
            {
                slider.value -= Time.deltaTime;
                if (slider.value <= 0)
                    increase = true;
            }
        }
        
    }

    public float ChargePowerLevel()
    {
        // guc dengesi: 20 / 16 / 8 / 4 / 2
        if ((slider.value >= 0 && slider.value < .2f) || (slider.value > .8f && slider.value <= 1f))
            return 2f;
        else if ((slider.value >= .2f && slider.value < .36f) || (slider.value > .64f && slider.value <= .8f))
            return 4f;
        else if ((slider.value >= .36f && slider.value < .44f) || (slider.value > .56f && slider.value <= .64f))
            return 6f;
        else if ((slider.value >= .44f && slider.value < .48f) || (slider.value > .52f && slider.value <= .56f))
            return 8f;
        else
            return 10f;
    }
}