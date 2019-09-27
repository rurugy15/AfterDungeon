using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { Always, Detected };

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private AttackType attackType;

    private CharacterShootController shooter;
    private PatrolAreaController patroller;

    private void Start()
    {
        patroller = GetComponent<PatrolAreaController>();

        if (GetComponent<CharacterShootController>())
            shooter = GetComponent<CharacterShootController>();
        else enabled = false;
    }

    private void Update()
    {
        shooter.Shoot(transform.position, patroller.isFacingRight);
    }
}
