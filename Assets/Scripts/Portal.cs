using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform Exit;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameObject player = coll.gameObject;
            PortalAction(player);
        }
    }

    protected virtual void PortalAction(GameObject player)
    {
        player.transform.position = Exit.position;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
