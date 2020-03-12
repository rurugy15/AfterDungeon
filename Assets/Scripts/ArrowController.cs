using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("Colliding Check")]
    [SerializeField] private Vector2 colliderPos;
    [SerializeField] private Vector2 colliderBox;

    private float speed;
    [SerializeField] private bool isGoRight;
    private GameObject lodgingObject = null;
    private bool isTherePlayer = false;
    public bool IsTherePlayer { get { return isTherePlayer; } }
    private Rigidbody2D rb2D;

    private float originY;

    public void Initiailize(float speed, bool isGoRight)
    {
        this.speed = speed;
        this.isGoRight = isGoRight;
    }

    private void Start()
    {
        originY = transform.position.y;
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

        Vector2 boxCorner_LeftDown = (Vector2)transform.position + drawPos - colliderBox / 2;
        Vector2 boxCorner_RightUp = boxCorner_LeftDown + new Vector2(Time.fixedDeltaTime * rb2D.velocity.x, colliderBox.y);

        Gizmos.DrawLine(boxCorner_LeftDown, boxCorner_RightUp);
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

        Vector2 boxCorner_LeftDown = (Vector2)transform.position + checkPos - colliderBox / 2;
        Vector2 boxCorner_RightUp = boxCorner_LeftDown + new Vector2(Time.fixedDeltaTime * rb2D.velocity.x, colliderBox.y);
        Collider2D[] colls = Physics2D.OverlapAreaAll(boxCorner_LeftDown, boxCorner_RightUp);
        //Collider2D[] colls = Physics2D.OverlapBoxAll((Vector2)transform.position + checkPos, colliderBox, 0);

        GameObject target = null;

        for (int i = 0; i < colls.Length; i++)
        {
            if (!colls[i].GetComponent<ContactArrow>()) continue;
            if (target == null) target = colls[i].gameObject;
            else
            {
                if (isGoRight && target.transform.position.x > colls[i].transform.position.x) target = colls[i].gameObject;
                if(!isGoRight && target.transform.position.x < colls[i].transform.position.x) target = colls[i].gameObject;
            }
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
