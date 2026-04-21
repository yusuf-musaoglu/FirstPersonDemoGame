using UnityEngine;
using UnityEngine.InputSystem;

public class Aquarium_Script : MonoBehaviour
{
    [SerializeField] private GameObject fish;

    [SerializeField] private int maxFish = 5; 

    private void Start()
    {
        SpawnMultipleChildren(); 
    }

    private void SpawnMultipleChildren()
    {
        for (int i = 0; i < maxFish; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-2f,2f), 0, Random.Range(-2f,2f));

            GameObject child = Instantiate(fish, transform.position + offset, Quaternion.identity, transform);
            child.transform.rotation = new Quaternion(0, Random.Range(1,360), 0, 1);
            
        }
    }
}
