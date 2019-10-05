using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { Always, Detected };

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private PatrolAreaController patroller;
    [SerializeField] private MonsterSight sight;
    [SerializeField] private CharacterShootController shooter;
    [SerializeField] private AttackType attackType;

    private bool canShoot = false;
    public bool isFacingRight = true;
    public bool isFindHero = false;

    private void Start()
    {
        if(shooter) canShoot = true;
    }

    private void FixedUpdate()
    {
        if (canShoot)
        {
            switch (attackType)
            {
                case AttackType.Always:
                    shooter.Shoot(transform.position, isFacingRight);
                    break;
                case AttackType.Detected:
                    if(isFindHero == true)
                        shooter.Shoot(transform.position, isFacingRight);
                    break;
            }
        }
    }

    public void FlipCharacter()
    {
        isFacingRight = !isFacingRight;
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        sight.FlipFacingDir();
    }
}