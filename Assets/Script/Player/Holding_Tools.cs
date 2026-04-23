using UnityEngine;
using UnityEngine.InputSystem;

public class Holding_Tools : MonoBehaviour
{
    InputAction next;
    InputAction previous;

    [SerializeField] private int selectedTool = 0;
    [SerializeField] private Transform toolHolder;
    [SerializeField] private GameObject[] tools;
    private GameObject hand;

    private void Start()
    {
        next = InputSystem.actions.FindAction("Next");
        previous = InputSystem.actions.FindAction("Previous");

        SelectTool();
    }

    private void Update()
    {
        int previousSelectedTool = selectedTool;

        if (next != null && next.WasPressedThisFrame())
        {
            selectedTool++;
            if (selectedTool >= tools.Length)
                selectedTool = 0; // Başa dön
        }

        if (previous != null && previous.WasPressedThisFrame())
        {
            selectedTool--;
            if (selectedTool < 0)
                selectedTool = tools.Length - 1; // Sona git
        }

        if (previousSelectedTool != selectedTool)
            SelectTool();
    }

    private void SelectTool()
    {
        if (hand != null) Destroy(hand);

        hand = Instantiate(tools[selectedTool], toolHolder.position, toolHolder.rotation);

        hand.transform.SetParent(toolHolder);
    }
}