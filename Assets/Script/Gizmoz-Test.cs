using UnityEngine;

public class Gizmoz : MonoBehaviour
{


    public Transform cube;
    public Vector3 size;
    [SerializeField] public Ray r;
    void Awake()
    {
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(cube.localPosition, size);
    }

}
