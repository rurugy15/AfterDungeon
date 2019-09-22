using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float defaultPos;
    [SerializeField] private float upperLimit;
    [SerializeField] private float lowerLimit;
    [SerializeField] private float speed;

    private GameObject player;
    private float targetPos;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        transform.position = new Vector3(0, player.transform.position.y + defaultPos, -10f);
    }

    private void Update()
    {
        targetPos = player.transform.position.y + defaultPos;

        if (targetPos == transform.position.y) return;

        if (targetPos - transform.position.y >= upperLimit)
        {
            FastAdjustment_Up();
            return;
        }
        else if (transform.position.y - targetPos >= lowerLimit)
        {
            FastAdjustment_Down();
            return;
        }

        SlowAdjustment();
    }

    private void FastAdjustment_Up()
    {
        transform.position = new Vector3(0, targetPos - upperLimit, -10f);
    }

    private void FastAdjustment_Down()
    {
        Debug.Log("아래쪽으로 맞추기");
        transform.position = new Vector3(0, targetPos + lowerLimit, -10f);
    }

    private void SlowAdjustment()
    {
        if (Mathf.Abs(transform.position.y - targetPos) < Mathf.Epsilon) return;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, targetPos, -10f), speed * Time.deltaTime);
    }
}
