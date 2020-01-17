using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [SerializeField] private LayerMask attackLayer;

    private Vector2 attackPos;
    private Vector2 attackSize;

    private void Start()
    {
        attackPos = transform.position;
        //attackSize = GetComponent<Collider2D>().size;
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (attackLayer == (attackLayer | (1 << coll.gameObject.layer)))
        {
            if (coll.gameObject.GetComponent<Player>())
            {
                coll.gameObject.GetComponent<Player>().Die();
            }
            else
            {
                Debug.Log("Sth is wrong at DamageOnTouch");
            }
        }
    }
}
