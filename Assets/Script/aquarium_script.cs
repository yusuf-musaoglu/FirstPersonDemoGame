using Unity.Mathematics;
using UnityEngine;

public class aquarium_script : MonoBehaviour
{

    [SerializeField] private GameObject fish;
    private int maxFish = 5;

    void Start()
    {
        SpawnMultipleChildren();

        
    }
    
    
    void SpawnMultipleChildren()
    {
        for (int i = 0; i < maxFish; i++)
        {
            Vector3 offset = new Vector3(0, 0, 0);

            GameObject child = Instantiate(fish, transform.position + offset, Quaternion.identity, transform);
            
            
        }
    }
}
