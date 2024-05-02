using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public float maxLeftOffset = 3f;
    public float maxRightOffset = 32f;

    public float maxBottomOffset = 0f;
    public float maxTopOffset = 3f;

    private float minPosX;
    private float maxPosX;
    private float minPosY;
    private float maxPosY;

    private void Start()
    {
        // Calculate the minimum and maximum positions based on the map's boundaries
        minPosX = offset.x - maxLeftOffset;
        maxPosX = offset.x + maxRightOffset;

        minPosY = offset.y - maxTopOffset;
        maxPosY = offset.y + maxBottomOffset;
    }

    private void LateUpdate()
    {
        // Calculate the desired camera position
        Vector3 desiredPosition = player.position + offset;
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minPosX, maxPosX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minPosY, maxPosY);

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}