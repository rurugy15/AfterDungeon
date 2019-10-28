using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterMoveController moveController;
    public CharacterShootController shootController;
    public Animator animator;

    [SerializeField] private float runSpeed = 40f;
    [Tooltip("jump 입력이 지속되는 frame 수")]
    [SerializeField] private int mildJumpFrame = 3;

    private float horizontalMove = 0f;
    private bool jump = false;
    private bool isFacingRight = true;

    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (horizontalMove > 0) isFacingRight = true;
        else if (horizontalMove < 0) isFacingRight = false;

        if (Input.GetButtonDown("Jump")) jump = true;
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Shoot");
            shootController.Shoot(transform.position, isFacingRight);
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    private void FixedUpdate()
    {
        moveController.Move(horizontalMove, jump);
        StartCoroutine(MildJump(mildJumpFrame));
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Monster")
        {
            Die();
        }
    }

    public void Die()
    {
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        rb2D.velocity = new Vector2(0, 10f);
        rb2D.gravityScale = 3f;

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    private IEnumerator MildJump(int frameCnt)
    {
        if (jump == false) yield break;

        for (int i = 0; i < frameCnt; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        jump = false;
    }
}
