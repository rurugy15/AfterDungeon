using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float defaultPos;
    [SerializeField] private float upperLimit;
    [SerializeField] private float lowerLimit;

    private GameObject player;
    private float targetY;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        transform.position = new Vector3(0, player.transform.position.y + defaultPos, -10f);
    }

    private void FixedUpdate()
    {
        targetY = player.transform.position.y + defaultPos;

        if (targetY == transform.position.y) return;

        if (targetY - transform.position.y >= upperLimit)
        {
            FastAdjustment_Up();
            return;
        }
        else if (transform.position.y - targetY >= lowerLimit)
        {
            FastAdjustment_Down();
            return;
        }

        SlowAdjustment();
    }

    private void FastAdjustment_Up()
    {
        Vector3 targetPos = new Vector3(0, targetY - upperLimit, -10f);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.01f);
    }

    private void FastAdjustment_Down()
    {
        Vector3 targetPos = new Vector3(0, targetY + lowerLimit, -10f);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.1f);
    }

    private void SlowAdjustment()
    {
        //if (Mathf.Abs(transform.position.y - targetY) < Mathf.Epsilon) return;

        Vector3 targetPos = new Vector3(0, targetY, -10f);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.3f);
    }
}