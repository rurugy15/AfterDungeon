using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform exit;
    public Vector2 ExitPos { get { return exit.position; } }

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
        player.transform.position = exit.position;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player.GetComponent<Player>().Save();
    }
}
