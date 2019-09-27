using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAreaController : MonoBehaviour
{
    public List<Vector2> patrolPoints;
    public float speed;
    public bool isFacingRight;

    private int targetPoint;

    private void Start()
    {
        transform.position = patrolPoints[0];
        targetPoint = 1;
        isFacingRight = true;
    }

    private void FixedUpdate()
    {
        MoveToNextPoint();
        FlipCharacter();
    }

    private void MoveToNextPoint()
    {
        if (transform.position.x < patrolPoints[targetPoint].x) isFacingRight = true;
        if (transform.position.x > patrolPoints[targetPoint].x) isFacingRight = false;

        transform.position = Vector2.MoveTowards(transform.position, patrolPoints[targetPoint], speed * Time.fixedDeltaTime);

        if ((patrolPoints[targetPoint] - (Vector2)transform.position).magnitude < Mathf.Epsilon)
        {
            targetPoint += 1;
            if (targetPoint == patrolPoints.Count) targetPoint = 0;
        }
    }

    private void FlipCharacter()
    {
        if (isFacingRight == true)
            GetComponent<SpriteRenderer>().flipX = false;
        else GetComponent<SpriteRenderer>().flipX = true;
    }
}
