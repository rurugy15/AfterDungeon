using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private ProjectileShooter shooter;

    private void Start()
    {
        shooter = GetComponent<ProjectileShooter>();
    }

    private void Update()
    {
        shooter.Shoot(transform.position, true);
    }
}
