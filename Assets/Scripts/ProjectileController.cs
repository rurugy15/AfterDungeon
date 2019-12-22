using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 100f;
    public bool isGoRight = true;
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private LayerMask lodgedLayer;
    [SerializeField] private LayerMask fallenLayer;

    [Header("Colliding Check")]
    [SerializeField] private Vector2 colliderPos;
    [SerializeField] private float colliderRadius;

    private bool isTherePlayer = true;
    private Rigidbody2D rb2D;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (isGoRight == true)
        {
            rb2D.velocity = new Vector2(speed, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            rb2D.velocity = new Vector2(-speed, 0);
            transform.rotation = Quaternion.Euler(0, 180f, 0);
        }
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.x) > 50f)
            Destroy(gameObject);

        CheckColliding();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector2 drawPos = isGoRight ? colliderPos : colliderPos * new Vector2(-1, 1);
        Gizmos.DrawWireSphere((Vector2)transform.position + drawPos, colliderRadius);
    }

    private void CheckColliding()
    {
        Vector2 checkPos = isGoRight ? colliderPos : colliderPos * new Vector2(-1, 1);

        Collider2D[] colls = Physics2D.OverlapCircleAll((Vector2)transform.position + checkPos, colliderRadius);

        foreach (Collider2D coll in colls)
        {
            if (lodgedLayer == (lodgedLayer | (1 << coll.gameObject.layer)))
            {
                rb2D.velocity = Vector2.zero;
                rb2D.bodyType = RigidbodyType2D.Static;

                float targetPosX = coll.transform.position.x;
                if (isGoRight == true) transform.position = new Vector2(targetPosX, transform.position.y);
                else transform.position = new Vector2(targetPosX, transform.position.y);

                if (!isTherePlayer)
                {
                    GetComponent<Collider2D>().isTrigger = false;
                    gameObject.layer = LayerMask.NameToLayer("Platform");
                    enabled = false;
                }
                return;
            }

            if (attackLayer == (attackLayer | (1 << coll.gameObject.layer)))
            {
                if (coll.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    coll.gameObject.GetComponent<Player>().Die();
                    return;
                }
                Destroy(coll.gameObject);
                Destroy(gameObject);
                return;
            }

            if (fallenLayer == (fallenLayer | (1 << coll.gameObject.layer)))
            {
                rb2D.velocity = new Vector2(0, rb2D.velocity.y);
                rb2D.bodyType = RigidbodyType2D.Dynamic;
                rb2D.gravityScale = 3f;
                GetComponent<Collider2D>().enabled = false;
                enabled = false;
                return;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            isTherePlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            isTherePlayer = false;
        }
    }
}
