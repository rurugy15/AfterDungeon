using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("정지했을 때 카메라의 (플레이어에 대한 상대적) 위치")]
    [SerializeField] private float defaultPos;
    [Tooltip("느린 조정의 상한선")]
    [SerializeField] private float upperLimit;
    [Tooltip("느린 조정의 하한선")]
    [SerializeField] private float lowerLimit;
    [Tooltip("카메라 움직임의 최대 속도")]
    [SerializeField] [Range(100f, 600f)] private float speedLimit = 300f;
    [SerializeField] private List<Transform> portalPos = new List<Transform>();

    private GameObject player;
    private float targetY;
    private Vector3 velocity = Vector3.zero;
    private float mySize;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        transform.position = new Vector3(0, player.transform.position.y + defaultPos, -10f);
        mySize = GetComponent<Camera>().orthographicSize;

        GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");
        foreach (GameObject portal in portals)
        {
            portalPos.Add(portal.transform);
        }

        portalPos.Sort(delegate (Transform a, Transform b)
        {
            if (a.position.y > b.position.y) return 1;
            else if (a.position.y < b.position.y) return -1;
            else return 0;
        });
    }

    private void Update()
    {
        if (player.GetComponent<Player>().enabled == false)
        {
            this.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        targetY = player.transform.position.y + defaultPos;

        int? boundaryPortalNum = UpperBoundaryPortalNum();
        if (boundaryPortalNum.HasValue && targetY >= portalPos[boundaryPortalNum.Value].position.y - mySize / 2)
            targetY = portalPos[boundaryPortalNum.Value].position.y - mySize / 2;

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
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.01f, speedLimit);
    }

    private void FastAdjustment_Down()
    {
        Vector3 targetPos = new Vector3(0, targetY + lowerLimit, -10f);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.1f, speedLimit);
    }

    private void SlowAdjustment()
    {
        Vector3 targetPos = new Vector3(0, targetY, -10f);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.3f, speedLimit);
    }

    private int? UpperBoundaryPortalNum()
    {
        for (int i = 0; i < portalPos.Count; i++)
        {
            if (player.transform.position.y < portalPos[i].transform.position.y)
                return i;
        }

        return null;
    }

    public Portal UpperBoundaryPortal()
    {
        int? num = UpperBoundaryPortalNum();

        if (num.HasValue) return portalPos[num.Value].GetComponent<Portal>();
        else return null;
    }
}