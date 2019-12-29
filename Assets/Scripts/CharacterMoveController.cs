using UnityEngine;
using System.Collections.Generic;

public class CharacterMoveController : MonoBehaviour
{
    [Header("Vertical Movement")]
    [SerializeField] [Range(0, 100f)] private float acceleration;               // 프레임당 가속도
    [SerializeField] [Range(0, 100f)] private float deceleration;               // 프레임당 감속도
    private bool m_FacingRight = true;

    [Header("Horizontal Movement")]
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform groundChecker;                           // A position marking where to check if the player is grounded.
    [SerializeField] private float maxHeight;                                   // 점프 최대 높이
    [SerializeField] private float ascentTime;                                  // 점프 상승 시간
    [SerializeField] private float flightTime;                                  // 점프 체공 시간
    private float jumpingGravity;
    private float fallingGravity;
    private float jumpingVelocity;
    private float terminalVelocity;

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Rigidbody2D m_Rigidbody2D;
    
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        jumpingGravity = 2 * maxHeight / (9.8f * (ascentTime * ascentTime));
        fallingGravity = 2 * maxHeight / (9.8f * Mathf.Pow((flightTime - ascentTime), 2));
        jumpingVelocity = 2 * maxHeight / ascentTime;
        terminalVelocity = Mathf.Sqrt(2 * fallingGravity * 9.8f * maxHeight);
    }

    private void FixedUpdate()
    {
        GroundChecking();

        // 떨어질 때
        if (m_Rigidbody2D.velocity.y <= 0)
        {
            m_Rigidbody2D.gravityScale = fallingGravity;

            if (m_Rigidbody2D.velocity.y <= -terminalVelocity) m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -terminalVelocity);
        }
    }

    private void GroundChecking()
    {
        // 상승 중에는 점프 불가
        if (m_Rigidbody2D.velocity.y >= 0.01f) return;

        Collider2D[] colls = Physics2D.OverlapBoxAll(groundChecker.position, new Vector2(0.75f, k_GroundedRadius), 0, m_WhatIsGround);

        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i].gameObject != gameObject)
            {
                m_Grounded = true;
                return;
            }
        }

        m_Grounded = false;
    }

    public void Move(float speed, bool jump)
    {
        HorizontalVelocityControl(speed);

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

        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;

            // 위로 올라갈때
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, jumpingVelocity);
            m_Rigidbody2D.gravityScale = jumpingGravity;
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

    private void HorizontalVelocityControl(float targetV)
    {
        float nowV = m_Rigidbody2D.velocity.x;
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

        m_Rigidbody2D.velocity = new Vector2(changedV, m_Rigidbody2D.velocity.y);
    }
}
