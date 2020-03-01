using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss : MonoBehaviour
{
    [Header("Event Position")]
    [SerializeField] private Vector2 eventPos;

    [Header("Info Holder")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject firePrefab;

    private Player player;
    private PlayerMove playerMover;

    private Vector2 boxSize = new Vector2(5f, 0.2f);

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerMover = player.GetComponent<PlayerMove>();
        transform.position = new Vector2(0, 999f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) Phase1Tween();
        if (Input.GetKeyDown(KeyCode.G)) Phase2Tween();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(eventPos, boxSize);
    }

    private void Phase1Tween()
    {
        Sequence phase1 = DOTween.Sequence();
        phase1.AppendCallback(() => BlockPlayerControl(true));
        phase1.Append(transform.DOMoveY(Camera.main.transform.position.y + 13, 1f));
        phase1.Insert(0, Camera.main.DOShakePosition(1f));
        phase1.AppendCallback(() => CreatePortal());
        phase1.AppendInterval(1f);
        phase1.AppendCallback(() => BlockPlayerControl(false));
    }

    private void Phase2Tween()
    {
        Sequence phase2 = DOTween.Sequence();
        int pattern = Random.Range(0, 3);

        if (pattern == 0)
        {
            phase2.Append(transform.DOMoveX(-5f, 0.5f));
            phase2.Append(transform.DOShakePosition(2f, 0.3f));
            phase2.AppendCallback(() => Fire());
            phase2.Append(transform.DOMoveX(5f, 0.5f));
            phase2.Append(transform.DOShakePosition(2f, 0.3f));
            phase2.AppendCallback(() => Fire());
            phase2.Append(transform.DOMoveX(0, 0.5f));
        }
        else if (pattern == 1)
        {
            phase2.Append(transform.DOMoveX(5f, 0.5f));
            phase2.Append(transform.DOShakePosition(2f, 0.3f));
            phase2.AppendCallback(() => Fire());
            phase2.Append(transform.DOMoveX(-5f, 0.5f));
            phase2.Append(transform.DOShakePosition(2f, 0.3f));
            phase2.AppendCallback(() => Fire());
            phase2.Append(transform.DOMoveX(0, 0.5f));
        }
        else if (pattern == 2)
        {
            phase2.Append(transform.DOShakePosition(3f, 0.3f));
            phase2.AppendCallback(() => Fire(45));
            phase2.AppendCallback(() => Fire());
            phase2.AppendCallback(() => Fire(-45));
        }

        phase2.AppendInterval(3f);
        phase2.AppendCallback(() => Phase2Tween());
    }

    private void BlockPlayerControl(bool blockControl)
    {
        player.canControl = !blockControl;

        if (blockControl) Debug.Log("플레이어로부터 캐릭터 제어 권한을 박탈합니다.");
        else Debug.Log("플레이어에게 캐릭터 제어 권한을 부여합니다.");
    }

    private void CreatePortal()
    {
        Debug.Log("포탈을 생성합니다.");
    }

    private bool CheckEvent(Vector2 pos)
    {
        Collider2D coll = Physics2D.OverlapBox(pos, boxSize, 0, playerLayer);

        if (coll && playerMover.IsGrounded) return true;
        else return false;
    }

    private void Fire(float angle = 0)
    {
        Vector2 dir = new Vector2(Mathf.Cos((angle - 90f) * Mathf.Deg2Rad), Mathf.Sin((angle - 90f) * Mathf.Deg2Rad));
        FireController fire = Instantiate(firePrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<FireController>();
        fire.Initialize(dir, 7f);
        Debug.Log(dir + " 방향으로 불을 발사합니다.");
    }
}
