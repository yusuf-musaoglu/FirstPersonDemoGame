using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class FishMovment : MonoBehaviour
{
    [Header("Ray Details")]
    public Vector3 forward;
    public Vector3 back;
    public Vector3 left;
    public Vector3 right;

    [Header("Rotation")]
    private float timer = 4;
    private float movementTime = 2f;
    private float persent;
    private float randomAngle;

    private Rigidbody rb;

    InputAction f;

    
    void Start()
    {
        f = InputSystem.actions.FindAction("Interact");
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Rotation());
    }
    void Update()
    {
        if (ShouldIMove())
        {
            
        }
    }
    private IEnumerator Rotation()
    {
        while (persent < 1)
        {
            persent += Time.deltaTime;
            float angle = persent / 1 * Random.Range(1,90);
            Debug.Log(angle);
            //rb.MoveRotation(Quaternion.AngleAxis(angle, WitchWay()));
            
            transform.Rotate(WitchWay() * angle);

            yield return null;
        }
        persent = 0;
    }
    private Vector3 WitchWay()
    {
        float r = Random.value;
        if (r < .33f)
            return Vector3.up;
        else if (r > .33f && r < 66f)
            return Vector3.down;
        else 
            return Vector3.zero;
    }
    private void Movement()
    {
        rb.MovePosition(transform.position + transform.forward);
    }
    private bool ShouldITurn()
    {
        return Random.value < 1f;
    }
    public bool ShouldIMove()
    {
        return Random.value < .01f;
    }
    private void OnDrawGizmos()
    {
        Ray r = new Ray(transform.position + forward, Vector3.forward);
        Gizmos.DrawRay(r);

        r = new Ray(transform.position + back, Vector3.back);
        Gizmos.DrawRay(r);

        r = new Ray(transform.position + left, Vector3.left);
        Gizmos.DrawRay(r);

        r = new Ray(transform.position + right, Vector3.right);
        Gizmos.DrawRay(r);
    }
}
