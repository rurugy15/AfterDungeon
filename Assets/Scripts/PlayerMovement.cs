﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isDashed;
    [SerializeField] private bool isFired;
    private bool isDashing;
    [SerializeField] private WallState wallState;

    [Header("Basic Movement")]
    [Tooltip("지면 수평 가속도")]
    [SerializeField] private float groundAccel;
    [Tooltip("공중 수평 가속도")]
    [SerializeField] private float airAccel;
    [Tooltip("기본 수평 이동속도")]
    [SerializeField] private float horizontalSpeed;
    [Tooltip("기본 중력")]
    [SerializeField] private float originGravity;

    [Header("Jump Movement")]
    [Tooltip("점프 속도 (오른쪽 점프 기준)")]
    [SerializeField] private Vector2 jumpVelocity;
    [Tooltip("대쉬 속도 (오른쪽 대쉬 기준)")]
    [SerializeField] private Vector2 dashVelocity;
    [Tooltip("지면으로 인정할 Layer")]
    [SerializeField] private LayerMask whatIsGround;
    [Tooltip("지면 존재 유무를 판정할 위치")]
    [SerializeField] private Transform groundChecker;

    [Header("Fire Movement")]
    [SerializeField] private Transform fireChecker;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float maxDistance;
    [SerializeField] private float fireVelocity;
    [SerializeField] private Vector2 fireJumpVelocity;
    [SerializeField] private Vector2 projJumpVelocity;
    [Tooltip("발사 버튼 누른 시간이 짧았을 때 화살 존재 시간")]
    [SerializeField] private float shortFireTime;
    [Tooltip("발사 버튼 누른 시간이 길었을 때 화살 존재 시간")]
    [SerializeField] private float longFireTime;
    private ProjectileController projectile;

    public enum WallState { None, Slide}
    [Header("Wall Movement")]
    [Tooltip("벽으로 인정할 Layer")]
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private Transform wallChecker;
    [Tooltip("벽에서 미끄러질 속도")]
    [SerializeField] private float slidingVelocity;
    [Tooltip("벽점프 속도 (오른쪽 점프 기준)")]
    [SerializeField] private Vector2 slidingJumpVelocity;

    [Header("Stamina")]
    [SerializeField] private float totalStamina;
    [SerializeField] private float stamina;
    [SerializeField] private float wallSlideStatmina;

    [Header("Fine Control")]
    [Tooltip("지면을 벗어났을 때 유효 점프 인정 시간")]
    [SerializeField] private float mildJumpTime;
    [Tooltip("이른 점프 입력을 했을 때 유효한 인풋 판정 시간")]
    [SerializeField] private float allowedJumpTime;
    [Tooltip("대쉬 지속 시간. 이 시간동안 중력이 0이 되고 다른 조작 불가")]
    [SerializeField] private float dashingTime;
    [Tooltip("벽을 벗어난 이후에도 벽 점프를 할 수 있는 시간")]
    [SerializeField] private float mildWallTime;
    [Tooltip("벽점프 이후 조작 강탈 시간")]
    [SerializeField] private float wallJumpExtortionTime;

    private Vector2 groundBox = new Vector2(0.7f, 0.2f);
    private Vector2 wallBox = new Vector2(0.2f, 0.7f);
    private Vector2 fireBox = new Vector3(2f,0.15f);
    private Rigidbody2D rb2D;
    private Animator animator;

    private bool isFacingRight = true;
    private bool isGravityControlled;
    private float lastGroundedTime = -100;                              // 너그러운 점프를 위한 마지막으로 지면에 있던 시간
    private float lastJumpInputTime = -999;                             // 이른 점프 인풋을 위한 마지막으로 점프를 누른 시점
    private float lastWallTime = -999f;                                 // 너그러운 벽점프를 위한 마지막 벽 인접 시간
    private int? closestWall = null;

    public bool IsFacingRight { get { return isFacingRight; } }

    public float Acceleration
    {
        get
        {
            if (isGrounded) return groundAccel;
            else return airAccel;
        }
    }

    public Vector2 WallCheckPos
    {
        get
        {
            if (isFacingRight) return transform.position + wallChecker.localPosition;
            else return transform.position - wallChecker.localPosition;
        }
    }

    public Vector2 FireCheckPos
    {
        get
        {
            if (isFacingRight) return transform.position + fireChecker.localPosition;
            else return transform.position - fireChecker.localPosition;
        }
    }


    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        rb2D.gravityScale = originGravity;
        isFired = false;
        isDashing = false;
    }

    private void FixedUpdate()
    {
        isGrounded = GroundChecking();
        closestWall = WallChecking();
        rb2D.gravityScale = GravityControl();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundChecker.position, groundBox);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(wallChecker.position, wallBox);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(fireChecker.position, fireBox);
    }

    private bool GroundChecking()
    {
        // 상승 중에는 점프 불가
        if (rb2D.velocity.y >= 0.01f)
        {
            //return false;
        }

        List<Collider2D> colliders = new List<Collider2D>();

        Collider2D[] colls = Physics2D.OverlapBoxAll(groundChecker.position, groundBox, 0, whatIsGround);
        foreach (Collider2D coll in colls)
        {
            colliders.Add(coll);
        }

        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                lastGroundedTime = Time.time;
                break;
            }
        }

        if (Time.time - lastGroundedTime <= mildJumpTime)
        {
            if (isGrounded == false) GroundingEvent();
            return true;
        }
        else
        {
            animator.SetBool("IsGrounded", false);
            return false;
        }
    }

    private int? WallChecking()
    {
        Collider2D[] colls = Physics2D.OverlapBoxAll(WallCheckPos, wallBox, 0, whatIsWall);
        if (colls.Length != 0)
        {
            lastWallTime = Time.time;
            return IsFacingRight ? 1 : -1;
        }

        if (Time.time - lastWallTime <= mildWallTime) return closestWall;
        else return null;
    }

    private bool FireChecking()
    {
        Collider2D[] colls = Physics2D.OverlapBoxAll(FireCheckPos, fireBox, 0, whatIsWall);
        if (colls.Length != 0)
            return false;
        else
            return true;
    }

    private void GroundingEvent()
    {
        animator.SetBool("IsGrounded", true);
        Stamina = totalStamina;
        isDashed = false;
    }

    private float GravityControl()
    {
        if (isGravityControlled) return rb2D.gravityScale;
        if (wallState == WallState.Slide) return 0f;
        return originGravity;
    }

    public void Move(float horizontal, bool jump, bool dash, bool fire)
    {
        if (jump) lastJumpInputTime = Time.time;

        GrabWall(horizontal);
        HorizontalMove(horizontal);
        if (AllowToJump()) JumpingMovement(horizontal);
        if (dash) Dash(horizontal);
        if (fire) Fire(horizontal);

        animator.SetFloat("Jump Speed", rb2D.velocity.y);
    }

    private void JumpingMovement(float horizontal)
    {
        Debug.Log("Is Ground = " + isGrounded
            + " Closest Wall = " + closestWall
            + " Wall State = " + wallState
            + " Horizontal = " + horizontal);

        if (wallState == WallState.None) Jump(horizontal);
        else if (!isGrounded && closestWall.HasValue) WallJump();

        lastJumpInputTime = -999f;
    }

    private void Fire(float horizontal)
    {
        if (isFired == false&&FireChecking())
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<ProjectileController>().Initialize(IsFacingRight, fireVelocity, maxDistance,this);
            isFired = true;
            this.projectile = projectile.GetComponent<ProjectileController>();

            if (isGrounded == false)
            {
                float x = fireJumpVelocity.x;
                float y = fireJumpVelocity.y;
                if (horizontal > 0) ApplyJumpVelocity(x, y);
                else if (horizontal < 0) ApplyJumpVelocity(-x, y);
                else ApplyJumpVelocity(0, y);
            }
        }
    }
    
    private void Dash(float horizontal)
    {
        #region Dash
        if ((isDashed == false) && (isDashing == false)/* && isGrounded == false*/)
        {
            isDashed = true;
            
            StartCoroutine(GravityControl(0, dashingTime));

            float x = dashVelocity.x;
            //float y = dashVelocity.y;
            if (IsFacingRight) StartCoroutine(DashMove(x, 0, dashingTime));
            else StartCoroutine(DashMove(-x, 0, dashingTime));
            //StartCoroutine(EndDash());
            Debug.Log("Stumping : " + rb2D.velocity);
        }
        #endregion
    }
    private IEnumerator DashMove(float x, float y, float dashingTime)
    {
        float startTime = Time.time;
        while(Time.time-startTime<dashingTime)
        {
            float elapsed = Time.time - startTime;
            if(elapsed<dashingTime/4)
            {
                rb2D.velocity = new Vector2(elapsed*x*4/dashingTime, y);
                Flip(x);
            }
            else if(dashingTime/4<=elapsed && elapsed<dashingTime*3/4)
            {
                rb2D.velocity = new Vector2(x, y);
                Flip(x);
            }
            else
            {
                rb2D.velocity = new Vector2((-1)*elapsed * x * 4 / dashingTime+4*x, y);
                Flip(x);
            }
            yield return null;
        }
        
        isDashing = false;
        if (isGrounded)
            isDashed = false;
    }

    private void Jump(float horizontal)
    {

        #region Normal Jump
        if(isGrounded)
        {
            float x = jumpVelocity.x;
            float y = jumpVelocity.y;
            if (horizontal > 0) ApplyJumpVelocity(x, y);
            else if (horizontal < 0) ApplyJumpVelocity(-x, y);
            else ApplyJumpVelocity(0, y);

            Debug.Log("Normal Jump : " + rb2D.velocity);
        }
        #endregion
    }

    private IEnumerator GravityControl(float value, float duration)
    {
        isGravityControlled = true;
        rb2D.gravityScale = value;

        yield return new WaitForSeconds(duration);

        isGravityControlled = false;
    }

    private void WallJump()
    {
        if (closestWall == 1) ApplyJumpVelocity(-slidingJumpVelocity.x, slidingJumpVelocity.y, wallJumpExtortionTime);
        else if(closestWall == -1) ApplyJumpVelocity(slidingJumpVelocity.x, slidingJumpVelocity.y, wallJumpExtortionTime);
        Debug.Log("Wall Jump : " + rb2D.velocity);

        lastWallTime = -999f;
        wallState = WallState.None;
    }

    // Control only horizontal velocity
    private void HorizontalMove(float dir)
    {
        if (isJumping) return;

        Flip(dir);

        float targetV = dir * horizontalSpeed;
        float nowV = rb2D.velocity.x;

        if (nowV == targetV) return;

        if (targetV > nowV)
        {
            nowV += Acceleration * Time.fixedDeltaTime;
            if (nowV > targetV) nowV = targetV;
        }
        else if (targetV < nowV)
        {
            nowV -= Acceleration * Time.fixedDeltaTime;
            if (nowV < targetV) nowV = targetV;
        }

        rb2D.velocity = new Vector2(nowV, rb2D.velocity.y);
    }

    private void ApplyJumpVelocity(float x, float y, float duration = 0f)
    {
        rb2D.velocity = new Vector2(x, y);
        Flip(x);
        animator.SetTrigger("Jump");

        if (duration != 0)
        {
            StartCoroutine(EscapeJumping(duration));
        }
    }

    public void GrabWall(float horizontal)
    {
        bool? goRight = null;
        if (horizontal > 0) goRight = true;
        else if (horizontal < 0) goRight = false;

        if (Stamina <= 0 || isGrounded)
        {
            wallState = WallState.None;
            animator.SetBool("Wall", false);
        }
        else if (closestWall.HasValue && (goRight == isFacingRight) && rb2D.velocity.y <= 0)
        {
            rb2D.velocity = new Vector2(0, -slidingVelocity);
            wallState = WallState.Slide;
            Stamina -= wallSlideStatmina * Time.deltaTime;
            animator.SetBool("Wall", true);
        }
        else 
        {
            wallState = WallState.None;
            animator.SetBool("Wall", false);
            animator.SetBool("Wall Jump Ready", false);
        }
    }

    public void ProjectileJump()
    {
        float x = projJumpVelocity.x;
        float y = projJumpVelocity.y;
        if (IsFacingRight) ApplyJumpVelocity(x, y);
        else ApplyJumpVelocity(-x, y);
    }

    private float Stamina
    {
        get { return stamina; }
        set
        {
            float target = value;
            if (target < 25 && stamina >= 25)
            {
                GetComponent<SpriteRenderer>().DOColor(new Color(1, 0.5f, 0.5f), 0.15f)
                    .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
            }
            else if(target >= 25)
            {
                GetComponent<SpriteRenderer>().DOKill();
                GetComponent<SpriteRenderer>().color = Color.white;
            }

            stamina = target;
        }
    }

    public bool AllowToJump()
    {
        if (Time.time - lastJumpInputTime <= allowedJumpTime) return true;
        else return false;
    }

    private void Flip(float dir)
    {
        if (dir == 0) return;
        if (dir > 0 == IsFacingRight) return;

        isFacingRight = !isFacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private IEnumerator EscapeJumping(float duration)
    {
        isJumping = true;

        yield return new WaitForSeconds(duration);

        isJumping = false;
    }

    public IEnumerator JumperAccelChange()
    {
        float originAirAccel = airAccel;

        airAccel = 0;

        yield return new WaitForSeconds(0.16f);

        airAccel = originAirAccel;
    }
    public void SetProjectileTime(float pressedTime)
    {
        if(pressedTime<=1.0f)
        {
            projectile.SetLimit(shortFireTime);
        }
        else
        {
            projectile.SetLimit(longFireTime);
        }
    }

    public void DashRefill()
    {
        isDashed = false;
    }
    
    public void FireEnd()
    {
        isFired = false;
    }
}