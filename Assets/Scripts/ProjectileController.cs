using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 100f;
    public bool isGoRight;
    [SerializeField] private LayerMask lodgedLayer;
    [SerializeField] private LayerMask fallenLayer;

    private Rigidbody2D rb2D;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (isGoRight == true) rb2D.velocity = new Vector2(speed, 0);
        else rb2D.velocity = new Vector2(-speed, 0);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (lodgedLayer == (lodgedLayer | (1 << coll.gameObject.layer)))
        {
            rb2D.velocity = Vector2.zero;
            rb2D.bodyType = RigidbodyType2D.Static;
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (fallenLayer == (fallenLayer | (1 << coll.gameObject.layer)))
        {
            rb2D.velocity = Vector2.zero;
            rb2D.bodyType = RigidbodyType2D.Dynamic;
            rb2D.gravityScale = 3f;
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
