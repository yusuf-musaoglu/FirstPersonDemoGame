using UnityEngine;
using UnityEngine.InputSystem;

public class Holding_Tools : MonoBehaviour
{
    [SerializeField] private int selectedTool = 0;
    InputAction next;
    InputAction previous;

    private void Start()
    {
        next = InputSystem.actions.FindAction("Next");
        previous = InputSystem.actions.FindAction("Previous");

        SelectTool();
    }

    private void Update()
    {
        int currentSelectedTool = selectedTool;
        if (next.WasPressedThisFrame())
        {
            if (selectedTool >= transform.childCount -1)
                selectedTool = 0;
            else
                selectedTool++;

        }
        if (previous.WasPressedThisFrame())
        {
            if (selectedTool <= 0)
                selectedTool = transform.childCount -1;
            else
                selectedTool--;
        }

        if (currentSelectedTool != selectedTool)
            SelectTool();
    }

    private void SelectTool()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedTool)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }
}