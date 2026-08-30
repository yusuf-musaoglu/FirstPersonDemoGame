using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class FishMovment : MonoBehaviour
{
    [Header("Ray Details")]
    private bool forwardRay;
    private bool leftRay;
    private bool rightRay;

    public bool startCount;

    private float moveTimer;
    [SerializeField] private float moveDuration;
    Vector3 currentPos;
    private bool inZone;

    [Header("Bait Details")]
    private FishingRod_Script frs;
    [SerializeField] private LayerMask baitLayer;
    [SerializeField] private float charmCoolDown;
    private Transform originParent;

    void Start()
    {
        frs = FindFirstObjectByType<FishingRod_Script>();

        originParent = transform.parent;
    }
    private void FixedUpdate()
    {
        if (!inZone) //inzone
        {
            RayDetection();

            moveTimer += Time.deltaTime;
            if (ShouldIMove())
            {
                StartCoroutine(Rotation(Random.Range(1f, 120f)));
            }
        }
        if (inZone)
        {
            if (TimerDeley())
            StartCoroutine(FishCharmed());
        }

        if (frs.isFishing && !GameManager.Instance.pickedAFish)
        {
            GameManager.Instance.PerformRandomSelection();
        }
    }

    public void ResetFunction()
    {
        inZone = false;
        transform.SetParent(originParent);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    } 

    private bool TimerDeley()
    {
        return GameManager.Instance.TimeMeter >= charmCoolDown ? true : false;
    }
    public void OnSelected(bool selected)
    {
        if (!selected)
        {
            inZone = true;
            charmCoolDown = GameManager.Instance.TimeMeter + 3f;
            Debug.Log("cooldown secilmeden once: " + charmCoolDown);
        }
    }
    public void OnNotSelected()
    {
        inZone = false;
    }
    private IEnumerator FishCharmed()
    {
        transform.SetParent(null);

        float timer = 0f;

        Vector3 relativePos = BaitHolder_Script.Instance.transform.position - transform.position;
        Quaternion rotationGoal = Quaternion.LookRotation(relativePos, Vector3.up);

        Quaternion currentRotation = transform.rotation;

        while (timer < 1f)
        {
            if (!inZone)
                yield break;

            timer += Time.deltaTime * 8;
            transform.rotation = Quaternion.Slerp(currentRotation, rotationGoal, timer);

            yield return null;
        }
    }
    public IEnumerator Rotation(float angle)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, angle * WitchWay(), 0);
        
        float timer = 0f;
        moveTimer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * 16;

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timer);
            
            yield return null;
        }

        transform.rotation = targetRotation;

        StartCoroutine(Movement());
    }
    private IEnumerator Movement()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position + transform.forward * Random.Range(1f,5f);

        float timer = 0f;
        
        while (timer < 1f)
        {
            timer += Time.deltaTime;

            if (forwardRay || inZone)
                yield break;

            currentPos = transform.position;

            transform.position = Vector3.Slerp(startPos, forwardRay ? currentPos : targetPos, timer);
           
            yield return null;
        }
        transform.position = forwardRay ? currentPos : targetPos;
    }
    public float WitchWay()
    {
        float r = Random.value;
        if (r < .3f && !rightRay)
            return 1;
        else if (r >.3f && r < .6f && !leftRay)
            return -1;
        else if (!forwardRay)
            return 0;
        else
            return 1;
    }
    public bool ShouldIMove()
    {
        return moveTimer >= moveDuration ? Random.value < .5f : false;
    }
    private void RayDetection()
    {
        forwardRay = Physics.Raycast(transform.position + (transform.forward * .4f), transform.forward, 1);
        rightRay = Physics.Raycast(transform.position + (transform.right * .2f), transform.right, 1);
        leftRay = Physics.Raycast(transform.position + (transform.right * -.2f), transform.right * -1, 1);
    }
    private void OnDrawGizmos()
    {
        Ray r = new Ray(transform.position + (transform.forward * .4f), transform.forward);
        Gizmos.DrawRay(r);

        Ray rR = new Ray(transform.position + (transform.right * .2f), transform.right);
        Gizmos.DrawRay(rR);

        Ray rL = new Ray(transform.position + (transform.right * -.2f), transform.right * -1);
        Gizmos.DrawRay(rL);

        Gizmos.DrawRay(transform.position + transform.forward * .7f, Vector3.up);
    }
}