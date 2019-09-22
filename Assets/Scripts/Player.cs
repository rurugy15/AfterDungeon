using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterMoveController moveController;
    public CharacterShootController shootController;
    public Animator animator;

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

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    private void FixedUpdate()
    {
        moveController.Move(horizontalMove, jump);
        jump = false;
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Monster")
        {
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            rb2D.velocity = new Vector2(0, 10f);
            rb2D.gravityScale = 3f;

            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
        }
    }
}
