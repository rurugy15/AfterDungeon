using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    [SerializeField] private bool canControl = true;

    private PlayerMovement mover;
    private Animator animator;
    private float horizontal = 0;
    private bool jump = false;
    private bool fire = false;

    private Vector2 originPos;

    private void Awake()
    {
        mover = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        SetSpawnPos(transform.position);
    }

    private void Update()
    {
        if (canControl)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            jump = Input.GetButtonDown("Jump");
            fire = Input.GetButtonDown("Fire");
        }        

        mover.Move(horizontal, jump, fire);
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        jump = false;
    }

    public void GetDamage()
    {
        if (!canControl) return;
        canControl = false;

        StartCoroutine(Die());
    }

    public void SetSpawnPos(Vector2 value)
    {
        originPos = value;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private IEnumerator Die()
    {
        animator.SetBool("Die", true);
        GetComponent<SpriteRenderer>().DOKill();
        GetComponent<SpriteRenderer>().color = Color.white;
        CanControl(false);

        yield return new WaitForSeconds(2f);

        animator.SetBool("Die", false);
        transform.position = originPos;
        CanControl(true);
    }

    public void CanControl(bool canControl)
    {
        this.canControl = canControl;
        horizontal = 0;
        jump = false;
        fire = false;
    }
}
