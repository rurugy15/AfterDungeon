
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [SerializeField] [Range(0, 100f)] private float acceleration = 5f;          // 프레임당 가속도
    [SerializeField] [Range(0, 100f)] private float deceleration = 5f;          // 프레임당 감속도
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                m_Grounded = true;
        }
    }


    public void Move(float speed, bool jump)
    {
        VelocityControl(speed);

        // If the input is moving the player right and the player is facing left...
        if (speed > 0 && !m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (speed < 0 && m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }

        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
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

    private void VelocityControl(float targetV)
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
