using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ContactArrow
{
    private List<GameObject> leverPlatforms;
    private bool isActive = false;

    private void Start()
    {
        leverPlatforms = new List<GameObject>();

        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Lever Platform");
        foreach (GameObject platform in platforms)
        {
            leverPlatforms.Add(platform);
        }

        ActivatePlatform();
    }

    public override void OnLodgingEnterAction(GameObject arrow)
    {
        ActivatePlatform();
        
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

    private void ActivatePlatform()
    {
        isActive = !isActive;

        if (isActive)
        {
            foreach (GameObject leverPlatform in leverPlatforms)
            {
                leverPlatform.GetComponent<SpriteRenderer>().color = Color.red;
                leverPlatform.GetComponent<Collider2D>().enabled = true;
            }
        }
        else
        {
            foreach (GameObject leverPlatform in leverPlatforms)
            {
                leverPlatform.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.5f);
                leverPlatform.GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}
