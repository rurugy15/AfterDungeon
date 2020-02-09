using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverPlatform : LodgingPlatform
{
    public void Activate()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        GetComponent<Collider2D>().enabled = true;
    }

    public void Deactivate()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.5f);
        GetComponent<Collider2D>().enabled = false;
    }
}
