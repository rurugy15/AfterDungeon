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
    [SerializeField] private float minInputTime;                    // 최소 인풋 시간
    [SerializeField] private float maxInputTime;                    // 최대 인풋 시간
    [SerializeField] private float flightTime;                      // 최대 인풋 시 점프 체공 시간
    const float maxInputHeight = 5f;                                // 최대 인풋이 끝났을 때 도달하는 높이
    const float maxHeight = 5.2f;                                   // 점프 최대 높이
    private float jumpTimeCounter;
    private bool canJump = false;

    [SerializeField] private float allowedJumpTime;                 // 플랫폼을 벗어난 뒤에도 점프를 할 수 있도록 허용된 시간
    private float lastGroundedTime;

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Rigidbody2D rb;
    private float jumpingGravity;
    private float fallingGravity;
    private float jumpingVelocity;
    private float terminalVelocity;

    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        jumpingVelocity = maxHeight / (maxInputTime - minInputTime);
        jumpingGravity = jumpingVelocity * jumpingVelocity / (2 * 9.81f * (maxHeight - maxInputHeight));
        fallingGravity = 2 * maxHeight / (9.81f * Mathf.Pow((flightTime - maxInputTime), 2));
        terminalVelocity = Mathf.Sqrt(2 * fallingGravity * 9.81f * maxHeight);
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
        if(Time.time - lastGroundedTime > allowedJumpTime)
            m_Grounded = false;
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
        if (!jump) jumpTimeCounter = 0;
        if (m_Grounded && !jump && !canJump) canJump = true;

        if (jump && jumpTimeCounter > 0)
        {
            jumpTimeCounter -= Time.fixedDeltaTime;
        }

        if (m_Grounded && jump && jumpTimeCounter == 0)
        {
            jumpTimeCounter = maxInputTime;
        }
        
        if (m_Grounded && jump && canJump && jumpTimeCounter <= maxInputTime - minInputTime)
        {
            m_Grounded = false;
            canJump = false;

            rb.velocity = new Vector2(rb.velocity.x, jumpingVelocity);
            rb.gravityScale = 0;
        }

        if (rb.velocity.y > 0)
        {
            rb.gravityScale = jumpingGravity;
            if (jump && jumpTimeCounter > 0)
            {
                rb.gravityScale = 0;
            }
        }
        else
        {
            rb.gravityScale = fallingGravity;

            if (rb.velocity.y <= -terminalVelocity) rb.velocity = new Vector2(rb.velocity.x, -terminalVelocity);
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
