using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlodgingPlatform : ContactArrow
{
    public override void OnLodgingEnterAction(GameObject arrow)
    {
        Rigidbody2D rb2D = arrow.GetComponent<Rigidbody2D>();

        rb2D.velocity = new Vector2(0, rb2D.velocity.y);
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.gravityScale = 3f;

        arrow.GetComponent<Collider2D>().enabled = false;
        arrow.GetComponent<ArrowController>().enabled = false;
    }

    public override void OnLodgingExitAction(GameObject arrow)
    {
    }

    public override void OnLodgingStayAction(GameObject arrow)
    {
    }
}
