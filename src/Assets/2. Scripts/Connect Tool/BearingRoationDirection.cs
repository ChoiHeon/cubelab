using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearingRoationDirection : MonoBehaviour
{
    [SerializeField] private float distance;

    public void ReverseRotationDirection()
    {
        Vector3 scale = transform.localScale;

        scale.x *= -1;
        transform.localScale = scale;
        transform.parent.GetComponent<Gizmo>().target.GetComponent<Bearing>().ChangeRotationDirection();
    }
}
