using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeOthers : MonoBehaviour
{
    [SerializeField] private LayerMask rideLayer;

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (rideLayer == (rideLayer | (1 << coll.gameObject.layer)))
        {
            coll.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D coll)
    {
        if (rideLayer == (rideLayer | (1 << coll.gameObject.layer)))
        {
            coll.gameObject.transform.SetParent(null);
        }
    }
}
