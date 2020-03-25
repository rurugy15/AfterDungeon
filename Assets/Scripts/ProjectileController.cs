using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private bool isGoingRight;
    private float speed;

    private bool isPlayerThere;
    private bool isFlying;
    private float endX;

    public void Initialize(bool isGoingRight, float speed, float distance)
    {
        this.isGoingRight = isGoingRight;
        this.speed = speed;

        isPlayerThere = true;
        isFlying = true;
        endX = transform.position.x + (isGoingRight ? distance : -distance);
    }

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        rb2D.velocity = isGoingRight ? new Vector2(speed, 0) : new Vector2(-speed, 0);
    }

    private void FixedUpdate()
    {
        if (isFlying == false)
        {
            rb2D.velocity = Vector2.zero;
            rb2D.bodyType = RigidbodyType2D.Static;

            return;
        }

        if (isGoingRight && transform.position.x > endX)
        {
            isFlying = false;
            transform.position = new Vector2(endX, transform.position.y);
        }

        if (!isGoingRight && transform.position.x < endX)
        {
            isFlying = false;
            transform.position = new Vector2(endX, transform.position.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            if (!isPlayerThere)
            {
                coll.gameObject.GetComponent<PlayerMovement>().ProjectileJump();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            if (isPlayerThere) isPlayerThere = false;
        }
    }
}
