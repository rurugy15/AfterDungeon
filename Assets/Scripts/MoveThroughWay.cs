using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveThroughWay : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float width;
    [SerializeField] private float height;
    private Vector2 minPos;
    private Vector2 maxPos;

    private enum MoveDirection { Right, Up, Left, Down };
    private MoveDirection dir = MoveDirection.Right;

    private void Start()
    {
        minPos = transform.position;
        maxPos = minPos + new Vector2(width, height);
    }

    private void FixedUpdate()
    {
        ChangeDir();
        Vector2 nowVelocity = Velocity();

        transform.position += (Vector3)nowVelocity * speed * Time.fixedDeltaTime;
    }

    private void ChangeDir()
    {
        if (dir == MoveDirection.Right && transform.position.x >= maxPos.x)
        {
            transform.position = new Vector2(maxPos.x, transform.position.y);
            dir = MoveDirection.Up;
        }

        if (dir == MoveDirection.Up && transform.position.y >= maxPos.y)
        {
            transform.position = new Vector2(transform.position.x, maxPos.y);
            dir = MoveDirection.Left;
        }

        if (dir == MoveDirection.Left && transform.position.x <= minPos.x)
        {
            transform.position = new Vector2(minPos.x, transform.position.y);
            dir = MoveDirection.Down;
        }

        if (dir == MoveDirection.Down && transform.position.y <= minPos.y)
        {
            transform.position = new Vector2(transform.position.x, minPos.y);
            dir = MoveDirection.Right;
        }
    }

    private Vector2 Velocity()
    {
        if (dir == MoveDirection.Right) return Vector2.right;
        else if (dir == MoveDirection.Up) return Vector2.up;
        else if (dir == MoveDirection.Left) return Vector2.left;
        else return Vector2.down;
    }
}
