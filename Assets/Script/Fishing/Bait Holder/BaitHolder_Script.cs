using UnityEngine;

public class BaitHolder_Script : MonoBehaviour
{
    private FishingRod_Script frs;
    private Transform originalParent;
    private Vector3 originalPos;

    private void Start()
    {
        frs = FindAnyObjectByType<FishingRod_Script>();
        originalParent = transform.parent;
        originalPos = transform.localPosition;
    }

    private void Update()
    {
        if (frs.fishingPose)
        {
            transform.SetParent(null);
        }
        else if (frs.resetFishingPose)
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalPos;
            frs.resetFishingPose = false;
            Debug.Log(frs.resetFishingPose);
        }

    }
}




