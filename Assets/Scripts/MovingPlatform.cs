using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private LayerMask contactLayer;
    [SerializeField] private float speed;

    private void OnCollisionStay2D(Collision2D coll)
    {
        if (contactLayer == (contactLayer | (1 << coll.gameObject.layer)))
        {
            PlayerMove mover = coll.gameObject.GetComponent<PlayerMove>();

            mover.ExternalVelocity = speed;
        }
    }

    private void OnCollisionExit2D(Collision2D coll)
    {
        if (contactLayer == (contactLayer | (1 << coll.gameObject.layer)))
        {
            PlayerMove mover = coll.gameObject.GetComponent<PlayerMove>();

            mover.ExternalVelocity = 0;
        }
    }
}
