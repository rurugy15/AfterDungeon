using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShootController : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float speed = 100f;

    public void Shoot(Vector2 shooterPos, bool isFacingRight)
    {
        GameObject project = Instantiate(projectile, shooterPos, Quaternion.identity);
        project.GetComponent<ProjectileController>().speed = speed;
        project.GetComponent<ProjectileController>().isGoRight = isFacingRight;
    }
}