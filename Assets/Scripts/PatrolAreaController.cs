using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAreaController : MonoBehaviour
{
    public List<Vector2> patrolPoints;
    public float speed;

    private int targetPoint;

    private void Start()
    {
        transform.position = patrolPoints[0];
        targetPoint = 1;
    }

    private void FixedUpdate()
    {
        MoveToNextPoint();
    }

    private void MoveToNextPoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolPoints[targetPoint], speed * Time.fixedDeltaTime);

        if ((patrolPoints[targetPoint] - (Vector2)transform.position).magnitude < Mathf.Epsilon)
        {
            targetPoint += 1;
            if (targetPoint == patrolPoints.Count) targetPoint = 0;
        }
    }
}
