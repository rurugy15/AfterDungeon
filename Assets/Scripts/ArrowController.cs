using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public float speed = 100f;
    public bool isGoRight = true;

    [Header("Colliding Check")]
    [SerializeField] private Vector2 colliderPos;
    [SerializeField] private float colliderRadius;

    private GameObject lodgingObject = null;
    private bool isTherePlayer = true;
    public bool IsTherePlayer { get { return isTherePlayer; } }
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

    private void OnLodgingEnter(GameObject target)
    {
        target.GetComponent<ContactArrow>().OnLodgingEnterAction(gameObject);
    }

    private void OnLodgingStay(GameObject target)
    {
        target.GetComponent<ContactArrow>().OnLodgingStayAction(gameObject);
    }

    private void OnLodgingExit(GameObject target)
    {
        target.GetComponent<ContactArrow>().OnLodgingExitAction(gameObject);
    }

    private void CheckColliding()
    {
        Vector2 checkPos = isGoRight ? colliderPos : colliderPos * new Vector2(-1, 1);

        Collider2D[] colls = Physics2D.OverlapCircleAll((Vector2)transform.position + checkPos, colliderRadius);
        GameObject target = null;

        foreach (Collider2D coll in colls)
        {
            if (coll.gameObject == gameObject) continue;
            if (coll.tag == "Player") continue;
            if (!coll.GetComponent<ContactArrow>()) continue;

            target = coll.gameObject;
        }

        if (lodgingObject == null)
        {
            if (target != null) OnLodgingEnter(target);
        }
        else
        {
            if (target == null) OnLodgingExit(lodgingObject);
            else if (lodgingObject.tag == target.tag) OnLodgingStay(lodgingObject);
            else OnLodgingExit(lodgingObject);
        }

        lodgingObject = target;
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
