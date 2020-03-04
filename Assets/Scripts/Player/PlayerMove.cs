using UnityEngine;
using System.Collections.Generic;

public class PlayerMove : MonoBehaviour
{
    [Header("Vertical Movement")]
    [SerializeField] [Range(0, 100f)] private float acceleration;   // 프레임당 가속도
    [SerializeField] [Range(0, 100f)] private float deceleration;   // 프레임당 감속도
    private bool m_FacingRight = true;

    [Header("Horizontal Movement")]
    [SerializeField] private LayerMask m_WhatIsGround;              // A mask determining what is ground to the character
    [SerializeField] private Transform groundChecker;               // A position marking where to check if the player is grounded.
    [SerializeField] private float jumpHeight;
    private float jumpVelocity;
    private bool canJump = false;
    private bool canDoubleJump = false;

    private Vector2 groundBox = new Vector2(0.73f, 0.2f);
    private bool isGrounded;            // Whether or not the player is grounded.
    private Rigidbody2D rb;
    private float terminalVelocity;

    private Vector3 velocity = Vector3.zero;
    public bool IsGrounded { get { return isGrounded; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        jumpVelocity = Mathf.Sqrt(2 * rb.gravityScale * 9.81f * jumpHeight);
        terminalVelocity = 2 * jumpVelocity;
    }

    private void FixedUpdate()
    {
        GroundChecking();
        if (rb.velocity.y < -terminalVelocity) rb.velocity = new Vector2(rb.velocity.x, -terminalVelocity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(groundChecker.position, groundBox);
    }

    private void GroundChecking()
    {
        // 상승 중에는 점프 불가
        if (rb.velocity.y >= 0.01f)
        {
            isGrounded = false;
            return;
        }

        isGrounded = false;

        Collider2D[] colls = Physics2D.OverlapBoxAll(groundChecker.position, groundBox, 0, m_WhatIsGround);

        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i].gameObject != gameObject)
            {
                isGrounded = true;
                return;
            }
        }    
    }

    public void Move(float speed, bool jump)
    {
        HorizontalVelocityControl(speed);
        JumpMovement(jump);

        // If the input is moving the player right and the player is facing left...
        if (speed > 0 && !m_FacingRight)
        {
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (speed < 0 && m_FacingRight)
        {
            Flip();
        }        
    }
    
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void JumpMovement(bool jump)
    {
        if (!jump) canJump = true;

        // 땅을 밟을 시 초기화
        if (isGrounded)
        {
            canDoubleJump = true;
        }

        if (isGrounded && jump && canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            canJump = false;
        }
        if (!isGrounded && jump && canJump && canDoubleJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            canJump = false;
            canDoubleJump = false;
        }
    }

    private void HorizontalVelocityControl(float targetV)
    {
        float nowV = rb.velocity.x;
        float changedV = 0;

        if (nowV == targetV) return;

        if (targetV > 0)
        {
            if (nowV >= 0)
            {
                changedV = nowV + acceleration * Time.fixedDeltaTime;
                if (changedV >= targetV) changedV = targetV;
            }
            else
            {
                changedV = nowV + deceleration * Time.fixedDeltaTime;
            }
        }
        else if (targetV < 0)
        {
            if (nowV <= 0)
            {
                changedV = nowV - acceleration * Time.fixedDeltaTime;
                if (changedV <= targetV) changedV = targetV;
            }
            else
            {
                changedV = nowV - deceleration * Time.fixedDeltaTime;
            }
        }
        else
        {
            if (nowV > 0)
            {
                changedV = nowV - deceleration * Time.fixedDeltaTime;
                if (changedV <= 0) changedV = 0;
            }
            else if (nowV < 0)
            {
                changedV = nowV + acceleration * Time.fixedDeltaTime;
                if (changedV >= 0) changedV = 0;
            }
            else
            {
                changedV = 0;
            }
        }

        rb.velocity = new Vector2(changedV, rb.velocity.y);
    }
}
