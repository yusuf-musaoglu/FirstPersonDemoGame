using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingRod_Script : MonoBehaviour
{
    private InputAction leftClick;

    [Header("Rod Details")]
    [SerializeField] private Transform fishingRod;
    [SerializeField] private Transform baitHolder;

    [Header("UI Bar Ditails")]
    [SerializeField] private GameObject chargeBarUI;
    public ChargeBar chargeBar;

    private GameObject currentChargeBar;
    private Transform canvas;

    [Header("Throw Details")]
    [SerializeField] private Rigidbody baitHolderRB;
    private bool isFishingNow;

    private Vector3 startPos;
    private Vector3 currentPos;
    private Vector3 endPos;

    private bool chargedCanceled = false;
    private bool isFullCharged;

    private float duration = 1f;
    private float atThisTime = 0;
    private float percent;
    private float curve;

    [Header("Bait Holder Detail")]
    [SerializeField] private LayerMask groundMask;
    public bool fishingPose = false;
    public bool resetFishingPose = false;

    
    private void Awake()
    {
        leftClick = InputSystem.actions.FindAction("LeftClick");

        startPos = fishingRod.localPosition;
        endPos = fishingRod.localPosition - Vector3.forward;

        Canvas c = FindAnyObjectByType<Canvas>();
        if (c != null)
            canvas = c.transform;

    }

    private void Update()
    {
        if (leftClick.IsPressed())
            RodStart();

        if (leftClick.WasReleasedThisFrame())
            chargedCanceled = true;
        
        if (chargedCanceled)
            RodEnd();

        if (isFishingNow && leftClick.WasPressedThisFrame())
            ResetRod();
        

        // if (isFishingNow && Physics.CheckBox(baitHolder.position + (baitHolder.up * -.1f), new Vector3(.15f,.2f,.15f), quaternion.identity, groundMask))
        //     FishingPose();
    }

    private void FishingPose()
    {
        fishingPose = true;
    }

    private void RodStart()
    {
        if (atThisTime < duration)
        {
            atThisTime += Time.deltaTime;

            percent = atThisTime / duration;
            curve = percent * percent * (2f * percent);
            fishingRod.localPosition = Vector3.Lerp(startPos, endPos, Mathf.Clamp01(curve));

            if (atThisTime > duration)
            {
                isFullCharged = true;

                if (chargeBarUI != null && canvas != null)
                {
                    currentChargeBar = Instantiate(chargeBarUI, canvas);
                    chargeBar = FindAnyObjectByType<ChargeBar>();
                }
            }
        }
    }
    private void RodEnd()
    {
        if (isFullCharged)
        {
            FishingPose();
            ThrowTheBait();

            isFullCharged = false;
            Destroy(currentChargeBar, .5f);
        }
        currentPos = fishingRod.localPosition;
        
        atThisTime -= chargedCanceled ? 5 * Time.deltaTime : Time.deltaTime;

        percent = atThisTime / duration;
        
        float radiusPersent = 1 - Mathf.Clamp01(percent);
        
        curve = radiusPersent * radiusPersent * (2f * radiusPersent);
        
        fishingRod.localPosition = Vector3.Lerp(currentPos, startPos, curve);
        
        if (fishingRod.localPosition == startPos)
        {
            chargedCanceled = false;
            percent = 0;
            atThisTime = 0;
        }
    }
    private void ThrowTheBait()
    {
        isFishingNow = true;

        baitHolderRB.constraints &= ~RigidbodyConstraints.FreezePosition; // rotasyonu unfreeze yapiyor

        Vector3 direction = transform.forward;
        direction.y = 1f;
        
        baitHolderRB.AddForce(direction.normalized * chargeBar.ChargePowerLevel(), ForceMode.Impulse);
    }
    private void ResetRod()
    {
        resetFishingPose = true;
        baitHolderRB.constraints = RigidbodyConstraints.FreezeAll;
        isFishingNow = false;
        fishingPose = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(baitHolder.position + (baitHolder.up * -.1f), new Vector3(.15f,.2f,.15f));
    }
}