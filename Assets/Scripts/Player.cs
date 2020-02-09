using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private Vector2 originPos;

    private void Start()
    {
        originPos = transform.position;
    }

    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (horizontalMove > 0) isFacingRight = true;
        else if (horizontalMove < 0) isFacingRight = false;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        if (Input.GetButtonUp("Jump"))
        {
            jump = false;
        }
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
        //StartCoroutine(MildJump(mildJumpFrame));
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
        StartCoroutine(BackToOrigin());
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

    private IEnumerator BackToOrigin()
    {
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();

        rb2D.velocity = new Vector2(0, 15f);

        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
