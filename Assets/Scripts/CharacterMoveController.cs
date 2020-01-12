using UnityEngine;
using System.Collections.Generic;

public class CharacterMoveController : MonoBehaviour
{
    [Header("Vertical Movement")]
    [SerializeField] [Range(0, 100f)] private float acceleration;   // 프레임당 가속도
    [SerializeField] [Range(0, 100f)] private float deceleration;   // 프레임당 감속도
    private bool m_FacingRight = true;

    [Header("Horizontal Movement")]
    [SerializeField] private LayerMask m_WhatIsGround;              // A mask determining what is ground to the character
    [SerializeField] private Transform groundChecker;               // A position marking where to check if the player is grounded.
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float jumpingGravity;
    [SerializeField] private float fallingGravity;
    [SerializeField] private float maxInputTime;
    private float jumpTimeCounter;
    private bool canJump = false;

    [SerializeField] private float allowedJumpTime;                 // 플랫폼을 벗어난 뒤에도 점프를 할 수 있도록 허용된 시간
    private float lastGroundedTime;

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Rigidbody2D rb;
    private float terminalVelocity;

    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        terminalVelocity = 2 * jumpVelocity;
    }

    private void FixedUpdate()
    {
        GroundChecking();
    }

    private void GroundChecking()
    {
        // 상승 중에는 점프 불가
        if (rb.velocity.y >= 0.01f) return;

        Collider2D[] colls = Physics2D.OverlapBoxAll(groundChecker.position, new Vector2(0.7f, k_GroundedRadius), 0, m_WhatIsGround);

        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i].gameObject != gameObject)
            {
                m_Grounded = true;
                lastGroundedTime = Time.time;
                return;
            }
        }

        // 마지막으로 땅에 있었던 시간에서 허용된 점프 시간보다 오래 지나면 땅에서 떨어진 것으로 판정
        if (Time.time - lastGroundedTime > allowedJumpTime)
        {
            m_Grounded = false;
            canJump = false;
        }            
    }

    public void Move(float speed, bool jump)
    {
        if (!canJump && !jump && m_Grounded) canJump = true;

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
        if (!jump) jumpTimeCounter = 0;

        // If the player is on ground and try to jump
        if (m_Grounded && jump && canJump)
        {
            jumpTimeCounter = maxInputTime;

            canJump = false;
            m_Grounded = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }

        // If the player is not ground and try to jump
        if (!m_Grounded && jump && jumpTimeCounter > 0)
        {
            jumpTimeCounter -= Time.fixedDeltaTime;
        }

        // Change gravity
        if (rb.velocity.y > 0)
        {
            rb.gravityScale = fallingGravity;
            if (jump && jumpTimeCounter > 0)
                rb.gravityScale = jumpingGravity;
        }
        else
        {
            rb.gravityScale = fallingGravity;
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
