using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    [SerializeField] private Vector2 dir;
    [SerializeField] private float speed;

    public void Initialize(Vector2 dir, float speed)
    {
        this.dir = dir;
        this.speed = speed;
    }

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = dir * speed;
        Debug.Log("Start");
    }
}
