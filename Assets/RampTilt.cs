using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampTilt : MonoBehaviour
{
    [SerializeField]
    public LayerMask rampLayerMask;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is on the specified layer
        if ((rampLayerMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            // Rotate the object forward by 30 degrees on the x-axis
            transform.Rotate(30f, 0f, 0f, Space.Self);
        }
    }
}
