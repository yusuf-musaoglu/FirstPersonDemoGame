using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingRod_Script : MonoBehaviour
{
    public static FishingRod_Script Instance {get; private set;}
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
    public bool throwingNow = false;

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
    [SerializeField] private LayerMask waterMask;
    private bool thouchTheWater;
    public bool resetFishingPose = false;
    public bool isFishing {get; private set;}
    

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

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

        if (isFishing && leftClick.WasPressedThisFrame()
            || Physics.CheckSphere(baitHolder.position + (baitHolder.up * -.1f), .1f, groundMask))
            ResetRod();

        if (throwingNow)
            if (Physics.CheckSphere(baitHolder.position - baitHolder.up * -.06f, .1f, waterMask))
                FishingPose();
    }

    private void FishingPose()
    {
        isFishing = true;

        baitHolder.transform.rotation = Quaternion.Euler(Vector3.zero);
        baitHolderRB.useGravity = false;

        baitHolderRB.constraints = RigidbodyConstraints.FreezeAll;

        ReelUp();
    }

    private void ReelUp()
    {
        
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
        throwingNow = true;
        baitHolderRB.useGravity = true;

        baitHolderRB.constraints &= ~RigidbodyConstraints.FreezeAll;

        Vector3 direction = transform.forward + transform.up;
        if (!thouchTheWater)
            baitHolderRB.AddForce(direction.normalized * chargeBar.ChargePowerLevel(), ForceMode.Impulse);
        
    }
    private void ResetRod()
    {
        resetFishingPose = true;
        baitHolderRB.constraints = RigidbodyConstraints.FreezeAll;
        throwingNow = false;
        isFishing = false;


        GameManager.Instance.resetFishPose = true;
        GameManager.Instance.pickedAFish = false;
    }    

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(baitHolder.position - baitHolder.up * -.06f, 0.1f);
    }
}