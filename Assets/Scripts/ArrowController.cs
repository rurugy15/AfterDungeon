using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private bool collidingCheck;

    [Header("Colliding Check")]
    [SerializeField] private Vector2 colliderPos;
    [SerializeField] private Vector2 colliderBox;

    private float speed;
    [SerializeField] private bool isGoRight;
     private GameObject lodgingObject;
    private bool isTherePlayer = false;
    public bool IsTherePlayer { get { return isTherePlayer; } }
    private Rigidbody2D rb2D;

    public void Initiailize(Vector2 shootPos, float speed, bool isGoRight)
    {
        rb2D = GetComponent<Rigidbody2D>();
        collidingCheck = true;
        transform.position = shootPos;
        transform.parent = null;
        lodgingObject = null;
        this.speed = speed;
        this.isGoRight = isGoRight;

        ComponentInitialize();
        TransformInitailize();
    }

    private void TransformInitailize()
    {
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

    private void ComponentInitialize()
    {
        gameObject.SetActive(true);
        enabled = true;
        GetComponent<EdgeCollider2D>().enabled = true;
        GetComponent<EdgeCollider2D>().isTrigger = true;
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.x) > 50f)
            gameObject.SetActive(false);

        if(collidingCheck) CheckColliding();
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

    public void Disable()
    {
        rb2D.velocity = new Vector2(0, rb2D.velocity.y);
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.gravityScale = 3f;
        collidingCheck = false;

        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        if (!gameObject.activeInHierarchy) yield break;

        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);
    }
}
