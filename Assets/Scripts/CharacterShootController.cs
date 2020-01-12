using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShootController : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float speed = 100f;
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private Vector2 shootPos;

    private bool canShoot = true;

    public void Shoot(Vector2 shooterPos, bool isFacingRight)
    {
        if (canShoot == false) return;

        canShoot = false;
        StartCoroutine(ShootDelay());

        Vector2 nowShootPos = isFacingRight ? shootPos : shootPos * new Vector2(-1, 1);

        GameObject project = Instantiate(projectile, shooterPos + nowShootPos, Quaternion.identity);
        project.GetComponent<ArrowController>().speed = speed;
        project.GetComponent<ArrowController>().isGoRight = isFacingRight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere((Vector2)transform.position + shootPos, 0.05f);
    }

    private IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(delay);

        canShoot = true;
    }
}