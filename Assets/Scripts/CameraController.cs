using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - player.position;
    }

    // set camera to follow player
    void Update()
    {
        Vector3 followPos = player.position + offset;
        followPos.x = 0;
        transform.position = followPos;
    }
}
