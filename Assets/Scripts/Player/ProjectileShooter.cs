using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProjectileShooter : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private Vector2 shootPos;

    private bool canShoot = true;

    public void Shoot(Vector2 shooterPos, bool isFacingRight)
    {
        if (canShoot == false) return;

        canShoot = false;
        StartCoroutine(ShootDelay());

        Vector2 nowShootPos = shooterPos;
        nowShootPos += isFacingRight ? shootPos : shootPos * new Vector2(-1, 1);

        ArrowManager.instance.Shoot(nowShootPos, speed, isFacingRight);
        //EditorApplication.isPaused = true;
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