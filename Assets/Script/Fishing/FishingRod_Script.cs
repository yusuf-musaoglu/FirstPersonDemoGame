using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingRod_Script : MonoBehaviour
{
    private InputAction leftClick;

    [SerializeField] private Transform fishingRod;
    [SerializeField] private Transform baitHolder;

    private Vector3 startPos;
    private Vector3 currentPos;
    private Vector3 endPos;

    private bool chargedCanceled = false;

    private float duration = 1f;
    private float atThisTime = 0;
    private float percent;
    private float curve;

    private void Awake()
    {
        leftClick = InputSystem.actions.FindAction("LeftClick");
        
        startPos = fishingRod.localPosition;
        endPos = fishingRod.localPosition - (Vector3.forward * .5f);

    }

    private void Update()
    {
        if (leftClick.IsPressed())
            RodStart();

        if (leftClick.WasReleasedThisFrame())
            chargedCanceled = true;
        
        if (chargedCanceled)
            RodEnd();


    }

    private void RodEnd()
    {
        currentPos = fishingRod.localPosition;
        
        atThisTime -= Time.deltaTime;

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

    private void RodStart()
    {
        if (atThisTime < duration)
        {
            atThisTime += Time.deltaTime;

            percent = atThisTime / duration;
            curve = percent * percent * (2f * percent);
            
            fishingRod.localPosition = Vector3.Lerp(startPos, endPos, Mathf.Clamp01(curve));
        }
        
    }
}
