using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechArrow : MonoBehaviour
{
    [SerializeField] private bool isFacingRight;
    [SerializeField] private float arrowSpeed;
    [SerializeField] private float shootDelay;

    private bool active;

    private void Start()
    {
        MechArrowManager.instance.Assign(this);
        StartCoroutine(ShootArrow());
    }

    private IEnumerator ShootArrow()
    {
        yield return new WaitForSeconds(shootDelay);

        if(active) Shot();

        StartCoroutine(ShootArrow());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    private void Shot()
    {
        Vector2 shotPos = transform.position;
        ArrowManager.instance.Shoot(shotPos, arrowSpeed, isFacingRight);
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }
}
