using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterMoveController moveController;
    public CharacterShootController shootController;

    public float runSpeed = 40f;

    private float horizontalMove = 0f;
    private bool jump = false;
    private bool isFacingRight = true;

    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (horizontalMove > 0) isFacingRight = true;
        else if (horizontalMove < 0) isFacingRight = false;

        if (Input.GetButtonDown("Jump")) jump = true;
        if (Input.GetButtonDown("Fire1")) shootController.Shoot(transform.position, isFacingRight);
    }

    private void FixedUpdate()
    {
        moveController.Move(horizontalMove, jump);
        jump = false;
    }
}
