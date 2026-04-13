using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class FishMovment : MonoBehaviour
{
    [Header("Ray Details")]
    private bool forwardRay;
    private bool leftRay;
    private bool rightRay;

    InputAction f;
    private float moveTimer;
    [SerializeField] private float moveDuration;
    Vector3 currentPos;
    

    
    void Start()
    {
        f = InputSystem.actions.FindAction("Interact");
    }
    void Update()
    {
        RayDetection();

        moveTimer += Time.deltaTime;
        if (ShouldIMove())
        {
            StartCoroutine(Rotation(Random.Range(1f, 120f)));
        }
    }

    private void RayDetection()
    {
        forwardRay = Physics.Raycast(transform.position + (transform.forward * .4f), transform.forward, 1);
        rightRay = Physics.Raycast(transform.position + (transform.right * .2f), transform.right, 1);
        leftRay = Physics.Raycast(transform.position + (transform.right * -.2f), transform.right * -1, 1);
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
            currentPos = transform.position;
            if (forwardRay)
                break;

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
    
    private void OnDrawGizmos()
    {
        Ray r = new Ray(transform.position + (transform.forward * .4f), transform.forward);
        Gizmos.DrawRay(r);

        Ray rR = new Ray(transform.position + (transform.right * .2f), transform.right);
        Gizmos.DrawRay(rR);

        Ray rL = new Ray(transform.position + (transform.right * -.2f), transform.right * -1);
        Gizmos.DrawRay(rL);
    }
}
