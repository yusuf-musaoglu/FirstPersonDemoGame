using UnityEngine;
using UnityEngine.InputSystem;

public class Aquarium_Script : MonoBehaviour
{
    InputAction d;

    [SerializeField] private GameObject fish;

    [SerializeField] private int maxFish = 5; 
    

    private void Start()
    {
        d = InputSystem.actions.FindAction("Interact");

        SpawnMultipleChildren(); 
    }

    private void Update()
    {
        // if (d.WasPressedThisFrame())
        //     SpawnMultipleChildren();
    }

    private void SpawnMultipleChildren()
    {
        for (int i = 0; i < maxFish; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-2f,2f), Random.Range(-2f,2f), Random.Range(-2f,2f));

            GameObject child = Instantiate(fish, transform.localPosition + offset, Quaternion.identity, transform);
            
        }
    }
}
