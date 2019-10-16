using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShootController : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float speed = 100f;
    [SerializeField] private float delay = 0.5f;

    private bool canShoot = true;

    public void Shoot(Vector2 shooterPos, bool isFacingRight)
    {
        if (canShoot == false) return;

        canShoot = false;
        StartCoroutine(ShootDelay());

        float delta = (isFacingRight ? 0.5f : -0.5f);

        GameObject project = Instantiate(projectile, shooterPos + new Vector2(delta,0), Quaternion.identity);
        project.GetComponent<ProjectileController>().speed = speed;
        project.GetComponent<ProjectileController>().isGoRight = isFacingRight;
    }

    private IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(delay);

        canShoot = true;
    }
}