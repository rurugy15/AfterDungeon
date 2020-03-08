using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruction : MonoBehaviour
{
    private float y;
    private void Start()
    {
        y = transform.position.y;
    }

    private void Update()
    {
        if (y - transform.position.y > 50f) Destroy(gameObject);
    }
}
