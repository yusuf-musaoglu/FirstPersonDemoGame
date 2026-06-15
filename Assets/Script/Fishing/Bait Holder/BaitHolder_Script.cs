using System.Collections.Generic;
using UnityEngine;

public class BaitHolder_Script : MonoBehaviour
{
    public static BaitHolder_Script Instance {get; private set;}
    private FishingRod_Script frs;

    private Transform originalParent;
    private Vector3 originalPos;

    public float radius = 3f;
    
    [SerializeField] private LayerMask fishLayer;
    public Collider[] fish;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        frs = FindAnyObjectByType<FishingRod_Script>();
        originalParent = transform.parent;
        originalPos = transform.localPosition;
    }

    private void Update()
    {
        BaitPosition();

        if (frs.isFishing)
        {
            //fish = Physics.OverlapSphere(transform.position, radius, fishLayer);
            
        }
    }


    private void BaitPosition()
    {
        if (frs.throwingNow)
            transform.SetParent(null);
        else if (frs.resetFishingPose)
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalPos;
            frs.resetFishingPose = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}




