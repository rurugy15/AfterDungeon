using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAreaController : MonoBehaviour
{
    [SerializeField] private List<Vector2> patrolPoints;
    public float speed;

    private MonsterAI AI;
    private int targetPoint;

    private void Start()
    {
        AI = GetComponent<MonsterAI>();
        transform.position = patrolPoints[0];
        targetPoint = 1;
    }

    private void FixedUpdate()
    {
        MoveToNextPoint();
    }

    private void MoveToNextPoint()
    {
        if (transform.position.x < patrolPoints[targetPoint].x && AI.isFacingRight == false
            || transform.position.x > patrolPoints[targetPoint].x && AI.isFacingRight == true)
            AI.FlipCharacter();

        transform.position = Vector2.MoveTowards(transform.position, patrolPoints[targetPoint], speed * Time.fixedDeltaTime);

        if ((patrolPoints[targetPoint] - (Vector2)transform.position).magnitude < Mathf.Epsilon)
        {
            targetPoint += 1;
            if (targetPoint == patrolPoints.Count) targetPoint = 0;
        }
    }

    
}
