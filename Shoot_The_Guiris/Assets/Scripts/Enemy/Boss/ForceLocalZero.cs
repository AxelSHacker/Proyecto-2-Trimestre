using UnityEngine;

public class ForceLocalZero : MonoBehaviour
{
    void LateUpdate()
    {
        transform.localPosition = Vector3.zero;
    }
}
