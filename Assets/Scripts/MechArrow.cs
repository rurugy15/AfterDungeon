using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechArrow : MonoBehaviour
{
    [SerializeField] private bool isFacingRight;
    [SerializeField] private float arrowSpeed;
    [SerializeField] private float shootDelay;

    private void Start()
    {
        StartCoroutine(ShootArrow());
    }

    private IEnumerator ShootArrow()
    {
        yield return new WaitForSeconds(shootDelay);

        Shot();

        StartCoroutine(ShootArrow());
    }

    private void Shot()
    {
        Vector2 shotPos = transform.position;
        ArrowManager.instance.Shoot(shotPos, arrowSpeed, isFacingRight);
    }
}
