using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ContactArrow
{
    private List<GameObject> leverPlatforms;

    private void Start()
    {
        leverPlatforms = new List<GameObject>();

        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Lever Platform");
        foreach (GameObject platform in platforms)
        {
            leverPlatforms.Add(platform);
        }
    }

    public override void OnLodgingEnterAction(GameObject arrow)
    {
        StartCoroutine(DisappearLeverPlatform());
        
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

    private IEnumerator DisappearLeverPlatform()
    {
        foreach (GameObject leverPlatform in leverPlatforms)
        {
            leverPlatform.SetActive(false);
        }

        Debug.Log("Disappear");

        yield return new WaitForSeconds(3f);

        Debug.Log("Appear");

        foreach (GameObject leverPlatform in leverPlatforms)
        {
            leverPlatform.SetActive(true);
        }
    }
}
