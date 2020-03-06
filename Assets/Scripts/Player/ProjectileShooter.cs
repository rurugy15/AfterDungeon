using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float speed = 50f;
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private Vector2 shootPos;

    private bool canShoot = true;
    private List<GameObject> myProjectiles = new List<GameObject>();

    public void Shoot(Vector2 shooterPos, bool isFacingRight)
    {
        if (canShoot == false) return;

        canShoot = false;
        StartCoroutine(ShootDelay());

        Vector2 nowShootPos = isFacingRight ? shootPos : shootPos * new Vector2(-1, 1);

        GameObject arrow = Instantiate(projectile, shooterPos + nowShootPos, Quaternion.identity);
        arrow.GetComponent<ArrowController>().Initiailize(speed, isFacingRight);
        
        myProjectiles.Add(arrow);
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

    public void DestroyAllArrows()
    {
        foreach (GameObject arrow in myProjectiles)
        {
            Destroy(arrow);
        }

        myProjectiles = new List<GameObject>();
    }
}